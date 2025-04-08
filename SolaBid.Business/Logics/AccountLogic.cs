using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;
using SolaBid.Business.ConnectableEntityExtensions;
using SolaBid.Business.Dtos.ApiDtos;
using SolaBid.Business.Dtos.EntityDtos;
using SolaBid.Business.Dtos.SingleObjs;
using SolaBid.Business.Models;
using SolaBid.Domain.Models.Entities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using SolaBid.Domain.Models;
using static SolaBid.Extensions.FileExtensions;

namespace SolaBid.Business.Logics
{
    public class AccountLogic
    {
        public async Task<LoginResult> Login(LoginDto login, SignInManager<AppUser> signInManager, ModelStateDictionary ModelState, string host)
        {
            var loginResult = new LoginResult();
            if (!ModelState.IsValid)
            {
                var modelResult = new ErrorHandlerLogic().GetModelErrors(ModelState);
                return TransactionConfig.Mapper.Map<LoginResult>(modelResult);
            }
            using (var context = TransactionConfig.AppDbContext)
            {
                var userIsExist = await context.Users
                    .Where(m => m.UserName == login.UserName)
                    .FirstOrDefaultAsync();
                if (userIsExist != null)
                {
                    if (!userIsExist.IsApproved)
                    {
                        loginResult.ErrorList.Add("Your account has not been verified by admin.");
                        loginResult.IsAuthorized = false;
                        return loginResult;
                    }
                    if (!userIsExist.IsActive)
                    {
                        loginResult.ErrorList.Add("Your account has been suspended by admin.");
                        loginResult.IsAuthorized = false;
                        return loginResult;
                    }
                }

                var result = await signInManager.PasswordSignInAsync(login.UserName,
                        login.Password, login.RememberMe, false);
                if (result.Succeeded)
                {
                    loginResult.IsAuthorized = true;
                    loginResult.FirstName = userIsExist.FirstName;
                    loginResult.LastName = userIsExist.LastName;
                    loginResult.Image = userIsExist.UserImage == null ? string.Empty : ConvertFileToBase64(userIsExist.UserImage, "appfiles");
                    loginResult.UserId = userIsExist.Id;
                    return loginResult;
                }
                else
                {
                    loginResult.ErrorList.Add("Email or password is not valid.");
                    loginResult.IsAuthorized = false;
                    return loginResult;
                }
            }
        }

        public async Task<ApiResult> RestorePassword(string password, string token, UserManager<AppUser> userManager)
        {
            var result = new ApiResult();
            try
            {
                using (var context = TransactionConfig.AppDbContext)
                {
                    var user = await context.Users.Where(m => m.RestorePasswordToken == token).FirstOrDefaultAsync();
                    if (user is null || string.IsNullOrEmpty(password) || string.IsNullOrEmpty(token) || string.IsNullOrEmpty(user.RestorePasswordToken) || DateTime.Now > user.RestoreExpireDate)
                    {
                        result.ErrorList.Add("Restore date expired.Please,try again.");
                        return result;
                    }
                    user.PasswordHash = userManager.PasswordHasher.HashPassword(user, password);
                    user.RestorePasswordToken = "";
                    user.RestoreExpireDate = DateTime.Now;
                    context.Users.Update(user);
                    await context.SaveChangesAsync();
                }
                result.OperationIsSuccess = true;
            }
            catch (Exception ex)
            {
                await ex.ErrorLog();
            }
            return result;
        }

        public async Task<bool> CheckRestoreToken(string token)
        {
            using (var context = TransactionConfig.AppDbContext)
            {
                return await context.Users.AnyAsync(m => m.RestorePasswordToken == token && DateTime.Now <= m.RestoreExpireDate);
            }
        }

        private void SendRestorePasswordEmailAsync(AppUser user, string root)
        {
            var dataBase = Statics.IsProductionEnvironment ? Statics.API_SOCAR_BASE : Statics.API_BASE;
            MailAddress address = new MailAddress("socarstpinfo@apertech.net");
            MailMessage mail = new MailMessage();
            mail.From = new MailAddress("socarstpinfo@apertech.net", "Sola Bid SOCAR-STP");
            mail.To.Add(user.Email.Trim());
            mail.Subject = "SolaBid Restore Password";
            mail.IsBodyHtml = true;
            string body = string.Empty;
            using (StreamReader reader = new StreamReader(Path.Combine(root, "templates", "ResetPassword.html")))
            { body = reader.ReadToEnd(); }
            body = body.Replace("{fullname}", $"{user.FirstName} {user.LastName}").Replace("{url}", $"{dataBase}restorepassword/{user.RestorePasswordToken}");
            mail.Body = body;
            using (var sc = new SmtpClient())
            {
                sc.Port = 587;
                sc.Host = "mail.apertech.net";
                sc.EnableSsl = true;
                sc.Credentials = new NetworkCredential("socarstpinfo@apertech.net", "Toshiba.509.");
                sc.Send(mail);
            }
        }

        public async Task<ApiResult> Restore(StringSingleId userEmail, string root)
        {
            try
            {
                var email = userEmail.Id;
                var result = new ApiResult();

                if (string.IsNullOrEmpty(email))
                {
                    result.ErrorList.Add("Email is not valid");
                    result.OperationIsSuccess = false;
                    return result;
                }

                using (var context = TransactionConfig.AppDbContext)
                {
                    var userEntity = await context.Users.Where(m => m.Email == email).FirstOrDefaultAsync();
                    if (userEntity is null)
                    {
                        result.ErrorList.Add("Email is not valid");
                        return result;
                    }
                    var userRestoreToken = Guid.NewGuid().ToString();
                    userEntity.RestorePasswordToken = userRestoreToken;
                    userEntity.RestoreExpireDate = DateTime.Now.AddHours(24);
                    await context.SaveChangesAsync();
                    SendRestorePasswordEmailAsync(userEntity, root);
                }
                result.OperationIsSuccess = true;
                return result;
            }
            catch (Exception ex)
            {
                await ex.ErrorLog();
                return new ApiResult();
            }
        }

        public async Task<List<string>> GetAdminUsersEmails()
        {
            var emailList = new List<string>();
            using (var context = TransactionConfig.AppDbContext)
            {
                var adminId = await context.Roles.Where(m => m.Name.ToLower().Contains("admin")).Select(m => m.Id).FirstOrDefaultAsync();
                var adminUsers = await context.UserRoles.Where(m => m.RoleId == adminId).Select(m => m.UserId).ToListAsync();
                foreach (var userId in adminUsers)
                {
                    var userEmail = context.Users.Where(m => m.Id == userId && !m.IsDeleted && m.IsActive && m.IsApproved).Select(m => m.Email).FirstOrDefault();
                    emailList.Add(userEmail);
                }
            }
            return emailList;
        }
        public async Task<ApiResult> RegistrationUser(AppUserDto regUser, UserManager<AppUser> userManager, string root, ModelStateDictionary ModelState)
        {
            var apiResult = new ApiResult();
            using (var context = TransactionConfig.AppDbContext)
            {
                if (!string.IsNullOrEmpty(regUser.UserImageBase64))
                {
                    regUser.UserImage = await regUser.UserImageBase64.SaveFileBase64(regUser.UserImageName, "userPhotos", root);
                }

                var appUserEntity = new AppUser
                {
                    BuyerId = null,
                    Email = regUser.Email,
                    UserName = regUser.Email,
                    FirstName = regUser.FirstName,
                    LastName = regUser.LastName,
                    IsActive = false,
                    IsApproved = false,
                    RegDate = DateTime.Now,
                    UserImage = regUser.UserImage,
                };

                var result = await userManager.CreateAsync(appUserEntity, regUser.Password);
                if (result.Succeeded)
                {
                    apiResult.OperationIsSuccess = true;
                    var emailList = await GetAdminUsersEmails();
                    _ = Task.Run(() => { SendNewRegistrationEmailAsync(emailList, root); });
                    return apiResult;
                }
                else
                {
                    if (result.Errors.Any(x => x.Code == "DuplicateUserName"))
                    {
                        apiResult.ErrorList.Add("Email is already declared");
                        return apiResult;
                    }
                    else
                    {
                        foreach (var error in result.Errors)
                            apiResult.ErrorList.Add(error.Description);
                    }
                }
            }
            return apiResult;
        }
        private void SendNewRegistrationEmailAsync(List<string> emailList, string root)
        {
            var dataBase = Statics.IsProductionEnvironment ? Statics.API_SOCAR_BASE : Statics.API_BASE;
            MailAddress address = new MailAddress("socarstpinfo@apertech.net");
            MailMessage mail = new MailMessage();
            mail.From = new MailAddress("socarstpinfo@apertech.net", "Sola Bid SOCAR-STP");
            foreach (var email in emailList)
            {
                if (!string.IsNullOrEmpty(email))
                {
                    mail.To.Add(email.Trim());
                }
            }
            mail.Subject = "SolaBid New Registration";
            mail.IsBodyHtml = true;
            string body = string.Empty;
            using (StreamReader reader = new StreamReader(Path.Combine(root, "templates", "RegistrationIsPending.html")))
            { body = reader.ReadToEnd(); }
            body = body.Replace("{url}", dataBase);
            mail.Body = body;
            using (var sc = new SmtpClient())
            {
                sc.Port = 587;
                sc.Host = "mail.apertech.net";
                sc.EnableSsl = true;
                sc.Credentials = new NetworkCredential("socarstpinfo@apertech.net", "Toshiba.509.");
                sc.Send(mail);
            }
        }
    }
}

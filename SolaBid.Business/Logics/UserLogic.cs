using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;
using SolaBid.Business.Dtos.ApiDtos;
using SolaBid.Business.Dtos.EntityDtos;
using SolaBid.Business.Models;
using SolaBid.Domain.Models.Entities;
using SolaBid.Extensions;
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
    public class UserLogic
    {
        public async Task<List<NewUsersDto>> GetNewUsers(string siteDatabase)
        {
            var newUserDto = new List<NewUsersDto>();
            using (var context = TransactionConfig.AppDbContext)
            {
                List<AppRole> userGroups = await context.Roles.Where(m => !m.IsDeleted).ToListAsync();
                List<BuyerDto> buyers = new SiteLineDbLogic(siteDatabase).GetBuyers();
                List<AppUser> newUserEntities = await context.Users.Where(m => !m.IsApproved).ToListAsync();
                newUserDto.AddRange(newUserEntities.Select(newUser => new NewUsersDto
                {
                    Id = newUser.Id,
                    Buyers = TransactionConfig.Mapper.Map<List<KeyValueTextBoxingDto>>(buyers),
                    UserName = newUser.UserName,
                    Fullname = newUser.FirstName + " " + newUser.LastName,
                    Groups = TransactionConfig.Mapper.Map<List<KeyValueTextBoxingDto>>(userGroups),
                    Image = ConvertFileToBase64(newUser.UserImage, "appfiles")
                }));
            }
            return newUserDto;
        }
        public async Task<List<NewUsersDto>> GetNewUsersForTest()
        {
            string siteDatabase = "SOCARSL_APP";
            var newUserDto = new List<NewUsersDto>();
            using (var context = TransactionConfig.AppDbContext)
            {
                List<AppRole> userGroups = await context.Roles.Where(m => !m.IsDeleted).ToListAsync();
                List<BuyerDto> buyers = new SiteLineDbLogic(siteDatabase).GetBuyers();
                List<AppUser> newUserEntities = await context.Users.Where(m => !m.IsApproved).ToListAsync();
                newUserDto.AddRange(newUserEntities.Select(newUser => new NewUsersDto
                {
                    Id = newUser.Id,
                    Buyers = TransactionConfig.Mapper.Map<List<KeyValueTextBoxingDto>>(buyers),
                    UserName = newUser.UserName,
                    Fullname = newUser.FirstName + " " + newUser.LastName,
                    Groups = TransactionConfig.Mapper.Map<List<KeyValueTextBoxingDto>>(userGroups),
                    Image = ConvertFileToBase64(newUser.UserImage, "appfiles")
                }));
            }
            return newUserDto;
        }
        public async Task<ApiResult> ApproveNewUsersFromMain(List<ApproveNewUserDto> approveNewUsers, ModelStateDictionary ModelState, string root)
        {
            var apiResult = new ApiResult();
            if (approveNewUsers.Count == 0)
            {
                apiResult.ErrorList.Add("No selected row");
                return apiResult;
            }
            try
            {
                using (var context = TransactionConfig.AppDbContext)
                {
                    foreach (ApproveNewUserDto approvedUser in approveNewUsers)
                    {
                        AppUser entityApprovingUser = await context.Users.FindAsync(approvedUser.Id);
                        foreach (var groupId in approvedUser.GroupIds)
                        {
                            _ = await context.UserRoles.AddAsync(new AspNetUserRoles
                            {
                                UserId = approvedUser.Id,
                                RoleId = groupId
                            });
                        }

                        entityApprovingUser.IsApproved = true;
                        entityApprovingUser.IsActive = true;
                        entityApprovingUser.BuyerId = approvedUser.BuyerId;
                        entityApprovingUser.BuyerUserName = approvedUser.BuyerUserName;
                        context.Users.Update(entityApprovingUser);
                    }
                    await context.SaveChangesAsync();
                    apiResult.OperationIsSuccess = true;
                    foreach (ApproveNewUserDto approvedUser in approveNewUsers)
                    {
                        var user = await context.Users.FindAsync(approvedUser.Id);
                        _ = Task.Run(() => { SendApproveUserEmailAsync(user, root); });
                    }
                }
            }
            catch (Exception ex)
            {
                apiResult.ErrorList.Add(ex.Message);
                return apiResult;
            }

            return apiResult;
        }

        public async Task<string> GetLayout(string userId, string key)
        {
            using (var context = TransactionConfig.AppDbContext)
            {
                return await context.Layouts.Where(m => m.UserId == userId && m.Key == key).Select(m => m.GridLayout).FirstOrDefaultAsync();
            }
        }

        public static async Task<string> GetUserSignature(string userId)
        {
            using (var context = TransactionConfig.AppDbContext)
            {
                var userSignature = await context.Users.Where(m => m.Id == userId).Select(m => m.UserSignature).FirstOrDefaultAsync();
                return string.IsNullOrEmpty(userSignature) ? "" : FileExtensions.ConvertFileToBase64(userSignature, "appfiles");
            }
        }

        public async Task<ApiResult> SetLayout(string userId, LayoutSaveDto layoutModel)
        {
            var res = new ApiResult();
            using (var context = TransactionConfig.AppDbContext)
            {
                var removeExistLayout = await context.Layouts.Where(m => m.Key == layoutModel.Key && m.UserId == userId).FirstOrDefaultAsync();
                if (removeExistLayout != null)
                {
                    context.Layouts.Remove(removeExistLayout);
                }
                await context.Layouts.AddAsync(new Layout
                {
                    GridLayout = layoutModel.GridLayout,
                    Key = layoutModel.Key,
                    UserId = userId
                });
                await context.SaveChangesAsync();
            }
            return res;
        }

        public static async Task<UserFullnameBuyerNameDto> GetUserFullNameBuyerName(string userId)
        {
            using (var context = TransactionConfig.AppDbContext)
            {
                return await context.Users.Where(m => m.Id == userId).Select(m => new UserFullnameBuyerNameDto
                {
                    BuyerName = m.BuyerUserName,
                    FullName = $"{m.FirstName} {m.LastName}"
                }).FirstOrDefaultAsync();
            }
        }

        private void SendApproveUserEmailAsync(AppUser user, string root)
        {
            var dataBase = Statics.IsProductionEnvironment ? Statics.API_SOCAR_BASE : Statics.API_BASE;
            MailAddress address = new MailAddress("socarstpinfo@apertech.net");
            MailMessage mail = new MailMessage();
            mail.From = new MailAddress("socarstpinfo@apertech.net", "Sola Bid SOCAR-STP");
            mail.To.Add(user.Email.Trim());
            mail.Subject = "SolaBid Approve Registration Result";
            mail.IsBodyHtml = true;
            string body = string.Empty;
            using (StreamReader reader = new StreamReader(Path.Combine(root, "templates", "RegistrationIsApproved.html")))
            { body = reader.ReadToEnd(); }
            body = body.Replace("{fullname}", $"{user.FirstName} {user.LastName}").Replace("{url}", dataBase);
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
        private void SendRejectUserEmailAsync(AppUser user, string root)
        {
            MailAddress address = new MailAddress("socarstpinfo@apertech.net");
            MailMessage mail = new MailMessage();
            mail.From = new MailAddress("socarstpinfo@apertech.net", "Sola Bid SOCAR-STP");
            mail.To.Add(user.Email.Trim());
            mail.Subject = "SolaBid Reject Registration Result";
            mail.IsBodyHtml = true;
            string body = string.Empty;
            using (StreamReader reader = new StreamReader(Path.Combine(root, "templates", "RegistrationIsRejected.html")))
            { body = reader.ReadToEnd(); }
            body = body.Replace("{fullname}", $"{user.FirstName} {user.LastName}");
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
        public async Task<AppUserDto> GetUserById(string userId)
        {
            var userDto = new AppUserDto();
            using (var context = TransactionConfig.AppDbContext)
            {
                AppUser userEntity = await context.Users.FindAsync(userId);
                return userEntity == null ? userDto : TransactionConfig.Mapper.Map<AppUserDto>(userEntity);
            }
        }

        public async Task<(string userId, string userName)> GetUserByUsername(string userName)
        {
            using (var context = TransactionConfig.AppDbContext)
            {
                AppUser userEntity = await context.Users.Where(m => m.UserName == userName).FirstOrDefaultAsync();
                return (userEntity.Id, userEntity.UserName);
            }
        }
        public async Task<List<ApprovedUserListDto>> GetApprovedUsers()
        {
            var approvedUsers = new List<ApprovedUserListDto>();
            using (var context = TransactionConfig.AppDbContext)
            {
                var approvedUsersEntity = await context.Users.Where(m => m.IsApproved && !m.IsDeleted).ToListAsync();
                approvedUsers = TransactionConfig.Mapper.Map<List<ApprovedUserListDto>>(approvedUsersEntity);
            }
            return approvedUsers;
        }
        public async Task<List<ApprovedUserListDto>> GetApprovedUsersForTest()
        {
            var approvedUsers = new List<ApprovedUserListDto>();
            using (var context = TransactionConfig.AppDbContext)
            {
                var approvedUsersEntity = await context.Users.Where(m => m.IsApproved && !m.IsDeleted).ToListAsync();
                approvedUsers = TransactionConfig.Mapper.Map<List<ApprovedUserListDto>>(approvedUsersEntity);
                foreach (var item in approvedUsers)
                {
                    item.GroupName = string.Join(',', item.Groups);
                }
            }
            return approvedUsers;
        }
        public async Task<List<UsersForPrivilegeDto>> GetUsersForPrivilege()
        {
            var approvedUsers = new List<UsersForPrivilegeDto>();
            using (var context = TransactionConfig.AppDbContext)
            {
                var approvedUsersEntity = await context.Users.Where(m => m.IsApproved && !m.IsDeleted).ToListAsync();
                approvedUsers = TransactionConfig.Mapper.Map<List<UsersForPrivilegeDto>>(approvedUsersEntity);
            }
            return approvedUsers;
        }
        public async Task<List<UserListDto>> GetUsers()
        {
            var approvedUsers = new List<UserListDto>();
            using (var context = TransactionConfig.AppDbContext)
            {
                var approvedUsersEntity = await context.Users.Where(m => m.IsApproved && !m.IsDeleted).ToListAsync();
                approvedUsers = TransactionConfig.Mapper.Map<List<UserListDto>>(approvedUsersEntity);
            }
            return approvedUsers;
        }
        public async Task<List<ApprovedUserListDto>> GetGroupUsersWithAll(string groupId)
        {
            var groupUsersWithAll = await GetApprovedUsers();
            using (var context = TransactionConfig.AppDbContext)
            {
                var groupUsers = await context.UserRoles.Where(m => m.RoleId == groupId).ToListAsync();
                if (groupUsers.Count > 0)
                {
                    foreach (var user in groupUsersWithAll)
                    {
                        if (groupUsers.Any(m => m.UserId == user.Id))
                        {
                            user.IsSelected = true;
                        }
                    }
                }
            }
            return groupUsersWithAll;
        }
        public async Task<UserFormDataDto> GetUserFormDatas(string userId, List<BuyerDto> userBuyers)
        {
            var userDatas = new UserFormDataDto();
            var userGroups = await new GroupLogic().GetUserGroupsWithAll(userId);
            using (var context = TransactionConfig.AppDbContext)
            {
                var userEntity = await context.Users.FindAsync(userId);
                userDatas = TransactionConfig.Mapper.Map<UserFormDataDto>(userEntity);

            }
            if (!string.IsNullOrEmpty(userDatas.UserImage))
            {
                userDatas.UserImage = userDatas.UserImage.Substring(11, userDatas.UserImage.Length - 11);
            }
            userDatas.UserGroupsWithAll = TransactionConfig.Mapper.Map<List<KeyValueTextBoxingDto>>(userGroups);
            userDatas.UserBuyersWithAll = TransactionConfig.Mapper.Map<List<KeyValueTextBoxingDto>>(userBuyers);
            return userDatas;
        }
        public async Task<ApiResult> EditUser(EditUserDto editUser, ModelStateDictionary ModelState, string root)
        {
            var apiResult = new ApiResult();

            using (var context = TransactionConfig.AppDbContext)
            {
                var userEntity = await context.Users.FindAsync(editUser.Id);
                userEntity.FirstName = editUser.FirstName;
                userEntity.LastName = editUser.LastName;
                userEntity.BuyerId = editUser.BuyerId == "0" ? null : editUser.BuyerId;
                userEntity.BuyerUserName = editUser.BuyerUserName;

                if (!string.IsNullOrEmpty(userEntity.UserImage))
                {
                    DeleteFile(root, "appfiles", userEntity.UserImage);
                    userEntity.UserImage = null;
                }

                if (editUser.UserImage != null)
                {
                    if (editUser.UserImage.Length > 10)
                    {
                        editUser.UserImage = "a" + editUser.UserImage.Substring(editUser.UserImage.Length - 8);
                    }
                    userEntity.UserImage = await editUser.UserImageBase64.SaveFileBase64(editUser.UserImage, "userPhotos", root);
                }

                if (!string.IsNullOrEmpty(editUser.UserSignature))
                {
                    DeleteFile(root, "userSignatures", userEntity.UserSignature);
                    userEntity.UserSignature = await editUser.UserSignatureBase64.SaveFileBase64(editUser.UserImage, "userSignatures", root);
                }
                else
                {
                    userEntity.UserSignature = null;
                }

                var deletedEntities = await context.UserRoles.Where(m => m.UserId == userEntity.Id).ToListAsync();
                if (deletedEntities != null)
                {
                    context.UserRoles.RemoveRange(deletedEntities);
                }
                foreach (var groupId in editUser.GroupIds)
                {
                    context.UserRoles.Add(new AspNetUserRoles
                    {
                        RoleId = groupId,
                        UserId = userEntity.Id
                    });
                }
                await context.SaveChangesAsync();
                apiResult.OperationIsSuccess = true;
            }
            return apiResult;
        }
        public async Task<ApiResult> DeleteUser(string userId, string root)
        {
            var result = new ApiResult();
            using (var context = TransactionConfig.AppDbContext)
            {
                var userEntity = await context.Users.Where(m => m.Id == userId).FirstOrDefaultAsync();
                if (userEntity == null)
                {
                    result.OperationIsSuccess = false;
                    result.ErrorList.Add("User Not Found");
                    return result;
                }
                userEntity.IsDeleted = true;
                context.Users.Update(userEntity);
                await context.SaveChangesAsync();
                result.OperationIsSuccess = true;
            }
            return result;
        }
        public async Task<ApiResult> RejectUser(string userId, string root)
        {
            var result = new ApiResult();
            using (var context = TransactionConfig.AppDbContext)
            {
                var userEntity = await context.Users.Where(m => m.Id == userId).FirstOrDefaultAsync();
                if (userEntity == null)
                {
                    result.OperationIsSuccess = false;
                    result.ErrorList.Add("User Not Found");
                    return result;
                }
                if (userEntity.UserImage != null && userEntity.UserImage.Length > 3)
                    DeleteFile(root, "appfiles", userEntity.UserImage);
                context.Users.Remove(userEntity);
                await context.SaveChangesAsync();
                result.OperationIsSuccess = true;
                _ = Task.Run(() => { SendRejectUserEmailAsync(userEntity, root); });
            }
            return result;
        }
        public async Task<string> GetBuyerNameByUserId(string userId)
        {
            using (var context = TransactionConfig.AppDbContext)
            {
                var entityUserBuyerName = await context.Users.Where(m => m.Id == userId).Select(m => m.BuyerUserName).FirstOrDefaultAsync();
                if (entityUserBuyerName == null)
                {
                    return null;
                }
                else
                {
                    return entityUserBuyerName;
                }
            }
        }

    }
}

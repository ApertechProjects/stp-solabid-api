using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using SolaBid.Business.Dtos.EntityDtos.CarNumber;
using SolaBid.Business.Logics.CommonLogic;
using SolaBid.Business.Models;
using SolaBid.Business.Models.Enum;
using SolaBid.Extensions;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using SolaBid.Domain.Models;
using static SolaBid.Business.Logics.CommonLogic.GetData;

namespace SolaBid.Business.Logics
{
    public class CarNumberLogic
    {

        public ApiResult MainAllAndWFA(string userId)
        {
            var res = new ApiResult(true)
            {
                Data = new MainAllAndWFADto
                {
                    All = FromQuery($"EXEC SP_CarNumberMainAll", isSiteLineDb: false).ConvertToClassListModel<MainAllDto>(),
                    WaitingForApprove = FromQuery($"EXEC SP_CarNumberMainWFA '{userId}'", isSiteLineDb: false).ConvertToClassListModel<MainWaitingForApproveDto>()
                }
            };
            return res;
        }

        public ApiResult Vendors()
        {
            var res = new GenericMapLogic<CarNumberVendorListDto>().BuildModel(FromQuery($"EXEC SP_CarNumberVendorList", isSiteLineDb: false), isList: true);
            return res;
        }

        public ApiResult OrderList(string vendorCode)
        {
            var res = new GenericMapLogic<OrderListDto>()
                  .BuildModel(FromQuery($"EXEC SP_CarNumberOrderList '{vendorCode}'", isSiteLineDb: false), isList: true);

            //if (!((List<OrderListDto>)res.Data).Any())
            //{
            //    ((List<OrderListDto>)res.Data).Add(new OrderListDto { PoNumber = "0000000" });
            //}
            return res;
        }

        public ApiResult Attachments(int mainId)
        {
            var res = new ApiResult(true);
            var donedAttachList = new List<AttachmentLoadDto>();
            var attachments = FromQuery($"EXEC SP_CarNumberAttachments {mainId}", isSiteLineDb: false).ConvertToClassListModel<CarNumberAttachmentLoadDto>();
            foreach (var attach in attachments)
            {
                donedAttachList.Add(new AttachmentLoadDto { AttachmentId = attach.CarNumberAttachmentId, FileBase64 = attach.FileBaseType + Convert.ToBase64String(attach.File, 0, attach.File.Length), FileName = attach.FileName });
            }
            res.Data = donedAttachList;
            return res;
        }

        public ApiResult Detail(int mainId)
        {
            var res = new GenericMapLogic<DetailDto>().BuildModel(FromQuery($"EXEC SP_CarNumberDetails {mainId}", isSiteLineDb: false), isList: true);
            //if (!((List<DetailDto>)res.Data).Any())
            //{
            //    ((List<DetailDto>)res.Data).Add(new DetailDto { CarNumberDetailId = 0 });
            //}
            return res;
        }

        public ApiResult Approvals(int mainId)
        {
            var res = new GenericMapLogic<ApprovalDto>()
                    .BuildModel(FromQuery($"EXEC SP_CarNumberApprovals {mainId}", isSiteLineDb: false), isList: true);
            //if (!((List<ApprovalDto>)res.Data).Any())
            //{
            //    ((List<ApprovalDto>)res.Data).Add(new ApprovalDto { Stage = 0 });
            //}
            return res;
        }

        public ApiResult Save(MainItemsSaveDto mainInfos, string userId, string root)
        {
            var apiRes = new ApiResult(true);
            if (mainInfos.NewItems.Any())
            {
                if (!mainInfos.NewItems.All(m => m.OrdersList.Count > 0))
                {
                    apiRes.OperationIsSuccess = false;
                    apiRes.IsError = true;
                    apiRes.ErrorList.Add("Order Number is Required");
                    return apiRes;
                }
                foreach (var item in mainInfos.NewItems)
                {

                    int entryValue = item.Entry ? 1 : 0;
                    int exitValue = item.Exit ? 1 : 0;
                    var newCarMainId = (int)(FromQuery
                        ($"EXEC SP_CarNumberMain_IUD " +
                        $"0," +
                        $"'{item.VendorCode}'," +
                        $"'{item.CarNumber}'," +
                        $"'{item.EQaimeNo}'," +
                        $"{item.Status}," +
                        $"'{item.Comment}'," +
                        $"'{item.DriverName}'," +
                        $"{entryValue}," +
                        $"{exitValue}," +
                        $"0"
                        , false).Rows[0][0]);

                    foreach (var po in item.OrdersList)
                    {
                        _ = FromQuery
                        ($"EXEC SP_CarNumberDetails_IUD " +
                        $"0," +
                        $"{newCarMainId}," +
                        $"'{po}'"
                        , false);
                    }

                    if (item.NewAttachments is not null && item.NewAttachments.Any())
                    {

                        using (var conn = TransactionConfig.AppDbContextManualConnection)
                        {
                            conn.Open();
                            SqlCommand cmd = new SqlCommand("SP_CarNumberAttachments_IUD", conn);
                            foreach (var attach in item.NewAttachments)
                            {
                                cmd.CommandType = CommandType.StoredProcedure;
                                cmd.Parameters.AddWithValue("@CarNumberAttachmentId", 0);
                                cmd.Parameters.AddWithValue("@CarNumberMainId", newCarMainId);
                                cmd.Parameters.AddWithValue("@FileName", attach.FileName);
                                cmd.Parameters.AddWithValue("@File", attach.FileBase64.ConvertBase64ToByte());
                                cmd.Parameters.AddWithValue("@FileBaseType", attach.FileBase64.GetFileBaseType());
                                cmd.ExecuteNonQuery();
                                cmd.Parameters.Clear();
                            }
                        }
                    }

                    if (item.Status == 1 && item.Stage == 0 || mainInfos.IsReject)
                    {
                        _ = Task.Run(() =>
                          {
                              SendCarNumberEmail(item.CarNumber, newCarMainId, !GetStageIsChanged(item.VendorClass, item.VendorExclude), userId, root, false,mainInfos.IsReject);
                          });
                    }
                }
            }

            if (mainInfos.EditedItems.Any())
            {
                var stage = mainInfos.EditedItems.First().Stage;
                if (stage == 1 && mainInfos.IsApprove)
                {
                    var validData = mainInfos.EditedItems.All(m => !string.IsNullOrEmpty(m.EQaimeNo));
                    if (!validData)
                    {
                        apiRes.OperationIsSuccess = false;
                        apiRes.IsError = true;
                        apiRes.ErrorList.Add("Eqaime is Required");
                        return apiRes;
                    }
                }

                if (stage == 2 && mainInfos.IsApprove)
                {
                    var validData = mainInfos.EditedItems.All(m => !string.IsNullOrEmpty(m.CarNumber));
                    if (!validData)
                    {
                        apiRes.OperationIsSuccess = false;
                        apiRes.IsError = true;
                        apiRes.ErrorList.Add("Car number is Required");
                        return apiRes;
                    }
                }

                foreach (var item in mainInfos.EditedItems)
                {
                    int entryValue = item.Entry ? 1 : 0;
                    int exitValue = item.Exit ? 1 : 0;
                    _ = FromQuery
                        ($"EXEC SP_CarNumberMain_IUD " +
                        $"{item.CarNumberMainId}," +
                        $"'{item.VendorCode}'," +
                        $"'{item.CarNumber}'," +
                        $"'{item.EQaimeNo}'," +
                        $"{item.Status}," +
                        $"'{item.Comment}'," +
                        $"'{item.DriverName}'," +
                        $"{entryValue}," +
                        $"{exitValue}," +
                        $"0"
                        , false);

                    if (item.OrdersList.Any())
                    {
                        _ = FromQuery
                               ($"EXEC SP_CarNumberDetails_IUD " +
                               $"0," +
                               $"{item.CarNumberMainId}"
                               , false);

                        foreach (var po in item.OrdersList)
                        {
                            _ = FromQuery
                               ($"EXEC SP_CarNumberDetails_IUD " +
                               $"0," +
                               $"{item.CarNumberMainId}," +
                               $"'{po}'"
                               , false);
                        }
                    }

                    if (item.NewAttachments is not null && item.NewAttachments.Any())
                    {
                        using (var conn = TransactionConfig.AppDbContextManualConnection)
                        {
                            conn.Open();
                            foreach (var attach in item.NewAttachments)
                            {
                                SqlCommand cmd = new SqlCommand("SP_CarNumberAttachments_IUD", conn);
                                cmd.CommandType = CommandType.StoredProcedure;
                                cmd.Parameters.AddWithValue("@CarNumberAttachmentId", 0);
                                cmd.Parameters.AddWithValue("@CarNumberMainId", item.CarNumberMainId);
                                cmd.Parameters.AddWithValue("@FileName", attach.FileName);
                                cmd.Parameters.AddWithValue("@File", attach.FileBase64.ConvertBase64ToByte());
                                cmd.Parameters.AddWithValue("@FileBaseType", attach.FileBase64.GetFileBaseType());
                                cmd.ExecuteNonQuery();
                                cmd.Parameters.Clear();
                            }
                        }
                    }
                    if (mainInfos.IsApprove || mainInfos.IsHold || mainInfos.IsReject)
                    {
                        var approvalStatus = mainInfos.IsApprove ? 1 : mainInfos.IsHold ? 2 : 99;
                        _ = FromQuery($"EXEC SP_CarNumberApprove {item.CarNumberMainId},{approvalStatus},{stage},'{userId}'", isSiteLineDb: false);
                    }
                    item.Stage = GetStageIsChanged(item.VendorClass, item.VendorExclude) ? 2 : item.Stage;
                    if (item.Status == 1 && mainInfos.IsApprove || item.Stage == 0 && item.Status == 1|| mainInfos.IsReject)
                    {
                        _ = Task.Run(() =>
                        {
                            SendCarNumberEmail(item.CarNumber, item.CarNumberMainId, item.Stage == 0 ? true : false, userId, root, item.Stage == 2 ? true : false,mainInfos.IsReject);
                        });
                    }
                }
            }

            if (mainInfos.DeletedMainIds.Any())
            {
                foreach (var item in mainInfos.DeletedMainIds)
                {
                    _ = FromQuery($"EXEC SP_CarNumberMain_IUD {item}", false);
                }
            }

            if (mainInfos.DeletedAttachmentIds.Any())
            {
                foreach (var attach in mainInfos.DeletedAttachmentIds)
                {
                    _ = FromQuery($"EXEC SP_CarNumberAttachments_IUD {attach}", false);
                }
            }
            return apiRes;

            bool GetStageIsChanged(string vendorClass, bool vendorExclude)
            {
                if (vendorClass.ToLower() == "local" && !vendorExclude)
                    return false;
                return true;

            };
        }

        private async void SendCarNumberEmail(string carNumber, int carNumberId, bool isApprover1, string userId, string root, bool isApproved, bool isRejected)
        {
            var dataBase = Statics.IsProductionEnvironment ? Statics.API_SOCAR_BASE : Statics.API_BASE;
            MailAddress address = new MailAddress("socarstpinfo@apertech.net");
            MailMessage mail = new MailMessage();
            mail.From = new MailAddress("socarstpinfo@apertech.net", $"Car Number {carNumber}");
            List<string> userEmails;
            using (var context = TransactionConfig.AppDbContext)
            {
                IQueryable<string> approverGroups;

                if (isApproved || isRejected)
                {
                    approverGroups = context.GroupAdditionalPrivileges.Where(m => m.AdditionalPrivilegeId == (byte)AdditionalPrivilegeses.CarApprover1 || m.AdditionalPrivilegeId == (byte)AdditionalPrivilegeses.CarSendToApprove).Select(m => m.AppRoleId).Distinct();
                }
                else
                {
                    approverGroups = context.GroupAdditionalPrivileges.Where(m => m.AdditionalPrivilegeId == (isApprover1 ? (byte)AdditionalPrivilegeses.CarApprover1 : (byte)AdditionalPrivilegeses.CarApprover2)).Select(m => m.AppRoleId);
                }
                var userIds = context.UserRoles.Where(m => approverGroups.Contains(m.RoleId)).Select(m => m.UserId).Distinct();
                userEmails = await context.Users.Where(m => userIds.Contains(m.Id)).Select(m => m.Email).ToListAsync();
            }
            if (isApproved)
            {
                userEmails.AddRange(new List<string> { "v.v.tagizada@socar-stp.az", "k.s.aslanli@socar-stp.az" });
            }

            foreach (var userEmail in userEmails)
            {
                mail.To.Add(userEmail);
            }
            mail.Subject = carNumber == null ? mail.Subject = $"Car Register Number {carNumberId}" : $"Car Number {carNumber}";

            mail.IsBodyHtml = true;
            string body = string.Empty;
            using (StreamReader reader = new StreamReader(Path.Combine(root, "templates", isApproved ? "CarNumberApproved.html" : isRejected ? "CarNumberRejected.html" : "CarNumberWaitingApprove.html")))
            { body = reader.ReadToEnd(); }
            body = body.Replace("{carnumberurl}", $"{dataBase}truck/carnumber");
            body = body.Replace("{carnumber}", carNumber);
            body = body.Replace("{carnumberid}", carNumberId.ToString());
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

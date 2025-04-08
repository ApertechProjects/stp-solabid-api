using ClosedXML.Excel;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using SolaBid.Business.ConnectableEntityExtensions;
using SolaBid.Business.Dtos.ApiDtos;
using SolaBid.Business.Dtos.ApiDtos.Register;
using SolaBid.Business.Dtos.EntityDtos;
using SolaBid.Business.Dtos.SingleObjs;
using SolaBid.Business.Models;
using SolaBid.Business.Models.Enum;
using SolaBid.Domain.Models.Entities;
using SolaBid.Extensions;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace SolaBid.Business.Logics
{
    public class RegisterLogic
    {
        public async Task<RegisterViewDto> GetRegister(string siteId, DateTime dateFrom, DateTime dateTo)
        {
            var site = await new SiteLogic().GetSite(siteId);
            var result = new RegisterViewDto();
            DataTable dailyRegisterDt = new();
            DataTable dailyRegisterSourcingDt = new();
            try
            {
                using (var conn = TransactionConfig.AppDbContextManualConnection)
                {
                    conn.Open();
                    string queryDailyRegister = $"EXEC dbo.SP_DailyRegister '{dateFrom.Date.ToString("yyyy.MM.dd")}','{dateTo.Date.ToString("yyyy.MM.dd")}'";
                    SqlCommand queryDailyRegisterCommand = new(queryDailyRegister, conn);
                    queryDailyRegisterCommand.CommandTimeout = 10000;
                    dailyRegisterDt.Load(await queryDailyRegisterCommand.ExecuteReaderAsync());


                    string queryDailyRegisterSourcing = "EXEC dbo.SP_DailyRegisterSourcing";
                    SqlCommand queryDailyRegisterSourcingCommand = new(queryDailyRegisterSourcing, conn);
                    dailyRegisterSourcingDt.Load(await queryDailyRegisterSourcingCommand.ExecuteReaderAsync());
                }
                result.DailyRegister = Utility.ConvertDataTable<PurchaseRegisterDto>(dailyRegisterDt);
                result.DailyRegisterSourcing = Utility.ConvertDataTable<SourceRegisterDto>(dailyRegisterSourcingDt);
                await FillBottomRegisterLists(result, null, site);

                var purchasingRegistersAsAll = TransactionConfig.Mapper.Map<List<AllRegisterDto>>(result.DailyRegister);
                var sourcingRegistersAsAll = TransactionConfig.Mapper.Map<List<AllRegisterDto>>(result.DailyRegisterSourcing);
                sourcingRegistersAsAll.ForEach(m =>
                {
                    m.Buyer = result.DailyRegisterSourcing[0].Buyers.Where(x => x.Value == m.Buyer).FirstOrDefault()?.Text;
                    m.Company = result.DailyRegisterSourcing[0].Companies.Where(x => x.Value == m.Company).FirstOrDefault()?.Text;
                    m.Currency = result.DailyRegisterSourcing[0].Currencies.Where(x => x.Value == m.Currency).FirstOrDefault()?.Text;
                    //m.Requester = result.DailyRegisterSourcing[0].Requesters.Where(x => x.Value == m.Requester).FirstOrDefault()?.Text;
                    m.Status = result.DailyRegisterSourcing[0].Statuses.Where(x => x.Value == m.Status).FirstOrDefault()?.Text;
                });
                result.All = purchasingRegistersAsAll.Concat(sourcingRegistersAsAll).ToList();
                int seqDailyRegister = result.DailyRegister.Count;
                for (int i = 0; i < result.DailyRegister.Count; i++)
                {
                    result.DailyRegister[i].Sequence = seqDailyRegister;
                    seqDailyRegister--;
                }
                int seqDailyRegisterSourcing = result.DailyRegisterSourcing.Count;
                for (int i = 0; i < result.DailyRegisterSourcing.Count; i++)
                {
                    result.DailyRegisterSourcing[i].Sequence = seqDailyRegisterSourcing;
                    seqDailyRegisterSourcing--;
                }
                int seqAll = result.All.Count;
                for (int i = 0; i < result.All.Count; i++)
                {
                    result.All[i].Sequence = seqAll;
                    seqAll--;
                }
                
                return result;
            }
            catch (Exception ex)
            {
                await ex.ErrorLog();
                result.DailyRegister = new();
                result.DailyRegisterSourcing = new();
                result.All = new();
                return result;
            }

        }

        public ApiResult ChangeFolderName(ChangeFolderNameModel model)
        {
            var res = new ApiResult();
            if (string.IsNullOrEmpty(model.oldName) || string.IsNullOrEmpty(model.newName))
            {
                res.ErrorList.Add("All fields required");
                return res;
            }
            using (var con = TransactionConfig.AppDbContextManualConnection)
            {
                using (SqlCommand cmd = new SqlCommand("SP_UodateDailyAttachmentVirtualFolderName", con))
                {
                    con.Open();
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@oldName", model.oldName);
                    cmd.Parameters.AddWithValue("@newName", model.newName);
                    cmd.Parameters.AddWithValue("@RequestNumber", CheckAndConvertData(model.RequestNumber));
                    cmd.Parameters.AddWithValue("@ComparisonNumber", CheckAndConvertData(model.ComparisonNumber));
                    cmd.Parameters.AddWithValue("@OrderNumber", CheckAndConvertData(model.OrderNumber));
                    cmd.ExecuteNonQuery();
                    cmd.Parameters.Clear();
                }
            }
            res.OperationIsSuccess = true;
            return res;
        }

        public async Task<string> GetRegisterFile(int dailyRegisterAttachmentId, string fileName)
        {
            BIDAttachment savedBidData;
            DataTable savedRegisterData = new DataTable();

            using (var context = TransactionConfig.AppDbContext)
            {
                savedBidData = await context.BIDAttachments.Where(m => m.Id == dailyRegisterAttachmentId && m.FileName == fileName).FirstOrDefaultAsync();
            }

            if (savedBidData == null)
            {
                using (var conn = TransactionConfig.AppDbContextManualConnection)
                {
                    var query = $"SELECT[b].[FileBaseType], [b].[FilePath] FROM[DailyRegisterAttachments]  AS[b] WHERE([b].[DailyRegisterAttachmentId] = {dailyRegisterAttachmentId}) AND([b].[FileName] = N'{fileName}')";
                    conn.Open();
                    SqlCommand cmd = new SqlCommand(query, conn);
                    savedRegisterData.Load(cmd.ExecuteReader());
                    conn.Close();
                }
            }
            if (savedBidData != null)
            {
                return FileExtensions.ConvertFileToBase64(savedBidData.FilePath, "appFiles", savedBidData.FileBaseType);
            }
            if (savedRegisterData.Rows.Count > 0)
            {
                return FileExtensions.ConvertFileToBase64(savedRegisterData.Rows[0]["FilePath"].ToString(), "appFiles", savedRegisterData.Rows[0]["FileBaseType"].ToString());
            }

            return "File Not Found";
        }

        public async Task<ApiResult> SaveAttachments(AttachmentCreateAndDeleteModel attachmentCreateAndDeleteModel, string root)
        {
            var res = new ApiResult();
            try
            {
                if (attachmentCreateAndDeleteModel.UploadedFiles.Any())
                {
                    using (var con = TransactionConfig.AppDbContextManualConnection)
                    {
                        using (SqlCommand cmd = new SqlCommand("DailyRegisterAttachments_IUD", con))
                        {
                            con.Open();
                            foreach (var uploadedFile in attachmentCreateAndDeleteModel.UploadedFiles)
                            {
                                foreach (var attach in uploadedFile.Files)
                                {
                                    var filePath = await attach.FileBase64.SaveFileBase64(attach.FileName, "registerFiles", root);
                                    var fileBaseType = attach.FileBase64.GetFileBaseType();
                                    cmd.CommandType = CommandType.StoredProcedure;
                                    cmd.Parameters.AddWithValue("@DailyRegisterAttachmentId", attach.DailyRegisterAttachmentId);
                                    cmd.Parameters.AddWithValue("@DailyRegisterId", CheckAndConvertData(attach.DailyRegisterId.ToString()));
                                    cmd.Parameters.AddWithValue("@RequestNumber", CheckAndConvertData(attach.RequestNumber));
                                    cmd.Parameters.AddWithValue("@ComparisonNumber", CheckAndConvertData(attach.ComparisonNumber));
                                    cmd.Parameters.AddWithValue("@OrderNumber", CheckAndConvertData(attach.OrderNumber));
                                    cmd.Parameters.AddWithValue("@VirtualFolderName", string.IsNullOrEmpty(uploadedFile.FolderName) ? "Files" : uploadedFile.FolderName);
                                    cmd.Parameters.AddWithValue("@FileBaseType", fileBaseType);
                                    cmd.Parameters.AddWithValue("@FileName", attach.FileName);
                                    cmd.Parameters.AddWithValue("@FilePath", filePath);
                                    cmd.ExecuteNonQuery();
                                    cmd.Parameters.Clear();
                                }
                            }
                        }
                    }
                }

                if (attachmentCreateAndDeleteModel.DeletedFolders.Any())
                {
                    using (var conn = TransactionConfig.AppDbContextManualConnection)
                    {
                        using (SqlCommand cmd = new SqlCommand("SP_VirtualFolderAttachments", conn))
                        {
                            conn.Open();

                            var deletedAttachDT = new DataTable();

                            foreach (var folder in attachmentCreateAndDeleteModel.DeletedFolders)
                            {
                                if (!string.IsNullOrEmpty(folder))
                                {
                                    cmd.CommandType = CommandType.StoredProcedure;
                                    cmd.Parameters.AddWithValue("@VirtualFolderName", folder);
                                    cmd.Parameters.AddWithValue("@RequestNumber", CheckAndConvertData(attachmentCreateAndDeleteModel.RequestNumber));
                                    cmd.Parameters.AddWithValue("@ComparisonNumber", CheckAndConvertData(attachmentCreateAndDeleteModel.ComparisonNumber));
                                    cmd.Parameters.AddWithValue("@OrderNumber", CheckAndConvertData(attachmentCreateAndDeleteModel.OrderNumber));
                                    deletedAttachDT.Load(cmd.ExecuteReader());
                                    cmd.Parameters.Clear();
                                    var resultModel = Utility.ConvertDataTable<RegisterAttachmentDto>(deletedAttachDT);
                                    foreach (var deleteAttachment in resultModel)
                                    {
                                        attachmentCreateAndDeleteModel.DeletedFiles.Add(new RegisterAttachmentCreateDto
                                        {
                                            FileName = deleteAttachment.FileName,
                                            DailyRegisterAttachmentId = deleteAttachment.DailyRegisterAttachmentId,
                                        });
                                    }
                                }
                            }

                        }
                    }
                }
                if (attachmentCreateAndDeleteModel.DeletedFiles.Any())
                {
                    using (var con = TransactionConfig.AppDbContextManualConnection)
                    {
                        using (SqlCommand cmd = new SqlCommand("DailyRegisterAttachments_IUD", con))
                        {
                            con.Open();
                            foreach (var attach in attachmentCreateAndDeleteModel.DeletedFiles)
                            {
                                FileExtensions.DeleteFile(root, "appFiles", attach.FilePath);
                                cmd.CommandType = CommandType.StoredProcedure;
                                cmd.Parameters.AddWithValue("@DailyRegisterAttachmentId", attach.DailyRegisterAttachmentId);
                                cmd.Parameters.AddWithValue("@VirtualFolderName", "");
                                cmd.ExecuteNonQuery();
                                cmd.Parameters.Clear();
                            }
                        }
                    }
                }
                res.OperationIsSuccess = true;
            }
            catch (Exception ex)
            {
                await ex.ErrorLog();
                res.ErrorList.Add("Operation Failed.");
            }
            return res;
        }

        public async Task<string> Export(int tabIndex, string siteId)
        {
            DataTable data = new();
            List<string> excludedColumns = new List<string> { "RFQSuppliers", "RFQSentDate", "PriseInUSD", "SourcingKPI", "ProcurementKPI", "IsChecked" };
            if (tabIndex == (byte)RegisterTypes.Purchase)
            {
                using (var conn = TransactionConfig.AppDbContextManualConnection)
                {
                    conn.Open();
                    string queryDailyRegister = "EXEC dbo.SP_DailyRegister";
                    SqlCommand queryDailyRegisterCommand = new(queryDailyRegister, conn);
                    data.Load(queryDailyRegisterCommand.ExecuteReader());
                }
            }
            else if (tabIndex == (byte)RegisterTypes.Source)
            {
                data = (await GetSourcingRegisters(siteId, true)).DailyRegisterSourcing.ToDataTable();
            }
            else if (tabIndex == (byte)RegisterTypes.All)
            {
                data = (await GetAllRegisters(siteId)).ToDataTable();
            }

            foreach (DataColumn column in data.Columns)
            {
                string colname = column.ColumnName;
                if (!excludedColumns.Contains(colname))
                {
                    data.Columns[colname].ColumnName = string.Concat(colname.Select(x => Char.IsUpper(x) ? " " + x : x.ToString())).TrimStart(' ');
                }
            }

            for (int rowIndex = 0; rowIndex < data.Rows.Count; rowIndex++)
            {
                data.Rows[rowIndex]["RFQSuppliers"] = data.Rows[rowIndex]["RFQSuppliers"].ToString().TrimStart(';');
            }


            XLWorkbook wb = new XLWorkbook();
            var registerWS = wb.Worksheets.Add(data, "RegisterExport");
            registerWS.Row(1).Style.Alignment.WrapText = true;
            registerWS.Columns().AdjustToContents();

            SetColWidth(data, registerWS, "Buyer", 14);
            SetColWidth(data, registerWS, "Request Number", 11);
            SetColWidth(data, registerWS, "Request Type", 10.5);
            SetColWidth(data, registerWS, "Requester", 17);
            SetColWidth(data, registerWS, "Status", 12.8);
            SetColWidth(data, registerWS, "Company", 10.5);
            SetColWidth(data, registerWS, "Requested For", 14.5);
            SetColWidth(data, registerWS, "Short Description", 11);
            SetColWidth(data, registerWS, "RFQSuppliers", 30);
            SetColWidth(data, registerWS, "Winner", 30);
            SetColWidth(data, registerWS, "Comparison Number", 13);
            SetColWidth(data, registerWS, "Currency", 7.9);
            SetColWidth(data, registerWS, "SourcingKPI", 8.3);
            SetColWidth(data, registerWS, "ProcurementKPI", 8.3);
            SetColWidth(data, registerWS, "PriseInUSD", 7);
            SetColWidth(data, registerWS, "Order First Approval", 10);
            SetColWidth(data, registerWS, "Order Second Approval", 10);
            SetColWidth(data, registerWS, "Due Date", 10);
            SetColWidth(data, registerWS, "Date Of Request", 10);
            SetColWidth(data, registerWS, "Received Date", 10);
            SetColWidth(data, registerWS, "RFQSentDate", 10);
            SetColWidth(data, registerWS, "Sourcing Closing Date", 10);
            SetColWidth(data, registerWS, "Comparison Date", 10);

            switch (tabIndex)
            {
                case (byte)RegisterTypes.All:
                    registerWS.Column(data.Columns["Id"].Ordinal + 1).Delete();
                    registerWS.Column(data.Columns["IsChecked"].Ordinal).Delete();
                    break;
                case (byte)RegisterTypes.Purchase:
                    break;
                case (byte)RegisterTypes.Source:
                    registerWS.Column("AH").Delete();
                    registerWS.Column("AG").Delete();
                    registerWS.Column("AF").Delete();
                    registerWS.Column(data.Columns["Requesters"].Ordinal + 1).Delete();
                    registerWS.Column(data.Columns["Companies"].Ordinal + 1).Delete();
                    registerWS.Column(data.Columns["Statuses"].Ordinal + 1).Delete();
                    registerWS.Column(data.Columns["Buyers"].Ordinal + 1).Delete();
                    registerWS.Column(data.Columns["Currencies"].Ordinal + 1).Delete();
                    registerWS.Column(data.Columns["IsChecked"].Ordinal + 1).Delete();
                    registerWS.Column(tabIndex == (byte)RegisterTypes.Source ? data.Columns["Daily Register Id"].Ordinal + 1 : 0).Delete();
                    registerWS.Column(data.Columns["Id"].Ordinal + 1).Delete();
                    break;
            }
            //wb.SaveAs(@"C:\Users\Afiq Mehdizade\Desktop\New folder\" + DateTime.Now.ToString("HH-mm-ss") + Guid.NewGuid().ToString() + ".xlsx");
            //return null;

            static void SetColWidth(DataTable data, IXLWorksheet registerWS, string colName, double width)
            {
                registerWS.Column(data.Columns[colName].Ordinal + 1).Width = width;
            }
            using (var ms = new MemoryStream())
            {
                wb.SaveAs(ms);
                return "data:application/vnd.openxmlformats-officedocument.spreadsheetml.sheet;base64," + Convert.ToBase64String(ms.ToArray());
            }
        }

        public async Task<ApiResult> Delete(IntegerSingleId id)
        {
            var apiResult = new ApiResult();
            try
            {
                using (var con = TransactionConfig.AppDbContextManualConnection)
                {
                    using (SqlCommand cmd = new SqlCommand("DailyRegister_IUD", con))
                    {
                        con.Open();
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@DailyRegisterId", id.Id);
                        cmd.ExecuteNonQuery();
                    }
                }
                apiResult.OperationIsSuccess = true;
            }
            catch (Exception ex)
            {
                await ex.ErrorLog();
                apiResult.ErrorList.Add("Operation Failed.Please,Connect Administration");
            }
            return apiResult;
        }

        public async Task<List<AllRegisterDto>> GetAllRegisters(string siteId)
        {
            var purchasingRegisters = await GetPurchasingRegisters();
            var sourcingRegisters = (await GetSourcingRegisters(siteId)).DailyRegisterSourcing;
            sourcingRegisters.ForEach(m =>
            {
                m.Buyer = m.Buyers.Where(x => x.Value == m.Buyer).FirstOrDefault()?.Text;
                m.Company = m.Companies.Where(x => x.Value == m.Company).FirstOrDefault()?.Text;
                m.Currency = m.Currencies.Where(x => x.Value == m.Currency).FirstOrDefault()?.Text;
                //m.Requester = m.Requesters.Where(x => x.Value == m.Requester).FirstOrDefault()?.Text;
                m.Status = m.Statuses.Where(x => x.Value == m.Status).FirstOrDefault()?.Text;
            });
            var purchasingRegistersAsAll = TransactionConfig.Mapper.Map<List<AllRegisterDto>>(purchasingRegisters);
            var sourcingRegistersAsAll = TransactionConfig.Mapper.Map<List<AllRegisterDto>>(sourcingRegisters);
            return purchasingRegistersAsAll.Concat(sourcingRegistersAsAll).ToList();
        }

        public async Task<SourcingRegisterViewDto> GetSourcingRegisters(string siteId, bool fillLists = false)
        {
            var site = await new SiteLogic().GetSite(siteId);
            var result = new SourcingRegisterViewDto();
            DataTable dailyRegisterSourcingDt = new();
            try
            {
                using (var conn = TransactionConfig.AppDbContextManualConnection)
                {
                    conn.Open();
                    string queryDailyRegisterSourcing = "EXEC dbo.SP_DailyRegisterSourcing";
                    SqlCommand queryDailyRegisterSourcingCommand = new(queryDailyRegisterSourcing, conn);
                    dailyRegisterSourcingDt.Load(queryDailyRegisterSourcingCommand.ExecuteReader());
                }
                result.DailyRegisterSourcing = Utility.ConvertDataTable<SourceRegisterDto>(dailyRegisterSourcingDt);
                await FillBottomRegisterLists(null, result, site);
                if (fillLists)
                {
                    result.DailyRegisterSourcing.ForEach(m =>
                    {
                        m.Buyer = m.Buyers.Where(x => x.Value == m.Buyer).FirstOrDefault()?.Text;
                        m.Company = m.Companies.Where(x => x.Value == m.Company).FirstOrDefault()?.Text;
                        m.Currency = m.Currencies.Where(x => x.Value == m.Currency).FirstOrDefault()?.Text;
                        //m.Requester = m.Requesters.Where(x => x.Value == m.Requester).FirstOrDefault()?.Text;
                        m.Status = m.Statuses.Where(x => x.Value == m.Status).FirstOrDefault()?.Text;
                    });
                }
                for (int i = 0; i < result.DailyRegisterSourcing.Count; i++)
                {
                    result.DailyRegisterSourcing[i].Sequence = i + 1;
                }

                return result;
            }
            catch (Exception ex)
            {
                await ex.ErrorLog();
                result.DailyRegisterSourcing = new();
                return result;
            }
        }

        public async Task<List<PurchaseRegisterDto>> GetPurchasingRegisters()
        {
            var result = new List<PurchaseRegisterDto>();
            DataTable dailyRegisterDt = new();
            try
            {
                using (var conn = TransactionConfig.AppDbContextManualConnection)
                {
                    conn.Open();
                    string queryDailyRegister = "EXEC dbo.SP_DailyRegister";
                    SqlCommand queryDailyRegisterCommand = new(queryDailyRegister, conn);
                    dailyRegisterDt.Load(queryDailyRegisterCommand.ExecuteReader());
                }

                for (int i = 0; i < result.Count; i++)
                {
                    result[i].Sequence = i + 1;
                }

                return Utility.ConvertDataTable<PurchaseRegisterDto>(dailyRegisterDt);

            }
            catch (Exception ex)
            {
                await ex.ErrorLog();
                return result;
            }
        }

        public async Task<ApiResult> CreateRegisterComparison(RequestComparisonCreateDto requestComparisonCreateDto)
        {
            var apiResult = new ApiResult();

            try
            {
                using (var con = TransactionConfig.AppDbContextManualConnection)
                {
                    using (SqlCommand cmd = new SqlCommand("SP_DailyRegisterAddComparison", con))
                    {
                        con.Open();
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@DailyRegisterId", requestComparisonCreateDto.DailyRegisterId);
                        cmd.Parameters.AddWithValue("@RequestNumber", requestComparisonCreateDto.RequestNumber);
                        cmd.Parameters.AddWithValue("@ComparisonNumber", requestComparisonCreateDto.ComparisonNumber);
                        cmd.ExecuteNonQuery();
                    }
                }
                apiResult.OperationIsSuccess = true;
            }
            catch (Exception ex)
            {
                await ex.ErrorLog();
                apiResult.ErrorList.Add("Operation Failed.Please,Connect Administration");
            }
            return apiResult;
        }

        public async Task<List<RequestComparisonViewDto>> GetRequestComparisons()
        {
            DataTable requestComparisonDt = new();
            try
            {
                using (var conn = TransactionConfig.AppDbContextManualConnection)
                {
                    conn.Open();
                    string query = "EXEC dbo.SP_DailyRegister_Comparisons";
                    SqlCommand cmd = new(query, conn);
                    requestComparisonDt.Load(cmd.ExecuteReader());
                }
                return Utility.ConvertDataTable<RequestComparisonViewDto>(requestComparisonDt);
            }
            catch (Exception ex)
            {
                await ex.ErrorLog();
                return new List<RequestComparisonViewDto>();
            }
        }

        public async Task<ApiResult> CreateTopRegister(List<PurchaseRegisterCreateDto> registerCreateDtos)
        {
            var apiResult = new ApiResult();
            if (!registerCreateDtos.Any())
                return null;
            try
            {
                using (var con = TransactionConfig.AppDbContextManualConnection)
                {
                    using (SqlCommand cmd = new SqlCommand("SP_DailyRegisterDescriptions_IU ", con))
                    {
                        con.Open();
                        foreach (var reg in registerCreateDtos)
                        {
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.Parameters.AddWithValue("@RequestNumber", CheckAndConvertData(reg.RequestNumber));
                            cmd.Parameters.AddWithValue("@ComparisonNumber", CheckAndConvertData(reg.ComparisonNumber));
                            cmd.Parameters.AddWithValue("@OrderNumber", CheckAndConvertData(reg.OrderNo));
                            cmd.Parameters.AddWithValue("@ShortDescription", CheckAndConvertData(reg.ShortDescription));
                            cmd.Parameters.AddWithValue("@RFQSentDate", CheckAndConvertData(reg.RFQSentDate, true));
                            cmd.Parameters.AddWithValue("@SourcingClosingDate", CheckAndConvertData(reg.SourcingClosingDate, true));
                            cmd.Parameters.AddWithValue("@RFQSuppliers", CheckAndConvertData(reg.ManualRFQSuppliers));
                            cmd.Parameters.AddWithValue("@DeliveryNoteNumber", CheckAndConvertData(reg.DeliveryNoteNumber));
                            cmd.ExecuteNonQuery();
                            cmd.Parameters.Clear();
                        }
                    }
                }
                apiResult.OperationIsSuccess = true;
            }
            catch (Exception ex)
            {
                await ex.ErrorLog();
                apiResult.ErrorList.Add("Operation Failed.Please,Connect Administration");
            }
            return apiResult;
        }

        public async Task<ApiResult> CreateBottomRegister(List<SourceRegisterCreateDto> registerCreateDtos)
        {
            var apiResult = new ApiResult();
            if (!registerCreateDtos.Any())
                return null;
            try
            {
                using (var con = TransactionConfig.AppDbContextManualConnection)
                {
                    using (SqlCommand cmd = new SqlCommand("DailyRegister_IUD", con))
                    {
                        con.Open();
                        foreach (var reg in registerCreateDtos)
                        {
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.Parameters.AddWithValue("@DailyRegisterId", reg.DailyRegisterId);
                            cmd.Parameters.AddWithValue("@Buyer", CheckAndConvertData(reg.Buyer));
                            cmd.Parameters.AddWithValue("@RequestNumber", CheckAndConvertData(reg.RequestNumber));
                            cmd.Parameters.AddWithValue("@RequestType", CheckAndConvertData(reg.RequestType));
                            cmd.Parameters.AddWithValue("@Requester", CheckAndConvertData(reg.Requester));
                            cmd.Parameters.AddWithValue("@Company", CheckAndConvertData(reg.Company));
                            cmd.Parameters.AddWithValue("@Status", CheckAndConvertData(reg.Status));
                            cmd.Parameters.AddWithValue("@RequestedFor", CheckAndConvertData(reg.RequestedFor));
                            cmd.Parameters.AddWithValue("@ShortDescription", CheckAndConvertData(reg.ShortDescription));
                            cmd.Parameters.AddWithValue("@RFQSuppliers", CheckAndConvertData(reg.RFQSuppliers));
                            cmd.Parameters.AddWithValue("@RFQSentDate", CheckAndConvertData(reg.RFQSentDate, true));
                            cmd.Parameters.AddWithValue("@DateOfRequest", CheckAndConvertData(reg.DateOfRequest, true));
                            cmd.Parameters.AddWithValue("@SourcingClosingDate", CheckAndConvertData(reg.SourcingClosingDate, true));
                            cmd.Parameters.AddWithValue("@ComparisonDate", CheckAndConvertData(reg.ComparisonDate, true));
                            cmd.Parameters.AddWithValue("@Winner", CheckAndConvertData(reg.Winner));
                            cmd.Parameters.AddWithValue("@Price", reg.Price?.Replace(" ", "").Replace(",", ""));
                            cmd.Parameters.AddWithValue("@Currency", CheckAndConvertData(reg.Currency));
                            cmd.Parameters.AddWithValue("@PriseInUSD", reg.PriseInUSD?.Replace(" ", "").Replace(",", ""));
                            cmd.Parameters.AddWithValue("@OrderNo", CheckAndConvertData(reg.OrderNo));
                            cmd.Parameters.AddWithValue("@DeliveryNoteNumber", CheckAndConvertData(reg.DeliveryNoteNumber));
                            cmd.Parameters.AddWithValue("@OrderFirstApproval", CheckAndConvertData(reg.OrderFirstApproval, true));
                            cmd.Parameters.AddWithValue("@OrderSecondApproval", CheckAndConvertData(reg.OrderSecondApproval, true));
                            cmd.ExecuteNonQuery();
                            cmd.Parameters.Clear();
                        }
                    }
                }
                apiResult.OperationIsSuccess = true;
            }
            catch (Exception ex)
            {
                await ex.ErrorLog();
                apiResult.ErrorList.Add("Operation Failed.Please,Connect Administration");
            }
            return apiResult;

        }

        //Register ATtachmet MOdel clasindan istifade olunacaq burdaki deyisiklikde
        public async Task<List<RegisterAttachmentModel>> GetPurchaseAttachments(PurchaseAttachmentGetModelDto value)
        {
            var result = new List<RegisterAttachmentModel>();
            var purchaseAttachmentDT = new DataTable();
            try
            {
                using (var conn = TransactionConfig.AppDbContextManualConnection)
                {
                    using (SqlCommand cmd = new SqlCommand("SP_DailyRegisterAttachments", conn))
                    {
                        conn.Open();
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@RequestNumber", CheckAndConvertData(value.RequestNumber));
                        cmd.Parameters.AddWithValue("@ComparisonNumber", CheckAndConvertData(value.ComparisonNumber));
                        cmd.Parameters.AddWithValue("@OrderNumber", CheckAndConvertData(value.OrderNumber));
                        purchaseAttachmentDT.Load(cmd.ExecuteReader());
                    }
                }
                var resultModel = Utility.ConvertDataTable<RegisterAttachmentDto>(purchaseAttachmentDT);
                if (resultModel.Any())
                {
                    var unNamedAttachments = resultModel.Where(m => m.FolderName == null || m.FolderName == "" || m.FolderName == " ").ToList();
                    foreach (var item in unNamedAttachments)
                    {
                        item.FolderName = "Files";
                    }
                    var folderList = resultModel.CustomDistinctBy(m => m.FolderName).Select(m => m.FolderName).ToList();
                    if (!folderList.Any(m => m == "Files"))
                    {
                        folderList.Add("Files");
                    }
                    foreach (var folder in folderList)
                    {
                        result.Add(new RegisterAttachmentModel
                        {
                            FolderName = folder,
                            Files = resultModel.Where(m => m.FolderName == folder).ToList()
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                await ex.ErrorLog();
            }
            return result;
        }

        public async Task<List<RegisterAttachmentModel>> GetSourceAttachments(int dailyRegisterId)
        {
            var result = new List<RegisterAttachmentModel>();
            var sourceAttachmentDT = new DataTable();
            try
            {
                using (var conn = TransactionConfig.AppDbContextManualConnection)
                {
                    conn.Open();
                    string queryDailyRegisterSourcing = $"EXEC dbo.SP_DailyRegisterSourcingAttachments {dailyRegisterId}";
                    SqlCommand queryDailyRegisterSourcingCommand = new(queryDailyRegisterSourcing, conn);
                    sourceAttachmentDT.Load(queryDailyRegisterSourcingCommand.ExecuteReader());
                }
                var resultModel = Utility.ConvertDataTable<RegisterAttachmentDto>(sourceAttachmentDT);
                if (resultModel.Any())
                {
                    var unNamedAttachments = resultModel.Where(m => m.FolderName == null || m.FolderName == "" || m.FolderName == " ").ToList();
                    foreach (var item in unNamedAttachments)
                    {
                        item.FolderName = "Files";
                    }
                    var folderList = resultModel.CustomDistinctBy(m => m.FolderName).Select(m => m.FolderName).ToList();
                    if (!folderList.Any(m => m == "Files"))
                    {
                        folderList.Add("Files");
                    }
                    foreach (var folder in folderList)
                    {
                        result.Add(new RegisterAttachmentModel
                        {
                            FolderName = folder,
                            Files = resultModel.Where(m => m.FolderName == folder).ToList()
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                await ex.ErrorLog();
            }
            return result;
        }

        //Local Methods
        private async Task FillBottomRegisterLists(RegisterViewDto registerView, SourcingRegisterViewDto sourcingRegisterViewDto, SiteDto site)
        {
            var bottomRegisterListDatas = await GetBottomRegisterListDatas(site);

            if (registerView != null)
            {
                registerView.DailyRegisterSourcing.ForEach(m =>
                {
                    m.Buyers = bottomRegisterListDatas.Buyers;
                    m.Companies = bottomRegisterListDatas.Companies;
                    m.Currencies = bottomRegisterListDatas.Currencies;
                    m.Statuses = bottomRegisterListDatas.Statuses;
                    //m.Requesters = bottomRegisterListDatas.Requesters;
                });
                registerView.EmptyRow = new SourceRegisterDto
                {
                    Buyers = bottomRegisterListDatas.Buyers,
                    Companies = bottomRegisterListDatas.Companies,
                    Currencies = bottomRegisterListDatas.Currencies,
                    Statuses = bottomRegisterListDatas.Statuses,
                    //Requesters = bottomRegisterListDatas.Requesters,
                };
            }
            else
            {
                sourcingRegisterViewDto.DailyRegisterSourcing.ForEach(m =>
                {
                    m.Buyers = bottomRegisterListDatas.Buyers;
                    m.Companies = bottomRegisterListDatas.Companies;
                    m.Currencies = bottomRegisterListDatas.Currencies;
                    m.Statuses = bottomRegisterListDatas.Statuses;
                    //m.Requesters = bottomRegisterListDatas.Requesters;
                });
                sourcingRegisterViewDto.EmptyRow = new SourceRegisterDto
                {
                    Buyers = bottomRegisterListDatas.Buyers,
                    Companies = bottomRegisterListDatas.Companies,
                    Currencies = bottomRegisterListDatas.Currencies,
                    Statuses = bottomRegisterListDatas.Statuses,
                    //Requesters = bottomRegisterListDatas.Requesters,
                };
            }
        }

        private async Task<(List<KeyValueTextBoxingDto> Buyers, List<KeyValueTextBoxingDto> Companies,
                             List<KeyValueTextBoxingDto> Currencies, List<KeyValueTextBoxingDto> Statuses
            //,List<KeyValueTextBoxingDto> Requesters
            )>
                                                                        GetBottomRegisterListDatas(SiteDto site)
        {
            var Buyers = TransactionConfig.Mapper.Map<List<KeyValueTextBoxingDto>>(new SiteLineDbLogic(site.SiteDatabase).GetBuyers());
            var Companies = await new SiteLogic().GetSites();
            var Currencies = new SiteLineDbLogic(site.SiteDatabase).GetCurrency();
            var Statuses = await new StatusLogic().GetStatusesAsKeyValue();
            //var Requesters = new SiteLineDbLogic(site.SiteDatabase).GetRequestersAsKeyValue();

            return (Buyers, Companies, Currencies, Statuses);
        }

        //private static List<KeyValueTextBoxingDto> GetRequestedForsDatas()
        //{
        //    return new List<string>(new string[] { "General", "Production", "Tools", "Others" }).Select(m => new KeyValueTextBoxingDto
        //    {
        //        Key = m,
        //        Value = m,
        //        Text = m
        //    }).ToList();
        //}

        static object CheckAndConvertData(string value, bool isDateTime = false)
        {
            if (isDateTime)
            {
                var res = Utility.ConvertHyphenStringToDatetime(value);
                return res == null ? DBNull.Value : res;
            }
            else
                return string.IsNullOrEmpty(value) ? DBNull.Value : value;
        }

    }
}

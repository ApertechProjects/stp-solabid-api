using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using SolaBid.Business.Dtos.ApiDtos;
using SolaBid.Business.Dtos.ApiDtos.ComparisonChartDtos;
using SolaBid.Business.Dtos.EntityDtos;
using SolaBid.Business.Dtos.EntityDtos.CarNumber;
using SolaBid.Business.Logics.CommonLogic;
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
using System.Text;
using System.Threading.Tasks;
using static SolaBid.Business.Logics.CommonLogic.GetData;


namespace SolaBid.Business.Logics
{
    public class VendorLogic
    {
        public async Task<ApiResult> Edit(VendorDto editVendor, string userId, string root)
        {
            var apiResult = new ApiResult();
            if (editVendor.importToSyteline)
            {
                await VendorSendSiteLineAsync("1", new List<int> { editVendor.Id });
                apiResult.OperationIsSuccess = true;
                return apiResult;
            }
            try
            {
                if (await IsUniqueVendorEdit(editVendor))
                {
                    using (var context = TransactionConfig.AppDbContext)
                    {
                        var vendorEntity = await context.Vendors.FindAsync(editVendor.Id);

                        if (vendorEntity == null)
                        {
                            apiResult.ErrorList.Add("Vendor not found");
                            return apiResult;
                        }
                        vendorEntity.Address1 = editVendor.Address1;
                        vendorEntity.Address2 = editVendor.Address2;
                        vendorEntity.Address3 = editVendor.Address3;
                        vendorEntity.Contact = editVendor.Contact;
                        vendorEntity.Country = editVendor.Country;
                        vendorEntity.DeliveryTerm = editVendor.DeliveryTerm;
                        vendorEntity.PaymentTerm = editVendor.PaymentTerm;
                        vendorEntity.EditDate = DateTime.Now;
                        vendorEntity.LastUpdateBy = userId;
                        vendorEntity.Phone = editVendor.Phone;
                        vendorEntity.TaxCode = editVendor.TaxCode;
                        vendorEntity.TaxId = editVendor.TaxId;
                        vendorEntity.VendorBlackList = editVendor.VendorBlackList;
                        vendorEntity.VendorName = editVendor.VendorName;
                        vendorEntity.ExternalEmail = editVendor.ExternalEmail;
                        if (!editVendor.HasSiteLine)
                        {
                            vendorEntity.Currency = editVendor.Currency;
                            vendorEntity.BankCode = editVendor.BankCode;
                        }

                        context.Vendors.Update(vendorEntity);

                        var baseFolder = Path.Combine(root, "appfiles", "VendorDocs", editVendor.Id.ToString());
                        var copyIsSuccessful = FileExtensions.CopyFolderToCopy(baseFolder, "VendorDocsCopy", editVendor.Id.ToString(), root);
                        if (copyIsSuccessful)
                        {
                            FileExtensions.RemoveFolder(root, Path.Combine("VendorDocs", editVendor.Id.ToString()));
                            context.VendorAttachments.RemoveRange(await context.VendorAttachments.Where(m => m.VendorId == editVendor.Id).ToListAsync());
                        }
                        //<<<-->>>//

                        if (editVendor.SendedAttachments != null)
                            foreach (var attachment in editVendor.SendedAttachments)
                            {
                                if (attachment.FileBase64 != "data:")
                                {
                                    context.VendorAttachments.Add(new VendorAttachment
                                    {
                                        VendorId = editVendor.Id,
                                        FilePath = await attachment.FileBase64.SaveVendorAttachment(attachment.FileName, root, editVendor.Id),
                                        FileBaseType = attachment.FileBase64.GetFileBaseType(),
                                        FileName = attachment.FileName,
                                    });
                                }
                            }
                        await context.SaveChangesAsync();
                        FileExtensions.RemoveFolder(root, Path.Combine("VendorDocsCopy", editVendor.Id.ToString()));
                        apiResult.OperationIsSuccess = true;
                    }
                    return apiResult;
                }

                apiResult.ErrorList.Add("Vendor Name or Tax Id already declared!");
                return apiResult;
            }
            catch (Exception ex)
            {
                var baseFolder = Path.Combine(root, "appfiles", "VendorDocsCopy", editVendor.Id.ToString());
                var copyIsSuccessful = FileExtensions.CopyFolderToCopy(baseFolder, "VendorDocs", editVendor.Id.ToString(), root);
                if (copyIsSuccessful)
                    FileExtensions.RemoveFolder(root, Path.Combine("BIDDocsCopy", editVendor.Id.ToString()));
                apiResult.ErrorList.Add(ex.InnerException.Message);
                return apiResult;
            }

        }

        public async Task VendorSendSiteLineAsync(string siteId, List<int> wonnedVendorIds)
        {
            try
            {
                var siteDatabase = await new SiteLogic().GetSiteDatabase(siteId);
                var siteName = await new SiteLogic().GetSiteName(siteId);
                var siteLineVendorCodes = new SiteLineDbLogic(siteDatabase).GetSiteLineVendorCodes(siteName);
                var vendors = await new VendorLogic().GetVendorsByIds(wonnedVendorIds);
                using (SqlConnection sqlConn = TransactionConfig.AppDbContextManualConnection)
                {
                    string sql = "[dbo].[RUS_CreateVendor]";
                    foreach (var vendor in vendors)
                    {
                        if (!siteLineVendorCodes.Any(m => m.Trim() == vendor.VendorCode.Trim()))
                        {
                            using (SqlCommand sqlCmd = new SqlCommand(sql, sqlConn))
                            {
                                sqlCmd.CommandType = CommandType.StoredProcedure;
                                sqlCmd.Parameters.AddWithValue("@site_ref", siteName);
                                sqlCmd.Parameters.AddWithValue("@VendorId", vendor.Id);
                                sqlConn.Open();
                                _ = await sqlCmd.ExecuteNonQueryAsync();
                                sqlCmd.Parameters.Clear();
                                //using (SqlDataAdapter sqlAdapter = new SqlDataAdapter(sqlCmd))
                                //{
                                //    sqlAdapter.Fill(dt);
                                //}
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {

            }
        }

        public async Task<ApiResult> SaveVendorAttachments(VendorAttachmentModelDto sendedAttachments, string root)
        {
            var result = new ApiResult();
            var baseFolder = Path.Combine(root, "appfiles", "VendorDocs", sendedAttachments.VendorId.ToString());
            var copyIsSuccessful = FileExtensions.CopyFolderToCopy(baseFolder, "VendorDocsCopy", sendedAttachments.VendorId.ToString(), root);
            using (var context = TransactionConfig.AppDbContext)
            {

                if (copyIsSuccessful)
                {
                    FileExtensions.RemoveFolder(root, Path.Combine("VendorDocs", sendedAttachments.VendorId.ToString()));
                    context.VendorAttachments.RemoveRange(await context.VendorAttachments.Where(m => m.VendorId == sendedAttachments.VendorId).ToListAsync());
                }
                //<<<-->>>//

                if (sendedAttachments.VendorAttachments != null || sendedAttachments.VendorAttachments.Any())
                    foreach (var vendorAtt in sendedAttachments.VendorAttachments)
                    {
                        if (vendorAtt.FileBase64 != "data:")
                        {
                            context.VendorAttachments.Add(new VendorAttachment
                            {
                                VendorId = sendedAttachments.VendorId,
                                FilePath = await vendorAtt.FileBase64.SaveVendorAttachment(vendorAtt.FileName, root, sendedAttachments.VendorId),
                                FileBaseType = vendorAtt.FileBase64.GetFileBaseType(),
                                FileName = vendorAtt.FileName
                            });
                        }
                    }
                await context.SaveChangesAsync();
            }

            FileExtensions.RemoveFolder(root, Path.Combine("VendorDocsCopy", sendedAttachments.VendorId.ToString()));
            result.OperationIsSuccess = true;
            return result;
        }

        public async Task<VendorAttachmentModelDto> GetVendorAttachmentsById(int vendorId, string host)
        {
            var result = new VendorAttachmentModelDto();
            result.VendorId = vendorId;
            using (var context = TransactionConfig.AppDbContext)
            {
                var vendorAttachments = await context.VendorAttachments.Where(m => m.VendorId == vendorId).ToListAsync();
                foreach (var attachm in vendorAttachments)
                {
                    result.VendorAttachments.Add(new VendorAttachmentForMain
                    {
                        FileBase64 = FileExtensions.ConvertFileToBase64(attachm.FilePath, "appfiles", attachm.FileBaseType),
                        FileName = attachm.FileName,
                        FileId = attachm.Id,
                        FileUrl = Path.Combine(host, "appfiles", attachm.FilePath),
                    });
                }
            }
            return result;
        }

        public async Task<ApiResult> Delete(int vendorId)
        {
            var apiResult = new ApiResult();

            using (var context = TransactionConfig.AppDbContext)
            {

                try
                {
                    var vendorsBIDReferanced = await GetBIDReferansedVendors();
                    var findedVendor = await context.Vendors.FindAsync(vendorId);
                    if (vendorsBIDReferanced.Any(m => m == findedVendor.VendorCode))
                    {
                        apiResult.ErrorList.Add("Query is not valid");
                        return apiResult;
                    }
                    context.Vendors.Remove(findedVendor);
                    _ = await context.SaveChangesAsync();
                    apiResult.OperationIsSuccess = true;
                    return apiResult;
                }
                catch (Exception ex)
                {
                    apiResult.ErrorList.Add(ex.InnerException.Message);
                    return apiResult;
                }
            }
        }


        public async Task<VendorEditDto> GetVendorEditDatas(string siteDatabase, int vendorId, string userId, string host, string siteName)
        {
            var vendorDatas = new VendorEditDto();
            var siteLineVendorIds = new SiteLineDbLogic(siteDatabase).GetSiteLineVendorCodes(siteName);
            vendorDatas.userCanEditVendorWithSiteLine = await new AdditionalPrivilegeLogic().UserCanEditVendorWithSiteLine(userId);

            using (var context = TransactionConfig.AppDbContext)
            {
                var vendorEntity = await context.Vendors.Include(m => m.VendorAttachments).Where(m => m.Id == vendorId).FirstOrDefaultAsync();
                if (vendorEntity == null)
                    return null;
                vendorDatas = TransactionConfig.Mapper.Map<VendorEditDto>(vendorEntity);
                GetVendorEditItems(siteDatabase, vendorDatas, siteName);
                vendorDatas.CreatedBy = await GetUserFullName(vendorDatas.CreatedBy);
                vendorDatas.LastUpdateBy = await GetUserFullName(vendorDatas.LastUpdateBy);
                vendorDatas.HasSiteLine = siteLineVendorIds.Any(m => m == vendorEntity.VendorCode);
                foreach (var attachm in vendorEntity.VendorAttachments)
                {

                    vendorDatas.SendedAttachments.Add(new SendedAttachmentDto
                    {
                        FileBase64 = FileExtensions.ConvertFileToBase64(attachm.FilePath, "appfiles", attachm.FileBaseType),
                        FileName = attachm.FileName,
                        FileUrl = Path.Combine(host, "appfiles", attachm.FilePath),
                        FileId = attachm.Id.ToString()
                    });
                }
            }

            return vendorDatas;
        }
        private async Task<string> GetUserFullName(string userId)
        {
            using (var context = TransactionConfig.AppDbContext)
            {
                var userEntity = await context.Users.FindAsync(userId);
                return userEntity == null ? " " : userEntity.FirstName + " " + userEntity.LastName;
            }
        }
        public async Task<List<VendorDto>> GetVendorsForVendorMain(string siteDatabase, string userId, string siteName)
        {
            var result = await GetVendors();
            var siteLineVendorIds = new SiteLineDbLogic(siteDatabase).GetSiteLineVendorCodes(siteName);
            var userCanEditVendorWithSiteLine = await new AdditionalPrivilegeLogic().UserCanEditVendorWithSiteLine(userId);

            var vendorsBIDReferanced = await GetBIDReferansedVendors();
            foreach (var vendor in result)
            {
                vendor.HasSiteLine = siteLineVendorIds.Any(m => m == vendor.VendorCode);
                vendor.canDelete = (!vendorsBIDReferanced.Any(m => m == vendor.VendorCode) && !vendor.HasSiteLine);
                vendor.canEditVendorWithSiteLine = userCanEditVendorWithSiteLine;
            }
            return result.OrderByDescending(m => m.CreatedDate).ToList();
        }
        public async Task<List<VendorDto>> GetVendors()
        {
            var result = new List<VendorDto>();
            using (var context = TransactionConfig.AppDbContext)
            {
                var vendorEntities = await context.Vendors.Include(m => m.VendorAttachments).ToListAsync();
                result = TransactionConfig.Mapper.Map<List<VendorDto>>(vendorEntities);
            }
            return result;
        }
        private async Task<List<string>> GetBIDReferansedVendors()
        {
            var result = new List<string>();
            using (var context = TransactionConfig.AppDbContext)
            {
                result = await context.Vendors
                    .Include(m => m.BIDReferances)
                    .Where(m => m.BIDReferances.Count > 0)
                    .Select(m => m.VendorCode)
                    .ToListAsync();
            }
            return result;
        }
        public async Task<List<KeyValueTextBoxingDto>> GetVendorsKeyValue()
        {
            var vendorResult = new List<KeyValueTextBoxingDto>();
            var vendors = await GetVendors();

            foreach (var vendor in vendors)
            {
                vendorResult.Add(new KeyValueTextBoxingDto
                {
                    Key = vendor.Id.ToString(),
                    Value = vendor.Id.ToString(),
                    Text = vendor.VendorName + " / " + vendor.VendorCode
                });
            }

            return vendorResult;
        }



        private string MakeIntoSequence(int lastVendorSequence)
        {
            if (lastVendorSequence == 0)
                return "VT00001";
            string output = (lastVendorSequence + 1).ToString();
            while (output.Length < 5)
                output = "0" + output;
            return "VT" + output;
        }
        public async Task<string> GenereteVendorCode()
        {
            string generetedCode = string.Empty;
            using (var context = TransactionConfig.AppDbContext)
            {
                var lastVendorSequence = await context.Sequences.Where(m => m.Key == "Vendor").Select(m => m.SequenceNumber).FirstOrDefaultAsync();
                generetedCode = MakeIntoSequence(lastVendorSequence);
            }
            return generetedCode;
        }

        public VendorCreateSelectListItemsDto GetVendorItems(string siteDatabase, string siteName)
        {
            return new VendorCreateSelectListItemsDto
            {
                Currencies = new SiteLineDbLogic(siteDatabase).GetCurrency(siteName),
                Countries = new SiteLineDbLogic(siteDatabase).GetCountries(siteName),
                TaxCodes = new SiteLineDbLogic(siteDatabase).GetTaxCode(siteName),
                DeliveryTerms = new SiteLineDbLogic(siteDatabase).GetDeliveryTerms(siteName),
                PaymentTerms = new SiteLineDbLogic(siteDatabase).GetPaymentTerms(siteName)
            };
        }

        public void GetVendorEditItems(string siteDatabase, VendorEditDto fillVendor, string siteName)
        {
            fillVendor.Currencies = new SiteLineDbLogic(siteDatabase).GetCurrency(siteName);
            fillVendor.Countries = new SiteLineDbLogic(siteDatabase).GetCountries(siteName);
            fillVendor.TaxCodes = new SiteLineDbLogic(siteDatabase).GetTaxCode(siteName);
            fillVendor.DeliveryTerms = new SiteLineDbLogic(siteDatabase).GetDeliveryTerms(siteName);
            fillVendor.PaymentTerms = new SiteLineDbLogic(siteDatabase).GetPaymentTerms(siteName);
            fillVendor.BankCodes = new SiteLineDbLogic(siteDatabase).GetBankCode(fillVendor.Currency, siteName);
        }

        public async Task<ApiResult> Create(VendorDto vendorDto, string userId, string root)
        {
            var apiResult = new ApiResult();
            var vendor = TransactionConfig.Mapper.Map<Vendor>(vendorDto);

            try
            {
                if (await IsUniqueVendorCreat(vendorDto.VendorName, vendorDto.TaxId))
                {

                    vendor.VendorCode = await GenereteVendorCode();
                    vendor.LastUpdateBy = vendor.CreatedBy = userId;
                    vendor.CreatedDate = vendor.EditDate = DateTime.Now;
                    using (var context = TransactionConfig.AppDbContext)
                    {
                        await context.Vendors.AddAsync(vendor);
                        await new SequenceLogic().UpdateSequence(context, "Vendor");
                        await context.SaveChangesAsync();

                        if (vendorDto.SendedAttachments != null)
                        {
                            foreach (var attachment in vendorDto.SendedAttachments)
                            {
                                context.VendorAttachments.Add(new VendorAttachment
                                {
                                    VendorId = vendor.Id,
                                    FilePath = await attachment.FileBase64.SaveVendorAttachment(attachment.FileName, root, vendor.Id),
                                    FileBaseType = attachment.FileBase64.GetFileBaseType(),
                                    FileName = attachment.FileName,
                                });
                            }
                            await context.SaveChangesAsync();
                        }

                        apiResult.OperationIsSuccess = true;
                        if (vendorDto.importToSyteline)
                            await VendorSendSiteLineAsync("1", new List<int> { vendor.Id });
                    }
                    return apiResult;
                }
                apiResult.ErrorList.Add("Vendor Name or Tax Id already declared!");
                return apiResult;
            }
            catch (Exception ex)
            {
                apiResult.ErrorList.Add(ex.InnerException.Message);
                using (var context = TransactionConfig.AppDbContext)
                {
                    var deleteVendor = await context.Vendors.FindAsync(vendor.Id);
                    var deletedVendorAttachments = await context.VendorAttachments.FindAsync(vendor.Id);
                    if (deleteVendor != null)
                    {
                        context.Vendors.Remove(deleteVendor);
                    }
                    if (deletedVendorAttachments != null)
                    {
                        context.VendorAttachments.Remove(deletedVendorAttachments);
                    }
                    await context.SaveChangesAsync();
                }
                return apiResult;
            }

        }
        async Task<bool> IsUniqueVendorCreat(string vendorName, string taxId)
        {
            bool IsExistVendorTaxId;
            Vendor vendorEntity;
            using (var context = TransactionConfig.AppDbContext)
            {
                vendorEntity = await context.Vendors.Where(m => m.VendorName.ToLower() == vendorName.ToLower()).FirstOrDefaultAsync();
                IsExistVendorTaxId = string.IsNullOrEmpty(taxId) ? false : await context.Vendors.AnyAsync(m => m.TaxId.ToLower() == taxId.ToLower());
            }
            if (IsExistVendorTaxId)
            {
                return false;
            }
            if (vendorEntity is null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        async Task<bool> IsUniqueVendorEdit(VendorDto vendor)
        {
            bool IsExistVendorTaxId;
            Vendor entityVendor;
            Vendor findedVendor;
            using (var context = TransactionConfig.AppDbContext)
            {
                entityVendor = await context.Vendors.Where(m => m.Id == vendor.Id).FirstOrDefaultAsync();
                findedVendor = await context.Vendors.Where(m => m.VendorName.ToLower().Trim() == vendor.VendorName.ToLower().Trim()).FirstOrDefaultAsync();
                IsExistVendorTaxId = string.IsNullOrEmpty(vendor.TaxId) ? false : await context.Vendors.AnyAsync(m => m.TaxId.ToLower().Trim() == vendor.TaxId.ToLower().Trim());
            }
            if (IsExistVendorTaxId && entityVendor.TaxId.Trim() != vendor.TaxId.Trim())
            {
                return false;
            }
            if (findedVendor is null || findedVendor.Id == entityVendor.Id)
            {
                return true;
            }

            if (findedVendor is not null)
            {
                return false;
            }
            else
            {
                return false;
            }
        }
        public async Task<ApiResult> InsertVendorsFromSiteLine(string siteDatabase, string userId, string siteName)
        {
            var apiResult = new ApiResult();
            try
            {
                var siteLineVendors = TransactionConfig.Mapper.Map<List<Vendor>>(new SiteLineDbLogic(siteDatabase).GetSiteLineVendors(userId, siteName));
                if (siteLineVendors.Count > 0)
                {
                    using (var context = TransactionConfig.AppDbContext)
                    {
                        var isEdit = false;
                        foreach (var vendor in siteLineVendors)
                        {
                            if (await context.Vendors.AnyAsync(m => m.VendorCode == vendor.VendorCode))
                            {
                                continue;
                            }
                            else
                            {
                                await context.Vendors.AddAsync(vendor);
                                isEdit = true;
                            }
                        }
                        if (isEdit)
                            await context.SaveChangesAsync();
                    }
                }
                apiResult.OperationIsSuccess = true;
                return apiResult;
            }
            catch (Exception ex)
            {
                apiResult.ErrorList.Add(ex.InnerException.Message);
                return apiResult;

            }

        }

        public async Task<List<Vendor>> GetVendorsByIds(List<int> wonnedVendorIds)
        {
            using (var context = TransactionConfig.AppDbContext)
            {
                var vendorCodes = await context.Vendors.Where(m => wonnedVendorIds.Contains(m.Id)).ToListAsync();
                if (vendorCodes == null)
                {
                    return new();
                }
                else
                {
                    return vendorCodes;
                }
            }
        }


 
    }
}

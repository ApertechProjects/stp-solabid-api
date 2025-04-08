using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;
using SolaBid.Business.Dtos.ApiDtos;
using SolaBid.Business.Dtos.EntityDtos;
using SolaBid.Business.Models;
using SolaBid.Domain.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolaBid.Business.Logics
{
    public class ApproveStageLogic
    {
        public async Task<List<ApproveStageMainDto>> GetApproveStageMains()
        {
            var approveStageMains = new List<ApproveStageMainDto>();
            using (var context = TransactionConfig.AppDbContext)
            {
                var appStageMainEntities = await context.ApproveStageMains.ToListAsync();
                approveStageMains = TransactionConfig.Mapper.Map<List<ApproveStageMainDto>>(appStageMainEntities);
            }
            return approveStageMains.OrderBy(m => m.ApproveStageName).ToList();
        }

        public async Task<List<ApproveStageMainFormDto>> GetApproveStageMainWithDetails(int appStageMainId)
        {
            var approveStageMains = new List<ApproveStageMainFormDto>();
            using (var context = TransactionConfig.AppDbContext)
            {
                var appStageMainEntities = await context.ApproveStageMains
                    .Include(m => m.ApproveStageDetails).ThenInclude(m => m.ApproveRoleApproveStageDetails)
                    .Where(m => m.Id == appStageMainId)
                    .ToListAsync();
                #region mapExternal
                foreach (var appStageMain in appStageMainEntities)
                {
                    var appStageAdding = new ApproveStageMainFormDto
                    {
                        ApproveStageMainName = appStageMain.ApproveStageName,
                        Description = appStageMain.Description,
                        Id = appStageMain.Id,
                    };
                    foreach (var appStageDetail in appStageMain.ApproveStageDetails)
                    {
                        var appStageDetailAdd = new ApproveStageDetailFormDto
                        {
                            Id = appStageDetail.Id,
                            ApproveStageDetailName = appStageDetail.ApproveStageDetailName,
                            ApproveStageDetailSequence = appStageDetail.Sequence,
                        };

                        foreach (var subDetail in appStageDetail.ApproveRoleApproveStageDetails)
                        {
                            appStageDetailAdd.ApproveRoles.Add(new ApproveStageDetailSubDetailsDto
                            {
                                AmountFrom = subDetail.AmountFrom,
                                AmountTo = subDetail.AmountTo,
                                ApproveRoleMainId = subDetail.ApproveRoleId,
                                Id = subDetail.Id
                            });
                        }

                        appStageAdding.ApproveStageDetails.Add(appStageDetailAdd);
                    }

                    approveStageMains.Add(appStageAdding);
                }
                #endregion
            }
            return approveStageMains;
        }

        public async Task<ApiResult> SaveApproveStage(ApproveStageMainFormDto approveStage, ModelStateDictionary ModelState, string userId)
        {
            var apiResult = new ApiResult();
            using (var context = TransactionConfig.AppDbContext)
            {
                var appStageMain = new ApproveStageMain
                {
                    ApproveStageName = approveStage.ApproveStageMainName,
                    CreatedDate = DateTime.Now,
                    EditedDate = DateTime.Now,
                    CreatedUserId = userId,
                    EditedUserId = userId,
                    Description = approveStage.Description
                };

                try
                {
                    if (context.ApproveStageMains.Any(m => m.ApproveStageName.ToLower().Trim() == appStageMain.ApproveStageName.ToLower().Trim()))
                    {
                        apiResult.ErrorList.Add($"{appStageMain.ApproveStageName} already declared");
                        return apiResult;
                    }
                    context.ApproveStageMains.Add(appStageMain);
                    context.SaveChanges();

                    foreach (var appDetail in approveStage.ApproveStageDetails)
                    {
                        var approveStageDetailEntity = new ApproveStageDetail
                        {
                            ApproveStageMainId = appStageMain.Id,
                            ApproveStageDetailName = appDetail.ApproveStageDetailName,
                            Sequence = appDetail.ApproveStageDetailSequence
                        };

                        await context.ApproveStageDetails.AddAsync(approveStageDetailEntity);
                        if (appDetail.ApproveRoles.Count > 0)
                        {
                            context.SaveChanges();
                            foreach (var detailSubDetails in appDetail.ApproveRoles)
                            {
                                await context.ApproveRoleApproveStageDetails.AddAsync(new ApproveRoleApproveStageDetail
                                {
                                    AmountFrom = detailSubDetails.AmountFrom,
                                    AmountTo = detailSubDetails.AmountTo,
                                    ApproveRoleId = detailSubDetails.ApproveRoleMainId,
                                    ApproveStageDetailId = approveStageDetailEntity.Id
                                });
                            }
                        }
                    }
                    await context.SaveChangesAsync();
                    apiResult.OperationIsSuccess = true;
                    return apiResult;
                }
                catch (Exception ex)
                {
                    if (appStageMain.Id > 0)
                    {
                        context.ApproveStageMains.Remove(appStageMain);
                        await context.SaveChangesAsync();
                    }

                    apiResult.ErrorList.Add(ex.Message);
                    return apiResult;
                }
            }

        }

        public async Task<ApiResult> EditApproveStage(ApproveStageMainFormDto approveStage, ModelStateDictionary ModelState, string userId)
        {
            var apiResult = new ApiResult();

            try
            {
                using (var context = TransactionConfig.AppDbContext)
                {
                    var editedAppStageMain = await context.ApproveStageMains
                        .Include(m => m.ApproveStageDetails)
                        .Where(m => m.Id == approveStage.Id)
                        .FirstOrDefaultAsync();

                    editedAppStageMain.ApproveStageName = approveStage.ApproveStageMainName;
                    editedAppStageMain.Description = approveStage.Description;
                    editedAppStageMain.EditedUserId = userId;
                    editedAppStageMain.EditedDate = DateTime.Now;

                    //Delete App Stage Details Start
                    var deletedAppStageDetails = await context.ApproveStageDetails.Where(m => approveStage.DeletedAppStageDetailsList.Contains(m.Id)).ToListAsync();
                    context.ApproveStageDetails.RemoveRange(deletedAppStageDetails);
                    //Delete App Stage Details End

                    //Delete App Stage Details SubDetails Start
                    var deletedAppStageDetailsSubDetails = await context.ApproveRoleApproveStageDetails.Where(m => approveStage.DeletedAppStageDetailSubDetailsList.Contains(m.Id)).ToListAsync();
                    context.ApproveRoleApproveStageDetails.RemoveRange(deletedAppStageDetailsSubDetails);
                    //Delete App Stage Details SubDetails End


                    foreach (var appStageDetail in approveStage.ApproveStageDetails)
                    {
                        if (appStageDetail.IsNew)
                        {
                            //Add New App Stage Details From Edit Start
                            var newAppStageDetail = new ApproveStageDetail
                            {
                                ApproveStageMainId = editedAppStageMain.Id,
                                ApproveStageDetailName = appStageDetail.ApproveStageDetailName,
                                Sequence = appStageDetail.ApproveStageDetailSequence
                            };
                            context.ApproveStageDetails.Add(newAppStageDetail);
                            context.SaveChanges();
                            foreach (var subDetail in appStageDetail.ApproveRoles)
                            {
                                await context.ApproveRoleApproveStageDetails.AddAsync(
                                    new ApproveRoleApproveStageDetail
                                    {
                                        AmountFrom = subDetail.AmountFrom,
                                        AmountTo = subDetail.AmountTo,
                                        ApproveRoleId = subDetail.ApproveRoleMainId,
                                        ApproveStageDetailId = newAppStageDetail.Id
                                    });
                            }
                            //Add New App Stage Details From Edit End
                        }
                        else
                        {
                            //Edit App Stage Details Start
                            var editedAppStageDetail = editedAppStageMain.ApproveStageDetails.Where(m => m.Id == appStageDetail.Id).FirstOrDefault();
                            editedAppStageDetail.ApproveStageDetailName = appStageDetail.ApproveStageDetailName;
                            editedAppStageDetail.Sequence = appStageDetail.ApproveStageDetailSequence;
                            //Edit App Stage Details End

                            //Edit App Stage Details Sub Detail Start
                            foreach (var detailSub in appStageDetail.ApproveRoles)
                            {
                                var editedDetailSub = context.ApproveRoleApproveStageDetails.Where(m => m.Id == detailSub.Id).FirstOrDefault();
                                if (editedDetailSub != null)
                                {
                                    editedDetailSub.AmountFrom = detailSub.AmountFrom;
                                    editedDetailSub.AmountTo = detailSub.AmountTo;
                                    editedDetailSub.ApproveRoleId = detailSub.ApproveRoleMainId;
                                    context.ApproveStageMains.Update(editedAppStageMain);

                                }
                                else
                                {
                                    await context.ApproveRoleApproveStageDetails.AddAsync(new ApproveRoleApproveStageDetail
                                    {
                                        AmountFrom = detailSub.AmountFrom,
                                        AmountTo = detailSub.AmountTo,
                                        ApproveRoleId = detailSub.ApproveRoleMainId,
                                        ApproveStageDetailId = appStageDetail.Id
                                    });
                                }
                            }
                            //Edit App Stage Details Sub Detail Stop
                        }
                    }
                    await context.SaveChangesAsync();
                }
                apiResult.OperationIsSuccess = true;
                return apiResult;
            }
            catch (Exception ex)
            {
                apiResult.ErrorList.Add(ex.Message);
                return apiResult;
            }
        }

        public async Task<ApiResult> DeleteApproveStageMain(int appStageMainId)
        {
            var result = new ApiResult();
            using (var context = TransactionConfig.AppDbContext)
            {
                try
                {
                    var appStageMainEntity = await context.ApproveStageMains.FindAsync(appStageMainId);
                    if (await context.ComparisonCharts.AnyAsync(m => m.ApproveStageId == appStageMainId))
                    {
                        result.ErrorList.Add($"{appStageMainEntity.ApproveStageName} is now being used") ;
                        return result;
                    }
                    context.ApproveStageMains.Remove(appStageMainEntity);
                    await context.SaveChangesAsync();
                    result.OperationIsSuccess = true;
                    return result;
                }
                catch (Exception ex)
                {
                    result.ErrorList.Add(ex.Message);
                    return result;
                }
            }
        }
    }
}

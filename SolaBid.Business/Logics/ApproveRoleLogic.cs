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
    public class ApproveRoleLogic
    {
        public async Task<List<ApproveRoleDto>> GetApproveRoles()
        {
            var approveRolesDto = new List<ApproveRoleDto>();
            using (var context = TransactionConfig.AppDbContext)
            {
                var approveRolesEntity = await context.ApproveRoles.ToListAsync();
                approveRolesDto = TransactionConfig.Mapper.Map<List<ApproveRoleDto>>(approveRolesEntity);
            }
            return approveRolesDto;
        }

        public async Task<List<KeyValueTextBoxingDto>> GetApproveRolesWithKeyValue()
        {
            var approveRoleList = await GetApproveRoles();
            return TransactionConfig.Mapper.Map<List<KeyValueTextBoxingDto>>(approveRoleList);
        }

        public async Task<List<ApproveRoleDto>> GetGroupApproveRolesWithAll(string groupId)
        {
            var approveRolesDto = await GetApproveRoles();
            using (var context = TransactionConfig.AppDbContext)
            {
                var groupApproveRoleEntity = await context.GroupApproveRoles.Where(m => m.AppRoleId == groupId).ToListAsync();
                if (groupApproveRoleEntity.Count > 0)
                {
                    foreach (var appRole in approveRolesDto)
                    {
                        if (groupApproveRoleEntity.Any(m => m.ApproveRoleId == appRole.Id))
                        {
                            appRole.IsSelected = true;
                        }
                    }
                }
            }
            return approveRolesDto;
        }

        public async Task<ApiResult> Create(ApproveRoleDto approveRoleDto, ModelStateDictionary modelState)
        {
            
            var apiResult = new ApiResult();

            using (var context = TransactionConfig.AppDbContext)
            {
                if (await context.ApproveRoles.AnyAsync(m => m.ApproveRoleName.ToLower().Trim() == approveRoleDto.ApproveRoleName.ToLower().Trim()))
                {
                    apiResult.ErrorList.Add($"{approveRoleDto.ApproveRoleName} already declared.");
                    return apiResult;
                }
                await context.ApproveRoles.AddAsync(new ApproveRole() { ApproveRoleName = approveRoleDto.ApproveRoleName, Description = approveRoleDto.Description });
                await context.SaveChangesAsync();
                apiResult.OperationIsSuccess = true;
            }
            return apiResult;
        }

        public async Task<ApiResult> Edit(ApproveRoleDto approveRoleDto, ModelStateDictionary modelState)
        {
            var apiResult = new ApiResult();

            using (var context = TransactionConfig.AppDbContext)
            {
                if (await context.ApproveRoles.AnyAsync(
                    m => m.ApproveRoleName.ToLower().Trim() == approveRoleDto.ApproveRoleName.ToLower().Trim() && m.Id != approveRoleDto.Id))
                {
                    apiResult.ErrorList.Add($"{approveRoleDto.ApproveRoleName} already declared.");
                    return apiResult;
                }

                var editEntity = await context.ApproveRoles.FindAsync(approveRoleDto.Id);
                editEntity.ApproveRoleName = approveRoleDto.ApproveRoleName;
                editEntity.Description = approveRoleDto.Description;

                context.ApproveRoles.Update(editEntity);
                await context.SaveChangesAsync();
                apiResult.OperationIsSuccess = true;

            }
            return apiResult;
        }

        public async Task<ApiResult> Delete(int approveRoleId)
        {
            var apiResult = new ApiResult();

            using (var context = TransactionConfig.AppDbContext)
            {
                var editEntity = await context.ApproveRoles
                    .Include(m=>m.GroupApproveRoles)
                    .Where(m=>m.Id==approveRoleId)
                    .FirstOrDefaultAsync();
                if (editEntity.GroupApproveRoles.Count>0)
                {
                    apiResult.ErrorList.Add($"{editEntity.ApproveRoleName} is now being used");
                    return apiResult;
                }
                context.ApproveRoles.Remove(editEntity);
                await context.SaveChangesAsync();
                apiResult.OperationIsSuccess = true;

            }
            return apiResult;
        }
    }
}

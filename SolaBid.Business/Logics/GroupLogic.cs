using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;
using SolaBid.Business.Dtos.ApiDtos;
using SolaBid.Business.Dtos.EntityDtos;
using SolaBid.Business.Models;
using SolaBid.Domain.Models.AppDbContext;
using SolaBid.Domain.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolaBid.Business.Logics
{
    public class GroupLogic
    {
        public async Task<ApiResult> AddGroup(GroupDatasDto groupDatas, RoleManager<AppRole> roleManager, ModelStateDictionary ModelState)
        {
            var apiResult = new ApiResult();

            if (await IsGroupDeleted(groupDatas.GroupFormDataDto.GroupName))
            {
                apiResult.ErrorList.Add($"{groupDatas.GroupFormDataDto.GroupName} already deleted.Please,Try another group name.");
                return apiResult;
            }
            var roleEntity = new AppRole
            { Name = groupDatas.GroupFormDataDto.GroupName, Description = groupDatas.GroupFormDataDto.Description };

            var roleResult = await roleManager.CreateAsync(roleEntity);
            if (roleResult.Succeeded)
            {
                var addedGroupId = await roleManager.FindByNameAsync(roleEntity.Name);
                using (var context = TransactionConfig.AppDbContext)
                {

                    await SetGroupUsers(groupDatas.GroupUserIdsDto, addedGroupId.Id, context);
                    await SetGroupPrivileges(groupDatas.SubMenuPrivilegesDto, addedGroupId.Id, context);
                    await SetGroupSiteWarehouses(groupDatas.SiteWarehouseIdsDto, addedGroupId.Id, context);
                    await SetGroupBuyers(groupDatas.BuyerIds, addedGroupId.Id, context);
                    await SetGroupApproveRoles(groupDatas.ApprovedRolesIds, addedGroupId.Id, context);
                    await SetGroupAdditionalPrivileges(groupDatas.AdditionalPrivilegesIds, addedGroupId.Id, context);
                    await context.SaveChangesAsync();
                }
                apiResult.OperationIsSuccess = true;
                return apiResult;
            }
            else
            {
                foreach (var error in roleResult.Errors)
                    apiResult.ErrorList.Add(error.Description);
            }

            return apiResult;

            async Task<bool> IsGroupDeleted(string groupName)
            {
                using (var context = TransactionConfig.AppDbContext)
                    return await context.Roles.AnyAsync(m => m.Name.ToLower() == groupName.ToLower() && m.IsDeleted);
            }

        }
        public async Task<ApiResult> EditGroup(GroupDatasDto groupDatas, RoleManager<AppRole> roleManager, ModelStateDictionary ModelState)
        {
            var apiResult = new ApiResult();

            using (var context = TransactionConfig.AppDbContext)
            {
                var roleEntity = await context.Roles.Where(m => m.Id == groupDatas.GroupFormDataDto.Id && !m.IsDeleted).FirstOrDefaultAsync();
                if (roleEntity == null)
                {
                    apiResult.ErrorList.Add("Group Not Found");
                    return apiResult;
                }
                try
                {
                    roleEntity.Description = groupDatas.GroupFormDataDto.Description;
                    roleEntity.Name = groupDatas.GroupFormDataDto.GroupName;
                    context.Roles.Update(roleEntity);
                    int saveResult = await context.SaveChangesAsync();
                    if (saveResult > 0)
                    {
                        await SetGroupUsers(groupDatas.GroupUserIdsDto, roleEntity.Id, context);
                        await SetGroupPrivileges(groupDatas.SubMenuPrivilegesDto, roleEntity.Id, context);
                        await SetGroupSiteWarehouses(groupDatas.SiteWarehouseIdsDto, roleEntity.Id, context);
                        await SetGroupBuyers(groupDatas.BuyerIds, roleEntity.Id, context);
                        await SetGroupApproveRoles(groupDatas.ApprovedRolesIds, roleEntity.Id, context);
                        await SetGroupAdditionalPrivileges(groupDatas.AdditionalPrivilegesIds, roleEntity.Id, context);

                        await context.SaveChangesAsync();
                        apiResult.OperationIsSuccess = true;
                        return apiResult;
                    }
                    else
                    {
                        apiResult.ErrorList.Add("Operation UnSuccessful");
                        return apiResult;
                    }
                }
                catch (Exception ex)
                {
                    apiResult.ErrorList.Add(ex.Message);
                    return apiResult;
                }
            }
        }
        private async Task SetGroupUsers(List<string> userIds, string groupId, SBDbContext context)
        {
            var deletedEntities = await context.UserRoles.Where(m => m.RoleId == groupId).ToListAsync();
            if (deletedEntities != null)
                context.UserRoles.RemoveRange(deletedEntities);

            if (userIds != null && userIds.Count > 0)
            {
                var addedList = userIds.Select(userId => new AspNetUserRoles
                {
                    RoleId = groupId,
                    UserId = userId
                });
                await context.UserRoles.AddRangeAsync(addedList);
            }
        }
        private async Task SetGroupPrivileges(List<SubMenuPrivilegesDto> menuPrivilegesDtos, string groupId, SBDbContext context)
        {
            var deletedEntities = await context.GroupMenus.Where(m => m.AppRoleId == groupId).ToListAsync();
            if (deletedEntities != null)
            {
                context.GroupMenus.RemoveRange(deletedEntities);
            }

            if (menuPrivilegesDtos != null && menuPrivilegesDtos.Count > 0)
            {
                var groupMenus = menuPrivilegesDtos.Select(privileges => new GroupMenu
                {
                    AppRoleId = groupId,
                    SubMenuId = privileges.SubMenuId,
                    Create = privileges.Create,
                    Delete = privileges.Delete,
                    Edit = privileges.Edit,
                    View = privileges.View,
                    Export = privileges.Export
                });
                await context.GroupMenus.AddRangeAsync(groupMenus);
            }
        }
        private async Task SetGroupSiteWarehouses(List<GroupWarehousesDto> siteWarehouseIds, string groupId, SBDbContext context)
        {
            var deletedEntities = await context.GroupSiteWarehouses.Where(m => m.AppRoleId == groupId).ToListAsync();
            if (deletedEntities != null)
                context.GroupSiteWarehouses.RemoveRange(deletedEntities);

            if (siteWarehouseIds != null && siteWarehouseIds.Count > 0)
            {
                await context.GroupSiteWarehouses.AddRangeAsync(siteWarehouseIds.Select(siteWarehouse => new GroupSiteWarehouse
                {
                    AppRoleId = groupId,
                    SiteId = siteWarehouse.SiteId,
                    WarehouseCode = siteWarehouse.WarehouseId,
                    WarehouseName = siteWarehouse.WarehouseName
                }));
            }
        }
        private async Task SetGroupBuyers(List<int> buyerIds, string groupId, SBDbContext context)
        {
            var deletedEntities = await context.GroupBuyers.Where(m => m.AppRoleId == groupId).ToListAsync();
            if (deletedEntities != null)
                context.GroupBuyers.RemoveRange(deletedEntities);

            if (buyerIds != null && buyerIds.Count > 0)
            {
                await context.GroupBuyers.AddRangeAsync(buyerIds.Select(buyerId => new GroupBuyer
                {
                    AppRoleId = groupId,
                    BuyerId = buyerId,
                }));
            }
        }
        private async Task SetGroupAdditionalPrivileges(List<int> additionalPrivilegesIds, string groupId, SBDbContext context)
        {
            var deletedEntities = await context.GroupAdditionalPrivileges.Where(m => m.AppRoleId == groupId).ToListAsync();
            if (deletedEntities != null)
            {
                context.GroupAdditionalPrivileges.RemoveRange(deletedEntities);
            }
            if (additionalPrivilegesIds != null && additionalPrivilegesIds.Count > 0)
            {
                await context.GroupAdditionalPrivileges.AddRangeAsync(additionalPrivilegesIds.Select(addPrivId => new GroupAdditionalPrivilege
                {
                    AppRoleId = groupId,
                    AdditionalPrivilegeId = addPrivId,
                }));
            }
        }
        private async Task SetGroupApproveRoles(List<int> approvedRolesIds, string groupId, SBDbContext context)
        {
            var deletedEntities = await context.GroupApproveRoles.Where(m => m.AppRoleId == groupId).ToListAsync();
            if (deletedEntities != null)
                context.GroupApproveRoles.RemoveRange(deletedEntities);

            if (approvedRolesIds != null && approvedRolesIds.Count > 0)
            {
                await context.GroupApproveRoles.AddRangeAsync(approvedRolesIds.Select(approvedRolesId => new GroupApproveRole
                {
                    AppRoleId = groupId,
                    ApproveRoleId = approvedRolesId
                }));
            }
        }
        public async Task<List<AppRoleDto>> GetUserGroupsByUserId(string userId)
        {
            var userGroups = new List<AppRoleDto>();

            using (var context = TransactionConfig.AppDbContext)
            {
                var userRolesEntity = context.UserRoles.Where(m => m.UserId == userId).Select(m => m.RoleId);
                var groupEntities = await context.Roles.Where(m => userRolesEntity.Contains(m.Id)).ToListAsync();

                userGroups.AddRange(groupEntities.Select(role => new AppRoleDto
                {
                    Id = role.Id,
                    Description = role.Description,
                    Name = role.Name
                }));
            }
            return userGroups;
        }
        public async Task<List<string>> GetUserGroupIdsByUserId(string userId)
        {
            var userGroups = new List<string>();
            using (var context = TransactionConfig.AppDbContext)
            {
                var userRolesEntityIds = await context.UserRoles
                    .Where(m => m.UserId == userId)
                    .Select(m => m.RoleId)
                    .ToListAsync();
                return userRolesEntityIds;
            }

        }
        public async Task<List<AppRoleDto>> GetUserGroupsWithAll(string userId)
        {
            var groupUsersWithAll = await GetGroups();
            var userGroups = await GetUserGroupsByUserId(userId);
            if (userGroups.Count > 0)
            {
                groupUsersWithAll.ForEach((groupEnt) =>
                {
                    if (userGroups.Any(m => m.Id == groupEnt.Id))
                    {
                        groupEnt.IsSelected = true;
                    }
                });
            }
            return groupUsersWithAll;
        }
        public async Task<List<AppRoleDto>> GetGroups()
        {
            var groupListDto = new List<AppRoleDto>();

            using (var context = TransactionConfig.AppDbContext)
            {
                var groupsEntity = await context.Roles.Where(m => !m.IsDeleted).ToListAsync();
                groupListDto = TransactionConfig.Mapper.Map<List<AppRoleDto>>(groupsEntity);
            }
            return groupListDto;
        }
        public async Task<List<AppRoleDto>> GetGroupsForTest()
        {
            var groupListDto = new List<AppRoleDto>();

            using (var context = TransactionConfig.AppDbContext)
            {
                var groupsEntity = await context.Roles.Where(m => !m.IsDeleted).ToListAsync();
                groupListDto = TransactionConfig.Mapper.Map<List<AppRoleDto>>(groupsEntity);
            }
            return groupListDto;
        }
        public async Task<ApiResult> DeleteGroup(string groupId)
        {
            var result = new ApiResult();
            using (var context = TransactionConfig.AppDbContext)
            {
                var groupEntity = await context.Roles.Where(m => m.Id == groupId && !m.IsDeleted).FirstOrDefaultAsync();
                if (groupEntity == null)
                {
                    result.OperationIsSuccess = false;
                    result.ErrorList.Add("Group Not Found");
                    return result;
                }
                groupEntity.IsDeleted = true;
                result.OperationIsSuccess = true;
                context.Roles.Update(groupEntity);
                await context.SaveChangesAsync();
            }
            return result;
        }
        public async Task<GroupFormDataDto> GetGroupFormDatas(string groupId)
        {
            var result = new GroupFormDataDto();
            using (var context = TransactionConfig.AppDbContext)
            {
                var groupEntity = await context.Roles.FindAsync(groupId);
                result.Id = groupEntity.Id;
                result.GroupName = groupEntity.Name;
                result.Description = groupEntity.Description;
            }

            return result;
        }
    }
}

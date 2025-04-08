using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;
using SolaBid.Business.Dtos.ApiDtos;
using SolaBid.Business.Dtos.EntityDtos;
using SolaBid.Business.Models;
using SolaBid.Business.Models.Enum;
using SolaBid.Domain.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolaBid.Business.Logics
{
    public class AdditionalPrivilegeLogic
    {
        public async Task<List<AdditionalPrivilegeDto>> GetAdditionalPrivileges()
        {
            var additionalPrivileges = new List<AdditionalPrivilegeDto>();
            using (var context = TransactionConfig.AppDbContext)
            {
                var addPrivs = await context.AdditionalPrivileges.ToListAsync();
                additionalPrivileges = TransactionConfig.Mapper.Map<List<AdditionalPrivilegeDto>>(addPrivs);
            }
            return additionalPrivileges;
        }

        public async Task<List<AdditionalPrivilegeDto>> GetGroupAdditionalPrivilegesWithAll(string groupId)
        {
            var additionalPrivileges = await GetAdditionalPrivileges();
            using (var context = TransactionConfig.AppDbContext)
            {
                var groupAddPrivileges = await context.GroupAdditionalPrivileges.Where(m => m.AppRoleId == groupId).ToListAsync();
                foreach (var privilege in additionalPrivileges)
                {
                    if (groupAddPrivileges.Any(m => m.AdditionalPrivilegeId == privilege.Id))
                    {
                        privilege.IsSelected = true;
                    }
                }
            }
            return additionalPrivileges;
        }

        public async Task<bool> UserCanSeeAmount(string userId)
        {
            using (var context = TransactionConfig.AppDbContext)
            {
                var userGroups = await context.UserRoles.Where(m => m.UserId == userId).Select(m => m.RoleId).ToListAsync();
                return await context.GroupAdditionalPrivileges
                    .AnyAsync(m => userGroups.Contains(m.AppRoleId) &&
                    m.AdditionalPrivilegeId == (byte)AdditionalPrivilegeses.CanSeeAmounts);
            }
        }

        public async Task<bool> UserCanEditVendorWithSiteLine(string userId)
        {
            using (var context = TransactionConfig.AppDbContext)
            {
                var userGroups = await context.UserRoles
                    .Where(m => m.UserId == userId)
                    .Select(m => m.RoleId)
                    .ToListAsync();
                return await context.GroupAdditionalPrivileges
                    .AnyAsync(m => userGroups.Contains(m.AppRoleId) &&
                    m.AdditionalPrivilegeId == (byte)AdditionalPrivilegeses.CanEditVendorCardWithSytelineData);
            }
        }

        public async Task<CarNumberAdditionalPrivilegeDto> CarNumberAdditionalPrivilege(string userId)
        {
            var result = new CarNumberAdditionalPrivilegeDto();
            var data = new List<GroupAdditionalPrivilege>();
            using (var context = TransactionConfig.AppDbContext)
            {
                var userGroups = context.UserRoles
                    .Where(m => m.UserId == userId)
                    .Select(m => m.RoleId);
                data = await context.GroupAdditionalPrivileges.Where(m => userGroups.Contains(m.AppRoleId)).ToListAsync();
            }
            result.CarSendToApprove = data.Any(m=>m.AdditionalPrivilegeId == (byte)AdditionalPrivilegeses.CarSendToApprove);
            result.CarApprover1 = data.Any(m=>m.AdditionalPrivilegeId == (byte)AdditionalPrivilegeses.CarApprover1);
            result.CarApprover2 = data.Any(m=>m.AdditionalPrivilegeId == (byte)AdditionalPrivilegeses.CarApprover2);
            return result;
        }
    }
}

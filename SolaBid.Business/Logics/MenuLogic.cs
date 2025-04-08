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
    public class MenuLogic
    {
        public async Task<List<MenuDto>> GetMenus()
        {
            var menuListDto = new List<MenuDto>();
            using (var context = TransactionConfig.AppDbContext)
            {
                var parentMenuList = await context.ParentMenus.Include(m => m.SubMenus).ToListAsync();
                foreach (var parentMenu in parentMenuList)
                {
                    menuListDto.Add(new MenuDto
                    {
                        ParentMenuId = parentMenu.Id,
                        OrderNumber = parentMenu.OrderNumber,
                        ParentMenuName = parentMenu.ParentMenuName,
                        SubMenus = TransactionConfig.Mapper.Map<List<SubMenuParentMenuDto>>(parentMenu.SubMenus)
                    });
                }
            }
            return menuListDto.OrderBy(m => m.OrderNumber).ToList();
        }

        public async Task<List<ParentMenuDto>> GetUserMenusForTest()
        {
            string userId = "80d2b28a-5487-40fe-a9a6-d0280e0b969c";
            var result = new List<ParentMenuDto>();
            var userGroups = (await new GroupLogic().GetUserGroupsByUserId(userId)).Select(m => m.Id).ToList();
            using (var context = TransactionConfig.AppDbContext)
            {
                var userGroupMenus = await context.GroupMenus
                    .Include(m => m.SubMenu)
                    .ThenInclude(m => m.ParentMenu)
                    .Where(m => userGroups.Contains(m.AppRoleId)).ToListAsync();

                var parentMenus = new HashSet<ParentMenu>();

                foreach (var userGroup in userGroupMenus)
                    parentMenus.Add(userGroup.SubMenu.ParentMenu);

                foreach (var parentMenu in parentMenus.OrderBy(m => m.OrderNumber).ToList())
                {
                    var subMenuByParentMenu = userGroupMenus.Where(m => m.SubMenu.ParentMenuId == parentMenu.Id).Select(m => m.SubMenu).ToHashSet<SubMenu>();

                    var parentMenuPriv = new ParentMenuDto()
                    {
                        Id = parentMenu.Id,
                        Icon = parentMenu.Icon,
                        ParentMenuName = parentMenu.ParentMenuName
                    };
                    foreach (var subMenuPriv in subMenuByParentMenu)
                    {
                        parentMenuPriv.SubMenus.Add(new SubMenuDto
                        {
                            Icon = subMenuPriv.Icon,
                            ParentMenuId = parentMenuPriv.Id,
                            Id = subMenuPriv.Id,
                            SubLink = subMenuPriv.SubLink,
                            SubMenuName = subMenuPriv.SubMenuName
                        });
                    }

                    result.Add(parentMenuPriv);
                }
            }
            return result;
        }

        public async Task<List<SubMenuForDXGroupGridDto>> GetUserMenusForGroupTest()
        {
            string userId = "80d2b28a-5487-40fe-a9a6-d0280e0b969c";
            var result = new List<SubMenuForDXGroupGridDto>();
            var userGroups = (await new GroupLogic().GetUserGroupsByUserId(userId)).Select(m => m.Id).ToList();
            using (var context = TransactionConfig.AppDbContext)
            {
                var userGroupMenus = await context.GroupMenus
                    .Include(m => m.SubMenu)
                    .ThenInclude(m => m.ParentMenu)
                    .Where(m => userGroups.Contains(m.AppRoleId)).ToListAsync();

                var parentMenus = new HashSet<ParentMenu>();

                foreach (var userGroup in userGroupMenus)
                    parentMenus.Add(userGroup.SubMenu.ParentMenu);

                foreach (var parentMenu in parentMenus.OrderBy(m => m.OrderNumber).ToList())
                {
                    result.Add(new()
                    {
                        Id = parentMenu.Id * 28,
                        MenuName = parentMenu.ParentMenuName,
                        ParentMenuId = 0

                    });
                    var subMenuByParentMenu = userGroupMenus.Where(m => m.SubMenu.ParentMenuId == parentMenu.Id).Select(m => m.SubMenu).ToHashSet<SubMenu>();
                    foreach (var subMenuPriv in subMenuByParentMenu)
                    {
                        result.Add(new SubMenuForDXGroupGridDto
                        {
                            Id = subMenuPriv.Id,
                            ParentMenuId = parentMenu.Id * 28,
                            MenuName = subMenuPriv.SubMenuName
                        });
                    }

                }
            }
            return result;
        }

        public async Task<List<MenuDto>> GetGroupMenusWithAll(string groupId)
        {
            var menuListDto = await GetMenus();
            using (var context = TransactionConfig.AppDbContext)
            {
                var parentMenuList = await context.GroupMenus.Where(m => m.AppRoleId == groupId).ToListAsync();
                if (parentMenuList.Count > 0)
                {
                    foreach (var parentMenus in menuListDto)
                    {
                        foreach (var subMenu in parentMenus.SubMenus)
                        {
                            var menuPrivileges = parentMenuList.Where(m => m.SubMenuId == subMenu.Id).FirstOrDefault();
                            if (menuPrivileges != null)
                            {
                                subMenu.Create = menuPrivileges.Create;
                                subMenu.Edit = menuPrivileges.Edit;
                                subMenu.Delete = menuPrivileges.Delete;
                                subMenu.View = menuPrivileges.View;
                                subMenu.Export = menuPrivileges.Export;
                            }
                        }
                    }

                }
            }
            return menuListDto;
        }

        public async Task<PrivilegeCheckerDto> GetPrivileges(string baseUrl, string userId)
        {
            var privilegeResult = new PrivilegeCheckerDto();
            var userGroupsIds = (await new GroupLogic().GetUserGroupsByUserId(userId)).Select(m => m.Id).ToList();
            using (var context = TransactionConfig.AppDbContext)
            {

                var groupBaseUrlPrivileges = await context.GroupMenus
                    .Include(m => m.SubMenu)
                    .Where(m => userGroupsIds.Contains(m.AppRoleId) && m.SubMenu.SubLink == baseUrl.Replace("-", "/")).Select(m => new { m.Create, m.Edit, m.Delete, m.View, m.Export })
                    .ToListAsync();

                if (groupBaseUrlPrivileges.Any(m => m.Create))
                {
                    privilegeResult.Create = true;
                }
                if (groupBaseUrlPrivileges.Any(m => m.Edit))
                {
                    privilegeResult.Edit = true;
                }
                if (groupBaseUrlPrivileges.Any(m => m.Delete))
                {
                    privilegeResult.Delete = true;
                }
                if (groupBaseUrlPrivileges.Any(m => m.View))
                {
                    privilegeResult.View = true;
                }
                if (groupBaseUrlPrivileges.Any(m => m.Export))
                {
                    privilegeResult.Export = true;
                }
            }

            return privilegeResult;
        }

        public async Task<List<ParentMenuDto>> GetUserMenus(string userId)
        {
            var result = new List<ParentMenuDto>();
            var userGroups = (await new GroupLogic().GetUserGroupsByUserId(userId)).Select(m => m.Id).ToList();
            using (var context = TransactionConfig.AppDbContext)
            {
                var userGroupMenus = await context.GroupMenus
                    .Include(m => m.SubMenu)
                    .ThenInclude(m => m.ParentMenu)
                    .Where(m => userGroups.Contains(m.AppRoleId)).ToListAsync();

                var parentMenus = new HashSet<ParentMenu>();

                foreach (var userGroup in userGroupMenus)
                    parentMenus.Add(userGroup.SubMenu.ParentMenu);

                foreach (var parentMenu in parentMenus.OrderBy(m => m.OrderNumber).ToList())
                {
                    var subMenuByParentMenu = userGroupMenus.Where(m => m.SubMenu.ParentMenuId == parentMenu.Id).Select(m => m.SubMenu).ToHashSet<SubMenu>();

                    var parentMenuPriv = new ParentMenuDto()
                    {
                        Id = parentMenu.Id,
                        Icon = parentMenu.Icon,
                        ParentMenuName = parentMenu.ParentMenuName
                    };
                    foreach (var subMenuPriv in subMenuByParentMenu)
                    {
                        parentMenuPriv.SubMenus.Add(new SubMenuDto
                        {
                            Icon = subMenuPriv.Icon,
                            ParentMenuId = parentMenuPriv.Id,
                            Id = subMenuPriv.Id,
                            SubLink = subMenuPriv.SubLink,
                            SubMenuName = subMenuPriv.SubMenuName
                        });
                    }

                    result.Add(parentMenuPriv);
                }
            }
            return result;
        }
    }
}

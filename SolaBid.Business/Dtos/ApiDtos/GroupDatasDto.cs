using System;
using System.Collections.Generic;
using System.Text;

namespace SolaBid.Business.Dtos.ApiDtos
{
    public class GroupDatasDto
    {

        public List<string> GroupUserIdsDto { get; set; }
        public List<int> BuyerIds { get; set; }
        public List<int> ApprovedRolesIds { get; set; }
        public List<int> AdditionalPrivilegesIds { get; set; }
        public GroupFormDataDto GroupFormDataDto { get; set; }
        public List<SubMenuPrivilegesDto> SubMenuPrivilegesDto { get; set; }
        public List<GroupWarehousesDto> SiteWarehouseIdsDto { get; set; }
    }
}

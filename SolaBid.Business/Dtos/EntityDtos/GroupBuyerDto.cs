using System;
using System.Collections.Generic;
using System.Text;

namespace SolaBid.Business.Dtos.EntityDtos
{
    public class GroupBuyerDto
    {
        public int BuyerId { get; set; }
        public string AppRoleId { get; set; }
        public AppRoleDto AppRole { get; set; }
    }
}

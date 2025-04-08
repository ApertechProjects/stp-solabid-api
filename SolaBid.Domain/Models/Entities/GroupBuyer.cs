using System;
using System.Collections.Generic;
using System.Text;

namespace SolaBid.Domain.Models.Entities
{
    public class GroupBuyer
    {
        public int Id { get; set; }
        public int BuyerId { get; set; }
        public string AppRoleId { get; set; }
        public AppRole AppRole { get; set; }
    }
}

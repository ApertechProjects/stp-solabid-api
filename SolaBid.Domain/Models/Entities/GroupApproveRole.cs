using System;
using System.Collections.Generic;
using System.Text;

namespace SolaBid.Domain.Models.Entities
{
    public class GroupApproveRole
    {
        public string AppRoleId { get; set; } //GroupId
        public int ApproveRoleId { get; set; }
        public AppRole AppRole { get; set; } //Group
        public ApproveRole ApproveRole { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Text;

namespace SolaBid.Domain.Models.Entities
{
   public class BaseOptions
    {
        public DateTime CreatedDate { get; set; }
        public DateTime EditedDate { get; set; }
        public string CreatedUserId { get; set; }
        public string EditedUserId { get; set; }
        public AppUser CreatedUser { get; set; }
        public AppUser EditedUser { get; set; }
    }
}

using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Text;

namespace SolaBid.Domain.Models.Entities
{
    public class AppUser : IdentityUser
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime RegDate { get; set; }
        public bool IsActive { get; set; }
        public bool IsApproved { get; set; }
        public string UserImage { get; set; }
        public string UserSignature { get; set; }
        public DateTime RestoreExpireDate { get; set; }
        public string RestorePasswordToken { get; set; }
        public string BuyerId { get; set; }
        public string BuyerUserName { get; set; }
        public bool IsDeleted { get; set; }
    }
}

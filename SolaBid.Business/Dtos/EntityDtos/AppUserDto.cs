using SolaBid.Business.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace SolaBid.Business.Dtos.EntityDtos
{
    public class AppUserDto
    {
    
        public string Id { get; set; }
        [Required]
        [MaxLength(128), MinLength(3)]
        public string FirstName { get; set; }
        [Required]
        [MaxLength(128), MinLength(3)]
        public string LastName { get; set; }
        [Required]
        [EmailAddress]
        [MaxLength(128)]
        public string Email { get; set; }
        [Required]
        [MaxLength(150)]
        public string Password { get; set; }
        public DateTime RegDate { get; set; }
        public bool IsActive { get; set; }
        public bool IsApproved { get; set; }
        public string UserImage { get; set; }
        public string UserSignature { get; set; }
        public string UserImageName { get; set; }
        public string UserImageBase64 { get; set; }
        public string BuyerId { get; set; }
        public string BuyerUserName { get; set; }
        public bool IsDeleted { get; set; }
    }

}

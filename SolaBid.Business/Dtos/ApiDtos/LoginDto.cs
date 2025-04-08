using SolaBid.Business.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace SolaBid.Business.Dtos.ApiDtos
{
    public class LoginDto
    {
        [Required]
        [EmailAddress]
        [MaxLength(128)]
        public string UserName { get; set; }
        [Required]
        [MaxLength(150)]
        public string Password { get; set; }
        public bool RememberMe { get; set; }
        public string JWTToken { get; set; }
        public string UserId { get; set; }
        [Required]
        public int SiteId { get; set; }
    }
}

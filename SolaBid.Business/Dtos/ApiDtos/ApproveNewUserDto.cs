using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace SolaBid.Business.Dtos.ApiDtos
{
    public class ApproveNewUserDto
    {
        [Required]
        public string Id { get; set; }
        [Required(ErrorMessage = "The Base Group field is required.")]
        public List<string> GroupIds { get; set; }
        public string BuyerUserName { get; set; }
        public string BuyerId { get; set; }
    }
}

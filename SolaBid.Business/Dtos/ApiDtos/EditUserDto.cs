using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SolaBid.Business.Dtos.ApiDtos
{
    public class EditUserDto
    {
        [Required]
        public string Id { get; set; }
        [Required]
        public string FirstName { get; set; }
        [Required]
        public string LastName { get; set; }
        public string UserImageBase64 { get; set; }
        public string UserImage { get; set; }
        public string UserSignatureBase64 { get; set; }
        public string UserSignature { get; set; }
        [Required(ErrorMessage = "The Base Group field is required.")]
        public List<string> GroupIds { get; set; }
        public string BuyerUserName { get; set; }
        public string BuyerId { get; set; }
    }
}

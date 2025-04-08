using System;
using System.Collections.Generic;
using System.Text;

namespace SolaBid.Business.Dtos.ApiDtos
{
    public class UserFormDataDto
    {
        public string Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string UserImageBase64 { get; set; }
        public string UserSignatureBase64 { get; set; }
        public string UserSignature { get; set; }
        public string UserImage { get; set; }
        public string BuyerId { get; set; }
        public string BuyerUserName { get; set; }
        public List<KeyValueTextBoxingDto> UserGroupsWithAll { get; set; }
        public List<KeyValueTextBoxingDto> UserBuyersWithAll { get; set; }
    }
}

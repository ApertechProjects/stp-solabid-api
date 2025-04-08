using System.Collections.Generic;

namespace SolaBid.Business.Models
{
    public class LoginResult
    {
        public LoginResult()
        {
            ErrorList = new List<string>();
        }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string JWTToken { get; set; }
        public string Image { get; set; }
        public List<string> ErrorList { get; set; }
        public bool IsAuthorized { get; set; }
        public string UserId { get; set; }
    }
}

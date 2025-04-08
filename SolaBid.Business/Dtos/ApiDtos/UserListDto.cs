using SolaBid.Business.Dtos.EntityDtos;
using System;
using System.Collections.Generic;
using System.Text;

namespace SolaBid.Business.Dtos.ApiDtos
{
    public class UserListDto
    {
        public UserListDto()
        {
            Groups = new List<AppRoleDto>();
        }
        public string Id { get; set; }
        public string UserName { get; set; }
        public string FullName { get; set; }
        public DateTime RegDate { get; set; }
        public string Image { get; set; }
        public List<AppRoleDto> Groups { get; set; }
    }
}

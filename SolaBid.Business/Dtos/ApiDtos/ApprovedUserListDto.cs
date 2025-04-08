using SolaBid.Business.Dtos.EntityDtos;
using System;
using System.Collections.Generic;
using System.Text;

namespace SolaBid.Business.Dtos.ApiDtos
{

    public class ApprovedUserListDto
    {
        public ApprovedUserListDto()
        {
            Groups = new List<AppRoleDto>();
        }
        public string Id { get; set; }
        public string UserName { get; set; }
        public string Fullname { get; set; }
        public string Image { get; set; }
        public string BuyerName { get; set; }
        public bool IsSelected { get; set; }
        public string  GroupName { get; set; }
        public List<AppRoleDto> Groups { get; set; }


    }
}

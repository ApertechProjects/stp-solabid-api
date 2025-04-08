using SolaBid.Business.Dtos.EntityDtos;
using SolaBid.Domain.Models.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace SolaBid.Business.Dtos.ApiDtos
{
    public class NewUsersDto
    {
        public NewUsersDto()
        {
            Groups = new List<KeyValueTextBoxingDto>();
            Buyers = new List<KeyValueTextBoxingDto>();
        }
        public string Id { get; set; }
        public string UserName { get; set; }
        public string Fullname { get; set; }
        public string Image { get; set; }
        public List<KeyValueTextBoxingDto> Groups { get; set; }
        public List<KeyValueTextBoxingDto> Buyers { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace SolaBid.Business.Dtos.ApiDtos
{
    public class GroupFormDataDto
    {
        public string Id { get; set; }
        [Required]
        public string GroupName { get; set; }
        public string Description { get; set; }
    }
}

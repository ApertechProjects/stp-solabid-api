using System.ComponentModel.DataAnnotations;

namespace SolaBid.Business.Dtos.ApiDtos
{
    public class LayoutSaveDto
    {
        [Required]
        public string Key { get; set; }
        [Required]
        public string GridLayout { get; set; }
    }
}

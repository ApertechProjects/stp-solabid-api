using System.Collections.Generic;
using System.Text;

namespace SolaBid.Business.Dtos.EntityDtos
{
    public class BuyerDto
    {
        public decimal Id { get; set; }
        public string Username { get; set; }
        public bool IsSelected { get; set; }
    }
}

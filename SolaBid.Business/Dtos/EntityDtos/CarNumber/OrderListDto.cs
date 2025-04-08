using System;

namespace SolaBid.Business.Dtos.EntityDtos.CarNumber
{
    public partial class OrderListDto
    {
        public System.String PoNumber { get; set; }
    }
    public partial class OrderListDto
    {
        public Guid Id { get { return Guid.NewGuid(); } }
    }
}

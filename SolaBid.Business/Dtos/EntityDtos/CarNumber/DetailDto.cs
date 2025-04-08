using System;

namespace SolaBid.Business.Dtos.EntityDtos.CarNumber
{
    public partial class DetailDto
    {
        public System.Int32 CarNumberDetailId { get; set; }
        public System.Int32 CarNumberMainId { get; set; }
        public System.String OrderNumber { get; set; }
    }
    public partial class DetailDto
    {
        public Guid Id { get { return Guid.NewGuid(); } }
    }
}

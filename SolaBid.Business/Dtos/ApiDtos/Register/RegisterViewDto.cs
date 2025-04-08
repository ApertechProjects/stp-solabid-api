using System.Collections.Generic;

namespace SolaBid.Business.Dtos.ApiDtos.Register
{
    public class RegisterViewDto
    {
        public List<PurchaseRegisterDto> DailyRegister { get; set; }
        public List<SourceRegisterDto> DailyRegisterSourcing { get; set; }
        public SourceRegisterDto EmptyRow { get; set; }
        public List<AllRegisterDto> All { get; set; }
        public bool IsChecked { get; }

    }
}

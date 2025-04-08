using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolaBid.Business.Dtos.EntityDtos.CarNumber
{
    public class MainItemsSaveDto
    {
        public MainItemsSaveDto()
        {
            NewItems = new();
            EditedItems = new();
            DeletedMainIds = new();
            DeletedAttachmentIds = new();
        }
        public List<InsertEditItemModelDto> NewItems { get; set; }
        public List<InsertEditItemModelDto> EditedItems { get; set; }
        public List<int> DeletedMainIds { get; set; }
        public List<int> DeletedAttachmentIds { get; set; }
        public bool IsApprove { get; set; }
        public bool IsHold { get; set; }
        public bool IsReject { get; set; }
    }
}

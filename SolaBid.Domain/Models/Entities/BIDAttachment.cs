using System;
using System.Collections.Generic;
using System.Text;

namespace SolaBid.Domain.Models.Entities
{
    public class BIDAttachment
    {
        public int Id { get; set; }
        public string FilePath { get; set; }
        public string FileBaseType { get; set; }
        public string FileName { get; set; }
        public BIDReferance BIDReferance { get; set; }
        public int BIDReferanceId { get; set; }

    }
}

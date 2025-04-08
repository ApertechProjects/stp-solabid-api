using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolaBid.Business.Dtos.ApiDtos
{
    public class UOMItem
    {
        public string PUOM { get; set; }
        public decimal Conv { get; set; }
        public string Description { get; set; }

        public string ItemConcat { get { return $"{PUOM}   {Description} {Conv}"; } }
    }
}

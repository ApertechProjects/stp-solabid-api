using System.Collections.Generic;

namespace SolaBid.Business.Models
{
    public class BIDOperationResult
    {
        public BIDOperationResult()
        {
            ErrorList = new List<string>();
        }
        public List<string> ErrorList { get; set; }
        public bool OperationIsSuccess { get; set; }
        public string BidNumber { get; set; }
        public int ComparisonId { get; set; }
    }
}

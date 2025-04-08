using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace SolaBid.Business.Dtos.ApiDtos.ComparisonChartDtos
{
    public class ComparisonChartMainListData
    {
        public Guid Id { get; set; }
        public string Site { get; set; }
        public string Status { get; set; }
        public string ApprovalStatus { get; set; }
        public bool SingleSource { get; set; }
        public string SingleSourceReason { get; set; }
        public string RequestNo { get; set; }
        public string CommentProcurement { get; set; }
        public decimal CheapestPrice { get; set; }
        public string ComparisonNumber { get; set; }
        public int ComparisonId { get; set; }
        public int ComparisonChartId { get; set; }
        public int RevisionNumber { get; set; }
        public string PoNumber { get; set; }
        public string Requester { get; set; }
        public string Buyer { get; set; }
        public DateTime? RequestDate { get; set; }
        public DateTime? ComparisonDeadline { get; set; }
        public string Destination { get; set; }
        public int ComparsionStatus { get; set; }
        public DateTime? ComparisonCreateDate { get; set; }
        public string FirstApproval { get; set; }
        public DateTime? FirstApprovalDate { get; set; }
        public string SecondApproval { get; set; }
        public DateTime? SecondApprovalDate { get; set; }
        public string ThirdApproval { get; set; }
        public DateTime? ThirdApprovalDate { get; set; }
        public string FourthApproval { get; set; }
        public DateTime? FourthApprovalDate { get; set; }
        public string FifthApproval { get; set; }
        public DateTime? FifthApprovalDate { get; set; }
        public string SixApproval { get; set; }
        public DateTime? SixApprovalDate { get; set; }
        public string SevenApproval { get; set; }
        public DateTime? SevenApprovalDate { get; set; }
        public string EightApproval { get; set; }
        public DateTime? EightApprovalDate { get; set; }
        public string NineApproval { get; set; }
        public DateTime? NineApprovalDate { get; set; }
        public string TenApproval { get; set; }
        public DateTime? TenApprovalDate { get; set; }
    }



    public class ComparisonChartMainListData2
    {
        public Guid Id { get; set; }
        public string Site { get; set; }
        public string Status { get; set; }
        public string ApprovalStatus { get; set; }
        public bool SingleSource { get; set; }
        public string SingleSourceReason { get; set; }
        public string RequestNo { get; set; }
        public string CommentProcurement { get; set; }
        public string CheapestPrice { get; set; }
        public string ComparisonNumber { get; set; }
        public int ComparisonId { get; set; }
        public int ComparisonChartId { get; set; }
        public int RevisionNumber { get; set; }
        public string PoNumber { get; set; }
        public string Requester { get; set; }
        public string Buyer { get; set; }
        public string RequestDate { get; set; }
        public string ComparisonDeadline { get; set; }
        public string Destination { get; set; }
        public int ComparsionStatus { get; set; }
        public string ComparisonCreateDate { get; set; }
        public string FirstApproval { get; set; }
        public string FirstApprovalDate { get; set; }
        public string SecondApproval { get; set; }
        public string SecondApprovalDate { get; set; }
        public string ThirdApproval { get; set; }
        public string ThirdApprovalDate { get; set; }
        public string FourthApproval { get; set; }
        public string FourthApprovalDate { get; set; }
        public string FifthApproval { get; set; }
        public string FifthApprovalDate { get; set; }
        public string SixApproval { get; set; }
        public string SixApprovalDate { get; set; }
        public string SevenApproval { get; set; }
        public string SevenApprovalDate { get; set; }
        public string EightApproval { get; set; }
        public string EightApprovalDate { get; set; }
        public string NineApproval { get; set; }
        public string NineApprovalDate { get; set; }
        public string TenApproval { get; set; }
        public string TenApprovalDate { get; set; }

        public ComparisonChartMainListData2 DeepCopy()
        {
            string json = JsonSerializer.Serialize(this);
            return JsonSerializer.Deserialize<ComparisonChartMainListData2>(json);
        }
    }
}

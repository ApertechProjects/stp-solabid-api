namespace SolaBid.Business.Dtos.Report
{
    public class CostSavedReportModel
    {
        public decimal TotalBidAmount { get; set; }
        public decimal CostSaved { get; set; }
        public string BuyerName { get; set; }
        public string CostSavedPercent
        {
            get
            {
                try
                {
                    return ((CostSaved / (TotalBidAmount + CostSaved)) * 100).ToString("#.##");
                }
                catch (System.Exception ex )
                {
                    return "0";
                }
            }
        }
    }
}

namespace Presupuesto.Models
{
    public class WeeklyReportViewModel
    {
        public decimal entries => transactions.Sum(x => x.entry);
        public decimal bills => transactions.Sum(x => x.bill);
        public decimal total { get; set; }
        public DateTime referenceDate { get; set; }
        public IEnumerable<GetByWeeklyResponse> transactions { get; set; }
    }
}

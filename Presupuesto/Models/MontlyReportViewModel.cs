namespace Presupuesto.Models
{
	public class MontlyReportViewModel
	{
		public IEnumerable<GetByMonthResponse> transactions { get; set; }
		public decimal entries => transactions.Sum(x => x.entry);
		public decimal bills => transactions.Sum(x => x.bill);
		public decimal total => entries - bills;
		public int year { get; set; }
	}
}

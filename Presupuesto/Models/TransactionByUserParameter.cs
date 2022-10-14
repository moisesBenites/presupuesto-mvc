namespace Presupuesto.Models
{
	public class TransactionByUserParameter
	{
		public int userId { get; set; }
		public DateTime initialDate { get; set; }
		public DateTime finalDate { get; set; }
	}
}

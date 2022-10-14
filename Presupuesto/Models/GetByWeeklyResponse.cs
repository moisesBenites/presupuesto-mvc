namespace Presupuesto.Models
{
	public class GetByWeeklyResponse
	{
		public int week { get; set; }
		public int amount { get; set; }
		public OperationTypeEnum operationTypeId{ get; set; }
		public decimal entry { get; set; }
		public decimal bill { get; set; }
		public DateTime initialDate { get; set; }
		public DateTime finalDate { get; set; }
	}
}

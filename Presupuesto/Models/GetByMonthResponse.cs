namespace Presupuesto.Models
{
	public class GetByMonthResponse
	{
		public int month { get; set; }
		public DateTime referenceDate { get; set; }
		public decimal amount { get; set; }
		public decimal entry { get; set; }
		public decimal bill { get; set; }
		public OperationTypeEnum operationTypeId { get; set; }
	}
}

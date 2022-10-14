namespace Presupuesto.Models
{
    public class GetTransactionByAccount
    {
        public int userId { get; set; }
        public int cuentaId { get; set; }
        public DateTime initialDate { get; set; }
        public DateTime finalDate { get; set; }
    }
}

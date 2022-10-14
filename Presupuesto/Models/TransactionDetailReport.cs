namespace Presupuesto.Models
{
    public class TransactionDetailReport
    {
        public DateTime initialDate { get; set; }
        public DateTime finalDate { get; set; }
        public decimal depositBalance => transactions.Sum(t => t.depositBalance);
        public decimal billBalance => transactions.Sum(t => t.billBalance);
        public decimal total => depositBalance - billBalance;
        public IEnumerable<TransactionByDate> transactions { get; set; }

        public class TransactionByDate
        {
            public DateTime date { get; set; }
            public IEnumerable<Transactions> transactions { get; set; }
            public decimal depositBalance => 
                transactions.Where(t => t.operationTypeId == OperationTypeEnum.Entry)
                .Sum(t => t.amount);
            public decimal billBalance =>
               transactions.Where(t => t.operationTypeId == OperationTypeEnum.Bill)
               .Sum(t => t.amount);
        }
    }
}

namespace Presupuesto.Models
{
    public class IndiceCuentasViewModel
    {
        public string tipoCuenta { get; set; }
        public IEnumerable<Cuenta> cuentas { get; set; }
        public decimal balance => cuentas.Sum(x => x.balance);
    }
}

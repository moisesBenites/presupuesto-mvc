using AutoMapper;
using Presupuesto.Models;

namespace Presupuesto.Services
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<Cuenta, CuentaCreacionViewModel>();
            CreateMap<UpdateTransactionViewModel, Transactions>().ReverseMap();
        }
    }
}

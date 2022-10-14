using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Presupuesto.Models;
using Presupuesto.Services;

namespace Presupuesto.Controllers
{
    public class CuentasController : Controller
    {
        private readonly IAccountRepository accountRepository;
        private readonly IUserService userService;
        private readonly IRepositorioCuentas repositorioCuentas;
        private readonly IMapper mapper;
        private readonly ITransactionRepository transactionRepository;
		private readonly IReportService reportService;

		public CuentasController(IAccountRepository accountRepository, 
            IUserService userService, 
            IRepositorioCuentas repositorioCuentas, 
            IMapper Mapper,
            ITransactionRepository transactionRepository,
            IReportService reportService)
        {
            this.accountRepository = accountRepository;
            this.userService = userService;
            this.repositorioCuentas = repositorioCuentas;
            this.mapper = Mapper;
            this.transactionRepository = transactionRepository;
			this.reportService = reportService;
		}

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var userId = userService.GetUserId();
            var cuentasConTipoCuenta = await repositorioCuentas.Find(userId);

            var modelo = cuentasConTipoCuenta
                .GroupBy(x => x.tipoCuenta)
                .Select(group => new IndiceCuentasViewModel
                {
                    tipoCuenta = group.Key,
                    cuentas = group.AsEnumerable()
                }).ToList();

            return View(modelo);
        }


        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var userId = userService.GetUserId();
            var model = new CuentaCreacionViewModel();
            model.TiposCuentas = await GetTypeAccounts(userId);

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Create(CuentaCreacionViewModel cuenta)
        {
            var userId = userService.GetUserId();
            var tipoCuenta = accountRepository.GetAccountById(cuenta.tipoCuentaId, userId);

            if (tipoCuenta is null)
            {
                return RedirectToAction("NotFound", "Home");
            }

            if (!ModelState.IsValid)
            {
                cuenta.TiposCuentas = await GetTypeAccounts(userId);
                return View(cuenta);
            }

            await repositorioCuentas.Create(cuenta);
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Edit(int id)
        {
            var userId = userService.GetUserId();
            var cuenta = await repositorioCuentas.GetById(id, userId);

            if (cuenta is null)
            {
                return RedirectToAction("NotFound", "Home");
            }

            var model = mapper.Map<CuentaCreacionViewModel>(cuenta);
            model.TiposCuentas = await GetTypeAccounts(userId);

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(CuentaCreacionViewModel cuentaEditar)
        {
            var userId = userService.GetUserId();
            var cuenta = await repositorioCuentas.GetById(cuentaEditar.id, userId);
            
            if (cuenta is null)
            {
                return RedirectToAction("NotFound", "Home");
            }

            var tipoCuenta = await accountRepository.GetAccountById(cuentaEditar.tipoCuentaId, userId);
            if (tipoCuenta is null)
            {
                return RedirectToAction("NotFound", "Home");
            }

            await repositorioCuentas.Update(cuentaEditar);

            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var userId = userService.GetUserId();
            var cuenta = await repositorioCuentas.GetById(id, userId);

            if (cuenta is null)
            {
                return RedirectToAction("NotFound", "Home");
            }

            return View(cuenta);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteAccount(int id)
        {
            var userId = userService.GetUserId();
            var cuenta = await repositorioCuentas.GetById(id, userId);

            if (cuenta is null)
            {
                return RedirectToAction("NotFound", "Home");
            }

            await repositorioCuentas.Delete(id);
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Detail(int id, int month, int year)
        {
            int userId = userService.GetUserId();
            var cuenta = await repositorioCuentas.GetById(id, userId);

            if (cuenta is null)
            {
                return RedirectToAction("NotFound", "Home");
            }

            ViewBag.Cuenta = cuenta.nombre;
            var model = await reportService.GetTransactionDetailReportByAccount(userId, cuenta.id, month, year, ViewBag);

            return View(model);
        }

        private async Task<IEnumerable<SelectListItem>> GetTypeAccounts(int userId)
        {
            var account = await accountRepository.GetAccount(userId);
            return account.Select(x => new SelectListItem(x.name, x.id.ToString()));
        }
    }
}

using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Presupuesto.Models;
using Presupuesto.Services;

namespace Presupuesto.Controllers
{
    public class AccountController : Controller
    {
        private readonly IAccountRepository _accountRepository;

        public AccountController(IAccountRepository accountRepository)
        {
            _accountRepository = accountRepository;
        }
        public async Task<IActionResult> Index()
        {
            int UserId = 1;

            var AccountObj = await _accountRepository.GetAccount(UserId);

            return View(AccountObj);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(Account account)
        {
            if (ModelState.IsValid == false)
            {
                return View(account);
            }

            account.userId = 1;
            bool IsAccountExists = await _accountRepository.Exists(account.name, account.userId);

            if (IsAccountExists)
            {
                ModelState.AddModelError(nameof(account.name), $"El nombre '{account.name}' ya existe");

                return View(account);
            }

            await _accountRepository.Create(account);

            return RedirectToAction("Index");   
        }

        [HttpGet]
        public async Task<IActionResult> VerifyExistsAccount(string name)
        {

            int UserId = 1;
            bool IsAccountExists = await _accountRepository.Exists(name, UserId);

            if (IsAccountExists)
            {
                return Json($"El nombre {name} ya existe");
            }

            return Json(true);
        }
    }
}

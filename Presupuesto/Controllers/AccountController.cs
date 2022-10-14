using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Presupuesto.Models;
using Presupuesto.Services;
namespace Presupuesto.Controllers
{
    public class AccountController : Controller
    {
        private readonly IAccountRepository _accountRepository;
        private readonly IUserService _userService;

        public AccountController(IAccountRepository accountRepository, IUserService userService)
        {
            _accountRepository = accountRepository;
            _userService = userService;
        }
        public async Task<IActionResult> Index()
        {
            int UserId = _userService.GetUserId();

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

            account.userId = _userService.GetUserId();
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
        public async Task<ActionResult> Edit(int id)
        {
            int UserId = _userService.GetUserId();
            var account = await _accountRepository.GetAccountById(id, UserId);

            if (account is null)
            {
                return RedirectToAction("NotFound", "Home");
            }

            return View(account);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(Account account)
        {
            int UserId = _userService.GetUserId();
            var accountExits = await _accountRepository.GetAccountById(account.id, UserId);

            if (accountExits is null)
            {
                return RedirectToAction("NotFound", "Home");
            }

            await _accountRepository.Update(account);
            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> VerifyExistsAccount(string name)
        {

            int UserId = _userService.GetUserId();
            bool IsAccountExists = await _accountRepository.Exists(name, UserId);

            if (IsAccountExists)
            {
                return Json($"El nombre {name} ya existe");
            }

            return Json(true);
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            int UserId = _userService.GetUserId();

            var account = await _accountRepository.GetAccountById(id, UserId);

            if (account is  null)
            {
                return RedirectToAction("NotFound", "Home");
            }

            return View(account);
        }

        [HttpPost] 
        public async Task<IActionResult> AccountDelete(int id)
        {
            int UserId = _userService.GetUserId();

            var account = await _accountRepository.GetAccountById(id, UserId);

            if (account is null)
            {
                return RedirectToAction("NotFound", "Home");
            }

            await _accountRepository.Delete(id);
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> Sort([FromBody] int[] ids)
        {
            int UserId = _userService.GetUserId();
            var accounts = await  _accountRepository.GetAccount(UserId);
            var accountIds = accounts.Select(x => x.id);

            var AccountIdsDoNotBelongToUser = ids.Except(accountIds).ToList();

            if (AccountIdsDoNotBelongToUser.Count() > 0)
            {
                return Forbid();
            }

            var sortAccounts = ids.Select((value, index) => new Account() { id = value, orderId = index + 1}).AsEnumerable(); 
            await _accountRepository.Sort(sortAccounts);

            return Ok();
        }
    }
}

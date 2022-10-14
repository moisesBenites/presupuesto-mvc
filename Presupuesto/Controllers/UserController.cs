using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Presupuesto.Models;

namespace Presupuesto.Controllers
{
    public class UserController : Controller
    {
        private readonly UserManager<User> userManager;

        public UserController(UserManager<User> userManager)
        {
            this.userManager = userManager;
        }
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = new User()
            {
                email = model.email
            };
            var response = await userManager.CreateAsync(user, password: model.password);

            if (response.Succeeded)
            {
                return RedirectToAction("Index", "Transaction");
            }
            else
            {
                foreach(var error in response.Errors)
                {
                    ModelState.AddModelError(String.Empty, error.Description);
                }
            }

            return View(model);
        }
    }
}

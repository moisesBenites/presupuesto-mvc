using Microsoft.AspNetCore.Mvc;
using Presupuesto.Models;
using Presupuesto.Services;

namespace Presupuesto.Controllers
{
    public class CategoryController : Controller
    {
        private readonly ICategoryRepository categoryRepository;
        private readonly IUserService userService;

        public CategoryController(ICategoryRepository categoryRepository, IUserService userService)
        {
            this.categoryRepository = categoryRepository;
            this.userService = userService;
        }

        public async Task<IActionResult> Index()
        {
            var userId = userService.GetUserId();
            var categories = await categoryRepository.GetCategories(userId);
            return View(categories);
        }
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(Category category)
        {
            if (!ModelState.IsValid)
            {
                return View(category);
            }

            var userId = userService.GetUserId();
            category.userId = userId;
            await categoryRepository.Create(category);
            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var userId = userService.GetUserId();
            var category = await categoryRepository.GetById(id, userId);

            if (category is null)
            {
                return RedirectToAction("NotFound", "Home");
            }


            return View(category);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(Category category)
        {
            if (!ModelState.IsValid)
            {
                return View(category);
            }

            var userId = userService.GetUserId();
            var existCategory = await categoryRepository.GetById(category.id, userId);

            if (existCategory is null)
            {
                return RedirectToAction("NotFound", "Home");
            }

            category.userId = userId;
            await categoryRepository.Update(category);

            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Delete(int id)
        {
            var userId = userService.GetUserId();
            var category = await categoryRepository.GetById(id, userId);

            if (category is null)
            {
                return RedirectToAction("NotFound", "Home");
            }

            return View(category);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            var userId = userService.GetUserId();
            var category = await categoryRepository.GetById(id, userId);

            if (category is null)
            {
                return RedirectToAction("NotFound", "Home");
            }

            await categoryRepository.Delete(id);
            return RedirectToAction("Index");
        }
    }
}

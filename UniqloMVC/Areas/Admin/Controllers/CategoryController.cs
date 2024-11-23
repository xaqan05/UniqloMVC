using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UniqloMVC.DataAcces;
using UniqloMVC.Models;
using UniqloMVC.ViewModels.Category;

namespace UniqloMVC.Areas.Admin.Controllers
{
    public class CategoryController(UniqloDbContext _context,IWebHostEnvironment _env) : Controller
    {
        public async Task<IActionResult> Index()
        {
            return View(await _context.Categories.ToListAsync());
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(CategoryCreateVM vm)
        {
            if (!ModelState.IsValid) return View();


            Category category = new Category
            {
                Name = vm.Name
            };

            await _context.AddAsync(category);

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
    }
}

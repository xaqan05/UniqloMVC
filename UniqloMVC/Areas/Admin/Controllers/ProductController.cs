using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UniqloMVC.DataAcces;
using UniqloMVC.Extensions;
using UniqloMVC.Models;
using UniqloMVC.ViewModels.Product;

namespace UniqloMVC.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ProductController(UniqloDbContext _context, IWebHostEnvironment _env) : Controller
    {
        public async Task<IActionResult> Index()
        {
            return View(await _context.Products.ToListAsync());
        }


        public async Task<IActionResult> Create()
        {

            ViewBag.Categories = await _context.Categories.Where(x => !x.IsDeleted).ToListAsync();

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(ProductCreateVM vm)
        {
            if (!ModelState.IsValid) return View();


            if (!vm.CoverFile.IsValidType("image"))
            {
                ModelState.AddModelError("File", "File type must be image");
                return View();
            }

            if (!vm.CoverFile.IsValidSize(5 * 1024))
            {
                ModelState.AddModelError("File", "File size must be less than 5MB");
                return View();
            }

            string newFileName = await vm.CoverFile.UploadAsync("wwwroot", "imgs", "products");


            using (Stream stream = System.IO.File.Create(Path.Combine(_env.WebRootPath, "imgs", "products", newFileName)))
            {
                await vm.CoverFile.CopyToAsync(stream);
            }

            Product product = new Product
            {
                Name = vm.Name,
                Description = vm.Description,
                CostPrice = vm.CostPrice,
                SellPrice = vm.SellPrice,
                Quantity = vm.Quantity,
                Discount = vm.Discount,
                CoverImage = newFileName,
                CategoryId = vm.CategoryId
            };

            await _context.AddAsync(product);

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        public IActionResult Update()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Update(int id, ProductCreateVM vm)
        {
            if (!ModelState.IsValid) return View();


            if (!vm.CoverFile.ContentType.StartsWith("image"))
            {
                ModelState.AddModelError("File", "File type must be image");
                return View();
            }

            if (vm.CoverFile.Length > 5 * 1024 * 1024)
            {
                ModelState.AddModelError("File", "File size must be less than 5MB");
                return View();
            }

            var data = await _context.Products.FindAsync(id);

            if (data is null) return View();

            string newFileName = Path.GetRandomFileName() + Path.GetExtension(vm.CoverFile.FileName);

            using (Stream stream = System.IO.File.Create(Path.Combine(_env.WebRootPath, "imgs", "products", newFileName)))
            {
                await vm.CoverFile.CopyToAsync(stream);
            }

            data.Name = vm.Name;
            data.Description = vm.Description;
            data.CostPrice = vm.CostPrice;
            data.SellPrice = vm.SellPrice;
            data.Quantity = vm.Quantity;
            data.Discount = vm.Discount;
            data.CategoryId = vm.CategoryId;

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Hide(int id, ProductCreateVM vm)
        {
            var data = await _context.Products.FindAsync(id);

            if (data is null) return View();

            data.IsDeleted = true;
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Show(int id, ProductCreateVM vm)
        {
            var data = await _context.Products.FindAsync(id);

            if (data is null) return View();

            data.IsDeleted = false;
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
    }
}

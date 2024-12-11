using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UniqloMVC.DataAcces;
using UniqloMVC.Enums;
using UniqloMVC.Extensions;
using UniqloMVC.Models;
using UniqloMVC.ViewModels.Product;

namespace UniqloMVC.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = nameof(Roles.Admin))]
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

            if (vm == null)
            {
                ModelState.AddModelError("File", "File is required or file size exceeds the limit.");
                return View(vm);
            }


            if (vm.OtherFiles != null && vm.OtherFiles.Any())
            {
                if (!vm.OtherFiles.All(x => x.IsValidType("image")))
                {
                    var fileNames = vm.OtherFiles.Where(x => !x.IsValidType("image")).Select(x => x.FileName);
                    ModelState.AddModelError("OtherFiles", string.Join(",", fileNames) + "are(is) not an image");
                }
                if (!vm.OtherFiles.All(x => x.IsValidSize(5*1024)))
                {
                    var fileNames = vm.OtherFiles.Where(x => !x.IsValidSize(5*1024)).Select(x => x.FileName);
                    ModelState.AddModelError("OtherFiles", string.Join(",", fileNames) + "must be less than 5MB");
                }
            }
            if (vm.CoverFile != null)
            {
                if (!vm.CoverFile.IsValidType("image"))
                    ModelState.AddModelError("File", "File type must be image");
                if (!vm.CoverFile.IsValidSize(5 * 1024))
                    ModelState.AddModelError("File", "File size must be less than 5MB");
            }

            if (!ModelState.IsValid)
            {
                ViewBag.Categories = await _context.Categories.Where(x => !x.IsDeleted).ToListAsync();
                return View();
            }


            Product product = new Product
            {
                Name = vm.Name,
                Description = vm.Description,
                CostPrice = vm.CostPrice,
                SellPrice = vm.SellPrice,
                Quantity = vm.Quantity,
                Discount = vm.Discount,
                CoverImage = await vm.CoverFile!.UploadAsync(_env.WebRootPath, "imgs", "products"),
                CategoryId = vm.CategoryId

            };

            List<ProductImage> images = [];

            foreach (var item in vm.OtherFiles ?? [])
            {
                string fileName = await item.UploadAsync(_env.WebRootPath, "imgs", "products");
                images.Add(new ProductImage
                {
                    FileUrl = fileName,
                    Product = product,

                });
            }

            await _context.ProductImages.AddRangeAsync(images);

            await _context.Products.AddAsync(product);

            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = "Mehsul ugurla elave edildi.";

            return View();
        }

        public async Task<IActionResult> Update(int? id)
        {
            if (!id.HasValue) return BadRequest();


            var data = await _context.Products.Where(x => x.Id == id.Value).Select(x => new ProductUpdateVM
            {
                CategoryId = x.CategoryId,
                Name = x.Name,
                Description = x.Description,
                CostPrice = x.CostPrice,
                SellPrice = x.SellPrice,
                Quantity = x.Quantity,
                Discount = x.Discount,
                CoverFileUrl = x.CoverImage,
                OtherFileUrls = x.Images.Select(y => y.FileUrl)
            }).FirstOrDefaultAsync();

            if (data is null) return NotFound();

            ViewBag.Categories = await _context.Categories.Where(x => !x.IsDeleted).ToListAsync();

            return View(data);
        }

        [HttpPost]
        public async Task<IActionResult> Update(int? id, ProductUpdateVM vm)
        {
            if (!id.HasValue) return BadRequest();
            var data = await _context.Products.FindAsync(id);
            if (data is null) return View();
            if (!ModelState.IsValid)
            {
                ViewBag.Categories = await _context.Categories.Where(x => !x.IsDeleted).ToListAsync();
                return View(vm);
            }

            if (vm.CoverFile != null)
            {
                if (!vm.CoverFile.IsValidType("image"))
                {
                    ModelState.AddModelError("CoverFile", "File type must be image");
                    ViewBag.Categories = await _context.Categories.Where(x => !x.IsDeleted).ToListAsync();
                    return View(vm);
                }

                if (!vm.CoverFile.IsValidSize(5 * 1024))
                {
                    ModelState.AddModelError("CoverFile", "File size must be less than 5MB");
                    ViewBag.Categories = await _context.Categories.Where(x => !x.IsDeleted).ToListAsync();
                    return View(vm);
                }

                string oldFilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "imgs", "products", data.CoverImage);

                if (System.IO.File.Exists(oldFilePath))
                {
                    System.IO.File.Delete(oldFilePath);
                }

                string newFileName = await vm.CoverFile.UploadAsync("wwwroot", "imgs", "products");
                data.CoverImage = newFileName;
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

        public async Task<IActionResult> Delete(int? id)
        {
            if (!id.HasValue) return BadRequest();

            var data = await _context.Products.FindAsync(id);

            if (data is null) return NotFound();

            string oldFilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "imgs", "products", data.CoverImage);

            if (System.IO.File.Exists(oldFilePath))
            {
                System.IO.File.Delete(oldFilePath);
            }

            _context.Products.Remove(data);
            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = "Mehsul ugurla silindi.";

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Hide(int? id)
        {
            if (!id.HasValue) return BadRequest();

            var data = await _context.Products.FindAsync(id);

            if (data is null) return View();

            data.IsDeleted = true;
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Show(int? id)
        {
            if (!id.HasValue) return BadRequest();

            var data = await _context.Products.FindAsync(id);

            if (data is null) return View();

            data.IsDeleted = false;
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
    }
}

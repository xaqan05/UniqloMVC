﻿using Microsoft.AspNetCore.Mvc;
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
            if (!ModelState.IsValid) return View(vm);


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
            ViewBag.Categories = await _context.Categories.Where(x => !x.IsDeleted).ToListAsync();

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Update(int? id)
        {
            if (!id.HasValue) return BadRequest();

            var data = await _context.Products.FindAsync(id);
            if (data is null) return NotFound();

            ProductUpdateVM vm = new();

            vm.Name = data.Name;
            vm.Description = data.Description;
            vm.CostPrice = data.CostPrice;
            vm.SellPrice = data.SellPrice;
            vm.Quantity = data.Quantity;
            vm.Discount = data.Discount;
            vm.CategoryId = data.CategoryId;

            ViewBag.Categories = await _context.Categories.Where(x => !x.IsDeleted).ToListAsync();

            return View(vm);
        }

        [HttpPost]
        public async Task<IActionResult> Update(int? id, ProductUpdateVM vm)
        {
            if (!id.HasValue) return BadRequest();
            var data = await _context.Products.FindAsync(id);
            if (data is null) return View();

            if (!ModelState.IsValid) return View(vm);

            if (vm.CoverFile != null)
            {
                if (!vm.CoverFile.IsValidType("image"))
                {
                    ModelState.AddModelError("File", "File type must be image");
                    return View(vm);
                }

                if (!vm.CoverFile.IsValidSize(5 * 1024))
                {
                    ModelState.AddModelError("File", "File size must be less than 5MB");
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

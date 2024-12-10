using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UniqloMVC.DataAcces;
using UniqloMVC.Enums;
using UniqloMVC.Extensions;
using UniqloMVC.Models;
using UniqloMVC.ViewModels.Slider;

namespace UniqloMVC.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = nameof(Roles.Admin))]
    public class SliderController(UniqloDbContext _context) : Controller
    {
        public async Task<IActionResult> Index()
        {
            return View(await _context.Sliders.ToListAsync());
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]

        public async Task<IActionResult> Create(SliderCreateVM vm)
        {
            if (vm == null)
            {
                ModelState.AddModelError("File", "File is required or file size exceeds the limit.");
                return View(vm);
            }

            if (!ModelState.IsValid) return View();
            
            if (!vm.File.IsValidType("image"))
            {
                ModelState.AddModelError("File", "File type must be image");
                return View();
            }

            if (!vm.File.IsValidSize(5 * 1024))
            {
                ModelState.AddModelError("File", "File size must be less than 5MB");
                return View();
            }
            string newFileName = await vm.File.UploadAsync("wwwroot", "imgs", "sliders");

            Slider slider = new Slider
            {
                ImageUrl = newFileName,
                Link = vm.Link,
                Subtitle = vm.Subtitle,
                Title = vm.Title,
            };
            await _context.AddAsync(slider);

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Update(int? id)
        {
            if (!id.HasValue) return BadRequest();

            var data = await _context.Sliders.FindAsync(id);

            if (data is null) return NotFound();

            SliderUpdateVM vm = new();

            vm.Title = data.Title;
            vm.Subtitle = data.Subtitle;
            vm.Link = data.Link;


            return View(vm);
        }

        [HttpPost]
        public async Task<IActionResult> Update(int? id, SliderUpdateVM vm)
        {
            if (!id.HasValue) return BadRequest();
            var data = await _context.Sliders.FindAsync(id);
            if (data is null) return NotFound();
            if (!ModelState.IsValid) return View(vm);


            if (vm.File != null)
            {

                if (!vm.File.IsValidType("image"))
                {
                    ModelState.AddModelError("File", "File type must be image");
                    return View(vm);
                }

                if (!vm.File.IsValidSize(5 * 1024))
                {
                    ModelState.AddModelError("File", "File size must be less than 5MB");
                    return View(vm);
                }

                string oldFilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "imgs", "sliders", data.ImageUrl);

                if (System.IO.File.Exists(oldFilePath))
                {
                    System.IO.File.Delete(oldFilePath);
                }

                string newFileName = await vm.File.UploadAsync("wwwroot", "imgs", "sliders");
                data.ImageUrl = newFileName;
            }

            data.Link = vm.Link;
            data.Subtitle = vm.Subtitle;
            data.Title = vm.Title;

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (!id.HasValue) return BadRequest();
            var data = await _context.Sliders.FindAsync(id);

            if (data is null) return View();


            string oldFilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "imgs", "sliders", data.ImageUrl);

            if (System.IO.File.Exists(oldFilePath))
            {
                System.IO.File.Delete(oldFilePath);
            }

            _context.Sliders.Remove(data);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Hide(int? id)
        {
            if (!id.HasValue) return BadRequest();

            var data = await _context.Sliders.FindAsync(id);

            if (data is null) return View();

            data.IsDeleted = true;
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Show(int? id)
        {
            if (!id.HasValue) return BadRequest();

            var data = await _context.Sliders.FindAsync(id);

            if (data is null) return View();

            data.IsDeleted = false;
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
    }
}

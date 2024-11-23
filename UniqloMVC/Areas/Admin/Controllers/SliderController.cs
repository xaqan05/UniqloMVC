using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UniqloMVC.DataAcces;
using UniqloMVC.Extensions;
using UniqloMVC.Models;
using UniqloMVC.ViewModels.Slider;

namespace UniqloMVC.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class SliderController(UniqloDbContext _context, IWebHostEnvironment _env) : Controller
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
            if (!ModelState.IsValid) return View();


            if (!vm.File.IsValidType("image"))
            {
                ModelState.AddModelError("File", "File type must be image");
                return View();
            }

            if (!vm.File.IsValidSize(5*1024))
            {
                ModelState.AddModelError("File", "File size must be less than 5MB");
                return View();
            }

            string newFileName = await vm.File.UploadAsync("wwwroot","imgs","products");


            using (Stream stream = System.IO.File.Create(Path.Combine(_env.WebRootPath, "imgs", "sliders", newFileName)))
            {
                await vm.File.CopyToAsync(stream);
            }

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

        public IActionResult Update()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Update(int id,SliderCreateVM vm)
        {
            if (!ModelState.IsValid) return View();


            if (!vm.File.ContentType.StartsWith("image"))
            {
                ModelState.AddModelError("File", "File type must be image");
                return View();
            }

            if (vm.File.Length > 5 * 1024 * 1024)
            {
                ModelState.AddModelError("File", "File size must be less than 5MB");
                return View();
            }

            var data = await _context.Sliders.FindAsync(id);

            if (data is null) return View();

            string newFileName = Path.GetRandomFileName() + Path.GetExtension(vm.File.FileName);

            using (Stream stream = System.IO.File.Create(Path.Combine(_env.WebRootPath, "imgs", "sliders", newFileName)))
            {
                await vm.File.CopyToAsync(stream);
            }

            data.ImageUrl = newFileName;
            data.Link = vm.Link;
            data.Subtitle = vm.Subtitle;
            data.Title = vm.Title;

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id is null) return BadRequest();
            var data = await _context.Sliders.FindAsync(id);

            if (data is null) return View();


            _context.Sliders.Remove(data);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Hide(int id,SliderCreateVM vm)
        {
            var data = await _context.Sliders.FindAsync(id);

            if (data is null) return View();

            data.IsDeleted = true;
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Show(int id,SliderCreateVM vm)
        {
            var data = await _context.Sliders.FindAsync(id);

            if (data is null) return View();

            data.IsDeleted = false;
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
    }
}

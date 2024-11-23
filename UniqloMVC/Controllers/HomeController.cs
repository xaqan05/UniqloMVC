using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UniqloMVC.DataAcces;
using UniqloMVC.ViewModels.Slider;

namespace UniqloMVC.Controllers
{
    public class HomeController(UniqloDbContext _context) : Controller
    {
        public async Task<IActionResult> Index()
        {
            var datas = await _context.Sliders
                .Where(x=> !x.IsDeleted)
                .Select(x => new SliderItemVM
            {
                ImageUrl = x.ImageUrl,
                Link = x.Link,
                Title = x.Title,
                Subtitle = x.Subtitle
            }).ToListAsync();

            return View(datas);
        }

        public IActionResult About()
        {
            return View();
        }

        public IActionResult Contact()
        {
            return View();
        }
    }
}

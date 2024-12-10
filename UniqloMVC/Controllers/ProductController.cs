using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UniqloMVC.DataAcces;
using UniqloMVC.ViewModels.Product;

namespace UniqloMVC.Controllers
{
    public class ProductController(UniqloDbContext _context) : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (!id.HasValue) return BadRequest();

            var data = await _context.Products.Where(x => x.Id == id).Include(x=> x.Images).FirstOrDefaultAsync();

            if(data is null) return NotFound();

            ProductDetailsVM vm = new ProductDetailsVM
            {
                ProductName = data.Name,
                ProductDescription = data.Description,
                SellPrice = data.SellPrice,
                Discount = data.Discount,
                CoverImageUrl = data.CoverImage,
                OtherFileUrls = data.Images
            };


            return View(vm);
        }
    }
}

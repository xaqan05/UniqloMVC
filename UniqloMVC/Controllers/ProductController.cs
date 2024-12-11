using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
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

            var data = await _context.Products
                .Where(x => x.Id == id)
                .Include(x => x.Images)
                .Include(x => x.Rating)
                .FirstOrDefaultAsync();

            if (data is null) return NotFound();

            ProductDetailsVM vm = new ProductDetailsVM
            {
                ProductName = data.Name,
                ProductDescription = data.Description,
                SellPrice = data.SellPrice,
                Discount = data.Discount,
                CoverImageUrl = data.CoverImage,
                OtherFileUrls = data.Images
            };

            ViewBag.Rating = 5;
            if (User.Identity?.IsAuthenticated ?? false)
            {
                string userId = User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)!.Value;

                int rating = await _context.ProductRatings.Where(x => x.UserId == userId && x.ProductId == id).Select(x => x.Rating).FirstOrDefaultAsync();
                ViewBag.Rating = rating == 0 ? 5 : rating;
            }

            return View(data);
        }

        public async Task<IActionResult> Rating(int productId, int rating)
        {
            string userId = User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)!.Value;
            var data = await _context.ProductRatings.Where(x => x.UserId == userId && x.ProductId == productId).FirstOrDefaultAsync();

            if (data is null)
            {
                await _context.ProductRatings.AddAsync(new Models.ProductRating
                {
                    UserId = userId,
                    ProductId = productId,
                    Rating = rating
                });
            }
            else
            {
                data.Rating = rating;
            }
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Details), new { Id = productId });
        }

        public async Task<IActionResult> Comment(int productId, string content)
        {
            string userId = User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)!.Value;
            var data = await _context.Comments.Where(x => x.UserId == userId && x.ProductId == productId).FirstOrDefaultAsync();

            if (data is null)
            {
                await _context.Comments.AddAsync(new Models.Comment
                {
                    UserId = userId,
                    ProductId = productId,
                    Content = content
                });
            }
            else
            {
                data.Content = content;
            }

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Details), new { Id = productId });
        }
    }
}

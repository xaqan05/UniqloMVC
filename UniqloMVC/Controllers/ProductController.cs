using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Text.Json;
using UniqloMVC.DataAcces;
using UniqloMVC.Models;
using UniqloMVC.ViewModels.Basket;
using UniqloMVC.ViewModels.Comment;
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
                .Include(x => x.Comments)
                .ThenInclude(c => c.User)
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

        public async Task<IActionResult> Comment(int productId, CommentCreateVM vm)
        {
            string userId = User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)!.Value;

            var data = await _context.Comments.Where(x => x.UserId == userId && x.ProductId == productId).FirstOrDefaultAsync();


            Comment comment = new Comment
            {
                Content = vm.Content,
                FullName = vm.FullName,
                Email = vm.Email,
                ProductId = productId,
                UserId = userId,
            };

            await _context.Comments.AddAsync(comment);

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Details), new { Id = productId });
        }


        public async Task<IActionResult> RemoveComment(int? id)
        {
            if (!id.HasValue) return BadRequest();
            string userId = User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)!.Value;

            var data = await _context.Comments.FindAsync(id);

            ViewBag.UserId = userId;

            if (data is null) return NotFound();

            _context.Comments.Remove(data);

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Details), new { Id = data.ProductId });
        }

        public async Task<IActionResult> AddBasket(int id)
        {
            if (!await _context.Products.AnyAsync(x => x.Id == id))
                return NotFound();

            var basketItems = JsonSerializer.Deserialize<List<BasketProductItemVM>>(Request.Cookies["basket"] ?? "[]");

            var item = basketItems.FirstOrDefault(x => x.Id == id);

            if (item is null)
            {
                item = new BasketProductItemVM
                {
                    Id = id,
                    Count = 1
                };
                basketItems.Add(item);
            }
            item.Count++;

            Response.Cookies.Append("basket", JsonSerializer.Serialize(basketItems));

            return View();
        }
    }
}

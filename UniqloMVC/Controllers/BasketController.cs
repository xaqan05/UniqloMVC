using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using UniqloMVC.DataAcces;
using UniqloMVC.ViewModels.Basket;

namespace UniqloMVC.Controllers
{
    public class BasketController(UniqloDbContext _context) : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
        public async Task<IActionResult> AddBasket(int id)
        {
            if (!await _context.Products.AnyAsync(x => x.Id == id))
                return NotFound();

            var basketItems = JsonSerializer.Deserialize<List<BasketProductItemVM>>(Request.Cookies["basket"] ?? "[]");

            var item = basketItems!.FirstOrDefault(x => x.Id == id);

            if (item is null)
            {
                item = new BasketProductItemVM
                {
                    Id = id,
                    Count = 1
                };
                basketItems!.Add(item);
            }
            item.Count++;

            Response.Cookies.Append("basket", JsonSerializer.Serialize(basketItems));

            return Ok();
        }
    }
}

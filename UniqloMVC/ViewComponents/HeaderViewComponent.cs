using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using UniqloMVC.DataAcces;
using UniqloMVC.ViewModels.Basket;

namespace UniqloMVC.ViewComponents
{
    public class HeaderViewComponent(UniqloDbContext _context) : ViewComponent
    {
        public async Task<IViewComponentResult> InvokeAsync()
        {
            var basketIds = JsonSerializer.Deserialize<List<BasketProductItemVM>>(Request.Cookies["basket"] ?? "[]");

            var products = await _context.Products
                .Where(x => basketIds!.Select(y => y.Id)
                .Any(y => y == x.Id))
                .Select(x => new ProductBasketItemVM
                {
                    Id = x.Id,
                    Name = x.Name,
                    Discount = x.Discount,
                    ImageUrl = x.CoverImage,
                    SellPrice = x.SellPrice
                }).ToListAsync();


            foreach (var item in products)
            {
                item.Count = basketIds!.FirstOrDefault(x => x.Id == item.Id)!.Count;
            }
            return View(products);
        }
    }
}

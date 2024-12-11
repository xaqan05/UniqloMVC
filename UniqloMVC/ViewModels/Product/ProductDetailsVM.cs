using UniqloMVC.Models;

namespace UniqloMVC.ViewModels.Product
{
    public class ProductDetailsVM
    {
        public int Id { get; set; }
        public string ProductName { get; set; } = null!;
        public string ProductDescription { get; set; } = null!;

        public decimal SellPrice { get; set; }
        public int Discount { get; set; }
        public string CoverImageUrl { get; set; } = null!;
        public IEnumerable<ProductImage>? OtherFileUrls { get; set; }

    }
}

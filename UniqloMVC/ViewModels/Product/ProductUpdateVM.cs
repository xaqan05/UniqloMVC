using System.ComponentModel.DataAnnotations;

namespace UniqloMVC.ViewModels.Product
{
    public class ProductUpdateVM
    {
        [MaxLength(32, ErrorMessage = "Name must be less than 32"), Required(ErrorMessage = "Name is required")]
        public string Name { get; set; } = null!;

        [MaxLength(256, ErrorMessage = "Description must be less than 256"), Required(ErrorMessage = "Description is required")]
        public string Description { get; set; } = null!;

        [Required(ErrorMessage = "CostPrice is required")]
        public decimal CostPrice { get; set; }

        [Required(ErrorMessage = "SellPrice is required")]
        public decimal SellPrice { get; set; }

        [Required(ErrorMessage = "Quantity is required")]
        public int Quantity { get; set; }

        public int Discount { get; set; }
        public string CoverFileUrl { get; set; } = null!;

        public IEnumerable<string>? OtherFileUrls { get; set; }
        public IFormFile? CoverFile { get; set; }

        public int? CategoryId { get; set; }
    }
}

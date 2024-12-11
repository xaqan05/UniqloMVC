﻿namespace UniqloMVC.Models
{
    public class Product : BaseEntity
    {
        public string Name { get; set; } = null!;
        public string Description { get; set; } = null!;

        public decimal CostPrice { get; set; }
        public decimal SellPrice { get; set; }

        public int Quantity { get; set; }

        public int Discount { get; set; }

        public string CoverImage { get; set; } = null!;

        public int? CategoryId { get; set; }

        public Category? Category { get; set; }

        public IEnumerable<ProductImage>? Images { get; set; }

        public ICollection<Tag>? Tags { get; set; }

        public ICollection<ProductRating> Rating { get; set; }

        public ICollection<Comment> Comments { get; set; }
    }
}

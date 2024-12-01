﻿namespace UniqloMVC.Models
{
    public class ProductImage : BaseEntity
    {
        public string FileUrl { get; set; }

        public int ProductId { get; set; }
        public Product Product { get; set; }
    }
}

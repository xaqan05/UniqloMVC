﻿namespace UniqloMVC.Models
{
    public class Basket : BaseEntity
    {
        public string? UserId { get; set; }
        public User? User { get; set; }
    }
}

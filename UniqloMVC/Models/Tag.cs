namespace UniqloMVC.Models
{
    public class Tag : BaseEntity
    {
        public string Name { get; set; } = null!;
        public ICollection<Product>? Products { get; set; }
    }
}

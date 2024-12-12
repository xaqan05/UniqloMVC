namespace UniqloMVC.Models
{
    public class Comment : BaseEntity
    {
        public string FullName { get; set; } = null!;
        public string Email { get; set; } = null!;

        public string Content { get; set; } = null!;

        public int? ProductId { get; set; }
        public Product? Product { get; set; }

        public string? UserId { get; set; }
        public User? User { get; set; }
    }
}

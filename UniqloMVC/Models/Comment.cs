namespace UniqloMVC.Models
{
    public class Comment : BaseEntity
    {
        public string Content { get; set; }

        public int ProductId { get; set; }
        public Product Product { get; set; }

        public string UserId { get; set; }
        public User User { get; set; }
    }
}

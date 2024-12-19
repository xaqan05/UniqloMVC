namespace UniqloMVC.Models
{
    public class BasketItems : BaseEntity
    {
        public int? BasketId { get; set; }
        public Basket? Basket { get; set; }

        public string? UserId { get; set; }
        public User? User { get; set; }
    }
}

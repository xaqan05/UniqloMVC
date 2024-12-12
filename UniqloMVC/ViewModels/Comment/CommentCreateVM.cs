using System.ComponentModel.DataAnnotations;

namespace UniqloMVC.ViewModels.Comment
{
    public class CommentCreateVM
    {

        [Required]
        public string FullName { get; set; } = null!;

        [Required]
        public string Email { get; set; } = null!;

        [Required]
        public string Content { get; set; } = null!;
    }
}

using System.ComponentModel.DataAnnotations;
using UniqloMVC.Models;

namespace UniqloMVC.ViewModels.Slider
{
    public class SliderCreateVM : BaseEntity
    {
        [MaxLength(32, ErrorMessage ="Title must be less than 32"), Required(ErrorMessage ="Title is required")] 
        public string Title { get; set; } = null!;

        [MaxLength(64, ErrorMessage = "Title must be less than 32"), Required(ErrorMessage = "Subtitle is required")]
        public string Subtitle { get; set; } = null!;

        public string? Link { get; set; }

        [Required]
        public IFormFile File { get; set; } = null!;
    }
}

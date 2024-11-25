using UniqloMVC.ViewModels.Product;
using UniqloMVC.ViewModels.Slider;

namespace UniqloMVC.ViewModels.Common
{
    public class HomeVM
    {
        public IEnumerable<ProductItemVM> Products { get; set; }

        public IEnumerable<SliderItemVM> Sliders { get; set; }
    }
}

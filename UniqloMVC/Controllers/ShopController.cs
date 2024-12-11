using Microsoft.AspNetCore.Mvc;

namespace UniqloMVC.Controllers
{
    public class ShopController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}

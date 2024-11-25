using Microsoft.AspNetCore.Mvc;

namespace UniqloMVC.Controllers
{
    public class AboutController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}

﻿using Microsoft.AspNetCore.Mvc;

namespace UniqloMVC.Controllers
{
    public class ContactController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}

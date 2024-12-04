using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using UniqloMVC.Models;
using UniqloMVC.ViewModels.Auths;

namespace UniqloMVC.Controllers
{
    public class AccountController(UserManager<User> _userManager, SignInManager<User> _signInManager) : Controller
    {
        public IActionResult Register()
        {
            return View();
        }


        [HttpPost]
        public async Task<IActionResult> Register(UserCreateVM vm)
        {
            if (!ModelState.IsValid)
                return View();

            User user = new User
            {
                Email = vm.Email,
                FullName = vm.FullName,
                UserName = vm.Username,
                ProfileImageUrl = "photo.jpg",
            };

            var result = await _userManager.CreateAsync(user, vm.Password);

            if (!result.Succeeded)
            {
                foreach (var item in result.Errors)
                {
                    ModelState.AddModelError("", item.Description);
                }
                return View();
            }

            return View();
        }

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]

        public async Task<IActionResult> Login(LoginVM vm)
        {
            if (!ModelState.IsValid) return View();


            User? user = null;

            if (vm.UsernameOrEmail.Contains('@'))
                user = await _userManager.FindByEmailAsync(vm.UsernameOrEmail);
            else
                user = await _userManager.FindByNameAsync(vm.UsernameOrEmail);

            if (user is null)
            {
                ModelState.AddModelError("", "Username or Password is wrong");
                return View();
            }


            var result = await _signInManager.PasswordSignInAsync(user, vm.Password, vm.RememberMe, true);

            if (!result.Succeeded)
            {
                if (result.IsNotAllowed)
                    ModelState.AddModelError("","Usernae or Password is wrong");
                if(result.IsLockedOut)
                    ModelState.AddModelError("", "Wait until" + user.LockoutEnd!.Value.ToString("yyyy-MM-dd HH:mm:ss"));

                return View();
            }

            return RedirectToAction("Index","Home");
        }
    }
}

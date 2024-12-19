using Microsoft.AspNetCore.Mvc;
using System.Net.Mail;
using System.Net;
using Microsoft.AspNetCore.Identity;
using UniqloMVC.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;
using UniqloMVC.ViewModels.ResetPassword;

namespace UniqloMVC.Controllers
{
    public class ResetPasswordController(UserManager<User> _userManager) : Controller
    {
        public IActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ForgotPassword(ResetPasswordVM vm)
        {
            if (vm is null)
            {
                ModelState.AddModelError("", "Email is required.");
                return View();
            }
            var user = await _userManager.Users.FirstOrDefaultAsync(u => u.Email == vm.Email);
            if (user == null)
            {
                ModelState.AddModelError("Email", "User not found");
                return View();
            }

            Random random = new Random();
            string randomCode = random.Next(1000, 10000).ToString();

            user.VerificationCode = randomCode;
            user.CodeExpiryTime = DateTime.UtcNow.AddMinutes(10);
            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    Console.WriteLine($"Update error: {error.Description}");
                }
                ModelState.AddModelError("", "Failed to update user. Please try again.");
                return View();
            }


            SmtpClient smtp = new SmtpClient
            {
                Host = "smtp.gmail.com",
                Port = 587,
                EnableSsl = true,
                Credentials = new NetworkCredential("xaganmi-bp215@code.edu.az", "irfr onbs nqdk mjcz")
            };

            MailAddress from = new MailAddress("xaganmi-bp215@code.edu.az", "Uniqlo");

            MailAddress to = new(user.Email);

            MailMessage message = new MailMessage(from, to)
            {
                Subject = "Verification Code",
                Body = $"Sizin dogrulama kodunuz: {user.VerificationCode}"
            };

            smtp.Send(message);

            HttpContext.Session.SetString("UserEmail", user.Email);

            return RedirectToAction(nameof(VerifyCode));
        }

        public IActionResult VerifyCode()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> VerifyCode(VerifyCodeVM vm)
        {
            var userEmail = HttpContext.Session.GetString("UserEmail");
            if (userEmail is null) return RedirectToAction(nameof(ForgotPassword));

            if (!ModelState.IsValid) return View();

            var user = await _userManager.Users.FirstOrDefaultAsync(x => x.Email == userEmail);

            if (user == null) return View();

            if (user.VerificationCode != vm.Code || user.CodeExpiryTime < DateTime.UtcNow)
            {
                return View();
            }
            HttpContext.Session.SetString("UserEmail", user.Email);
            return RedirectToAction(nameof(ChangePassword));
        }

        public IActionResult ChangePassword()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ChangePassword(string password)
        {
            var userEmail = HttpContext.Session.GetString("UserEmail");
            if ((userEmail is null)) return RedirectToAction(nameof(ForgotPassword));

            var user = await _userManager.Users.FirstOrDefaultAsync(x => x.Email == userEmail);
            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            await _userManager.ResetPasswordAsync(user, token, password);
            HttpContext.Session.Remove("UserEmail");


            return View();
        }
    }
}

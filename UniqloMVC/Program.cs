using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using UniqloMVC.DataAcces;
using UniqloMVC.Models;

namespace UniqloMVC
{
    public class Program
    {
        public static void Main(string[] args)
        {

            var builder = WebApplication.CreateBuilder(args);
            builder.Services.AddControllersWithViews();

            builder.Services.Configure<FormOptions>(options =>
            {
                options.MultipartBodyLengthLimit = 52428800;
            });


            builder.Services.AddDbContext<UniqloDbContext>(opt =>
            {
                opt.UseSqlServer(builder.Configuration.GetConnectionString("MSSql"));
            });

            builder.Services.AddIdentity<User, IdentityRole>(opt =>
            {
                opt.Password.RequireNonAlphanumeric = true;
                opt.Password.RequireDigit = true;
                opt.Password.RequireLowercase = true;
                opt.Password.RequireUppercase = true;
                opt.Lockout.MaxFailedAccessAttempts = 5;
                opt.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
            }).AddDefaultTokenProviders().AddEntityFrameworkStores<UniqloDbContext>();

            var app = builder.Build();

            app.UseStaticFiles();

            app.MapControllerRoute(
                name: "register",
                pattern: "register",
                defaults: new { controller = "Account", action = "Register" }
                );

            app.MapControllerRoute(
            name: "areas",
            pattern: "{area:exists}/{controller=Dashboard}/{action=Index}/{id?}"
            );

            app.MapControllerRoute("default", "{controller=Home}/{action=Index}/{id?}");

            app.Run();
        }
    }
}

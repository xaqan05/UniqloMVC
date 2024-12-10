﻿using Microsoft.AspNetCore.Identity;
using UniqloMVC.Enums;
using UniqloMVC.Models;

namespace UniqloMVC.Extensions
{
    public static class SeedExtension
    {
        public async static void UseUserSeed(this IApplicationBuilder app)
        {
            using (var scope = app.ApplicationServices.CreateScope())
            {
                var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();
                var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();


                if (!roleManager.Roles.Any())
                {
                    foreach (Roles role in Enum.GetValues(typeof(Roles)))
                    {
                        await roleManager.CreateAsync(new IdentityRole(role.ToString()));
                    }
                }

                if (!userManager.Users.Any(x => x.NormalizedUserName == "ADMIN"))
                {
                    User admin = new User
                    {
                        FullName = "admin",
                        UserName = "admin",
                        Email = "admin@gmail.com",
                        ProfileImageUrl = "admin.jpg"
                    };

                    await userManager.CreateAsync(admin,"admin253admin");

                    await userManager.AddToRoleAsync(admin,nameof(Roles.Admin));
                }

            }
        }


    }
}


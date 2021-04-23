using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ValdymoSistema.Data.Entities;

namespace ValdymoSistema.Data
{
    public static class DatabaseSeeder
    {
        public static async Task CreateRoles(IServiceProvider serviceProvider, IConfiguration config)
        {
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = serviceProvider.GetRequiredService<UserManager<User>>();

            string[] roleNames = { "Administrator", "Operator", "Worker"};
            IdentityResult roleResult;

            foreach (var roleName in roleNames)
            {
                var roleExist = await roleManager.RoleExistsAsync(roleName);

                if (!roleExist)
                {
                    roleResult = await roleManager.CreateAsync(new IdentityRole(roleName));
                }
            }

            var powerUser = new User
            {
                UserName = config["AdministratorAccount:UserEmail"],
                Email = config["AdministratorAccount:UserEmail"],
                EmailConfirmed = true
            };

            string powerUserPassword = config["AdministratorAccount:UserPassword"];

            var user = await userManager.FindByEmailAsync(config["AdministratorAccount:UserEmail"]);

            if (user == null)
            {
                var createPowerUser = await userManager.CreateAsync(powerUser, powerUserPassword);
                if (createPowerUser.Succeeded)
                {
                    await userManager.AddToRoleAsync(powerUser, "Admin");
                }
            }
        }
    }
}

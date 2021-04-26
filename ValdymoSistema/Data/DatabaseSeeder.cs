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

            string[] roleNames = { "Administrator", "Operator", "Worker" };
            IdentityResult roleResult;

            foreach (var roleName in roleNames)
            {
                var roleExist = await roleManager.RoleExistsAsync(roleName);

                if (!roleExist)
                {
                    roleResult = await roleManager.CreateAsync(new IdentityRole(roleName));
                }
            }

            var adminUser = new User
            {
                UserName = config["AdministratorAccount:UserEmail"],
                Email = config["AdministratorAccount:UserEmail"],
                EmailConfirmed = true
            };

            string adminUserPassword = config["AdministratorAccount:UserPassword"];

            var user = await userManager.FindByEmailAsync(config["AdministratorAccount:UserEmail"]);

            if (user == null)
            {
                var createPowerUser = await userManager.CreateAsync(adminUser, adminUserPassword);
                if (createPowerUser.Succeeded)
                {
                    await userManager.AddToRoleAsync(adminUser, "Operator");
                }
            }

            var operatorUser = new User
            {
                UserName = config["OperatorAccount:UserEmail"],
                Email = config["OperatorAccount:UserEmail"],
                EmailConfirmed = true
            };

            string operatorUserPassword = config["OperatorAccount:UserPassword"];

            user = await userManager.FindByEmailAsync(config["OperatorAccount:UserEmail"]);

            if (user == null)
            {
                var createPowerUser = await userManager.CreateAsync(operatorUser, operatorUserPassword);
                if (createPowerUser.Succeeded)
                {
                    await userManager.AddToRoleAsync(operatorUser, "Administrator");
                }
            }

            var workerUser = new User
            {
                UserName = config["WorkerAccount:UserEmail"],
                Email = config["WorkerAccount:UserEmail"],
                EmailConfirmed = true
            };

            string workerUserPassword = config["WorkerAccount:UserPassword"];

            user = await userManager.FindByEmailAsync(config["WorkerAccount:UserEmail"]);

            if (user == null)
            {
                var createPowerUser = await userManager.CreateAsync(workerUser, workerUserPassword);
                if (createPowerUser.Succeeded)
                {
                    await userManager.AddToRoleAsync(workerUser, "Worker");
                }
            }
        }
        
    }
}

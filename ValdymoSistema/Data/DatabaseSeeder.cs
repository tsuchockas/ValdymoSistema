using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ValdymoSistema.Controllers;
using ValdymoSistema.Data.Entities;
using static ValdymoSistema.Data.Entities.Light;

namespace ValdymoSistema.Data
{
    public static class DatabaseSeeder
    {
        public static async Task CreateRoles(IServiceProvider serviceProvider, IConfiguration config)
        {
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = serviceProvider.GetRequiredService<UserManager<User>>();
            var databaseContext = serviceProvider.GetRequiredService<ApplicationDbContext>();
            databaseContext.Users.RemoveRange(databaseContext.Users);
            databaseContext.SaveChanges();
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
                EmailConfirmed = false
            };

            string adminUserPassword = config["AdministratorAccount:UserPassword"];

            var user = await userManager.FindByEmailAsync(config["AdministratorAccount:UserEmail"]);

            if (user == null)
            {
                var createPowerUser = await userManager.CreateAsync(adminUser, adminUserPassword);
                if (createPowerUser.Succeeded)
                {
                    await userManager.AddToRoleAsync(adminUser, "Administrator");
                }
            }
            //var operatorLights = new List<Light>();
            //operatorLights.Add(databaseContext.Lights.OrderBy(l => l.LightId).FirstOrDefault());
            //operatorLights.Add(databaseContext.Lights.OrderBy(l => l.LightId).LastOrDefault());
            var operatorUser = new User
            {
                UserName = config["OperatorAccount:UserEmail"],
                Email = config["OperatorAccount:UserEmail"],
                EmailConfirmed = false,
                //Lights = operatorLights
            };
            
            string operatorUserPassword = config["OperatorAccount:UserPassword"];

            user = await userManager.FindByEmailAsync(config["OperatorAccount:UserEmail"]);

            if (user == null)
            {
                var createPowerUser = await userManager.CreateAsync(operatorUser, operatorUserPassword);
                if (createPowerUser.Succeeded)
                {
                    await userManager.AddToRoleAsync(operatorUser, "Operator");
                    //foreach (var opLight in operatorLights)
                    //{
                    //    opLight.Users.Add(operatorUser);
                    //}
                }
            }
            //workerLights.Add(databaseContext.Lights.OrderBy(l => l.LightId).FirstOrDefault());
            var workerUser = new User
            {
                UserName = config["WorkerAccount:UserEmail"],
                Email = config["WorkerAccount:UserEmail"],
                EmailConfirmed = false,
                //Lights = workerLights
            };
            
            string workerUserPassword = config["WorkerAccount:UserPassword"];

            user = await userManager.FindByEmailAsync(config["WorkerAccount:UserEmail"]);

            if (user == null)
            {
                var createPowerUser = await userManager.CreateAsync(workerUser, workerUserPassword);
                if (createPowerUser.Succeeded)
                {
                    await userManager.AddToRoleAsync(workerUser, "Worker");
                    //foreach (var workLight in workerLights)
                    //{
                    //    workLight.Users.Add(workerUser);
                    //}
                }
            }
            await databaseContext.SaveChangesAsync();
        }

        public static async Task SeedData(IServiceProvider serviceProvider, IConfiguration config)
        {
            var databaseContext = serviceProvider.GetRequiredService<ApplicationDbContext>();
            databaseContext.Lights.RemoveRange(databaseContext.Lights);
            databaseContext.Triggers.RemoveRange(databaseContext.Triggers);
            databaseContext.Rooms.RemoveRange(databaseContext.Rooms);
            databaseContext.LightEvents.RemoveRange(databaseContext.LightEvents);
            var seedData = File.ReadAllText(@"SeedData.json");
            JObject seedJson = JObject.Parse(seedData);
            var rooms = seedJson.SelectToken("BuildingData.Rooms");
            foreach (var room in rooms)
            {
                var floorNumber = (int)room.SelectToken("FloorNumber");
                var roomName = room.SelectToken("RoomName");
                var triggers = room.SelectToken("Triggers");
                var triggersToDb = new List<Trigger>();
                
                foreach (var trigger in triggers)
                {
                    var triggerName = trigger.SelectToken("TriggerName");
                    var pins = trigger.SelectToken("Lights");
                    var lightsToDb = new List<Light>();
                    for (int i = 0; i < pins.Count(); i++)
                    {
                        var pinNumber = (int)pins[i].SelectToken("Pin");
                        var light = new Light
                        {
                            ControllerPin = pinNumber,
                            LightId = new Guid(),
                            CurrentState = Light.LightState.Off,
                            Users = new List<User>()
                        };
                        databaseContext.Add<Light>(light);
                        lightsToDb.Add(light);
                    }
                    var triggerToDb = new Trigger
                    {
                        TriggerId = new Guid(),
                        TriggerName = (string)triggerName,
                        Lights = lightsToDb
                    };
                    databaseContext.Add<Trigger>(triggerToDb);
                    triggersToDb.Add(triggerToDb);
                }
                var roomToDb = new Room
                {
                    FloorNumber = floorNumber,
                    RoomId = new Guid(),
                    RoomName = (string)roomName,
                    Triggers = triggersToDb
                };
                databaseContext.Add<Room>(roomToDb);
                await databaseContext.SaveChangesAsync();
            }
        }

        public static async Task SeedLightEvents(IServiceProvider serviceProvider)
        {
            var databaseContext = serviceProvider.GetRequiredService<ApplicationDbContext>();
            databaseContext.RemoveRange(databaseContext.LightEvents);
            var lights = databaseContext.Lights.OrderBy(l => l.LightId).ToList();
            var randBrightness = new Random();
            var startDate = new DateTime(2021, 05, 11, 8, 00, 00);
            var brightness = 0;
            double energyUsage = 0.0;
            var lightState = LightState.On;
            foreach (var light in lights)
            {
                startDate = new DateTime(2021, 05, 11, 8, 00, 00);
                for (int i = 0; i < 100; i++)
                {
                    if (i % 2 == 0 && i != 98)
                    {
                        lightState = LightState.On;
                        brightness = randBrightness.Next(30, 100);
                        energyUsage = Math.Round((15 * (brightness * 0.01) * 1) / 1000, 4);
                    }
                    else if (i % 5 == 0)
                    {
                        lightState = LightState.On;
                        brightness = randBrightness.Next(30, 100);
                        energyUsage = Math.Round((15 * (brightness * 0.01) * 1) / 1000, 4);
                    }
                    else if (i == 98)
                    {
                        lightState = LightState.Burnt;
                        brightness = 0;
                        energyUsage = 0.0;
                    }
                    else
                    {
                        lightState = LightState.Off;
                        brightness = 0;
                        energyUsage = 0.0;
                    }
                    var lightEvent = new LightEvent
                    {

                        LightEventId = new Guid(),
                        Light = light,
                        Brightness = brightness,
                        EnergyUsage = energyUsage,
                        DateTime = startDate,
                        CurrentLightState = lightState
                    };
                    databaseContext.Add(lightEvent);
                    if (lightState == LightState.On)
                    {
                        startDate = startDate.AddMinutes(randBrightness.Next(15, 90));
                    }
                    else
                    {
                        startDate = startDate.AddMinutes(5);
                    }
                }
            }
            await databaseContext.SaveChangesAsync();
        }

        //private static async Task SeedRandomData()
        //{
        //    string[] floorRooms = { "201", "202", "301", "302" };
        //    foreach (var floorRoom in floorRooms)
        //    {

        //    }

        //}
    }
}

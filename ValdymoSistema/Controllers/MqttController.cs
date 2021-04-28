using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ValdymoSistema.Data;
using ValdymoSistema.Services.Extensions;

namespace ValdymoSistema.Controllers
{
    public class MqttController
    {
        public static void Test()
        {
            using (var serviceScope = ServiceActivator.GetScope())
            {
                var _database = serviceScope.ServiceProvider.GetRequiredService<IDatabaseController>();
                //var _database = _services.GetRequiredService<IDatabaseController>();
                var rooms = _database.GetAllRooms();
                Console.WriteLine(rooms.Count());
            }
        }
    }
}

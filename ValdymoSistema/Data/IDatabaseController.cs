using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ValdymoSistema.Data.Entities;
using ValdymoSistema.Models;
using static ValdymoSistema.Data.Entities.Light;

namespace ValdymoSistema.Data
{
    public interface IDatabaseController
    {
        IEnumerable<Light> GetLightsForUser(string UserName);
        Room GetRoomForTrigger(Trigger trigger);
        Trigger GetTriggerForLight(Light light);
        Light GetLightById(Guid lightId);
        void ChangeLightState(Light light, LightState lightState, int brightness, double energyUsage);
        Light GetLightFromMqttMessage(string roomName, int floorNumber, int controllerPin, string controllerName);
        IEnumerable<Room> GetAllRooms();
        IEnumerable<string> GetOperatorEmails();
        bool AddRoom(AddRoomViewModel model);
        bool AddTrigger(AddTriggerViewModel model);
        bool AddLight(AddLightViewModel model);
        bool DeleteLight(Guid lightId);
        IEnumerable<User> GetAllUser();
        Dictionary<Light, Dictionary<LightEvent, LightEvent>> GetEnergyUsage(GetEnergyUsageViewModel model);
        Room GetRoom(Guid roomId);
    }
}

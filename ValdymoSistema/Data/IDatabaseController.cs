using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ValdymoSistema.Data.Entities;
using static ValdymoSistema.Data.Entities.Light;

namespace ValdymoSistema.Data
{
    public interface IDatabaseController
    {
        IEnumerable<Light> GetLightsForUser(string UserName);
        Room GetRoomForTrigger(Trigger trigger);
        Trigger GetTriggerForLight(Light light);
        Light GetLightById(Guid lightId);
        void ChangeLightState(Light light, LightState lightState);
        Light GetLightFromMqttMessage(string roomName, int floorNumber, int controllerPin, string controllerName);
        IEnumerable<Room> GetAllRooms();
        IEnumerable<string> GetOperatorEmails();
    }
}

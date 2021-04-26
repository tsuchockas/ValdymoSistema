using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ValdymoSistema.Services.Mqtt
{
    public class ExtarnalService
    {
        private readonly IMqttClientService mqttClientService;
        public ExtarnalService(MqttClientServiceProvider provider)
        {
            mqttClientService = provider.MqttClientService;
        }
    }
}

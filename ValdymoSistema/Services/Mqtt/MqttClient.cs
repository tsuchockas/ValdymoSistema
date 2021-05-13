using Microsoft.Extensions.DependencyInjection;
using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Client.Connecting;
using MQTTnet.Client.Disconnecting;
using MQTTnet.Client.Options;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ValdymoSistema.Data;
using ValdymoSistema.Services.Extensions;
using ValdymoSistema.Services.Mqtt;
using static ValdymoSistema.Data.Entities.Light;

namespace ValdymoSistema.Services
{
    public class MqttClient : IMqttClientService
    {
        private IMqttClient mqttClient;
        private IMqttClientOptions options;

        public MqttClient(IMqttClientOptions options)
        {
            this.options = options;
            mqttClient = new MqttFactory().CreateMqttClient();
            ConfigureMqttClient();
        }

        private void ConfigureMqttClient()
        {
            mqttClient.ConnectedHandler = this;
            mqttClient.DisconnectedHandler = this;
            mqttClient.ApplicationMessageReceivedHandler = this;
        }

        public async Task HandleApplicationMessageReceivedAsync(MqttApplicationMessageReceivedEventArgs eventArgs)
        {
            var mqttTopic = eventArgs.ApplicationMessage.Topic;
            if (mqttTopic.Equals("system/config"))
            {
                var topicToSubscribe = Encoding.UTF8.GetString(eventArgs.ApplicationMessage.Payload);
                await mqttClient.SubscribeAsync(topicToSubscribe);
            }
            else
            {
                try
                {
                    var floorNumber = int.Parse(mqttTopic.Split('/')[1]);
                    var roomName = mqttTopic.Split('/')[2];
                    var controllerName = mqttTopic.Split('/')[3];
                    var mqttMessage = Encoding.UTF8.GetString(eventArgs.ApplicationMessage.Payload);
                    var newLightStateString = mqttMessage.Split(';')[0];
                    LightState newState = LightState.Off;
                    double energyUsage = 0.0;
                    var brightness = 0;
                    var unblockLight = false;
                    switch (newLightStateString)
                    {
                        case "On":
                            newState = LightState.On;
                            energyUsage = double.Parse(mqttMessage.Split(';')[2].Replace('.', ','));
                            brightness = int.Parse(mqttMessage.Split(';')[3]);
                            break;
                        case "Off":
                            newState = LightState.Off;
                        break;
                        case "Burnt":
                            newState = LightState.Burnt;
                            break;
                        case "Blocked":
                            newState = LightState.Blocked;
                            break;
                        case "Unblocked":
                            newState = LightState.Off;
                            unblockLight = true;
                            break;
                    }
                    var controllerPin = int.Parse(mqttMessage.Split(';')[1]);
                    
                    using (var serviceScope = ServiceActivator.GetScope())
                    {
                        var _database = serviceScope.ServiceProvider.GetRequiredService<IDatabaseController>();
                        var lightToChange = _database.GetLightFromMqttMessage(roomName, floorNumber, controllerPin, controllerName);
                        if ((lightToChange.CurrentState != newState && lightToChange.CurrentState != LightState.Blocked) || unblockLight)
                        {
                            _database.ChangeLightState(lightToChange, newState, brightness, energyUsage);
                            if (newState == LightState.Burnt)
                            {
                                var operatorEmails = _database.GetOperatorEmails();
                                foreach (var email in operatorEmails)
                                {
                                    var _emailSender = serviceScope.ServiceProvider.GetRequiredService<IEmailSender>();
                                    _emailSender.SendEmailAsync(email, "Perdegė lempa", $"Perdegė lempa {floorNumber} aukšto patalpoje {roomName}.\n" +
                                        $" Valdiklis: {controllerName} \n" +
                                        $"Valdiklio jungtis {controllerPin}");
                                }
                            }
                        }
                    }
                }
                catch
                {

                }
            }
        }

        public async Task HandleConnectedAsync(MqttClientConnectedEventArgs eventArgs)
        {
            await mqttClient.SubscribeAsync("system/config");
            //await mqttClient.SubscribeAsync("system/1/101/Kampinis");
        }

        public Task HandleDisconnectedAsync(MqttClientDisconnectedEventArgs eventArgs)
        {
            throw new System.NotImplementedException();
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            await mqttClient.ConnectAsync(options);
            if (!mqttClient.IsConnected)
            {
                await mqttClient.ReconnectAsync();
            }
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                var disconnectOption = new MqttClientDisconnectOptions
                {
                    ReasonCode = MqttClientDisconnectReason.NormalDisconnection,
                    ReasonString = "NormalDiconnection"
                };
                await mqttClient.DisconnectAsync(disconnectOption, cancellationToken);
            }
            await mqttClient.DisconnectAsync();
        }

        public async Task PublishMessageAsync(string topic, string message)
        {
            if (mqttClient.IsConnected)
            {
                await mqttClient.PublishAsync(topic, message);
            }
        }
    }
}

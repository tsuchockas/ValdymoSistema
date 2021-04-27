using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Client.Connecting;
using MQTTnet.Client.Disconnecting;
using MQTTnet.Client.Options;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ValdymoSistema.Data;
using ValdymoSistema.Services.Mqtt;
using static ValdymoSistema.Data.Entities.Light;

namespace ValdymoSistema.Services
{
    public class MqttClient : IMqttClientService
    {
        private IMqttClient mqttClient;
        private IMqttClientOptions options;
        private readonly IDatabaseController _database;

        public MqttClient(IMqttClientOptions options, IDatabaseController database)
        {
            this.options = options;
            _database = database;
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
            var floorNumber = int.Parse(mqttTopic.Split(';')[0]);
            var roomName = mqttTopic.Split(';')[1];
            var controllerName = mqttTopic.Split(';')[2];
            var mqttMessage = System.Text.Encoding.UTF8.GetString(eventArgs.ApplicationMessage.Payload);
            var newLightStateString = mqttMessage.Split(';')[0];
            LightState newState = LightState.Off;
            switch (newLightStateString)
            {
                case "On":
                    newState = LightState.On;
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
            }
            var controllerPin = int.Parse(mqttMessage.Split(';')[1]);
            var lightToChange = _database.GetLightFromMqttMessage(roomName, floorNumber, controllerPin, controllerName);
            _database.ChangeLightState(lightToChange, newState);
        }

        public async Task HandleConnectedAsync(MqttClientConnectedEventArgs eventArgs)
        {
            System.Console.WriteLine("connected");
            await mqttClient.SubscribeAsync("hello/world2");
            var rooms = _database.GetAllRooms();
            foreach (var room in rooms)
            {
                foreach (var trigger in room.Triggers)
                {
                    var mqttTopicToSubscribe = $"{room.FloorNumber}/{room.RoomName}/{trigger.TriggerName}";
                    await mqttClient.SubscribeAsync(mqttTopicToSubscribe);
                }
            }
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
            await mqttClient.PublishAsync(topic, message);
        }
    }
}

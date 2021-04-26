using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Client.Connecting;
using MQTTnet.Client.Disconnecting;
using MQTTnet.Client.Options;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ValdymoSistema.Services.Mqtt;

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
            var messageText = "BabaBooey";
            await mqttClient.PublishAsync("Testing/2", messageText);   
        }

        public async Task HandleConnectedAsync(MqttClientConnectedEventArgs eventArgs)
        {
            System.Console.WriteLine("connected");
            await mqttClient.SubscribeAsync("hello/world2");
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

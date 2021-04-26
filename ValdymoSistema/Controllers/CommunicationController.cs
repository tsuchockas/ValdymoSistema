using Humanizer;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Configuration;
using MQTTnet;
using MQTTnet.Client.Options;
using MQTTnet.Extensions.ManagedClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ValdymoSistema.Services
{
    public class CommunicationController : IEmailSender
    {
        private readonly IConfiguration _config;
        private IManagedMqttClient _mqttClient;

        public CommunicationController(IConfiguration config, IManagedMqttClient mqttClient)
        {
            _config = config;
            _mqttClient = mqttClient;
        }

        public Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            throw new NotImplementedException();
        }

        //public async Task ConnectMqttAsync()
        //{
            

        //    var messageBuilder = new MqttClientOptionsBuilder()
        //        .WithClientId(clientId)
        //        .WithCredentials(mqttUsername, mqttPassword)
        //        .WithTcpServer(mqttIp, mqttPort)
        //        .WithTls()
        //        .WithCleanSession();

        //    var options = messageBuilder.Build();

        //    var managedOptions = new ManagedMqttClientOptionsBuilder()
        //        .WithAutoReconnectDelay(TimeSpan.FromSeconds(5))
        //        .WithClientOptions(options)
        //        .Build();

        //    _mqttClient = new MqttFactory().CreateManagedMqttClient();
        //    _mqttClient.UseApplicationMessageReceivedHandler(evt =>
        //    {
        //        try
        //        {
        //            var topic = evt.ApplicationMessage.Topic;
        //            if (!string.IsNullOrWhiteSpace(topic))
        //            {
        //                var payload = Encoding.UTF8.GetString(evt.ApplicationMessage.Payload);
        //                Console.WriteLine($"Topic: {topic}. Message Received: {payload}");
        //            }
        //        }
        //        catch (Exception ex)
        //        {
        //            Console.WriteLine(ex.Message, ex);
        //        }
        //    });
        //    await _mqttClient.StartAsync(managedOptions);
        //}

        public async Task PublishMqttAsync(string topic, string payload, bool retainFlag = true, int qos = 1)
        {
            await _mqttClient.PublishAsync(new MqttApplicationMessageBuilder()
                .WithTopic(topic)
                .WithPayload(payload)
                .WithQualityOfServiceLevel((MQTTnet.Protocol.MqttQualityOfServiceLevel)qos)
                .WithRetainFlag(retainFlag)
                .Build());
        }

        public async Task SubscribeMqttAsync(string topic, int qos = 1)
        {
            await _mqttClient.SubscribeAsync(new MqttTopicFilterBuilder()
                .WithTopic(topic)
                .WithQualityOfServiceLevel((MQTTnet.Protocol.MqttQualityOfServiceLevel)qos)
                .Build());
        }
    }
}

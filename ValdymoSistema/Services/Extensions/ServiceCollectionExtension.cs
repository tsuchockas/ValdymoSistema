using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MQTTnet.Client.Options;
using System;
using ValdymoSistema.Services.Mqtt;
using ValdymoSistema.Services.Mqtt.Options;

namespace ValdymoSistema.Services.Extensions
{
    public static class ServiceCollectionExtension
    {
        public static IServiceCollection AddMqttClientHostedService(this IServiceCollection services, IConfiguration config)
        {
            services.AddMqttClientServiceWithConfig(aspOptionBuilder =>
            {
                var clientId = new Guid().ToString();
                var mqttIp = config["MqttClient:MqttBrokerIp"];
                var mqttUsername = config["MqttClient:MqttUsername"];
                var mqttPassword = config["MqttClient:MqttUserPassword"];
                var mqttPort = int.Parse(config["MqttClient:MqttBrokerPort"]);

                aspOptionBuilder
                .WithCredentials(mqttUsername, mqttPassword)
                .WithClientId(clientId)
                .WithTcpServer(mqttIp, mqttPort);
            });
            return services;
        }

        private static IServiceCollection AddMqttClientServiceWithConfig(this IServiceCollection services, Action<AspCoreMqttClientOptionBuilder> configure)
        {
            services.AddSingleton<IMqttClientOptions>(serviceProvider =>
            {
                var optionBuilder = new AspCoreMqttClientOptionBuilder(serviceProvider);
                configure(optionBuilder);
                return optionBuilder.Build();
            });
            services.AddSingleton<MqttClient>();
            services.AddSingleton<IHostedService>(serviceProvider =>
            {
                return serviceProvider.GetService<MqttClient>();
            });
            services.AddSingleton<MqttClientServiceProvider>(serviceProvider =>
            {
                var mqttClientService = serviceProvider.GetService<MqttClient>();
                var mqttClientServiceProvider = new MqttClientServiceProvider(mqttClientService);
                return mqttClientServiceProvider;
            });
            return services;
        }
    }
}

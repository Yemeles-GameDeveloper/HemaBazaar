using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Framing;
using System.Text;
using Microsoft.Extensions.Logging;
using IConnection = RabbitMQ.Client.IConnection;

namespace HemaBazaar.MVC.Services
{
    public class RabbitMqProducerService : IDisposable
    {
        private IConnection? _connection;
        private IChannel? _channel;
        private readonly IConfiguration _configuration;
        private readonly ILogger<RabbitMqProducerService> _logger;
        private readonly string _queueName;
        private readonly string _hostName;
        private readonly int _port;
        private readonly string _userName;
        private readonly string _password;

        public RabbitMqProducerService(IConfiguration configuration, ILogger<RabbitMqProducerService> logger)
        {
            _configuration = configuration;
            _logger = logger;

            _queueName = _configuration["RabbitMq:QueueName"] ?? "invoice-queue";
            _hostName = _configuration["RabbitMq:HostName"] ?? "localhost";
            _port = int.TryParse(_configuration["RabbitMq:Port"], out var portValue) ? portValue : 5672;
            _userName = _configuration["RabbitMq:UserName"] ?? "guest";
            _password = _configuration["RabbitMq:Password"] ?? "guest";
        }

        private async Task<bool> EnsureConnectedAsync(CancellationToken cancellationToken = default)
        {
            if (_connection is not null && _connection.IsOpen && _channel is not null && _channel.IsOpen)
                return true;

            try
            {
                var factory = new ConnectionFactory
                {
                    HostName = _hostName,
                    Port = _port,
                    UserName = _userName,
                    Password = _password
                };

                _connection = await factory.CreateConnectionAsync(cancellationToken);
                _channel = await _connection.CreateChannelAsync(cancellationToken: cancellationToken);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "RabbitMQ producer could not connect. Host: {Host}, Port: {Port}", _hostName, _port);
                return false;
            }
        }

        public async Task SendMessageAsync<T>(T data, CancellationToken cancellationToken = default)
        {
            if (!await EnsureConnectedAsync(cancellationToken))
            {
                _logger.LogWarning("RabbitMQ publish skipped because broker is unavailable.");
                return;
            }

            string json = JsonConvert.SerializeObject(data);
            byte[] body = Encoding.UTF8.GetBytes(json);

            await _channel!.QueueDeclareAsync(
                queue: _queueName,
                durable: true,
                exclusive: false,
                autoDelete: false,
                arguments: null,
                cancellationToken: cancellationToken
            );

            var properties = new BasicProperties
            {
                Persistent = true,
                ContentType = "application/json"
            };

            await _channel.BasicPublishAsync(
                exchange: "",
                routingKey: _queueName,
                mandatory: false,
                basicProperties: properties,
                body: body,
                cancellationToken: cancellationToken
            );

            _logger.LogInformation("Invoice message published to RabbitMQ queue {QueueName}. PayloadType: {PayloadType}", _queueName, typeof(T).Name);
        }

        public void Dispose()
        {
            _channel?.Dispose();
            _connection?.Dispose();
        }
    }
}

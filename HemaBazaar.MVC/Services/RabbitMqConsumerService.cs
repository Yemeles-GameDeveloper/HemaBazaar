

using Application.Helpers;
using Application.ViewModels;
using DinkToPdf;
using DinkToPdf.Contracts;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using Microsoft.Extensions.Logging;

namespace HemaBazaar.MVC.Services
{
    public class RabbitMqConsumerService : BackgroundService
    {
        private readonly IConverter _converter;
        private IConnection? _connection;
        private IChannel? _channel;
        private readonly IConfiguration _configuration;
        private readonly ILogger<RabbitMqConsumerService> _logger;
        private readonly string _queueName;
        private readonly string _hostName;
        private readonly int _port;
        private readonly string _userName;
        private readonly string _password;

        public RabbitMqConsumerService(IConverter converter, IConfiguration configuration, ILogger<RabbitMqConsumerService> logger)
        {
            _converter = converter;
            _configuration = configuration;
            _logger = logger;

            _queueName = _configuration["RabbitMq:QueueName"] ?? "invoice-queue";
            _hostName = _configuration["RabbitMq:HostName"] ?? "localhost";
            _port = int.TryParse(_configuration["RabbitMq:Port"], out var portValue) ? portValue : 5672;
            _userName = _configuration["RabbitMq:UserName"] ?? "guest";
            _password = _configuration["RabbitMq:Password"] ?? "guest";
        }

        public async Task StartConsumerAsync(CancellationToken stoppingToken)
        {
            if (_channel is null)
            {
                _logger.LogWarning("RabbitMQ channel is not available. Consumer start skipped.");
                return;
            }

            await _channel.QueueDeclareAsync(
                queue: _queueName,
                durable: true,
                exclusive: false,
                autoDelete: false,
                arguments: null
            );

            var consumer = new AsyncEventingBasicConsumer(_channel);

            consumer.ReceivedAsync += async (sender, ea) =>
            {
                try
                {
                    var json = Encoding.UTF8.GetString(ea.Body.ToArray());
                    InvoiceViewModel? model = JsonConvert.DeserializeObject<InvoiceViewModel>(json);

                    if (model is null)
                    {
                        _logger.LogWarning("RabbitMQ message could not be deserialized to InvoiceViewModel. Message dropped.");
                        await _channel.BasicAckAsync(ea.DeliveryTag, false);
                        return;
                    }

                    string html = InvoiceHtmlBuilder.Build(model);

                    var globalSettings = new GlobalSettings
                    {
                        ColorMode = ColorMode.Color,
                        Orientation = Orientation.Portrait,
                        PaperSize = PaperKind.A4,
                        Margins = new MarginSettings
                        {
                            Top = 10,
                            Bottom = 10,
                            Left = 10,
                            Right = 10,
                        }
                    };

                    var objectSettings = new ObjectSettings
                    {
                        PagesCount = true,
                        HtmlContent = html,
                        WebSettings = { DefaultEncoding = "utf-8" },
                        HeaderSettings = new HeaderSettings
                        {
                            FontName = "Arial",
                            FontSize = 9,
                            Right = "Page [page]/[toPage]",
                            Line = true
                        },
                        FooterSettings = new FooterSettings
                        {
                            FontName = "Arial",
                            FontSize = 9,
                            Line = true,
                            Center = "HemaBazaar - Bill"
                        }
                    };

                    var pdfDoc = new HtmlToPdfDocument
                    {
                        GlobalSettings = globalSettings,
                        Objects = { objectSettings }
                    };

                    byte[] pdfBytes = _converter.Convert(pdfDoc);

                    _logger.LogInformation("Sending invoice email to {CustomerMail} for invoice {InvoiceNumber}", model.CustomerMail, model.InvoiceNumber);

                    await new EmailProcess(_configuration).SendEmail(
                        subject: "HemaBazaar E-Bill",
                        "<h1>Please find attached the invoice for the products you have purchased.</br> Thank you for choosing our company.</h1>",
                        fileBytes: pdfBytes,
                        isHTML: true,
                        emailAddresses: model.CustomerMail,
                        contentType: "application/pdf",
                        fileName: "Hemabazaar E-Bill.pdf");

                    _logger.LogInformation("Invoice email sent successfully to {CustomerMail} for invoice {InvoiceNumber}", model.CustomerMail, model.InvoiceNumber);
                    await _channel.BasicAckAsync(ea.DeliveryTag, false);
                    _logger.LogInformation("Invoice processed and acknowledged for {CustomerMail}.", model.CustomerMail);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error while processing invoice message. Message will be requeued.");
                    await _channel.BasicNackAsync(ea.DeliveryTag, false, true);
                }
            };

            await _channel.BasicConsumeAsync(
                queue: _queueName,
                autoAck: false,
                consumer: consumer);

            await Task.Delay(Timeout.Infinite, stoppingToken);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            try
            {
                var factory = new ConnectionFactory
                {
                    HostName = _hostName,
                    Port = _port,
                    UserName = _userName,
                    Password = _password
                };

                _connection = await factory.CreateConnectionAsync(stoppingToken);
                _channel = await _connection.CreateChannelAsync(cancellationToken: stoppingToken);

                await StartConsumerAsync(stoppingToken);
            }
            catch (OperationCanceledException)
            {
                _logger.LogInformation("RabbitMQ consumer stopping due to cancellation.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "RabbitMQ consumer could not start. Host: {Host}, Port: {Port}", _hostName, _port);
                await Task.Delay(Timeout.Infinite, stoppingToken);
            }
        }

        public override void Dispose()
        {
            try
            {
                _channel?.Dispose();
                _connection?.Dispose();
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Error while disposing RabbitMQ consumer resources.");
            }

            base.Dispose();
        }
    }

    
}

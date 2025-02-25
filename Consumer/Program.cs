using Microsoft.Extensions.Configuration;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Shared;

namespace Consumer;

public class Program
{
    static async Task Main(string[] args)
    {
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json")
            .Build();

        var consumer = new MessageConsumer(configuration);
        await consumer.StartAsync();
    }
}

public class MessageConsumer
{
    private readonly IConfiguration _configuration;
    private readonly IConnection _connection;
    private readonly IModel _channel;

    public MessageConsumer(IConfiguration configuration)
    {
        _configuration = configuration;
        var factory = CreateConnectionFactory();
        _connection = factory.CreateConnection();
        _channel = _connection.CreateModel();
        InitializeQueue();
    }

    private ConnectionFactory CreateConnectionFactory()
    {
        return new ConnectionFactory
        {
            HostName = _configuration["RabbitMQ:HostName"],
            UserName = _configuration["RabbitMQ:UserName"],
            Password = _configuration["RabbitMQ:Password"]
        };
    }

    private void InitializeQueue()
    {
        _channel.QueueDeclare(
            queue: RabbitMQConstants.QueueName,
            durable: true,
            exclusive: false,
            autoDelete: false,
            arguments: null);

        _channel.BasicQos(
            prefetchSize: 0,
            prefetchCount: 1,
            global: false);
    }

    public async Task StartAsync()
    {
        try
        {
            Console.WriteLine("消費者已啟動，等待接收訊息...");

            var consumer = new EventingBasicConsumer(_channel);
            consumer.Received += MessageReceived;

            _channel.BasicConsume(
                queue: RabbitMQConstants.QueueName,
                autoAck: false,
                consumer: consumer);

            Console.WriteLine("按 Enter 鍵退出程式");
            Console.ReadLine();
            await Task.CompletedTask;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"發生錯誤：{ex.Message}");
        }
        finally
        {
            Dispose();
        }
    }

    private async void MessageReceived(object? sender, BasicDeliverEventArgs ea)
    {
        try
        {
            var body = ea.Body.ToArray();
            var message = Encoding.UTF8.GetString(body);
            
            Console.WriteLine($"收到訊息：{message}");

            await ProcessMessageAsync(message);

            _channel.BasicAck(
                deliveryTag: ea.DeliveryTag,
                multiple: false);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"處理訊息時發生錯誤：{ex.Message}");
            _channel.BasicNack(
                deliveryTag: ea.DeliveryTag,
                multiple: false,
                requeue: true);
        }
    }

    private async Task ProcessMessageAsync(string message)
    {
        // 模擬非同步處理訊息
        await Task.Delay(1000);
    }

    private void Dispose()
    {
        _channel?.Dispose();
        _connection?.Dispose();
    }
}

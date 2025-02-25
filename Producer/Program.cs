using Microsoft.Extensions.Configuration;
using RabbitMQ.Client;
using System.Text;
using Shared;

namespace Producer;

public class Program
{
    static async Task Main(string[] args)
    {
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json")
            .Build();

        var producer = new MessageProducer(configuration);
        await producer.StartAsync();
    }
}

public class MessageProducer
{
    private readonly IConfiguration _configuration;
    private readonly IConnection _connection;
    private readonly IModel _channel;

    public MessageProducer(IConfiguration configuration)
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
    }

    public async Task StartAsync()
    {
        try
        {
            var properties = _channel.CreateBasicProperties();
            properties.Persistent = true;

            Console.WriteLine("生產者已啟動，準備發送消息...");
            
            while (true)
            {
                Console.WriteLine("\n請輸入要發送的訊息 (輸入 'exit' 退出)：");
                var message = Console.ReadLine();

                if (string.IsNullOrEmpty(message)) continue;
                if (message.ToLower() == "exit") break;

                await PublishMessageAsync(message, properties);
            }
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

    private async Task PublishMessageAsync(string message, IBasicProperties properties)
    {
        var body = Encoding.UTF8.GetBytes(message);
        _channel.BasicPublish(
            exchange: "",
            routingKey: RabbitMQConstants.QueueName,
            basicProperties: properties,
            body: body);

        Console.WriteLine($"已發送訊息：{message}");
        await Task.CompletedTask;
    }

    private void Dispose()
    {
        _channel?.Dispose();
        _connection?.Dispose();
    }
} 
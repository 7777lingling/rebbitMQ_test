using RabbitMQ.Client;
using System.Text;

namespace Producer;

public class Program
{
    static void Main(string[] args)
    {
        var factory = new ConnectionFactory 
        { 
            HostName = "localhost",
            UserName = "guest",
            Password = "guest"
        };
        
        try
        {
            using var connection = factory.CreateConnection();
            using var channel = connection.CreateModel();

            // 宣告隊列
            const string queueName = "test_queue";
            channel.QueueDeclare(
                queue: queueName,
                durable: true,      // 持久化隊列
                exclusive: false,    // 非排他性
                autoDelete: false,   // 不自動刪除
                arguments: null);

            // 設置消息持久化
            var properties = channel.CreateBasicProperties();
            properties.Persistent = true;

            Console.WriteLine("生產者已啟動，準備發送消息...");
            
            while (true)
            {
                Console.WriteLine("\n請輸入要發送的訊息 (輸入 'exit' 退出)：");
                var message = Console.ReadLine();

                if (string.IsNullOrEmpty(message)) continue;
                if (message.ToLower() == "exit") break;

                var body = Encoding.UTF8.GetBytes(message);

                // 發布訊息
                channel.BasicPublish(
                    exchange: "",        // 使用默認交換機
                    routingKey: queueName,
                    basicProperties: properties,
                    body: body);

                Console.WriteLine($"已發送訊息：{message}");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"發生錯誤：{ex.Message}");
        }
    }
} 
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Threading;

namespace Consumer;

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

            const string queueName = "test_queue";
            
            // 宣告隊列（確保隊列存在）
            channel.QueueDeclare(
                queue: queueName,
                durable: true,
                exclusive: false,
                autoDelete: false,
                arguments: null);

            // 設置公平分發
            channel.BasicQos(
                prefetchSize: 0,     // 不限制訊息大小
                prefetchCount: 1,     // 一次只處理一條訊息
                global: false);

            Console.WriteLine("消費者已啟動，等待接收訊息...");

            var consumer = new EventingBasicConsumer(channel);
            
            // 處理接收到的訊息
            consumer.Received += (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                
                Console.WriteLine($"收到訊息：{message}");

                // 模擬處理訊息所需的時間
                Thread.Sleep(1000);

                // 手動確認訊息已處理
                channel.BasicAck(
                    deliveryTag: ea.DeliveryTag,
                    multiple: false);
            };

            // 開始消費訊息
            channel.BasicConsume(
                queue: queueName,
                autoAck: false,      // 關閉自動確認
                consumer: consumer);

            Console.WriteLine("按 Enter 鍵退出程式");
            Console.ReadLine();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"發生錯誤：{ex.Message}");
        }
    }
}

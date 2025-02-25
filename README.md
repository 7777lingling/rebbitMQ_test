# RabbitMQ 測試專案

這是一個使用 .NET 和 RabbitMQ 的簡單訊息佇列示範專案。

## 專案結構 

- `Producer/`: 訊息生產者專案
  - `Program.cs`: 主程式，負責發送訊息到 RabbitMQ
  - `Producer.csproj`: 生產者專案配置文件

- `Consumer/`: 訊息消費者專案
  - `Program.cs`: 主程式，負責接收並處理來自 RabbitMQ 的訊息
  - `Consumer.csproj`: 消費者專案配置文件

## 系統需求

- .NET 6.0 或更高版本
- RabbitMQ 伺服器

## 如何運行

1. 確保 RabbitMQ 伺服器已啟動並運行

2. 運行消費者://bash
cd Consumer
dotnet run

3. 運行生產者://bash
cd Producer
dotnet run

## 功能特點

- 使用 RabbitMQ 實現可靠的訊息佇列
- 支援異步訊息處理
- 簡單的生產者-消費者模式實現

## 專案結構說明

這是一個示範如何使用 RabbitMQ 進行訊息佇列的基礎專案。專案包含兩個主要組件：

1. 生產者 (Producer)：
   - 負責產生並發送訊息到 RabbitMQ 佇列

2. 消費者 (Consumer)：
   - 負責從 RabbitMQ 佇列接收並處理訊息


## 設定檔

在 `appsettings.json` 中設定 RabbitMQ 的連接資訊:

{

    "RabbitMQ": {
        "HostName": "localhost",
        "UserName": "guest",
        "Password": "guest"
    }
}

## 運行結果

運行生產者 (Producer) 和消費者 (Consumer) 後，訊息將會被發送到 RabbitMQ 佇列，並由消費者接收和處理。

## 注意事項

- 確保 RabbitMQ 伺服器已啟動
- 確保生產者和消費者都能正確連接到 RabbitMQ
- 確保訊息佇列存在且能夠被消費者接收

## 參考資料 

- [RabbitMQ 官方文件](https://www.rabbitmq.com/documentation.html)
- [.NET 官方文件](https://docs.microsoft.com/en-us/dotnet/csharp/)   



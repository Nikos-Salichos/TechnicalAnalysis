# TechnicalAnalysis

## Project Purpose
The primary objective of this project is to download financial data from various providers and apply technical indicators to comprehensively analyze this data. Additionally, the project has expanded to include gathering data on new layer 1 blockchains from three different providers.

### Design Highlights
This project features a versatile system that seamlessly connects with multiple financial providers to obtain essential data. Integration has been achieved with the following providers:

- **[Binance](https://www.binance.com/)**: Utilizing the REST API for data retrieval.
- **[Alpaca](https://alpaca.markets/)**: Employing the C# SDK for Alpaca for streamlined integration.
- **[UniswapV3](https://app.uniswap.org/explore)**: Utilizing GraphQL for efficient data access within the Ethereum ecosystem.
- **[PancakeswapV3](https://pancakeswap.finance/info/v3)**:Utilizing GraphQL for efficient data access within the Binance ecosystem.
- **[Alternative.me](https://alternative.me/crypto/fear-and-greed-index/)**: Utilizing the REST API for data retrieval for the Crypto Fear And Greed Index.
- **[CNN Stock Fear And Greed Index](https://rapidapi.com/rpi4gx/api/fear-and-greed-index)**: Utilizing the REST API for data retrieval from RapidApi.
- **[CoinPaprika](https://coinpaprika.com/)**, **[CoinMarketCap](https://coinmarketcap.com/)**, **[CoinRanking](https://coinranking.com/)**: Utilizing the REST API for data retrieval and filtering for layer 1 blockchains.
- **[WallStreetZen](https://www.wallstreetzen.com/)**: Preparing for integration (coming soon).

### Design Components
The project is built around two core services:

1. **Sync Service**: Responsible for initiating data updates by communicating with the respective exchange providers. It effectively manages the synchronization of financial data.
2. **Analysis Service**: The heart of the system, orchestrating the composition of data retrieved from the database. It then applies various technical indicators using extension methods to derive valuable insights.

### Testing and User-Friendly Interface
The project incorporates Swagger to facilitate easy testing of endpoints, ensuring a user-friendly experience for both developers and end-users.

### Architecture, Technologies & Tools
The project follows the principles of [Clean Architecture](https://blog.cleancoder.com/uncle-bob/2012/08/13/the-clean-architecture.html) with some personal modifications. [Here's an architecture diagram](https://miro.com/app/board/uXjVMsAK0lU=/?share_link_id=213007625723) (pending update with the latest providers).

Technologies & Tools used:
- [ASP.NET Core 8](https://learn.microsoft.com/en-us/aspnet/core/introduction-to-aspnet-core?view=aspnetcore-8.0)
- [Docker](https://www.docker.com/)
- [Mediatr](https://github.com/jbogard/MediatR) ([CQRS](https://docs.microsoft.com/en-us/azure/architecture/guide/architecture-styles/cqrs) pattern)
- [PostgreSQL](https://www.postgresql.org/)
- [Redis](https://redis.io/)
- [Dapper](https://github.com/DapperLib/Dapper)
- Structured logging with [Seq](https://datalust.co/seq)
- Multiple NuGet packages ([Serilog](https://serilog.net/), [Polly](https://www.pollydocs.org/), [FluentValidation](https://docs.fluentvalidation.net/en/latest/), [MailKit](https://github.com/jstedfast/MailKit), [HtmlAgilityPack](https://html-agility-pack.net/), [Moq](https://github.com/devlooped/moq), [Xunit](https://xunit.net/docs/getting-started/netfx/visual-studio), [NetArchTest](https://github.com/BenMorris/NetArchTest), [TestContainers](https://dotnet.testcontainers.org/), [StockIndicators](https://dotnet.stockindicators.dev/), [OoplesFinanceStockIndicators](https://github.com/ooples/OoplesFinance.StockIndicators), [WireMock-Net](https://github.com/WireMock-Net/WireMock.Net), [AlpacaSDK](https://github.com/alpacahq/alpaca-trade-api-csharp) )
- [Brotli](https://devblogs.microsoft.com/dotnet/introducing-support-for-brotli-compression/) Compression in endpoints
- [Hangfire](https://www.hangfire.io/) for jobs

### How to Start - Startup Configuration
- Add your `appsettings.prod.json` file and ensure it is located at `./TechnicalAnalysis.Infrastructure.Host/appsettings.prod.json:/app/appsettings.prod.json` (or modify the Docker path in the Docker Compose file).
- Fill in your own details (such as API keys, etc.).
- Navigate to the root folder and run `docker compose up`.

Swagger: `http://localhost:5000/swagger/index.html` <br>
Seq Events: `http://localhost:5341/#/events` <br>
Hangfire Jobs: `http://localhost:5000/hangfire` <br>


### Roadmap
- Create an endpoint where consumers can pass candlestick data, and indicators will be produced on this data.
- Implement WallStreetZen.
- Add integration tests using TestContainers.

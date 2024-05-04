# TechnicalAnalysis

## Project Purpose
The primary objective of this project is to download financial data from various providers and apply technical indicators to comprehensively analyze this data. Additionally, the project has expanded to include gathering data on new layer 1 blockchains from three different providers.

### Design Highlights
This project features a versatile system that seamlessly connects with multiple financial providers to obtain essential data. Integration has been achieved with the following providers:

- **Binance**: Utilizing the REST API for data retrieval.
- **Alpaca**: Employing the C# SDK for Alpaca for streamlined integration.
- **UniswapV3 and PancakeswapV3**: Utilizing GraphQL for efficient data access.
- **Crypto Fear And Greed Index**: Utilizing the REST API for data retrieval from [Alternative.me](https://alternative.me/crypto/fear-and-greed-index/).
- **CNN Stock Fear And Greed Index**: Utilizing the REST API for data retrieval from RapidApi.
- **CoinPaprika**, **CoinMarketCap**, **CoinRanking**: Utilizing the REST API for data retrieval and filtering for layer 1 blockchains.
- **WallStreetZen**: Preparing for integration (coming soon).

### Design Components
The project is built around two core services:

1. **Sync Service**: Responsible for initiating data updates by communicating with the respective exchange providers. It effectively manages the synchronization of financial data.
2. **Analysis Service**: The heart of the system, orchestrating the composition of data retrieved from the database. It then applies various technical indicators using extension methods to derive valuable insights.

### Testing and User-Friendly Interface
The project incorporates Swagger to facilitate easy testing of endpoints, ensuring a user-friendly experience for both developers and end-users.

### Architecture, Technologies & Tools
The project follows the principles of Clean Architecture with some personal modifications. [Here's an architecture diagram](https://miro.com/app/board/uXjVMsAK0lU=/?share_link_id=213007625723) (pending update with the latest providers).

Technologies & Tools used:
- C# (.NET 8)
- Docker
- Mediatr (applied CQRS pattern)
- PostgreSQL
- Redis
- Dapper
- Structured logging with Seq
- Multiple NuGet packages (HttpClient, Serilog, Polly, Fluent Validation, MailKit, HtmlAgilityPack, Moq, Xunit, .NetArchTest.Rules, TestContainers, Stock.Indicators, OoplesFinance.StockIndicators)
- Brotli Compression in endpoints
- Hangfire for jobs
- Docker

### How to Start - Startup Configuration
- Add your `appsettings.prod.json` file and ensure it is located at `./TechnicalAnalysis.Infrastructure.Host/appsettings.prod.json:/app/appsettings.prod.json` (or modify the Docker path in the Docker Compose file).
- Fill in your own details (such as API keys, etc.).
- Navigate to the root folder and run `docker compose up`.

### Roadmap
- Develop client endpoints.
- Create an endpoint where consumers can pass candlestick data, and indicators will be produced on this data.
- Implement WallStreetZen.
- Add integration tests using TestContainers.
- Export to NuGet (Client + Common models only).

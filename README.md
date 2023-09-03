# TechnicalAnalysis

## Project Purpose
My project's primary objective is to download financial data from various providers and apply technical indicators to analyze this data comprehensively.

### Design Highlights
I have designed a versatile system that seamlessly connects with multiple financial providers to obtain essential financial data. I have integrated with the following providers:

- **Binance**: Utilizing the REST API for data retrieval.
- **Alpaca**: Employing the C# SDK for Alpaca for streamlined integration.
- **UniswapV3 and PancakeswapV3**: Utilizing GraphQL for efficient data access.
- **WallStreetZen**: Preparing for integration (coming soon).

### Design Components
My project is built around two core services:

1. **Sync Service**: This service is responsible for initiating data updates by communicating with the respective exchange providers. It effectively manages the synchronization of financial data.

2. **Analysis Service**: The analysis service is the heart of the system, orchestrating the composition of data retrieved from the database. It then applies various technical indicators using extension methods to derive valuable insights.

### Testing and User-Friendly Interface
I've incorporated Swagger to facilitate easy testing of endpoints, ensuring a user-friendly experience for both developers and end-users.

### Startup Configuration
To set up the project, you'll need to populate the `appsettings.json` file with your specific configuration details.

### Architecture, technologies & tools
I have follow the principals of Clean Architecture with some personal changes.
Architecture Diagram: https://miro.com/app/board/uXjVMsAK0lU=/?share_link_id=213007625723

Technologies & Tools:
- .C# (.NET 7)
- Docker
- Mediatr
- PostgreSql
- Redis
- Dapper
- Structured logging with Seq
- Multiple nugets(HttpClient,  Serilog, Polly, Fluent Validation, MailKit, HtmlAgilityPack, Moq, Xunit, .NetArchTest.Rules, TestContainers)


Roadmap:
- Develop client endpoints
- Endpoint that consumer can pass the candlestick data and we will produce indicators on this data
- Script to create table  in docker composer and in new project with DbUp
Add integration tests:
  -Test containers: https://dotnet.testcontainers.org/
  -Wiremock: https://wiremock.org/

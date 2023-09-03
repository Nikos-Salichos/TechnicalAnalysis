# TechnicalAnalysis




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

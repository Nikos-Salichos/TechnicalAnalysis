# Base images
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 5000
EXPOSE 5001

# Create the directory and give appropriate permissions
RUN mkdir -p /app/BacktestData
RUN chmod -R 777 /app/BacktestData

# Build image
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["TechnicalAnalysis.Infrastructure.Host/TechnicalAnalysis.Infrastructure.Host.csproj", "TechnicalAnalysis.Infrastructure.Host/"]
COPY ["TechnicalAnalysis.Application/TechnicalAnalysis.Application.csproj", "TechnicalAnalysis.Application/"]
COPY ["TechnicalAnalysis.Domain/TechnicalAnalysis.Domain.csproj", "TechnicalAnalysis.Domain/"]
RUN dotnet restore "TechnicalAnalysis.Infrastructure.Host/TechnicalAnalysis.Infrastructure.Host.csproj"
COPY . .
WORKDIR "/src/TechnicalAnalysis.Infrastructure.Host"
RUN dotnet build "TechnicalAnalysis.Infrastructure.Host.csproj" -c Release -o /app/build

# Publish
FROM build AS publish
RUN dotnet publish "TechnicalAnalysis.Infrastructure.Host.csproj" -c Release -o /app/publish /p:UseAppHost=false

#Verify appsettings
COPY TechnicalAnalysis.Infrastructure.Host/appsettings.json /app/appsettings.prod.json

# Final stage
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "TechnicalAnalysis.Infrastructure.Host.dll"]

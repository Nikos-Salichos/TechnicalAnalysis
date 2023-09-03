#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["TechnicalAnalysis.Infrastructure.Host/TechnicalAnalysis.Infrastructure.Host.csproj", "TechnicalAnalysis.Infrastructure.Host/"]
COPY ["Application/Application.csproj", "Application/"]
COPY ["Domain/Domain.csproj", "Domain/"]
COPY ["Infrastructure/Infrastructure.csproj", "Infrastructure/"]
RUN dotnet restore "TechnicalAnalysis.Infrastructure.Host/TechnicalAnalysis.Infrastructure.Host.csproj"
COPY . .
WORKDIR "/src/TechnicalAnalysis.Infrastructure.Host"
RUN dotnet build "TechnicalAnalysis.Infrastructure.Host.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "TechnicalAnalysis.Infrastructure.Host.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "TechnicalAnalysis.Infrastructure.Host.dll"]
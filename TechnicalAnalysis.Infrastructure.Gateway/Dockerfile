#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
USER app
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["TechnicalAnalysis.Infrastructure.Gateway/TechnicalAnalysis.Infrastructure.Gateway.csproj", "TechnicalAnalysis.Infrastructure.Gateway/"]
RUN dotnet restore "./TechnicalAnalysis.Infrastructure.Gateway/./TechnicalAnalysis.Infrastructure.Gateway.csproj"
COPY . .
WORKDIR "/src/TechnicalAnalysis.Infrastructure.Gateway"
RUN dotnet build "./TechnicalAnalysis.Infrastructure.Gateway.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./TechnicalAnalysis.Infrastructure.Gateway.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

#Copy pfx file
#COPY ["../aspnetapp.pfx", "/app/aspnetapp.pfx"]

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "TechnicalAnalysis.Infrastructure.Gateway.dll"]
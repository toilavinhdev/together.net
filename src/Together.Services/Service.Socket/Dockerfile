FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 5005
EXPOSE 6005

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["src/Together.Services/Service.Socket/Service.Socket.csproj", "src/Together.Services/Service.Socket/"]
COPY ["src/Together.Infrastructure/Infrastructure.Logging/Infrastructure.Logging.csproj", "src/Together.Infrastructure/Infrastructure.Logging/"]
COPY ["src/Together.Infrastructure/Infrastructure.RabbitMQ/Infrastructure.RabbitMQ.csproj", "src/Together.Infrastructure/Infrastructure.RabbitMQ/"]
COPY ["src/Together.Infrastructure/Infrastructure.Redis/Infrastructure.Redis.csproj", "src/Together.Infrastructure/Infrastructure.Redis/"]
COPY ["src/Together.Infrastructure/Infrastructure.SharedKernel/Infrastructure.SharedKernel.csproj", "src/Together.Infrastructure/Infrastructure.SharedKernel/"]
COPY ["src/Together.Infrastructure/Infrastructure.WebSocket/Infrastructure.WebSocket.csproj", "src/Together.Infrastructure/Infrastructure.WebSocket/"]
RUN dotnet restore "src/Together.Services/Service.Socket/Service.Socket.csproj"
COPY ["src/Together.Services/Service.Socket/", "src/Together.Services/Service.Socket/"]
COPY ["src/Together.Infrastructure/Infrastructure.Logging/", "src/Together.Infrastructure/Infrastructure.Logging/"]
COPY ["src/Together.Infrastructure/Infrastructure.RabbitMQ/", "src/Together.Infrastructure/Infrastructure.RabbitMQ/"]
COPY ["src/Together.Infrastructure/Infrastructure.Redis/", "src/Together.Infrastructure/Infrastructure.Redis/"]
COPY ["src/Together.Infrastructure/Infrastructure.SharedKernel/", "src/Together.Infrastructure/Infrastructure.SharedKernel/"]
COPY ["src/Together.Infrastructure/Infrastructure.WebSocket/", "src/Together.Infrastructure/Infrastructure.WebSocket/"]
WORKDIR "/src/src/Together.Services/Service.Socket"
RUN dotnet build "Service.Socket.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Service.Socket.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Service.Socket.dll", "--urls", "http://*:5005;https://*:6005"]

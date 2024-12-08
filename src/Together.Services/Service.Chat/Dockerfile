FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 5003
EXPOSE 6003

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["src/Together.Services/Service.Chat/Service.Chat.csproj", "src/Together.Services/Service.Chat/"]
COPY ["src/Together.Infrastructure/Infrastructure.DataModels/Infrastructure.DataModels.csproj", "src/Together.Infrastructure/Infrastructure.DataModels/"]
COPY ["src/Together.Infrastructure/Infrastructure.PostgreSQL/Infrastructure.PostgreSQL.csproj", "src/Together.Infrastructure/Infrastructure.PostgreSQL/"]
COPY ["src/Together.Infrastructure/Infrastructure.Logging/Infrastructure.Logging.csproj", "src/Together.Infrastructure/Infrastructure.Logging/"]
COPY ["src/Together.Infrastructure/Infrastructure.RabbitMQ/Infrastructure.RabbitMQ.csproj", "src/Together.Infrastructure/Infrastructure.RabbitMQ/"]
COPY ["src/Together.Infrastructure/Infrastructure.Redis/Infrastructure.Redis.csproj", "src/Together.Infrastructure/Infrastructure.Redis/"]
COPY ["src/Together.Infrastructure/Infrastructure.SharedKernel/Infrastructure.SharedKernel.csproj", "src/Together.Infrastructure/Infrastructure.SharedKernel/"]
RUN dotnet restore "src/Together.Services/Service.Chat/Service.Chat.csproj"
COPY ["src/Together.Services/Service.Chat/", "src/Together.Services/Service.Chat/"]
COPY ["src/Together.Infrastructure/Infrastructure.DataModels/", "src/Together.Infrastructure/Infrastructure.DataModels/"]
COPY ["src/Together.Infrastructure/Infrastructure.PostgreSQL/", "src/Together.Infrastructure/Infrastructure.PostgreSQL/"]
COPY ["src/Together.Infrastructure/Infrastructure.Logging/", "src/Together.Infrastructure/Infrastructure.Logging/"]
COPY ["src/Together.Infrastructure/Infrastructure.RabbitMQ/", "src/Together.Infrastructure/Infrastructure.RabbitMQ/"]
COPY ["src/Together.Infrastructure/Infrastructure.Redis/", "src/Together.Infrastructure/Infrastructure.Redis/"]
COPY ["src/Together.Infrastructure/Infrastructure.SharedKernel/", "src/Together.Infrastructure/Infrastructure.SharedKernel/"]
WORKDIR "/src/src/Together.Services/Service.Chat"
RUN dotnet build "Service.Chat.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Service.Chat.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Service.Chat.dll", "--urls", "http://*:5003;https://*:6003"]

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 5004
EXPOSE 6004

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["src/Together.Services/Service.Notification/Service.Notification.csproj", "src/Together.Services/Service.Notification/"]
COPY ["src/Together.Infrastructure/Infrastructure.DataModels/Infrastructure.DataModels.csproj", "src/Together.Infrastructure/Infrastructure.DataModels/"]
COPY ["src/Together.Infrastructure/Infrastructure.PostgreSQL/Infrastructure.PostgreSQL.csproj", "src/Together.Infrastructure/Infrastructure.PostgreSQL/"]
COPY ["src/Together.Infrastructure/Infrastructure.Logging/Infrastructure.Logging.csproj", "src/Together.Infrastructure/Infrastructure.Logging/"]
COPY ["src/Together.Infrastructure/Infrastructure.RabbitMQ/Infrastructure.RabbitMQ.csproj", "src/Together.Infrastructure/Infrastructure.RabbitMQ/"]
COPY ["src/Together.Infrastructure/Infrastructure.Redis/Infrastructure.Redis.csproj", "src/Together.Infrastructure/Infrastructure.Redis/"]
COPY ["src/Together.Infrastructure/Infrastructure.SharedKernel/Infrastructure.SharedKernel.csproj", "src/Together.Infrastructure/Infrastructure.SharedKernel/"]
RUN dotnet restore "src/Together.Services/Service.Notification/Service.Notification.csproj"
COPY ["src/Together.Services/Service.Notification/", "src/Together.Services/Service.Notification/"]
COPY ["src/Together.Infrastructure/Infrastructure.DataModels/", "src/Together.Infrastructure/Infrastructure.DataModels/"]
COPY ["src/Together.Infrastructure/Infrastructure.PostgreSQL/", "src/Together.Infrastructure/Infrastructure.PostgreSQL/"]
COPY ["src/Together.Infrastructure/Infrastructure.Logging/", "src/Together.Infrastructure/Infrastructure.Logging/"]
COPY ["src/Together.Infrastructure/Infrastructure.RabbitMQ/", "src/Together.Infrastructure/Infrastructure.RabbitMQ/"]
COPY ["src/Together.Infrastructure/Infrastructure.Redis/", "src/Together.Infrastructure/Infrastructure.Redis/"]
COPY ["src/Together.Infrastructure/Infrastructure.SharedKernel/", "src/Together.Infrastructure/Infrastructure.SharedKernel/"]
WORKDIR "/src/src/Together.Services/Service.Notification"
RUN dotnet build "Service.Notification.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Service.Notification.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Service.Notification.dll", "--urls", "http://*:5004;https://*:6004"]

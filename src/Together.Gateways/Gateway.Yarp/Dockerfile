FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 5000

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["src/Together.Gateways/Gateway.Yarp/Gateway.Yarp.csproj", "src/Together.Gateways/Gateway.Yarp/"]
COPY ["src/Together.Infrastructure/Infrastructure.Logging/Infrastructure.Logging.csproj", "src/Together.Infrastructure/Infrastructure.Logging/"]
RUN dotnet restore "src/Together.Gateways/Gateway.Yarp/Gateway.Yarp.csproj"
COPY ["src/Together.Gateways/Gateway.Yarp/", "src/Together.Gateways/Gateway.Yarp/"]
COPY ["src/Together.Infrastructure/Infrastructure.Logging/", "src/Together.Infrastructure/Infrastructure.Logging/"]
WORKDIR "/src/src/Together.Gateways/Gateway.Yarp"
RUN dotnet build "Gateway.Yarp.csproj" -c Release -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "Gateway.Yarp.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Gateway.Yarp.dll", "--urls", "http://*:5000"]

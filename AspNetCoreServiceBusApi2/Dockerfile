#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["AspNetCoreServiceBusApi2/ServiceBusReceiverApi.csproj", "AspNetCoreServiceBusApi2/"]
COPY ["ServiceBus.Infraestructure/ServiceBus.Infraestructure.csproj", "ServiceBus.Infraestructure/"]
COPY ["ServiceBusMessaging/ServiceBusMessaging.csproj", "ServiceBusMessaging/"]
RUN dotnet restore "AspNetCoreServiceBusApi2/ServiceBusReceiverApi.csproj"
COPY . .
WORKDIR "/src/AspNetCoreServiceBusApi2"
RUN dotnet build "ServiceBusReceiverApi.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "ServiceBusReceiverApi.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ServiceBusReceiverApi.dll"]
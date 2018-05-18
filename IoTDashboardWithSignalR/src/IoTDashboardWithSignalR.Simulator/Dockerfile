FROM microsoft/dotnet:2.0-runtime AS base
WORKDIR /app

FROM microsoft/dotnet:2.0-sdk AS build
WORKDIR /src
COPY src/IoTDashboardWithSignalR.Simulator/IoTDashboardWithSignalR.Simulator.csproj src/IoTDashboardWithSignalR.Simulator/
RUN dotnet restore src/IoTDashboardWithSignalR.Simulator/IoTDashboardWithSignalR.Simulator.csproj
COPY . .
WORKDIR /src/src/IoTDashboardWithSignalR.Simulator
RUN dotnet build IoTDashboardWithSignalR.Simulator.csproj -c Release -o /app

FROM build AS publish
RUN dotnet publish IoTDashboardWithSignalR.Simulator.csproj -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "IoTDashboardWithSignalR.Simulator.dll"]

FROM microsoft/aspnetcore:2.0.8 AS base
WORKDIR /app
EXPOSE 80

FROM microsoft/aspnetcore-build:2.0 AS build
WORKDIR /src
COPY src/IoTDashboardWithSignalR.Web/IoTDashboardWithSignalR.Web.csproj src/IoTDashboardWithSignalR.Web/
RUN dotnet restore src/IoTDashboardWithSignalR.Web/IoTDashboardWithSignalR.Web.csproj
COPY . .
WORKDIR /src/src/IoTDashboardWithSignalR.Web
RUN dotnet build IoTDashboardWithSignalR.Web.csproj -c Release -o /app

FROM build AS publish
RUN dotnet publish IoTDashboardWithSignalR.Web.csproj -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "IoTDashboardWithSignalR.Web.dll"]

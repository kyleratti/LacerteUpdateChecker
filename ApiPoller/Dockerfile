FROM mcr.microsoft.com/dotnet/runtime:6.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["ApiPoller/ApiPoller.csproj", "ApiPoller/"]
RUN dotnet restore "ApiPoller/ApiPoller.csproj"
COPY . .
WORKDIR "/src/ApiPoller"
RUN dotnet build "ApiPoller.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "ApiPoller.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ApiPoller.dll"]

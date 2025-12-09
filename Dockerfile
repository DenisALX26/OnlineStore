FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY ["OnlineStoreApp/OnlineStoreApp.csproj", "OnlineStoreApp/"]
RUN dotnet restore "OnlineStoreApp/OnlineStoreApp.csproj"
COPY . .
WORKDIR "/src/OnlineStoreApp"
RUN dotnet build "OnlineStoreApp.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "OnlineStoreApp.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "OnlineStoreApp.dll"]


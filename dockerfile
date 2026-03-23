FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy csproj and restore as distinct layers
COPY ["SalesBicycleStore.sln", "./"]
COPY ["SalesBicycleStore/SalesBicycleStore.csproj", "SalesBicycleStore/"]
RUN dotnet restore "SalesBicycleStore/SalesBicycleStore.csproj"

# Copy everything else and build
COPY . .
WORKDIR "/src/SalesBicycleStore"
RUN dotnet build "SalesBicycleStore.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "SalesBicycleStore.csproj" -c Release -o /app/publish

# Build runtime image
FROM mcr.microsoft.com/dotnet/runtime:9.0 AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "SalesBicycleStore.dll"]

# 1. Asama: Derleme (Build)
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Proje dosyalarini kopyala ve bagimliliklari yukle
COPY . .
RUN dotnet restore "BlockchainAnalysis.App/BlockchainAnalysis.App.csproj"

# Kodu derle
RUN dotnet publish "BlockchainAnalysis.App/BlockchainAnalysis.App.csproj" -c Release -o /app/publish

# 2. Asama: Calistirma (Runtime)
FROM mcr.microsoft.com/dotnet/runtime:10.0 AS final
WORKDIR /app
COPY --from=build /app/publish .

# Uygulamayi baslat
ENTRYPOINT ["dotnet", "BlockchainAnalysis.App.dll"]

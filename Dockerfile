# 1. Aşama: Derleme (Build)
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Proje dosyalarını kopyala ve bağımlılıkları yükle
COPY . .
RUN dotnet restore "BlockchainAnalysis.App/BlockchainAnalysis.App.csproj"

# Kodu derle
RUN dotnet publish "BlockchainAnalysis.App/BlockchainAnalysis.App.csproj" -c Release -o /app/publish

# 2. Aşama: Çalıştırma (Runtime)
FROM mcr.microsoft.com/dotnet/runtime:8.0 AS final
WORKDIR /app
COPY --from=build /app/publish .

# Uygulamayı başlat
ENTRYPOINT ["dotnet", "BlockchainAnalysis.App.dll"]
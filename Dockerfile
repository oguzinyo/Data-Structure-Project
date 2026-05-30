# Kök dizindeki Dockerfile (Backend için)
# 1. Aşama: Derleme (Build)
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Tüm projeleri kopyala ve restore et
COPY . .
RUN dotnet restore "BlockchainAnalysis.App/BlockchainAnalysis.App.csproj"

# Kodu derle
RUN dotnet publish "BlockchainAnalysis.App/BlockchainAnalysis.App.csproj" -c Release -o /app/publish

# 2. Aşama: Çalıştırma (Runtime)
FROM mcr.microsoft.com/dotnet/runtime:10.0 AS final
WORKDIR /app
COPY --from=build /app/publish .

LABEL maintainer="ummet_devops"

# Uygulamayı başlat
ENTRYPOINT ["dotnet", "BlockchainAnalysis.App.dll"]
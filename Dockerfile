# 1. Aşama: Derleme (Build)
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Tüm projeleri kopyala ve restore et
COPY . .
RUN dotnet restore "BlockchainAnalysis.Api/BlockchainAnalysis.Api.csproj"

# Web API kodunu derle
RUN dotnet publish "BlockchainAnalysis.Api/BlockchainAnalysis.Api.csproj" -c Release -o /app/publish

# 2. Aşama: Çalıştırma (Runtime) - Web sunucusu için ASP.NET imajı kullanılmalıdır
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app
COPY --from=build /app/publish .
LABEL maintainer="ummet_devops"

# API'nin konteyner içinde 5050 portundan yayın yapmasını sağla
ENV ASPNETCORE_URLS=http://+:5050

# Uygulamayı başlat
ENTRYPOINT ["dotnet", "BlockchainAnalysis.Api.dll"]
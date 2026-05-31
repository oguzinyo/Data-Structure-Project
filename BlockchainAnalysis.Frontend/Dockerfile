# 1. Aşama: Bağımlılıklar ve Build
FROM node:20-alpine AS build
WORKDIR /app

# Sadece package dosyalarını kopyalayıp npm ci ile temiz kurulum yapıyoruz
COPY package*.json ./
RUN npm ci

# Kalan tüm dosyaları kopyala ve Angular projesini derle
COPY . .
RUN npm run build

# 2. Aşama: SSR (Server Side Rendering) Sunucusunu Çalıştırma
FROM node:20-alpine AS runtime
WORKDIR /app

# Derlenmiş dosyaları ve package.json'ı runtime ortamına al
COPY --from=build /app/dist ./dist
COPY --from=build /app/package*.json ./

LABEL maintainer="ummet_devops"

# Angular SSR varsayılan portu
EXPOSE 4000

# package.json'daki script'i kullanarak SSR sunucusunu başlatıyoruz
CMD ["npm", "run", "serve:ssr:BlockchainAnalysis.Frontend"]
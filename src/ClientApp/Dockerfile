#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src2
COPY ["src/ClientApp/ClientApp.csproj", "src/ClientApp/"]
COPY ["src/Client/Client.csproj", "src/Client/"]
RUN dotnet restore "src/ClientApp/ClientApp.csproj"
COPY . .
WORKDIR "/src2/src/ClientApp"
RUN dotnet build "ClientApp.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "ClientApp.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM nginx:alpine AS final
WORKDIR /usr/share/nginx/html
COPY --from=publish /app/publish/wwwroot .
COPY src/ClientApp/nginx.conf /etc/nginx/nginx.conf
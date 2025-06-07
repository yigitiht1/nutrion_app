FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ["Proje2.sln", "./"]
COPY ["API/API.csproj", "API/"]

RUN dotnet restore "Proje2.sln"

COPY . .

WORKDIR "/src/API"
RUN dotnet publish -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .

EXPOSE 8080

# Railway'nin PORT ortam değişkenini kullanmak için
ENV ASPNETCORE_URLS=http://+:${PORT:-8080}

ENTRYPOINT ["dotnet", "API.dll"]
#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Teste Tecnico INDT/Teste Tecnico INDT.csproj", "Teste Tecnico INDT/"]
RUN dotnet restore "./Teste Tecnico INDT/Teste Tecnico INDT.csproj"
COPY . .
WORKDIR "/src/Teste Tecnico INDT"
RUN dotnet build "./Teste Tecnico INDT.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Teste Tecnico INDT.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
# banco por padrão dentro do container, pode ser sobrescrito por env
ENV ConnectionStrings__Default=Data Source=/app/propostas.db
ENTRYPOINT ["dotnet", "Teste Tecnico INDT.dll"]
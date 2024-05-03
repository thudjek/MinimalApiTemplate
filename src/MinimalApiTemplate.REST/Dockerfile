FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["src/MinimalApiTemplate.REST/MinimalApiTemplate.REST.csproj", "src/MinimalApiTemplate.REST/"]
RUN dotnet restore "./src/MinimalApiTemplate.REST/MinimalApiTemplate.REST.csproj"
COPY . .
WORKDIR "/src/src/MinimalApiTemplate.REST"
RUN dotnet build "./MinimalApiTemplate.REST.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./MinimalApiTemplate.REST.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "MinimalApiTemplate.REST.dll"]
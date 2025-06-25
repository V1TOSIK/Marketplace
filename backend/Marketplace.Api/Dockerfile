FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 8081


FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Marketplace.Api/Marketplace.Api.csproj", "Marketplace.Api/"]
COPY ["AuthModule/AuthModule.Application/AuthModule.Application.csproj", "AuthModule/AuthModule.Application/"]
COPY ["AuthModule/AuthModule.Domain/AuthModule.Domain.csproj", "AuthModule/AuthModule.Domain/"]
COPY ["AuthModule/AuthModule.Persistence/AuthModule.Persistence.csproj", "AuthModule/AuthModule.Persistence/"]
COPY ["AuthModule/AuthModule.Infrastructure/AuthModule.Infrastructure.csproj", "AuthModule/AuthModule.Infrastructure/"]
COPY ["AuthModule/AuthModule.Composition/AuthModule.Composition.csproj", "AuthModule/AuthModule.Composition/"]
RUN dotnet restore "./Marketplace.Api/Marketplace.Api.csproj"
COPY . .
WORKDIR "/src/Marketplace.Api"
RUN dotnet build "./Marketplace.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Marketplace.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Marketplace.Api.dll"]
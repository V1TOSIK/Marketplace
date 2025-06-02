FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 8081


FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Marketplace.Api/Marketplace.Api.csproj", "Marketplace.Api/"]
COPY ["AuthUserModule/AuthUserModule.Application/AuthUserModule.Application.csproj", "AuthUserModule/AuthUserModule.Application/"]
COPY ["AuthUserModule/AuthUserModule.Domain/AuthUserModule.Domain.csproj", "AuthUserModule/AuthUserModule.Domain/"]
COPY ["AuthUserModule/AuthUserModule.Persistence/AuthUserModule.Persistence.csproj", "AuthUserModule/AuthUserModule.Persistence/"]
COPY ["AuthUserModule/AuthUserModule.Infrastructure/AuthUserModule.Infrastructure.csproj", "AuthUserModule/AuthUserModule.Infrastructure/"]
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
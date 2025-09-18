FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 8081


FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Marketplace.Api/Marketplace.Api.csproj", "Marketplace.Api/"]
COPY ["SharedKernel/SharedKernel.csproj", "SharedKernel/"]
COPY ["AuthModule/AuthModule.Application/AuthModule.Application.csproj", "AuthModule/AuthModule.Application/"]
COPY ["AuthModule/AuthModule.Domain/AuthModule.Domain.csproj", "AuthModule/AuthModule.Domain/"]
COPY ["AuthModule/AuthModule.Persistence/AuthModule.Persistence.csproj", "AuthModule/AuthModule.Persistence/"]
COPY ["AuthModule/AuthModule.Infrastructure/AuthModule.Infrastructure.csproj", "AuthModule/AuthModule.Infrastructure/"]
COPY ["AuthModule/AuthModule.Composition/AuthModule.Composition.csproj", "AuthModule/AuthModule.Composition/"]

COPY ["UserModule/UserModule.Application/UserModule.Application.csproj", "UserModule/UserModule.Application/"]
COPY ["UserModule/UserModule.Domain/UserModule.Domain.csproj", "UserModule/UserModule.Domain/"]
COPY ["UserModule/UserModule.Persistence/UserModule.Persistence.csproj", "UserModule/UserModule.Persistence/"]
COPY ["UserModule/UserModule.Composition/UserModule.Composition.csproj", "UserModule/UserModule.Composition/"]

COPY ["ProductModule/ProductModule.Application/ProductModule.Application.csproj", "ProductModule/ProductModule.Application/"]
COPY ["ProductModule/ProductModule.Domain/ProductModule.Domain.csproj", "ProductModule/ProductModule.Domain/"]
COPY ["ProductModule/ProductModule.Persistence/ProductModule.Persistence.csproj", "ProductModule/ProductModule.Persistence/"]
COPY ["ProductModule/ProductModule.Composition/ProductModule.Composition.csproj", "ProductModule/ProductModule.Composition/"]

COPY ["MediaModule/MediaModule.Application/MediaModule.Application.csproj", "MediaModule/MediaModule.Application/"]
COPY ["MediaModule/MediaModule.Domain/MediaModule.Domain.csproj", "MediaModule/MediaModule.Domain/"]
COPY ["MediaModule/MediaModule.Persistence/MediaModule.Persistence.csproj", "MediaModule/MediaModule.Persistence/"]
COPY ["MediaModule/MediaModule.Infrastructure/MediaModule.Infrastructure.csproj", "MediaModule/MediaModule.Infrastructure/"]
COPY ["MediaModule/MediaModule.Composition/MediaModule.Composition.csproj", "MediaModule/MediaModule.Composition/"]

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
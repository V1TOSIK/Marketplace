FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

COPY ["DeleteProductWorker/DeleteProductWorker.csproj", "DeleteProductWorker/"]
COPY ["ProductModule/ProductModule.Application/ProductModule.Application.csproj", "ProductModule/ProductModule.Application/"]
COPY ["ProductModule/ProductModule.Persistence/ProductModule.Persistence.csproj", "ProductModule/ProductModule.Persistence/"]
COPY ["SharedKernel/SharedKernel.csproj", "SharedKernel/"]

RUN dotnet restore "./DeleteProductWorker/DeleteProductWorker.csproj"
COPY . .
WORKDIR "/src/DeleteProductWorker"
RUN dotnet build "./DeleteProductWorker.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./DeleteProductWorker.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "DeleteProductWorker.dll"]
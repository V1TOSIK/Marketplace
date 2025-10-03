FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

COPY ["DeleteMediaWorker/DeleteMediaWorker.csproj", "DeleteMediaWorker/"]
COPY ["MediaModule/MediaModule.Application/MediaModule.Application.csproj", "MediaModule/MediaModule.Application/"]
COPY ["MediaModule/MediaModule.Persistence/MediaModule.Persistence.csproj", "MediaModule/MediaModule.Persistence/"]
COPY ["MediaModule/MediaModule.Infrastructure/MediaModule.Infrastructure.csproj", "MediaModule/MediaModule.Infrastructure/"]
COPY ["SharedKernel/SharedKernel.csproj", "SharedKernel/"]

RUN dotnet restore "./DeleteMediaWorker/DeleteMediaWorker.csproj"
COPY . .
WORKDIR "/src/DeleteMediaWorker"
RUN dotnet build "./DeleteMediaWorker.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./DeleteMediaWorker.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "DeleteMediaWorker.dll"]
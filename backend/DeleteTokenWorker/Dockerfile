FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

COPY ["DeleteTokenWorker/DeleteTokenWorker.csproj", "DeleteTokenWorker/"]
COPY ["AuthModule/AuthModule.Application/AuthModule.Application.csproj", "AuthModule/AuthModule.Application/"]
COPY ["AuthModule/AuthModule.Persistence/AuthModule.Persistence.csproj", "AuthModule/AuthModule.Persistence/"]
COPY ["SharedKernel/SharedKernel.csproj", "SharedKernel/"]

RUN dotnet restore "./DeleteTokenWorker/DeleteTokenWorker.csproj"
COPY . .
WORKDIR "/src/DeleteTokenWorker"
RUN dotnet build "./DeleteTokenWorker.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./DeleteTokenWorker.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "DeleteTokenWorker.dll"]
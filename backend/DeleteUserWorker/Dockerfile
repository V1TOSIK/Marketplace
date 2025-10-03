FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

COPY ["DeleteUserWorker/DeleteUserWorker.csproj", "DeleteUserWorker/"]
COPY ["AuthModule/AuthModule.Application/AuthModule.Application.csproj", "AuthModule/AuthModule.Application/"]
COPY ["AuthModule/AuthModule.Persistence/AuthModule.Persistence.csproj", "AuthModule/AuthModule.Persistence/"]
COPY ["SharedKernel/SharedKernel.csproj", "SharedKernel/"]

RUN dotnet restore "./DeleteUserWorker/DeleteUserWorker.csproj"
COPY . .
WORKDIR "/src/DeleteUserWorker"
RUN dotnet build "./DeleteUserWorker.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./DeleteUserWorker.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "DeleteUserWorker.dll"]
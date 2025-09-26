FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

COPY ["DeleteTokenWorker/DeleteTokenWorker.csproj", "DeleteTokenWorker/"]
COPY ["AuthModule/AuthModule.Application/AuthModule.Application.csproj", "AuthModule/AuthModule.Application/"]
COPY ["AuthModule/AuthModule.Persistence/AuthModule.Persistence.csproj", "AuthModule/AuthModule.Persistence/"]
COPY ["SharedKernel/SharedKernel.csproj", "SharedKernel/"]

RUN dotnet restore "DeleteTokenWorker/DeleteTokenWorker.csproj"
COPY . .
WORKDIR "/src/DeleteTokenWorker"
RUN dotnet publish "DeleteTokenWorker.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "DeleteTokenWorker.dll"]

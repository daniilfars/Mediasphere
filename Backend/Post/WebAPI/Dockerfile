# См. статью по ссылке https://aka.ms/customizecontainer, чтобы узнать как настроить контейнер отладки и как Visual Studio использует этот Dockerfile для создания образов для ускорения отладки.

# Этот этап используется при запуске из VS в быстром режиме (по умолчанию для конфигурации отладки)
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081


# Этот этап используется для сборки проекта службы
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Backend/Post/WebAPI/WebAPI.csproj", "Backend/Post/WebAPI/"]
COPY ["Backend/Shared/Shared.csproj", "Backend/Shared/"]
COPY ["Backend/Post/Infrastructure/Infrastructure.csproj", "Backend/Post/Infrastructure/"]
COPY ["Backend/Post/Application/Application.csproj", "Backend/Post/Application/"]
COPY ["Backend/Post/Domain/Domain.csproj", "Backend/Post/Domain/"]
RUN dotnet restore "./Backend/Post/WebAPI/WebAPI.csproj"
COPY . .
WORKDIR "/src/Backend/Post/WebAPI"
RUN dotnet build "./WebAPI.csproj" -c $BUILD_CONFIGURATION -o /app/build

# Этот этап используется для публикации проекта службы, который будет скопирован на последний этап
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./WebAPI.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# Этот этап используется в рабочей среде или при запуске из VS в обычном режиме (по умолчанию, когда конфигурация отладки не используется)
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "WebAPI.dll"]
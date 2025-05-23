FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Attendances.Systems/Attendances.System.Api/Attendances.System.Api.csproj", "Attendances.Systems/Attendances.System.Api/"]
COPY ["Attendances.Infrastructures/Attendances.RestWrappers/Attendances.RestWrapper.MoodleApi/Attendances.RestWrapper.MoodleApi.csproj", "Attendances.Infrastructures/Attendances.RestWrappers/Attendances.RestWrapper.MoodleApi/"]
COPY ["Attendances.Applications/Attendances.Application.Commons/Attendances.Application.Commons.csproj", "Attendances.Applications/Attendances.Application.Commons/"]
COPY ["Attendances.Domains/Attendances.Domains.Core/Attendances.Domains.Core.csproj", "Attendances.Domains/Attendances.Domains.Core/"]
COPY ["Attendances.Infrastructures/Attendances.RestWrappers/Attendances.RestWrapper.MoodleCourses/Attendances.RestWrapper.MoodleCourses.csproj", "Attendances.Infrastructures/Attendances.RestWrappers/Attendances.RestWrapper.MoodleCourses/"]
COPY ["Attendances.Shared/Attendances.Shared.Commons/Attendances.Shared.Commons.csproj", "Attendances.Shared/Attendances.Shared.Commons/"]
COPY ["Attendances.Shared/Attendances.Shared.Security/Attendances.Shared.Security.csproj", "Attendances.Shared/Attendances.Shared.Security/"]
COPY ["Attendances.Applications/Attendances.Application.Tokens/Attendances.Application.Tokens.csproj", "Attendances.Applications/Attendances.Application.Tokens/"]
COPY ["Attendances.Domains/Attendances.Domains.Users/Attendances.Domains.Users.csproj", "Attendances.Domains/Attendances.Domains.Users/"]
RUN dotnet restore "Attendances.Systems/Attendances.System.Api/Attendances.System.Api.csproj"
COPY . .
WORKDIR "/src/Attendances.Systems/Attendances.System.Api"
RUN dotnet build "Attendances.System.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "Attendances.System.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Attendances.System.Api.dll"]

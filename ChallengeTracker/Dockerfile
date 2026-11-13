FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
COPY api/ChallengeTrackerApi/ChallengeTrackerApi.csproj api/ChallengeTrackerApi/
RUN dotnet restore api/ChallengeTrackerApi/ChallengeTrackerApi.csproj
COPY api/ChallengeTrackerApi/ api/ChallengeTrackerApi/
RUN dotnet publish api/ChallengeTrackerApi/ChallengeTrackerApi.csproj -c Release -o /app/publish --no-restore /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app
COPY --from=build /app/publish .
ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080
USER $APP_UID
ENTRYPOINT ["dotnet", "ChallengeTrackerApi.dll"]

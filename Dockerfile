FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY ["FeedbackApi.Api/FeedbackApi.Api.csproj", "FeedbackApi.Api/"]
COPY ["FeedbackApi.Application/FeedbackApi.Application.csproj", "FeedbackApi.Application/"]
COPY ["FeedbackApi.Domain/FeedbackApi.Domain.csproj", "FeedbackApi.Domain/"]
COPY ["FeedbackApi.Infrastructure/FeedbackApi.Infrastructure.csproj", "FeedbackApi.Infrastructure/"]
RUN dotnet restore "FeedbackApi.Api/FeedbackApi.Api.csproj"
COPY . .
WORKDIR "/src/FeedbackApi.Api"
RUN dotnet build "FeedbackApi.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "FeedbackApi.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "FeedbackApi.Api.dll"]

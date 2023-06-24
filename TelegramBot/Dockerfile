FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
ENV TZ Europe/Minsk

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["TelegramBot/TelegramBot.fsproj", "TelegramBot/"]
RUN dotnet restore "TelegramBot/TelegramBot.fsproj"
COPY . .
WORKDIR "/src/TelegramBot"
RUN dotnet build "TelegramBot.fsproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "TelegramBot.fsproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "TelegramBot.dll"]

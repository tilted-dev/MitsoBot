FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
ENV TZ Europe/Minsk

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["VkontakteBot/VkontakteBot.fsproj", "VkontakteBot/"]
RUN dotnet restore "VkontakteBot/VkontakteBot.fsproj"
COPY . .
WORKDIR "/src/VkontakteBot"
RUN dotnet build "VkontakteBot.fsproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "VkontakteBot.fsproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "VkontakteBot.dll"]

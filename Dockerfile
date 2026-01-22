FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["the-backfield/the-backfield.csproj", "the-backfield/"]
RUN dotnet restore "./the-backfield/the-backfield.csproj"
COPY . .
WORKDIR "/src/the-backfield"
RUN dotnet build "./the-backfield.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./the-backfield.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "the-backfield.dll"]
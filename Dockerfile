FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY ["VIANDRIVEFULL.csproj", "./"]
RUN dotnet restore "VIANDRIVEFULL.csproj"
COPY . .
RUN dotnet publish "VIANDRIVEFULL.csproj" -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 8080
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "VIANDRIVEFULL.dll"]
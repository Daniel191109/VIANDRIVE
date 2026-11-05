FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY ["TuProyecto.csproj", "./"]
RUN dotnet restore "TuProyecto.csproj"
COPY . .
RUN dotnet publish "TuProyecto.csproj" -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 8080
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "TuProyecto.dll"]
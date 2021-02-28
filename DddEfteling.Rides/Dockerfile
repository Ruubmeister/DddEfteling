#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:5.0-buster-slim AS base
WORKDIR /app
EXPOSE 3997

FROM mcr.microsoft.com/dotnet/sdk:5.0-buster-slim AS build
WORKDIR /src
COPY ["DddEfteling.Rides/DddEfteling.Rides.csproj", "DddEfteling.Rides/"]
RUN dotnet restore "DddEfteling.Rides/DddEfteling.Rides.csproj"
COPY . .
WORKDIR "/src/DddEfteling.Rides"
RUN dotnet build "DddEfteling.Rides.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "DddEfteling.Rides.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "DddEfteling.Rides.dll"]
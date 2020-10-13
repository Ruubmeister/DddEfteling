#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-buster-slim AS base
WORKDIR /app
EXPOSE 3995

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
WORKDIR /src
COPY ["DddEfteling.Visitors/DddEfteling.Visitors.csproj", "DddEfteling.Visitors/"]
RUN dotnet restore "DddEfteling.Visitors/DddEfteling.Visitors.csproj"
COPY . .
WORKDIR "/src/DddEfteling.Visitors"
RUN dotnet build "DddEfteling.Visitors.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "DddEfteling.Visitors.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "DddEfteling.Visitors.dll"]
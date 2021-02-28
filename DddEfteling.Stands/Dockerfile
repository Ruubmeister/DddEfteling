#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:5.0-buster-slim AS base
WORKDIR /app
EXPOSE 3996

FROM mcr.microsoft.com/dotnet/sdk:5.0-buster-slim AS build
WORKDIR /src
COPY ["DddEfteling.Stands/DddEfteling.Stands.csproj", "DddEfteling.Stands/"]
RUN dotnet restore "DddEfteling.Stands/DddEfteling.Stands.csproj"
COPY . .
WORKDIR "/src/DddEfteling.Stands"
RUN dotnet build "DddEfteling.Stands.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "DddEfteling.Stands.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "DddEfteling.Stands.dll"]
#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build

WORKDIR /src

COPY /src/. .

WORKDIR "/src/TaskManager.WebApi"

RUN dotnet restore "TaskManager.WebApi.csproj"

RUN dotnet build "TaskManager.WebApi.csproj" -c Release --no-restore

FROM build AS publish
RUN dotnet publish "TaskManager.WebApi.csproj" -c Release --no-restore --no-build -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "TaskManager.WebApi.dll"]
FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["BackendService/BackendService.csproj", "BackendService/"]
RUN dotnet restore "BackendService/BackendService.csproj"
COPY . .
WORKDIR "/src/BackendService"
RUN dotnet build "BackendService.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "BackendService.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "BackendService.dll"]

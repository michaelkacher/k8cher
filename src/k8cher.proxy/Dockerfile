# This dockerfile will be used for building deployment docker image artifacts
FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["k8cher.proxy.csproj", "./"]
RUN dotnet restore "./k8cher.proxy.csproj"
COPY . .
WORKDIR /src/.
RUN dotnet build "k8cher.proxy.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "k8cher.proxy.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "k8cher.proxy.dll"]
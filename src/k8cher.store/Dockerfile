# This dockerfile will be used for building deployment docker image artifacts
FROM mcr.microsoft.com/dotnet/aspnet:6.0.0-preview.7 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["k8cher.store.csproj", "./"]
RUN dotnet restore "./k8cher.store.csproj"
COPY . .
WORKDIR /src/.
RUN dotnet build "k8cher.store.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "k8cher.store.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "k8cher.store.dll"]
# Utiliser l'image officielle de .NET comme image de base
FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80

# Utiliser l'image de build pour compiler l'application
FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["/home/fadoua/Bureau/Khedma/Entretien_darBlocchain/LeaveManagementSystemAPI.csproj", "LeaveManagementSystemAPI/"]
RUN dotnet restore "/home/fadoua/Bureau/Khedma/Entretien_darBlocchain/LeaveManagementSystemAPI.csproj"
COPY . .
WORKDIR "/src/LeaveManagementSystemAPI"
RUN dotnet build "LeaveManagementSystemAPI.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "LeaveManagementSystemAPI.csproj" -c Release -o /app/publish

# Créer l'image finale
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "LeaveManagementSystemAPI.dll"]
# Étape de build
FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build

WORKDIR /app

# Copier les fichiers projet
COPY . .

# Restaurer les dépendances
RUN dotnet restore PgnNotifications.Client.sln

# Construire la solution
RUN dotnet build PgnNotifications.Client.sln --configuration Release --verbosity minimal

# (Facultatif) Publier l'application si nécessaire
# RUN dotnet publish PgnNotifications.Client.sln -c Release -o /app/publish

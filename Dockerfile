# Étape de build
FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build

# Installer les paquets de base
RUN \
    echo "Installation des paquets de base" \
    && apt-get update \
    && apt-get install -y --no-install-recommends \
    awscli \
    gnupg \
    make \
    jq

# Définir le répertoire de travail
WORKDIR /app

# Copier tous les fichiers du projet dans le conteneur
COPY . .

# Restaurer les dépendances
RUN dotnet restore PgnNotifications.Client.sln

# Construire la solution
RUN dotnet build PgnNotifications.Client.sln --configuration Release --verbosity minimal

# (Facultatif) Publier l'application si nécessaire
# RUN dotnet publish PgnNotifications.Client.sln -c Release -o /app/publish

# Étape de build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

# Installer les paquets système nécessaires avec versions sécurisées
RUN apt-get update && \
    apt-get install -y --no-install-recommends \
        wget \
        libc-bin \
        awscli \
        gnupg \
        make \
        jq \
    && apt-get clean && \
    rm -rf /var/lib/apt/lists/*


# Définir le répertoire de travail
WORKDIR /app

# Copier les fichiers du projet
COPY . .

# Restaurer les dépendances
RUN dotnet restore PgnNotifications.Client.sln

# Construire la solution
RUN dotnet build PgnNotifications.Client.sln --configuration Release --verbosity minimal
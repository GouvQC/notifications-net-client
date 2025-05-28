# Étape de build
FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build

# Installer les paquets système nécessaires avec versions sécurisées
RUN apt-get update && \
    apt-get install -y --no-install-recommends \
        wget=1.21-1+deb11u2 \
        libc-bin=2.31-13+deb11u13 \
        awscli \
        gnupg \
        make \
        jq \
    && apt-get clean && \
    rm -rf /var/lib/apt/lists/*

# Installer CycloneDX pour .NET
RUN dotnet tool install CycloneDX --tool-path /tools


# Ajouter les outils .NET au PATH
ENV PATH="/tools:$PATH"

# Définir le répertoire de travail
WORKDIR /app

# Copier les fichiers du projet
COPY . .

# Restaurer les dépendances
RUN dotnet restore PgnNotifications.Client.sln

# Construire la solution
RUN dotnet build PgnNotifications.Client.sln --configuration Release --verbosity minimal

# Générer le SBOM
RUN /tools/cyclonedx dotnet -p ./PgnNotifications.Client.sln -o sbom.xml



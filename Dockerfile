# �tape de build
FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build

# Installer les paquets syst�me n�cessaires avec versions s�curis�es
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
RUN dotnet tool install --global CycloneDX --version 0.7.2


# Ajouter les outils .NET au PATH
ENV PATH="$PATH:/root/.dotnet/tools"

# D�finir le r�pertoire de travail
WORKDIR /app

# Copier les fichiers du projet
COPY . .

# Restaurer les d�pendances
RUN dotnet restore PgnNotifications.Client.sln

# Construire la solution
RUN dotnet build PgnNotifications.Client.sln --configuration Release --verbosity minimal

# G�n�rer le SBOM (Software Bill of Materials)
RUN cyclonedx dotnet -p ./PgnNotifications.Client.sln -o sbom.xml

# �tape de build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

# Installer les paquets syst�me n�cessaires avec versions s�curis�es
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


# Installer CycloneDX
RUN dotnet tool install CycloneDX.Dotnet --tool-path /tools 

# D�finir le r�pertoire de travail
WORKDIR /app

# Copier les fichiers du projet
COPY . .

# Restaurer les d�pendances
RUN dotnet restore PgnNotifications.Client.sln

# Construire la solution
RUN dotnet build PgnNotifications.Client.sln --configuration Release --verbosity minimal

# Ex�cuter l�image officielle CycloneDX pour g�n�rer le SBOM
RUN docker run --rm -v $(pwd):/src cyclonedx/cyclonedx-dotnet -p /src/PgnNotifications.Client.sln -o /src/sbom.xml



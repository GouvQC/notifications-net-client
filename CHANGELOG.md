## 1.0.1 (2025-06-20)

### Ajouts
* Ajout d’un fichier `.trivyignore` pour ignorer la vulnérabilité transitive non exploitable `CVE-2025-26646`.
* Ajout de références NuGet spécifiques à `net462` pour améliorer la compatibilité (`Microsoft.Build.Tasks.Core`).

### Modifications
* Mise à jour du package NuGet vers la version `1.0.1`.
* Ajustement du workflow GitHub Actions :
  * Utilisation de `dotnet-version: 8.0.x` pour une résolution dynamique des versions mineures.
  * Publication automatique du package `1.0.1` sur NuGet.
* Nettoyage du `Dockerfile` (suppression de lignes vides inutiles).
* Ajout de nouveaux fichiers à ignorer dans `.gitignore`.

### Corrections
* Suppression de messages de débogage inutiles dans `NotificationClient.cs`.

---

## 1.0.0 (2025-06-20)

### Modifications
* Adaptation de la bibliothèque suite à l'intégration de PGN dans PGGAPI.

# Versions antérieures

Historique non enregistré – veuillez consulter les *pull requests* sur GitHub.

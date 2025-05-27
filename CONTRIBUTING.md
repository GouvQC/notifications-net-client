# Lignes directrices sur la contribution

Nous vous remercions de l’intérêt que vous portez à contribuer aux différents projets du Ministère de la Cybersécurité et du Numérique (MCN). Qu’il s’agisse d’un rapport de bogue, d’une nouvelle fonctionnalité, d’une correction ou d’une documentation supplémentaire, nous apprécions grandement les commentaires et les contributions de notre communauté.

Veuillez lire ce document avant de soumettre des problèmes ou des demandes d’extraction pour vous assurer que nous disposons de toutes les informations nécessaires pour répondre efficacement à votre rapport de bogue ou à votre contribution.

## Signaler des bogues/demandes de fonctionnalités

Nous vous invitons à utiliser la boîte du Soutien au développement <assistance.pgn@mcn.gouv.qc.ca> pour signaler des bogues ou suggérer de nouvelles fonctionnalités.

Lorsque vous déposez un problème, nous vérifions rapidement dans le carnet du projet si le problème a déjà été identifié afin de vous informer rapidement s’il y a déjà eu un signalement. On vous demande d’inclure autant d’informations que possible pour faciliter la compréhension du problème rencontré. Des détails comme ceux-ci sont très utiles:

* Un cas de test reproductible ou une série d’étapes;
* La version de notre code utilisé;
* Toutes les modifications que vous avez apportées en rapport avec le problème;
* Tout ce qui est inhabituel dans votre environnement ou votre déploiement.

## Contribuer via des « Pull Requests »

Les contributions via les « pull requests » sont très appréciées. Avant de nous envoyer un « pull request », veuillez vous assurer que :

1. Vous travaillez sur la dernière source sur la branche principale;
2. Vous vérifiez les demandes d’extraction qui sont ouvertes et les fusions récentes pour vous assurer que quelqu’un d’autre n’a pas déjà résolu le problème;
3. Vous ouvrez un problème pour discuter de travaux importants.

Pour nous envoyer un « pull request », veuillez :

1. Faire un « Fork » du référentiel;
2. Modifier la source; (Veuillez vous concentrer sur le changement spécifique ainsi que la contribution que vous souhaitez apporter. Si vous restructurez également tout le code, il nous sera difficile de se concentrer sur votre changement);
3. Assurez-vous que les tests locaux réussissent;
4. Engagez-vous dans votre « fork » à l’aide de messages de validation clairs;
5. Envoyez-nous un « pull request », en répondant à toutes les questions par défaut dans l’interface « pull request »;
6. Faites attention à toute défaillance d’IC automatisée signalée dans la demande d’extraction et restez impliqué dans la conversation.

GitHub fournit un document supplémentaire sur [le forking d’un référentiel](https://help.github.com/articles/fork-a-repo/) et [la création d’une pull request](https://help.github.com/articles/creating-a-pull-request/).

## Trouver des contributions sur lesquels travailler

Examiner les problèmes existants est un excellent moyen de trouver quelque chose sur lequel contribuer. Comme nos projets, par défaut, utilisent les étiquettes de problème GitHub par défaut (amélioration / bogue / duplicata / aide recherchée / invalide / question / wontfix), l’examen de tous les problèmes « aide recherchée » est un excellent point de départ.

## Code de conduite

Ce projet a adopté le *Code de conduite du MCN*. Ce code de conduite couvre toutes les activités des projets sous la gouverne du MCN; qu’il s’agisse de code, de documentation ou de commentaires sur les problèmes ou les relations publiques

Pour plus d’informations, veuillez adresser une demande à la boîte <assistance.pgn@mcn.gouv.qc.ca>, votre message sera traité de manière sécurisée et confidentielle.

## Notifications de problèmes de sécurité

Si vous découvrez un problème de sécurité potentiel dans un projet appartenant au MCN, nous prônons une divulgation responsable des brèches de sécurité. La procédure de divulgation est détaillé dans le document [SECURITY.md](SECURITY.md). Veuillez ne pas créer de problème github public.

## Licences

Voir le fichier [LICENCE](LICENSE) qui identifie les licences potentiellement requises pour soutenir un de nos projets. Nous vous demanderons de confirmer la licence en lien avec votre contribution.

## Configuration

### Conteneur Docker

This app uses dependencies that are difficult to install locally. In order to make local development easy, we run app commands through a Docker container. Run the following to set this up:

Il s'agit d'une base de code écrite pour prendre en charge uniquement .Net C#.

Cette application utilise des dépendances difficiles à installer localement. Afin de faciliter le développement local, nous exécutons les commandes de l'application via un conteneur Docker. Exécutez la commande suivante pour configurer cela :

```shell
make bootstrap-with-docker
```

Comme le conteneur met en cache des éléments tels que les paquets, vous devrez relancer cette commande si vous changez les versions des paquets.

### `environment.sh`

Dans le répertoire racine du dépôt, exécutez :

```
notify-pass credentials/client-integration-tests > environment.sh
```

À moins que vous ne fassiez partie de l’équipe MCN, vous ne pourrez pas exécuter cette commande ni les tests d'intégration. Cependant, le fichier doit quand même exister — exécutez plutôt `touch environment.sh`.


## Tests

Pour exécuter les tests, vous devrez d’abord créer les binaires :

```
make build-with-docker
```

### Unit Tests

Pour exécuter les tests unitaires :

```
make test-with-docker
```

### Integration Tests

Pour exécuter les tests d’intégration :

```
make integration-test-with-docker
```
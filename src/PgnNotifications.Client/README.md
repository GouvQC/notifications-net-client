# Client .NET C# pour la Plateforme Gouvernementale de Notification

Ce client permet d’envoyer des **courriels** et des **messages texte (SMS)** en utilisant l’API de la [Plateforme Gouvernementale de Notification](https://admin.notification.gouv.qc.ca/).

## Fonctionnalités

- Envoi de courriels et de SMS via l’API gouvernementale
- Prise en charge des modèles personnalisés (templates)
- Données de personnalisation pour les messages
- Programmation différée (Scheduled send)
- Suivi des notifications via une référence client

## Exemple d’utilisation

```csharp
// Exemple minimal
var client = NotificationClientBuilder.Create()
    .WithBaseUrl("https://gw-acc-tst.mcn.api.gouv.qc.ca/pgn/") // optionnel, sinon utilise la valeur par défaut (baseUrl de prod)
    .WithApiKey("service_id + api_key")
    .WithClientId("ton-client-id-pggapi")
    .WithTimeout(TimeSpan.FromSeconds(10))
    .WithHandlerBuilder(hb =>
        hb.WithRetry(3)
          .WithLogging(msg => Console.WriteLine($"[LOG] {msg}"))
    )
    .Build();

// Envoi d’un courriel à partir d’un modèle PGN
var response = await client.SendEmailAsync(
    emailAddress: "utilisateur@exemple.com",
    templateId: "modele-abc123",
    personalisation: new Dictionary<string, dynamic> // optionnel, si le template exige des variables
    {
        ["nom"] = "Jean Dupont"
    }
);

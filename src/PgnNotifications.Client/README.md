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
var client = new NotificationClient(...);
await client.SendEmailAsync(new EmailRequest
{
    EmailAddress = "utilisateur@exemple.com",
    TemplateId = "modele-abc123",
    Personalisation = new Dictionary<string, dynamic>
    {
        ["nom"] = "Jean Dupont"
    }
});

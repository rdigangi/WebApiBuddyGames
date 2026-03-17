# WebApiBuddyGames

[![License: MIT](https://img.shields.io/github/license/rdigangi/WebApiBuddyGames)](https://github.com/rdigangi/WebApiBuddyGames/blob/main/LICENSE)
![.NET](https://img.shields.io/badge/.NET-10-512BD4)
![PostgreSQL](https://img.shields.io/badge/PostgreSQL-17-336791)
![EF Core](https://img.shields.io/badge/EF%20Core-10-6E4AFF)

Web API ASP.NET Core (.NET 10) per gestione utenti, partite e risultati.

## Caratteristiche principali

- Swagger (OpenAPI) abilitato
- Architettura a layer:
   - `Api/Controllers`: endpoint REST
   - `Application/Services`: logica applicativa
   - `Application/Repositories`: contratti repository
   - `Infrastructure/Repositories`: accesso a PostgreSQL
   - `Domain/Entities` e `Domain/Dto`: modelli e DTO
- Connessione a PostgreSQL con Entity Framework Core
- Health check su `/health`
- Registrazione utente con hash/salt password (PBKDF2)

## Prerequisiti

- .NET SDK 10
- PostgreSQL disponibile localmente o remoto
- (Opzionale) `dotnet-ef` per gestire migration manualmente

## Configurazione

Imposta la connection string in `appsettings.json`:

```json
"ConnectionStrings": {
   "Postgres": "Host=localhost;Port=5432;Database=webapibuddygames;Username=postgres;Password=postgres"
}
```

Puoi sovrascrivere i valori in `appsettings.Development.json` o tramite variabili ambiente.

## Avvio progetto

1. Ripristina dipendenze:
    - `dotnet restore`
2. Compila:
    - `dotnet build`
3. Avvia API:
    - `dotnet run`
4. Apri Swagger UI:
    - `http://localhost:<porta>/swagger`

## Endpoint disponibili (principali)

- `POST /api/authentication/register`
   - body: `username`, `password`, `email`, `nome`, `cognome`
- CRUD REST:
   - `/api/utenti`
   - `/api/partite`
   - `/api/partiteutenti`
   - `/api/risultati`

Tutte le risposte sono JSON.

## Migrations Entity Framework

Installazione tool (se non presente):

- `dotnet tool install --global dotnet-ef`

Creare una migration:

- `dotnet ef migrations add <NomeMigration>`

Applicare migration al database:

- `dotnet ef database update`

Rimuovere ultima migration non applicata:

- `dotnet ef migrations remove`

## Struttura sintetica

```text
Api/
   Controllers/
Application/
   Common/
   Extensions/
   Repositories/
   Services/
      Interfaces/
      Implementations/
Domain/
   Entities/
   Dto/
      RequestDto/
Infrastructure/
   Data/
   Extensions/
   Repositories/
Migrations/
```

## Versionamento

- Repository pronto per Git (`.gitignore` e `.gitattributes` presenti)
- Licenza MIT inclusa in [LICENSE](LICENSE)

## Note

- Le password non vengono mai salvate in chiaro.
- I record applicativi usano `Attivo` + `DataCancellazione` per gestione soft delete.

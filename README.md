# WebApiBuddyGames

[![License: MIT](https://img.shields.io/github/license/rdigangi/WebApiBuddyGames)](https://github.com/rdigangi/WebApiBuddyGames/blob/main/LICENSE)
![.NET](https://img.shields.io/badge/.NET-10-512BD4)
![PostgreSQL](https://img.shields.io/badge/PostgreSQL-17-336791)
![EF Core](https://img.shields.io/badge/EF%20Core-10-6E4AFF)

Web API ASP.NET Core (.NET 10) per gestione utenti, ruoli, partite e risultati.

## Caratteristiche principali

- Swagger (OpenAPI) abilitato
- Swagger UI con pulsante `Authorize` per inserire JWT Bearer token
- Architettura a layer:
   - `Api/Controllers`: endpoint REST
   - `Application/Services`: logica applicativa
   - `Application/Repositories`: contratti repository
   - `Infrastructure/Repositories`: accesso a PostgreSQL
   - `Domain/Entities` e `Domain/Dto`: modelli e DTO
- Connessione a PostgreSQL con Entity Framework Core
- Health check su `/health`
- Registrazione utente con hash/salt password (PBKDF2)
- Login con verifica credenziali (username o email + password)
- JWT Access Token con claim base (`sub`, `username`, `email`, `role`)
- Refresh Token con rotazione e persistenza su database
- Claim `nome` incluso nel JWT per esigenze UI immediate
- Gestione ruoli tramite tabelle `ruoli` e `utenti_ruoli`
- Seed iniziale ruoli: `Amministratore`, `Utente`
- Gestione immagine profilo su Cloudflare R2 (salvataggio URL nel DB utenti)

## Prerequisiti

- .NET SDK 10
- PostgreSQL disponibile localmente o remoto
- (Opzionale) `dotnet-ef` per gestire migration manualmente

## Configurazione

Imposta la configurazione in `appsettings.json`:

```json
"ConnectionStrings": {
   "Postgres": "Host=localhost;Port=5432;Database=webapibuddygames;Username=postgres;Password=postgres"
},
"Jwt": {
   "Issuer": "WebApiBuddyGames",
   "Audience": "WebApiBuddyGames.Client",
   "SecretKey": "CHANGE_ME_WITH_A_VERY_LONG_SECRET_KEY_32+",
   "AccessTokenMinutes": 15,
   "RefreshTokenDays": 7
},
"CloudflareR2": {
   "AccountId": "<account-id>",
   "AccessKeyId": "<access-key-id>",
   "SecretAccessKey": "<secret-access-key>",
   "BucketName": "<bucket-name>",
   "ServiceUrl": "https://<account-id>.r2.cloudflarestorage.com",
   "PublicBaseUrl": "https://<tuo-dominio-pubblico-o-r2-dev-url>/<bucket-name>"
}
```

Puoi sovrascrivere i valori in `appsettings.Development.json` o tramite variabili ambiente.

### Segreti locali (consigliato)

Per evitare di salvare chiavi sensibili nel repository, usa `dotnet user-secrets`:

- `dotnet user-secrets init`
- `dotnet user-secrets set "CloudflareR2:AccountId" "<value>"`
- `dotnet user-secrets set "CloudflareR2:AccessKeyId" "<value>"`
- `dotnet user-secrets set "CloudflareR2:SecretAccessKey" "<value>"`
- `dotnet user-secrets set "CloudflareR2:BucketName" "<value>"`
- `dotnet user-secrets set "CloudflareR2:ServiceUrl" "<value>"`
- `dotnet user-secrets set "CloudflareR2:PublicBaseUrl" "<value>"`

## Avvio progetto

1. Ripristina dipendenze:
    - `dotnet restore`
2. Compila:
    - `dotnet build`
3. Avvia API:
    - `dotnet run`
4. Apri Swagger UI:
    - `http://localhost:<porta>/swagger`

Per testare endpoint protetti da Swagger:

1. Esegui login (`POST /api/authentication/login`)
2. Copia `accessToken`
3. Clicca `Authorize` in alto a destra
4. Inserisci `Bearer <accessToken>`

## Endpoint disponibili (principali)

- `POST /api/authentication/register`
   - body: `username`, `password`, `email`, `nome`, `cognome`
- `POST /api/authentication/login`
   - body: `usernameOrEmail`, `password`
   - response: `accessToken`, `refreshToken`, scadenze UTC
- `POST /api/authentication/refresh`
   - body: `refreshToken`
   - response: nuovi `accessToken` + `refreshToken`
- `GET /me`
   - header: `Authorization: Bearer <accessToken>`
   - response: profilo completo utente loggato (`id`, `username`, `nome`, `cognome`, `email`, `profileImageUrl`, `ruoli`)
- `POST /me/profile-image`
   - header: `Authorization: Bearer <accessToken>`
   - body: `multipart/form-data` con campo `file`
   - upload immagine su R2 e aggiornamento `ProfileImageUrl`
   - vincoli file: formati `jpg/jpeg`, `png`, `webp`, `gif`; dimensione massima `5 MB`
- `DELETE /me/profile-image`
   - header: `Authorization: Bearer <accessToken>`
   - rimozione immagine su R2 + reset `ProfileImageUrl`
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

Migration attualmente presenti:

- `InitialCreate`
- `AddRuoliUtentiRuoli`
- `AddRefreshTokensJwtAuth`
- `AddProfileImageUrlAndCloudflareR2`

## Modello dati ruoli

- `ruoli`: anagrafica ruoli applicativi (con campi base comuni e identity)
- `utenti_ruoli`: associazione tra utente e ruolo (con campi base comuni e identity)
- Vincolo univoco su coppia `UtenteId` + `RuoloId`

## Modello dati refresh token

- `refresh_tokens`: token di rinnovo (in DB viene salvato solo hash SHA-256)
- Collegamento con `utenti` tramite `UtenteId`
- Rotazione refresh token ad ogni chiamata di refresh

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

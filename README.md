# WebApiBuddyGames

Struttura base per una Web API ASP.NET Core (.NET 10) con:

- Swagger (OpenAPI) abilitato
- Architettura a layer:
   - Controllers: endpoint HTTP
   - Services: logica di business
   - Repositories: accesso al database
- Connessione a PostgreSQL con Entity Framework Core
- Health check su `/health`

## Configurazione

Modifica la connection string in `appsettings.json`:

```json
"ConnectionStrings": {
  "Postgres": "Host=localhost;Port=5432;Database=webapibuddygames;Username=postgres;Password=postgres"
}
```

## Avvio

1. Ripristina e compila:
   - `dotnet restore`
   - `dotnet build`
2. Avvia:
   - `dotnet run`
3. Apri Swagger UI:
   - `/swagger`

## Entity Framework migrations

Installa lo strumento EF (se non presente):

- `dotnet tool install --global dotnet-ef`

Crea una migration iniziale:

- `dotnet ef migrations add InitialCreate`

Applica la migration al DB:

- `dotnet ef database update`

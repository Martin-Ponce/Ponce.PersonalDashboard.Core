# Running in a Container

This covers building and running the API as a Docker container: the exact commands, where to run them
from, and how configuration (connection string, provider, and everything else) is supplied at run time via
environment variables instead of `appsettings.json`.

> This assumes Docker Desktop (or another local Docker engine) is installed and running.

## Quick start: Docker Compose (API + SQL Server)

[`docker-compose.yml`](../docker-compose.yml) at the solution root runs the API with its own SQL Server
container (data kept in the `db-data` volume). The identity provider keeps running on the host
(`dotnet run`, `http://localhost:5008`) and the web app separately (`ng serve` on `:4200`, or its own image
on `:8080`).

```powershell
copy .env.example .env    # then set MSSQL_SA_PASSWORD in .env (never commit it; .env is git-ignored)
docker compose up -d --build
```

- The API listens on `http://localhost:5010` (change it with `API_PORT` in `.env`), the same address the web
  app uses during development.
- On first start, SQL Server needs a few seconds; the API waits for its health check, then
  `EnsureDatabaseCreated` creates the `PersonalDashboard` database and its tables in the container. It's a
  different database from your LocalDB one, so it starts empty.
- `docker compose logs -f api` shows the API's console log; `docker compose down` stops everything
  (`down -v` also deletes the database volume).

Every other setting in `.env.example` is optional and documented there. The ones the compose file sets:

| Env var | Default in compose | Why |
|---|---|---|
| `DATABASE_CONNECTION_STRING` | `Server=db;…;User Id=sa;Password=${MSSQL_SA_PASSWORD}` | SQL Server authentication, see [below](#against-a-real-sql-server-instance) |
| `AUTH_AUTHORITY` | `http://localhost:5008/` | The issuer, as browsers see it |
| `AUTH_INTERNAL_AUTHORITY` | `http://host.docker.internal:5008/` | Where the container reaches it (next section) |
| `AUTH_REQUIRE_HTTPS_METADATA` | `false` | The local IdP runs over plain http |
| `CORS_ORIGINS` | `http://localhost:4200,http://localhost:8080` | The web app's dev server and its container |

## The identity provider from inside a container

Tokens are issued by the address browsers use, e.g. `http://localhost:5008/`, and `AUTH_AUTHORITY` must be
exactly that issuer. But inside a container, `localhost` is the container itself, so the API couldn't
download the IdP's discovery document and signing keys and would reject every token (`401`).

`AUTH_INTERNAL_AUTHORITY` fixes that: JWT bearer sends its requests to the IdP (the discovery document, and
the keys at the `jwks_uri` it lists) to the internal address instead
([`InternalAuthorityHandler`](../Ponce.PersonalDashboard.Core.API/Helpers/Security/InternalAuthorityHandler.cs)),
while tokens must still carry the public issuer. On Docker Desktop, `host.docker.internal` is your machine;
the compose file also maps it on Linux engines (`host-gateway`).

- If the API still logs that it can't reach the IdP, make sure the IdP accepts connections from the
  container: start it listening on every interface (`dotnet run --urls http://0.0.0.0:5008`) and allow the
  port through the firewall. Its public URL (`IDENTITY_PUBLIC_BASE_URL`) stays `http://localhost:5008`.
- Where the issuer's address is reachable from the container too (a real domain in another environment),
  leave `AUTH_INTERNAL_AUTHORITY` unset.

## Where to run the commands from

Always run `docker build`/`docker run` from the **solution root**
(`Ponce.PersonalDashboard.Core/`), not from inside `Ponce.PersonalDashboard.Core.API/`. The Dockerfile lives
at [`Ponce.PersonalDashboard.Core.API/Dockerfile`](../Ponce.PersonalDashboard.Core.API/Dockerfile), but the
API project references the other three (`Application`, `Domain`, `Infrastructure`), so the Docker **build
context** has to include all four project folders. Docker's `COPY` instruction can never reach outside the
build context — if you built from inside the `API` folder, the sibling projects would be invisible to it
and the build would fail. Point `-f` at the nested Dockerfile instead and keep the context at the root:

```powershell
docker build -f Ponce.PersonalDashboard.Core.API/Dockerfile -t apitemplate:latest .
```

## How configuration reaches the container

The app never reads environment variables directly for its own sake — every configuration value is read
from `IConfiguration` first (`appsettings.json` → `appsettings.{Environment}.json` → user secrets in
`Development`, see [Getting Started](01-getting-started.md#configuration)), and *then* optionally
overridden by a specific environment variable, via
[`EnvironmentVariablesReader.TryOverwriteWithEnviromentValue`](../Ponce.PersonalDashboard.Core.API/Helpers/EnvironmentVariablesReader.cs):

```csharp
public static string? TryOverwriteWithEnviromentValue(this string? originalValue, string environmentVariableName)
{
    var environmentValue = Environment.GetEnvironmentVariable(environmentVariableName.ToUpper().Trim());
    if (!string.IsNullOrEmpty(environmentValue))
        return environmentValue;
    return originalValue;
}
```

That's exactly what makes `docker run -e <NAME>=<value>` work: there's no `appsettings.json` baked into the
image with real values (and there shouldn't be — it still only has the placeholder values from
[`appsettings.json`](../Ponce.PersonalDashboard.Core.API/appsettings.json)), so every setting that matters
for a given environment gets supplied as a container env var instead.

Each setting has its **own specific env var name**, defined right where it's read in
`API/Extensions/Configuration/*ConfigurationsReader.cs` — there's no single auto-derived naming rule. The
full list as of this template:

| Setting | `appsettings.json` key | Env var | Notes |
|---|---|---|---|
| DB connection string | `DatabaseConnections:Database:ConnectionString` | `DATABASE_CONNECTION_STRING` | Required |
| DB provider | `DatabaseConnections:Database:Provider` | `DATABASE_DATABASE_PROVIDER` | `SqlServer` / `Postgres` / `Sqlite` |
| Rate limit | `MaxCallsPerMinute` | `MAX_CALLS` | Integer |
| Cache enabled | `Cache:Enabled` | `CACHE_ENABLED` | Boolean |
| Cache sliding expiration | `Cache:SlidingExpirationInMinutes` | `CACHE_SLIDING_DURATION` | Integer minutes |
| Cache absolute expiration | `Cache:AbsoluteExpirationInMinutes` | `CACHE_ABSOLUTE_DURATION` | Integer minutes |
| CORS allowed origins | `Cors:Origins` | `CORS_ORIGINS` | Comma-separated |
| CORS exposed headers | `Cors:Headers` | `CORS_HEADERS` | Comma-separated |
| Identity provider URL | `Authentication:Authority` | `AUTH_AUTHORITY` | Required. Must match the `issuer` in the IdP's `/.well-known/openid-configuration` exactly |
| Require HTTPS for IdP metadata | `Authentication:RequireHttpsMetadata` | `AUTH_REQUIRE_HTTPS_METADATA` | Boolean, default `true`; `false` only for a local IdP over plain http |
| Where the API reaches the IdP | `Authentication:InternalAuthority` | `AUTH_INTERNAL_AUTHORITY` | Optional. Set it when the container can't reach the issuer's address (see [The identity provider from inside a container](#the-identity-provider-from-inside-a-container)) |
| Development user (no IdP) | `Authentication:UseDevelopmentUser` | `AUTH_USE_DEVELOPMENT_USER` | Boolean, default `false`. Ignored outside `Development`, so it never applies to the container |
| Telemetry enabled | `Telemetry:Enabled` | `TELEMETRY_ENABLED` | Boolean |
| Telemetry traces destination | `Telemetry:TracesDestination` | `TELEMETRY_TRACES_DESTINATION` | URL |
| Telemetry logs destination | `Telemetry:LogsDestination` | `TELEMETRY_LOGS_DESTINATION` | URL |
| Telemetry logs min. level | `Telemetry:LogsMinimumLevel` | `TELEMETRY_LOGS_MINIMUM_LEVEL` | Serilog level name (e.g. `Error`) |
| Log HTTP requests | `Logger:LogHttpRequests` | `LOG_HTTP` | Boolean |
| DB logger enabled | `Logger:Database:Enabled` | `LOGGER_DATABASE_ENABLED` | Boolean |
| DB logger min. level | `Logger:Database:MinimumLevel` | `LOGGER_DATABASE_MINIMUM_LEVEL` | Serilog level name |

The lookup is case-insensitive (the name is upper-cased before reading it), but write them upper-case for
clarity, as above. If you add a new configuration section later, follow the same pattern: read the
`appsettings.json` value, then chain `.TryOverwriteWithEnviromentValue("YOUR_ENV_VAR_NAME")` on it in the
relevant `*ConfigurationsReader.cs` file, and add a row to this table.

## Running the container

### Quick test with SQLite (no external database needed)

```powershell
docker run -p 5000:8080 `
  -e DATABASE_CONNECTION_STRING="Data Source=/tmp/container-test.db" `
  -e DATABASE_DATABASE_PROVIDER="Sqlite" `
  --name apitemplate apitemplate:latest
```

The database file is created in the container's `/tmp` on first run (the app runs as a non-root user that
can't write to `/app`, except its `Logs` folder). It's
ephemeral — removing the container discards it — which is exactly what you want for a quick smoke test.
(If you paste a multi-line command like this into PowerShell, make sure each backtick `` ` `` is the very
last character on its line — a trailing space after it silently breaks the line continuation and
PowerShell will try to run the next line as its own command.)

### Against a real SQL Server instance

Point the connection string at the SQL Server instance reachable from inside the container. If that
instance is running on your own machine (not in a container), use Docker Desktop's special DNS name
`host.docker.internal` instead of `.`/`localhost` — from inside a Linux container, `localhost` means "the
container itself":

```powershell
docker run -p 5000:8080 `
  -e DATABASE_CONNECTION_STRING="Server=host.docker.internal;Initial Catalog=MyDb;User Id=MyUser;Password=MyPassword;Encrypt=False;TrustServerCertificate=True" `
  -e DATABASE_DATABASE_PROVIDER="SqlServer" `
  --name apitemplate apitemplate:latest
```

**Use SQL Server authentication (`User Id`/`Password`), not `Trusted_Connection=True`.** The container
image is Linux-based and has no Windows domain identity to negotiate Windows Integrated Authentication
with — attempting it fails at connection time with something like:

```
Cannot load library libgssapi_krb5.so.2
...The type initializer for 'NetSecurityNative' threw an exception.
```

This isn't a missing-package problem you can just `apt-get install` your way out of — Integrated Auth
needs a real Kerberos-configured, effectively domain-joined client, which a plain container isn't. If your
SQL Server instance only has Windows Authentication enabled, enable "SQL Server and Windows Authentication
mode" (mixed mode) and create a SQL login first.

### Against PostgreSQL

Same shape, swapping the provider and using Postgres's own connection string format:

```powershell
docker run -p 5000:8080 `
  -e DATABASE_CONNECTION_STRING="Host=host.docker.internal;Database=MyDb;Username=MyUser;Password=MyPassword" `
  -e DATABASE_DATABASE_PROVIDER="Postgres" `
  --name apitemplate apitemplate:latest
```

## Verifying it worked

```powershell
curl http://localhost:5000/
```

This is the welcome/health page (see [`HomeMapping.cs`](../Ponce.PersonalDashboard.Core.API/Extensions/Endpoints/HomeMapping.cs))
— it lists every health check (`Liveness`, `Database`) with a green/red/amber status, which is the fastest
way to confirm both that the container started *and* that it can actually reach the database you pointed
it at. Then exercise a real endpoint (remember the `X-Version: 1.0` header, see
[Getting Started](01-getting-started.md#running-the-api)):

```powershell
curl http://localhost:5000/api/exampleitems -H "X-Version: 1.0"
```

## Cleaning up

```powershell
docker rm -f apitemplate   # stop + remove the container
docker rmi apitemplate:latest   # remove the built image
```

Re-running `docker run` with the same `--name` while a previous container by that name still exists (even
stopped) fails with a name conflict — `docker rm -f apitemplate` first (as shown in the run examples above)
handles that.

## Table-creation caveat still applies

Whichever provider you point the container at, remember `EnsureDatabaseCreated` in `Program.cs` only
creates tables in a database that doesn't exist yet, or exists with zero tables — see the note in
[Getting Started](01-getting-started.md#running-the-api). Pointing the container at an existing database
that already has other tables in it will not create any tables for entities added since.

## Lokal backendintegration

Frontend använder en Next.js-rewrite för att vidarebefordra
`/api/...` till .NET-backend.

### Adresser

- Frontend: https://localhost:3000
- Backend: https://localhost:4443
- API-adresser i browserkod: /api/...

### Frontendkonfiguration

Kopiera `.env.example` till `.env.local`.

BACKEND_URL används av Next-servern och ska inte ha
NEXT_PUBLIC_-prefix.

Databasanslutning och demonstrationskontots uppgifter
konfigureras endast i backendens User Secrets:

- ConnectionStrings:SqlServer
- Seed:StaffEmail
- Seed:StaffPassword

### Certifikat

Lita på och exportera .NET:s utvecklingscertifikat:

```powershell
dotnet dev-certs https --trust

dotnet dev-certs https `
  --export-path "$env:TEMP\leadflow-backend.crt" `
  --format PEM
```

Ange följande i frontendterminalen innan Next startas:

```powershell
$env:NODE_EXTRA_CA_CERTS = "$env:TEMP\leadflow-backend.crt"
```

Inställningen behöver sättas igen i en ny terminalsession.
Exportera certifikatet igen om utvecklingscertifikatet byts ut.

### Databas

Från Backend, efter att User Secrets har konfigurerats:

```powershell
dotnet ef database update `
  --project src/LeadFlow.Infrastructure `
  --startup-project src/LeadFlow.Api
```

Kommandot kräver att dotnet-ef finns installerat.

### Starta backend — terminal 1

Från Backend:

```powershell
dotnet run --project src/LeadFlow.Api --launch-profile "https development"
```

### Starta frontend — terminal 2

Från frontend:

```powershell
npm ci

$env:NODE_EXTRA_CA_CERTS = "$env:TEMP\leadflow-backend.crt"

npm run dev -- --experimental-https --port 3000
```

Next kan behöva godkännande för att installera sin lokala
certifikatutfärdare första gången HTTPS används.

Öppna https://localhost:3000/auth/login.

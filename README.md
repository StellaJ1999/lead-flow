## Kör projektet lokalt

### Förutsättningar

- .NET 10 SDK
- Node.js och npm
- SQL Server Express LocalDB med instansen `MSSQLLocalDB`

### 1. Konfigurera backend och databasen

Öppna PowerShell i repots rot och kör:

```powershell
cd Backend
dotnet tool restore

dotnet user-secrets set "ConnectionStrings:SqlServer" "Server=(localdb)\MSSQLLocalDB;Database=LeadFlow;Trusted_Connection=True;TrustServerCertificate=True" --project src/LeadFlow.Api
dotnet user-secrets set "Seed:StaffEmail" "demo@example.com" --project src/LeadFlow.Api
dotnet user-secrets set "Seed:StaffPassword" "<välj ett eget demolösenord>" --project src/LeadFlow.Api
```

Byt ut lösenordsplatshållaren mot ett eget lösenord för det lokala demokontot. Uppgifterna sparas lokalt i .NET User Secrets.

Tillämpa den befintliga migrationen för att skapa eller uppdatera databasen:

```powershell
dotnet ef database update --project src/LeadFlow.Infrastructure --startup-project src/LeadFlow.Api
```

### 2. Förbered HTTPS och starta backend

Kör följande i samma terminal:

```powershell
dotnet dev-certs https --trust
dotnet dev-certs https --export-path "$env:TEMP\leadflow-backend.crt" --format PEM

dotnet run --project src/LeadFlow.Api --launch-profile "https development"
```

Backend körs på `https://localhost:4443`. Låt terminalen vara öppen.

### 3. Starta frontend

Öppna en ny PowerShell-terminal i repots rot:

```powershell
cd frontend
Copy-Item .env.example .env.local
npm ci
```

Kontrollera att `.env.local` innehåller följande:

```dotenv
BACKEND_URL=https://localhost:4443
```

Starta frontend:

```powershell
$env:NODE_EXTRA_CA_CERTS = "$env:TEMP\leadflow-backend.crt"
npm run dev -- --experimental-https --port 3000
```

Sätt `NODE_EXTRA_CA_CERTS` igen varje gång du startar frontend från en ny terminal.

### 4. Prova kärnflödet

1. Öppna [det publika formuläret](https://localhost:3000) och skicka in en intresseanmälan med fiktiva uppgifter.
2. Öppna [inloggningssidan](https://localhost:3000/auth/login) och logga in med demokontot.
3. Leta upp anmälan i leadlistan. Prova filtret **New** och öppna leadet.
4. Ändra status till **Contacted**, lägg till en uppföljningsanteckning och avsluta sedan leadet med ett utfall.

### Kör automatiserade tester

Kör från mappen `Backend`:

```powershell
dotnet test LeadFlow.slnx
```

De automatiserade lagringstesterna använder SQLite i minnet och kan köras utan den lokala SQL Server-databasen.

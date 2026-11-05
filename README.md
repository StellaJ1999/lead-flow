# LeadFlow

LeadFlow är en webbapplikation för mindre företag som vill samla intresseanmälningar och följa upp potentiella kunder.

Besökare kan skicka in en intresseanmälan utan att logga in. Inloggade medarbetare kan hantera inkomna leads, ändra status och dokumentera uppföljningen.

Projektet är ett examinationsarbete inom teknikfördjupning med fokus på AI och utveckling i .NET.

## Bakgrund och mål

Mindre företag kan få intresseanmälningar via formulär och e-post utan ett gemensamt arbetssätt för uppföljning. Det kan göra det svårt att se vilka potentiella kunder som har kontaktats och vad nästa steg är.

Målet med projektet är att utveckla en avgränsad webbapplikation som stödjer flödet från inskickad intresseanmälan till dokumenterad uppföljning. Lösningen ska kunna demonstreras lokalt och innehålla relevanta tester och dokumentation.

## Funktioner

### För besökare

- Skicka en intresseanmälan utan inloggning.
- Ange namn, e-postadress och meddelande.
- Ange företagsnamn som ett valfritt fält.
- Få valideringsmeddelanden och en bekräftelse när anmälan har sparats.

### För inloggade medarbetare

- Se inkomna leads i en tabell.
- Filtrera listan på status.
- Öppna en detaljsida för ett lead.
- Markera ett lead som kontaktat.
- Avsluta ett lead med ett angivet utfall.
- Återöppna avslutade leads.
- Lägga till uppföljningsanteckningar.
- Se anteckningar med författare och tidpunkt, med den senaste först.
- Radera leads från detaljsidan.

## Kärnflöde

Intresseanmälan → New → Contacted → Closed

| Status | Betydelse |
| --- | --- |
| `New` | En ny intresseanmälan har registrerats. |
| `Contacted` | Leadet har markerats som kontaktat. |
| `Closed` | Uppföljningen har avslutats. |

När ett lead avslutas måste ett utfall anges:

| Utfall | Betydelse |
| --- | --- |
| `Customer` | Kontakten blev kund. |
| `NotInterested` | Kontakten var inte intresserad. |

Ett avslutat lead kan återöppnas. Då får det status `Contacted` och det tidigare avslutsutfallet rensas.

## Teknik

| Område | Teknik |
| --- | --- |
| Backend | C#, .NET 10, ASP.NET Core Web API |
| Frontend | Next.js, TypeScript |
| Styling | Tailwind CSS |
| Databas | SQL Server |
| Databasåtkomst | Entity Framework Core 10 |
| Inloggning | ASP.NET Core Identity |
| Automatiserade tester | xUnit |
| Databas för automatiserade tester | SQLite i minnet |
| Manuell testning | Postman och webbläsarens DevTools |
| Versionshantering | Git och GitHub |
| Planering | GitHub Issues |
| Dokumentation | Google Docs och README |

## Uppbyggnad

Frontend kommunicerar med backend via en proxy. Backend använder controllers, services, repositories och domänmodeller.

- **Controllers** tar emot API-anrop och returnerar svar.
- **Services** hanterar applikationslogiken.
- **Repositories** använder Entity Framework Core för att läsa och spara data.
- **Domänmodeller** innehåller regler för leads, statusändringar och uppföljningsanteckningar.

## Validering och säkerhet

### Validering

Servern kontrollerar obligatoriska fält, e-postformat och maximala fältlängder. Giltiga intresseanmälningar sparas som leads med status `New`. Interna värden bestäms av servern.

Uppföljningsanteckningar valideras också. Tom text, enbart blanksteg och text som överstiger tillåten längd avvisas.

### Inloggning och behörighet

ASP.NET Core Identity används för lösenordsverifiering och cookieinloggning genom `SignInManager.PasswordSignInAsync`.

Interna API-anrop för att läsa, ändra och radera leads kräver en inloggad användare. Behörighetskontrollen sker i API:t och gäller även vid direkta anrop utanför frontend.

Fel lösenord och okänd e-postadress ger samma felmeddelande vid inloggning.

### CSRF-skydd

Anrop som ändrar data kräver en CSRF-token i headern `X-CSRF-TOKEN`. Token valideras tillsammans med en separat CSRF-cookie.

Kravet gäller även det publika intresseformuläret, trots att besökaren inte behöver vara inloggad.

## Lokal utvecklingsmiljö

Projektets lokala utvecklingsmiljö använder:

- .NET 10.
- Node.js för Next.js.
- SQL Server LocalDB.
- Lokal HTTPS för backend.
- User Secrets för konfiguration av lokala startdata.
- Entity Framework Core-migrationer för databasen.

Den lokala SQL Server-instansen är `(localdb)\MSSQLLocalDB`.

## Testning och testresultat

### Automatiserade tester

Vid den dokumenterade testkörningen passerade **45 av 45 tester**.

| Testområde | Antal |
| --- | ---: |
| Affärsregler för leads och uppföljningsanteckningar | 19 |
| Validering av intresseanmälningar | 3 |
| Lagring, läsning och radering av leads | 3 |
| Statusändringar som sparas eller avvisas | 13 |
| Uppföljningsanteckningar som sparas eller avvisas | 7 |
| **Totalt** | **45** |

Testerna omfattar bland annat:

- Obligatoriska fält, e-postformat och maximala fältlängder.
- Tillåtna och otillåtna kombinationer av status och avslutsutfall.
- Att återöppnade leads får status `Contacted` och att avslutsutfallet rensas.
- Att avvisade statusändringar inte påverkar lagrade uppgifter, även vid en senare sparning.
- Att leads sparas och kan läsas tillbaka med en ny databaskontext.
- Validering av uppföljningsanteckningar.
- Hantering av saknat lead och saknad författare.
- Att anteckningar sparas med rätt text, leadkoppling, författar-ID och skapandetid.

### Manuell testning

API-endpoints har testats manuellt i Postman. Testningen omfattade inloggning, utloggning, hämtning av aktuell användare och CSRF-token samt hantering av leads och uppföljningsanteckningar.

Kommunikationen mellan frontend och backend har kontrollerats i webbläsarens DevTools, bland annat för att verifiera att autentiseringscookien sätts vid inloggning och att skyddade anrop nekas efter utloggning.

| Testfall | Resultat |
| --- | --- |
| Inloggning med giltiga uppgifter | `204 No Content` |
| Fel lösenord eller okänd e-postadress | `401 Unauthorized` |
| Skyddat anrop utan inloggning | Åtkomst nekas |
| Skapa lead utan CSRF-token | `400 Bad Request`, inget lead skapas |
| Skapa lead med felaktig CSRF-token | `400 Bad Request`, inget lead skapas |
| Skapa lead med giltiga uppgifter och korrekt CSRF-token | `201 Created`, leadet sparas |

### Begränsningar i testningen

De automatiserade databastesterna använder SQLite i minnet, medan applikationen använder SQL Server. Testerna verifierar därför inte SQL Server-specifikt beteende eller att SQL Server-migrationerna fungerar.

Testsviten innehåller inte automatiserade HTTP-tester som verifierar autentisering, CSRF-skydd och modellvalidering tillsammans genom hela API:t.

## Beslut och avgränsningar

Projektets ursprungliga inriktning och huvudsakliga teknikval behölls under arbetet.

Den 29 oktober implementerades statusfiltrering i frontend på den redan hämtade leadlistan. Jag beslutade att avstå från filtrering i backend eftersom jag låg efter i tidsplanen och behövde avgränsa arbetet och prioritera kärnfunktionerna.

Filtrering på serversidan och mer avancerad sökning ingår därför inte i slutversionen.

## Möjlig vidareutveckling

- Filtrering och sökning på serversidan.
- Automatiserade HTTP-tester för autentisering, CSRF-skydd och modellvalidering.
- Tester mot SQL Server för databasspecifikt beteende och migrationer.

## Användning av AI

AI har använts som stöd för planering, kodförslag, felsökning, tekniska förklaringar, kodgranskning, commit-meddelanden och dokumentation.

Jag har främst använt OpenAI Codex, men även Copilot och Googles sökmotors AI.

### Arbetssätt

AI användes inledningsvis för att diskutera projektidéer med CRM-tema och dela upp arbetet i GitHub Issues.

Inför implementationen använde jag stegvisa planer, egen research och dokumentation för att förstå koncepten. Vid kodförslag efterfrågade jag ingående förklaringar och granskade koden innan jag skrev in den manuellt i projektet.

Tidigare kursprojekt användes som referens för struktur och lösningar. Det hjälpte mig att hålla projektet på en rimlig nivå och inom den tillgängliga tidsramen.

### Reflektion

Det har varit en utmaning att bedöma kvaliteten i AI-genererad kod. Förslagen har därför behövt kontrolleras genom dokumentation, tidigare kunskaper, kompilering och testning.

Ett exempel var ett kompileringsfel där `SignInAsync` användes med en parameter som metoden inte stödde. Vid felsökningen korrigerades anropet till `PasswordSignInAsync`.

Manuell testning i Postman och webbläsaren har varit en viktig del av arbetet för att kontrollera att lösningarna fungerar.

## Projektets genomförande

| Datum | Arbete |
| --- | --- |
| 17/10 | Problemformulering, teknikval och planering med stöd av AI. |
| 21/10 | Domänmodeller, affärsregler, Identity och konfiguration av lokala startdata. |
| 22/10 | Validering, databasmigration och inloggning. |
| 23/10 | Backendstöd för att skapa och hämta leads. |
| 28/10 | Proxy, cookies, CSRF-skydd och arbete med intresseformuläret. |
| 29/10 | Leadlista, statusfiltrering och arbete med detaljsidan. |
| 2/11 | Statusändringar med avslutsutfall och återöppning. |
| 3/11 | Uppföljningsanteckningar, tester, felsökning och rapportarbete. |
| 5/11 | Färdigställande och inlämning. |

Arbetet har planerats med GitHub Issues och dokumenterats genom commits och en projektdagbok.
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

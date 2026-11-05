# LeadFlow
## Problemformulering

Mindre företag kan få intresseanmälningar via exempelvis formulär och e-post utan ett gemensamt arbetssätt för uppföljning. Det kan göra det svårt att se vilka potentiella kunder som har kontaktats och vad nästa steg är. Projektet undersöker hur en avgränsad webbapplikation i .NET kan stödja flödet från inskickad intresseanmälan till dokumenterad uppföljning.

## Projektmål

Utveckla en webbapplikation där en besökare kan lämna en intresseanmälan och en inloggad användare kan se, filtrera och följa upp inkomna leads. Lösningen ska kunna demonstreras lokalt och innehålla relevanta tester samt dokumentation.
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

## Projektdagbok

### 17/10

Jag skapade det här Google-dokumentet för att planera projektet och skriva ner vad jag gör. Jag gjorde tekniska val för applikationen och delade upp arbetet i steg utifrån tiden jag har. Jag använde AI för att bolla projektidéer och planera arbetet i sprintar.

### 21/10

Jag skapade domänmodellerna och bestämde vilka regler som gäller för leads. Ett nytt lead får status New och måste ha namn, e-postadress och meddelande. Företagsnamn är valfritt. Ett lead kan markeras som kontaktat (Contacted) eller avslutat (Closed). När ett lead avslutas måste jag ange om det blev kund (Customer) eller inte var intresserade (NotInterested). Om leadet öppnas igen får det status Contacted och det tidigare utfallet tas bort.

Jag gjorde det möjligt att lägga till flera uppföljningsanteckningar per lead. Varje anteckning innehåller text, författarens ID och tidpunkten då den skapades. Jag satte också upp Identity med ett tidigare projekt som förebild och konfigurerade hur startdata läggs in lokalt med hjälp av User Secrets. För den lokala SQL Server-instansen använder jag (localdb)\MSSQLLocalDB som servernamn.

### 22/10

Jag lade till regler för hur entiteterna ska valideras och skapade en migration för databasen. Jag byggde också inloggningen i LeadFlow. Jag skapade en modell som validerar e-postadress och lösenord, en AuthService som använder Identitys SignInManager och en AuthController som tar emot inloggningsanrop.

En lyckad inloggning ger svaret 204 No Content. Fel lösenord och okänd e-postadress ger båda samma felmeddelande med 401 Unauthorized. Jag satte upp lokal HTTPS och testade inloggningen i Postman. Jag använde AI för kodförslag, förklaringar och kodgranskning.

### 23/10

Jag byggde backendstödet för att skapa och hämta leads. Jag tog fram modeller för den information som skickas till och från API:t, skapade en LeadService och lade till ett repository som använder EF Core för att läsa och spara leads i databasen.

Jag lade till ett publikt POST-anrop för att skicka in intresseanmälningar och ett skyddat GET-anrop för att hämta den interna leadlistan. Jag gick också igenom hur tjänsterna registreras och hur jag kan kontrollera åtkomsten i Postman med och utan en inloggningscookie. Jag använde AI för commit-meddelanden, kodförslag, förklaringar och kodgranskning.

### 28/10

Jag arbetade med hur Next.js och .NET kommunicerar. Det handlade om att ställa in proxyn, hantera cookies och sätta upp CSRF-skydd. Jag byggde också vidare på API:t för intresseanmälningar genom att lägga till en modell för inkommande data. Servern kontrollerar att de obligatoriska fälten är ifyllda, att e-postadressen har rätt format och att fälten håller sig inom tillåtna längder. Giltiga anmälningar sparas som leads med status New, och servern bestämmer de interna värdena.

Jag planerade också det publika formuläret med tydliga fältnamn, validering, felmeddelanden och en bekräftelse när anmälan har skickats. Jag planerade hur formuläret ska visa att det väntar på svar, hindra dubbla inskick och behålla det användaren har fyllt i om något går fel. Formuläret ska fungera på både mobil och dator.

### 29/10

Jag arbetade med detaljsidan för leads. Jag utökade API-svaret så att det innehåller leadets avslutsutfall och uppföljningsanteckningar. För varje anteckning hämtar jag vem som skrev den och när den skapades. De senaste anteckningarna visas först.

I frontend arbetade jag med att visa anteckningarna, lägga till ett meddelande när listan är tom och visa utfallet när ett lead är avslutat. Jag använde AI för kodförslag, förklaringar och vägledning.

Jag byggde leadlistan som en tabell och lade till filtrering på status i frontend. Jag beslutade att avstå från filtrering i backend eftersom jag låg efter i tidsplanen och behövde avgränsa arbetet.

### 2/11

Jag kopplade reglerna för statusändringar till ett skyddat API-anrop och ett formulär på leadets detaljsida. När jag avslutar ett lead behöver jag välja Customer eller Not Interested. Om jag öppnar leadet igen tas det tidigare utfallet bort. Formuläret visar hur sparningen gick och uppdaterar informationen på sidan.

Jag testade statusändringen i Postman. När jag fick problem med autentiseringen löste jag det genom att logga in igen och hämta en ny CSRF-token. Jag använde AI för kodförslag, förklaringar, commit-meddelanden och felsökning.

### 3/11

Byggde stöd för uppföljningsanteckningar. Jag lade till ett formulär på leadets detaljsida och kopplade det till API:t så att historiken uppdateras direkt när en anteckning har sparats. I backend lade jag till en skyddad endpoint, validerade inkommande data och skärpte reglerna i domänmodellen. Jag skrev också tester som täcker valideringen och kontrollerar att anteckningarna sparas i databasen. Debuggade frontend, status och antecknings formuläret låg under samma förälder och hade båda key={[lead.id](http://lead.id)}, lade till varsin unik nyckel.  Lade till en Logga in/Leadlista knapp på homepage. Färdigställande av rapporten, skrev om testning, AI, teknikval och resultat.

### 5/11

Inlämning och färdigställande.

## Testning och testresultat

Alla API-endpoints är manuellt testade i Postman. Detta omfattade in och utloggning, hämtning av Current User CSRF-token samt CRUD-anrop för leads och lägga till uppföljninganteckningar.

Ett exempel var inloggningen. Vid giltiga uppgifter fick jag 204 No Content. Vid fel lösenord eller okänd e-postadress gav API:t 401 Unauthorized  med samma felmeddelande. Ett annat exempel var statusändring. Det första anropet misslyckades, men efter att jag loggat in igen och hämtat en ny CSRF-token gick anropet igenom.

Jag testade kommunikationen mellan frontend och backend manuellt via DevTools Network där jag följde anropen och kontrollerade att autentiseringscookien sattes vid inloggning. Skyddade anrop fungerade när jag var inloggad men nekades när jag var utloggad. Efter utloggning kontrollerade jag att ett nytt anrop gav unauthorized.

För att testa CSRF-skyddet skickade jag anrop med fetch från Console. Jag hämtade först leadlistan och försökte sedan skapa ett lead, först utan CSRF-token och sedan med en felaktig token. Båda anropen gav 400 Bad request. När jag hämtade leadlistan igen såg jag att inget nytt lead hade skapats. Därefter hämtade jag en giltig CSRF-token och upprepade anropet. API:t svarade då med 201 Created och det nya leadet fanns i listan.

Utöver de manuella testerna använder jag mig av xUnit-tester för validering, statusregler och lagring.

För status finns det tester som kontrollerar tillåtna och otillåtna kombinationer av status och avslutsfall, de omfattar bland annat att ett lead kan avslutas med ett giltigt utfall, att ett återöppnat lead får status Contacted och att avslutsutfallet då rensas. Persistenstesterna kontrollerar även att avvisade ändringar inte påverkar det lagrade leadet, inklusive om en senare sparning görs.

Testerna för intresseanmälan omfattar giltiga uppgifter, obligatoriska fält, felaktigt e-postformat och maximala fältlängder. Ett persistensfall skapar ett lead genom applikationens service och läser sedan tillbaka det med en ny databaskontext. På så sätt kontrolleras att uppgifterna har sparats i testdatabasen och inte enbart finns kvar i samma objekt i minnet.

För uppföljningsanteckningar finns tester för giltig text, tom text, enbart blanksteg och för lång text. Det finns också testfall för saknat lead och saknad författare. Vid giltiga anteckningar kontrolleras bland annat text, koppling till lead, författar-ID och skapandetid. Dessa tester kontrollerar service- och databaslagret.

Testningen har begränsningar. De automatiserade databastesterna använder SQLite i minnet, medan applikationen använder SQL Server. De ger därför stöd för de testade lagringsfallen, men verifierar inte SQL Server-specifikt beteende eller att SQL Server-migrationerna fungerar. Den granskade testsviten innehåller inte heller automatiserade HTTP-tester som verifierar autentisering, CSRF och modellvalidering tillsammans genom hela API:t.

Vid körning passerar 45 av 45 tester.

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

Kommunikationen mellan frontend och backend har kontrollerats i webbläsarens DevTools, bland annat för att verifiera att autentiseringscookien sätts vid inloggning och att skyddade anrop nekas efter utloggning.

| Testfall | Resultat |
| --- | --- |
| Inloggning med giltiga uppgifter | `204 No Content` |
| Fel lösenord eller okänd e-postadress | `401 Unauthorized` |
| Skyddat anrop utan inloggning | Åtkomst nekas |
| Skapa lead utan CSRF-token | `400 Bad Request`, inget lead skapas |
| Skapa lead med felaktig CSRF-token | `400 Bad Request`, inget lead skapas |
| Skapa lead med giltiga uppgifter och korrekt CSRF-token | `201 Created`, leadet sparas |

## Användning av AI
Då temat för denna kurs har varit AI har jag använt mig av det i större omfattning än jag tidigare gjort. Jag har använt det som ett verktyg när det kommer till projektets alla delar så som planering, kodförslag, felsökning, tekniska förklringar, och dokumentation. Jag har främst använt mig av Open AIs Codex. men även av Copilot och googles sökmotors AI. 
I projektets början användes AI för att diskutera projektidéer, ta fram ett projekt med CRM-tema då jag ville göra nåt kopplat till min LIA. AI hjälpte till att dela upp arbetet i Github-issues som jag därefter utförde mitt arbete efter samt matade in till AI för att få en steg för steg planering för det jag skulle göra utan implementation, när jag fick steg för steg planeringen gjorde jag research och kollade upp dokumentation för att få förståelse för koncept innan jag bad om kodförslag. Med varje kodförslag jag bad om, instruerade jag AI att ge mig stegvis implementation med ingående förklaringar samt commit-meddelanden. För att få full förståelse för vad jag gjorde skrev jag också ned mycket för hand så som flöden koncept. 
Jag lät aldrig AI göra några egna ändringar utan granskade all kod innan jag manuellt skrev in den i projektet, trots det känner jag att det är en stor utmaning att kontrollera kvaliteten i den AI-genererade koden, därför utgick jag från hur jag gjort tidigare projekt från kurser där vi fått instruktioner och resonemang på hur man kan utföra vissa lösningar på ett bra sätt. Jag tycker att AI ofta kan överkomplicera saker och ting. För att ändå hålla det på den nivå jag ville för att avgränsa projektet inom en tidsram, bad jag den gå in i dessa tidigare projekt och använda den som en slags mall. Detta ansåg jag fungerade väldigt bra. Jag såg också till att kolla upp de lösningar AI ville använda eller använde tidigare kunskaper för att bedöma nödvändighet. Så som skydd mot CSRF vilket var aktuellt i föregående kurs/projekt. 
Förstående för att AIs förslag inte alltid fungerar är också viktigt och därför blev manuell testning en viktig del av projektet, genom postman och i webbläsaren. Under testning/felsökning förekom fel i implementationen, då använde jag AI för att identifiera problemet. Ett exempel var ett kompileringsfel där SignInAsync användes med en parameter som metoden inte stödde. AI hjälpte till att identifiera felet och ändrade anropet till PasswordSignInAsync. Den senare metoden används i den nuvarande implementationen tillsammans med användarens lösenord. 

## Motivering av teknikval

### ASP.NET Core Identity.
Jag valde Identity eftersom jag hade använt det i ett tidigare projekt och kunde bygga vidare på ett arbetssätt jag kände till. Med SignInManager.PasswordSignInAsync kunde jag använda ramverkets stöd för lösenordsverifiering och cookieinloggning. 

### Åtkomst till interna API-anrop.
Leadlistan och detaljsidan innehåller känsliga kontaktuppgifter och uppföljningsanteckningar. Därför lade jag behörighetskontrollen i API:t, där den gäller även vid direkta anrop utanför frontend. Anropen för att läsa, ändra och radera leads kräver en inloggad användare. Det publika formuläret är öppet för besökare. 

### CSRF-skydd vid cookieinloggning.
Webbläsaren skickar autentiseringscookien automatiskt när villkoren för cookien är uppfyllda. Cookien identifierar den inloggade användaren. För anrop som kan ändra data kräver API:t dessutom en CSRF-token i headern X-CSRF-TOKEN och validerar den tillsammans med en separat CSRF-cookie. Kravet gäller även det publika formuläret. När jag testade att skapa ett lead gav både en saknad och en felaktig token 400 Bad Request, och listan visade att inget lead hade skapats. Med en giltig token fick jag 201 Created och kunde se det nya leadet i listan. 

## Lösning och resultat

I slutversionen av LeadFlow kan en besökare skicka in en intresseanmälan utan att logga in. Besökaren fyller i namn, e-postadress och meddelande och kan även ange företagsnamn. Servern validerar uppgifterna och sparar en giltig anmälan som ett lead med status New. Besökaren får en bekräftelse när anmälan har sparats.
Efter inloggning kan en medarbetare se inkomna leads och filtrera listan på New, Contacted och Closed eller visa alla statusar. Statusfiltreringen blev klar och görs i frontend på den redan hämtade listan. Filtrering på serversidan och mer avancerad sökning ingick inte i slutversionen men är något man skulle kunna utöka med.
Från listan kan medarbetaren öppna ett lead, markera det som kontaktat och lägga till uppföljningsanteckningar. Anteckningarna visas med författare och tidpunkt, med den senaste först. När ett lead avslutas måste medarbetaren ange om kontakten blev kund eller inte var intresserad. Ett avslutat lead kan återöppnas. Då får det status Contacted och det tidigare utfallet rensas. Det går även att radera ett lead från detaljsidan.
Under projektet behöll jag den ursprungliga inriktningen och de huvudsakliga teknikvalen. Dock beslutade jag att avstå från filtrering i backend och i stället filtrera den hämtade leadlistan i frontend. Eftersom jag låg efter i tidsplanen behövde jag avgränsa arbetet och prioritera projektets kärnfunktioner. Filtrering i backend lämnades därför som en möjlig vidareutveckling. 


## Lokal utvecklingsmiljö

Projektets lokala utvecklingsmiljö använder:

- .NET 10.
- Node.js för Next.js.
- SQL Server LocalDB.
- Lokal HTTPS för backend.
- User Secrets för konfiguration av lokala startdata.
- Entity Framework Core-migrationer för databasen.

Den lokala SQL Server-instansen är `(localdb)\MSSQLLocalDB`.

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

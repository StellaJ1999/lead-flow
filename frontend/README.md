# LeadFlow frontend

Next.js App Router, TypeScript och Tailwind CSS.

## Starta

```powershell
npm ci
npm run dev
```

Öppna http://localhost:3000.

## Kontroller

```powershell
npm run lint
npm run build
```

## Struktur

- `src/app`: routes, layouts och globala stilar. Håll sidorna tunna och använd komponenter från features.
- `src/app/auth/login`: förberedd för inloggningssidan `/auth/login`.
- `src/app/(internal)/leads`: förberedd för leadlistan `/leads` och detaljsidan `/leads/[id]`.
- `src/features/auth`: inloggningens komponenter, API-anrop i `lib` och TypeScript-typer i `types`.
- `src/features/leads`: leadfunktionens komponenter, API-anrop i `lib` och TypeScript-typer i `types`.
- `src/styles/theme.css`: gemensamma färger och tema.
- `src/styles/fonts.css`: typsnitt.
- `public/images`: framtida statiska bilder.

Tomma mappar innehåller `.gitkeep` så att de följer med i Git. Ta bort filen när mappen får innehåll.

Endast startsidan är implementerad. Övriga routemappar är förberedda och får en URL först när en `page.tsx` skapas. `(internal)` grupperar routes utan att ändra URL:en; gruppen ger inget behörighetsskydd. Autentisering och backendintegration implementeras senare.

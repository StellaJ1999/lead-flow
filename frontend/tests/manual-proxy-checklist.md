# Manuell verifiering av proxy, cookies och CSRF

Samtliga kontroller godkända:

- API-anrop når backend genom frontendproxyn.
- Login fungerar och autentiseringscookien sätts.
- Skyddade anrop lyckas efter login.
- Skyddade anrop nekas utan inloggning.
- Skrivningar utan eller med felaktig CSRF-token nekas.
- Nekade skrivningar lämnar data oförändrade.
- Skrivning med giltig CSRF-token lyckas.
- Logout fungerar och efterföljande /me ger 401.
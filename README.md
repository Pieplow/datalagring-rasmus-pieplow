Datalagring – Utbildningssystem

Detta projekt är ett datalagringssystem för ett utbildningsföretag.
Systemet hanterar:

- Kurser

- Kurstillfällen

- Instruktörer

- Deltagare

- Registreringar

Backend är byggt med ASP.NET Core Minimal API och använder Entity Framework Core (Code First) mot en relationsdatabas.
Frontend är en WPF-applikation som kommunicerar med API:et via HTTP.

Projektet är uppbyggt enligt DDD och Clean Architecture med tydlig separation mellan:

- Presentation (API + WPF)

- Application (Services / use cases)

- Domain (Entities)

- Infrastructure (EF Core, DbContext, rå SQL)

- Tests

Funktionalitet:

- Full CRUD för centrala delar av systemet

- Registrering med kapacitetskontroll och dublettkontroll

- Transaktionshantering med rollback

- Rå SQL via EF Core (FromSqlRaw)

- Caching av läsoperationer

- Enhetstester för centrala delar

Så startar du projektet:

1. Förutsättningar:

Visual Studio 2022+

.NET 8 SDK

SQL Server LocalDB

2. Skapa databasen:

Öppna Package Manager Console och kör:

Update-Database

Detta skapar databasen via migrations.

3. Starta backend (API)

Sätt API-projektet som Startup Project

Kör projektet

4. Starta WPF-appen

Sätt WPF-projektet som Startup Project

Kör projektet

(API:et måste vara igång innan WPF startas.)

**Tester:**

Tester finns i testprojektet och kan köras via Test Explorer i Visual Studio.
Tester verifierar bland annat:

Registrering

Kapacitetskontroll

Dublettkontroll

Transaktioner


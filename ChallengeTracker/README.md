# ChallengeTracker

Školní fullstack projekt: výzvy, členství, denní pokrok a žebříček. Navazuje na původní CRUD MVP a zachovává jeho tmavý vzhled. React 19 + TypeScript, ASP.NET Core 10 Minimal API, EF Core a SQLite.

![ChallengeTracker](docs/screenshots/final-overview.png)

## Stažení z GitHubu

```bash
git clone https://github.com/pavlis29/wbs-coding-school.git
cd wbs-coding-school/ChallengeTracker
```

## Spuštění na Macu

Potřebuješ .NET 10 SDK, Node.js 22+ a npm. V terminálu otevři tuto složku a spusť:

```bash
./start.sh
```

Otevři **http://localhost:5173**, zvol registraci a vytvoř vlastní účet. Heslo: alespoň 8 znaků, velké i malé písmeno, číslice a speciální znak. Skript spustí oba servery; Ctrl+C je ukončí. První běh stáhne závislosti, vytvoří lokální klíč a databázi a provede migrace.

Pokud už běží stará verze na stejných portech, ukonči ji v jejím terminálu, nebo použij:

```bash
API_PORT=5082 FRONTEND_PORT=5174 ./start.sh
```

Pak otevři http://localhost:5174. Konfigurace frontendu se nastaví automaticky.

- `.local/challenge.db`: SQLite data, která přežijí restart.
- `.local/backend.env`: vygenerovaný JWT klíč a heslo ukázkových účtů; nepatří do Gitu.
- `.local/api.log`: log backendu.
- Ukázkové účty: `pavel@example.test`, `alex@example.test`. Heslo určuje `Seed__Password` v lokálním souboru; není pevně zapsané v projektu. Vlastní registrace je jednodušší.
- Původní `AppData/challenge-data.json` se nepřepisuje. Vývojový import převádí staré výzvy bez zápisů se stejnými ID. Pokud najde staré zápisy bez identity, zastaví se s vysvětlením, aby se nic neztratilo ani nepřiřadilo nesprávnému uživateli.

## Krátké demo

1. Registrace a vytvoření veřejné výzvy s dnešním datem uvnitř jejího období.
2. Úprava názvu, připojení druhého účtu, spuštění výzvy a zápis pokroku.
3. Úprava zápisu, změna žebříčku a smazání zápisu.
4. Soukromá výzva: sdílení odkazu, žádost druhého účtu a schválení vlastníkem.
5. Ukázka, že druhý účet nemůže upravit cizí výzvu ani zápis.
6. Smazání výzvy odstraní také její členství a zápisy.

## Ověření

```bash
./scripts/check.sh
```

Spustí automatické testy a produkční sestavení frontendu. Skutečný výsledek posledního ověření: [TEST_REPORT.md](docs/TEST_REPORT.md). Stav školních požadavků: [REQUIREMENTS.md](docs/REQUIREMENTS.md).

## Struktura

Backend: `Endpoints/` řeší HTTP, `Services/` pravidla, `Models/` entity, `Dtos/` vstupy a výstupy, `Data/` databázi a migrace, `Errors/` chyby, `Filters/` společné kontroly. Testy jsou v `api/ChallengeTrackerApi.Tests/`.

Frontend: `actions/` změny dat, `data/` čtení a cache, `contexts/` přihlášení, `pages/` stránky, `components/` společné části, `layouts/` navigace, `types/` typy a `utils/` API klient, datumy a validace.

## Konfigurace

`start.sh` ji nastaví automaticky. Pro samostatný start použij environment variables:

| Proměnná | Význam |
| --- | --- |
| ASPNETCORE_ENVIRONMENT | Development: automatické migrace a seed |
| ASPNETCORE_URLS | například http://localhost:5080 |
| Database__Provider | Sqlite nebo SqlServer |
| ConnectionStrings__DefaultConnection | SQLite soubor nebo SQL Server connection string |
| Jwt__Key | vlastní náhodný klíč, alespoň 32 bajtů |
| Seed__Password | volitelné heslo vývojových účtů |
| Cors__AllowedOrigins__0 | například http://localhost:5173 |
| VITE_API_BASE_URL | například http://localhost:5080, bez /api |

V adresáři `api/` obnovíš EF nástroj přes `dotnet tool restore`. Dokumentace je v Development na **/scalar/v1**, OpenAPI na **/openapi/v1.json**. Health: **/health**, **/health/live**, **/health/ready**. Ready skutečně kontroluje databázi.

## Docker a SQL Server

Docker Desktop není součást projektu. Compose používá SQL Server Developer pro vývojové předvedení. Hranice ověření je v test reportu.

```bash
./scripts/configure-docker.sh
docker compose up --build -d
docker compose logs migrate api
```

Konfigurační skript vytvoří `.env` s náhodnými tajemstvími a existující soubor nepřepíše. SQL Server startuje první; služba `migrate` provede migrace a pak API poslouchá na localhost:5080. Frontend v dalším terminálu:

```bash
cd frontend
npm ci
VITE_API_BASE_URL=http://localhost:5080 npm run dev
```

SQL Server image vyžaduje `linux/amd64`; na Apple Silicon potřebuje vhodně nastavenou emulaci. Produkce neseeduje demo účty, použij registraci. Pro veřejné nasazení nastav vlastní databázový účet, HTTPS a důvěryhodný certifikát. Azure může použít API image, SQL Server/Azure SQL connection string a samostatný migrační krok. Zveřejnění zdrojového kódu na GitHubu samo o sobě nespouští veřejně dostupnou aplikaci; pro lokální demo použij start.sh.

## Podklady

- [České vysvětlení](docs/EXPLANATION_CZ.md)
- [Endpointy](docs/ENDPOINTS.md)
- [Ruční scénáře](docs/SMOKE_TEST.md)
- [Německá prezentace](docs/PRESENTATION_DE.md)

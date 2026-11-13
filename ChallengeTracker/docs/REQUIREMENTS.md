# Kontrolní seznam FR001–FR026

Požadavky vycházejí z dodaného PDF z 8. listopadu 2026. Tabulka popisuje implementaci; důkaz konkrétních spuštěných kontrol je v [TEST_REPORT.md](TEST_REPORT.md). Přítomnost kódu sama o sobě neznamená ověření všech prostředí.

| ID | Požadavek | Implementace |
| --- | --- | --- |
| FR001 | API solution, OpenAPI a Scalar | api/ChallengeTrackerApi.slnx; Program.cs; dokumentace jen Development |
| FR002 | EF Core 10 + SQLite | Data/ApplicationDbContext.cs; lokální dotnet-ef nástroj |
| FR003 | User, Challenge, Membership, ProgressEntry | Models/; Guid, DateOnly, DateTimeOffset a explicitní vztahy |
| FR004 | Migrace a vývojový seed | Data/Migrations/Sqlite; DbSeeder; dva účty, Open výzva, členství; zachovaný legacy JSON |
| FR005 | Identity + JWT | AuthService; register/login/me; klíč výhradně z konfigurace |
| FR006 | Vytvoření a čtení výzev | ChallengeService; filtry visibility/status/mine; navíc update/delete pro kompletní CRUD |
| FR007 | Přechody stavů | ChallengeRules; pouze vlastník; neplatný přechod 409 |
| FR008 | Join, Leave, Approve | MembershipService; public Active, private Pending, schválení vlastníkem |
| FR009 | Denní pokrok | ProgressService; aktivní členství, kladné množství, UTC den, období, unikátní DB index |
| FR010 | Editace a mazání pokroku | Autor; editace před uplynutím 24 h; mazání bez časového omezení |
| FR011 | Vypočtený leaderboard | LINQ GroupBy/Sum; žádný uložený total sloupec |
| FR012 | DTO a validace | Dtos/ recordy, DataAnnotations, AddValidation, invariantní desetinné meze |
| FR013 | ProblemDetails | Errors/GlobalExceptionHandler; exception handler a status code pages |
| FR014 | Struktura API | Endpoints, Filters, Errors, Dtos, Models, Services, Data; rozhraní služeb |
| FR015 | Logování a health | Serilog; /health, /health/ready s DB kontrolou, /health/live bez DB |
| FR016 | Limity POST | 10 pokrok/min/účet; 5 ostatních POST/min/účet; 429, Retry-After; health bez limitu |
| FR017 | CORS | Pojmenovaná Frontend policy, konfigurovatelné origins; Retry-After dostupné klientovi |
| FR018 | Automatické testy | ChallengeTrackerApi.Tests; pravidla, oprávnění, souběh, SQLite persistence a další scénáře |
| FR019 | React 19 + TypeScript + Vite | Router, QueryClient, VITE_API_BASE_URL bez starého /api |
| FR020 | Struktura frontendu | actions, components, contexts, data, layouts, pages, utils, types, app.tsx |
| FR021 | Frontend auth | Registrace/login, AuthContext, JWT localStorage, chráněné routes, vyčištění cache při logout |
| FR022 | Dashboard | My Challenges podle členství; Discover veřejné Open; čtecí queries bez zápisů |
| FR023 | Detail výzvy | Členové, dnešní pokrok, leaderboard, join/leave/approve a formulář pokroku |
| FR024 | Vytvoření výzvy | /create, zod, POST a přesměrování na detail |
| FR025 | Chyby a optimistická změna | ProblemDetails a chyby polí; 429; optimistický pokrok a total s rollbackem |
| FR026 | Docker + SQL Server | Multi-stage Dockerfile; Compose DB/migrace/API; samostatné SQLServer migrace; env secrets |

## Hranice předání

- SQLite je hlavní lokální varianta. Docker/SQL Server konfigurace je připravená; její skutečný runtime vyžaduje Docker a je vykázán odděleně v reportu.
- Azure veřejné nasazení se neprovádělo. Konfigurace je oddělená od kódu; přihlašovací tajemství nejsou v repozitáři.
- Původní JSON s výzvami bez pokroku se bezpečně importuje. Staré zápisy s volným jménem místo účtu vyžadují explicitní mapování a nejsou potichu převáděny.
- Datumy zápisů mají jednotný UTC den. Editace končí přesně při dosažení 24 hodin od CreatedAt.
- Noví členové se připojují k Open výzvě. Vlastník je automaticky aktivním členem a vlastní výzvu nemůže opustit; může ji smazat.
- Člen po odchodu ztrácí přístup k soukromé výzvě; jeho dřívější pokrok se uchová. Mazat vlastní zápis může autor přes API i po odchodu.

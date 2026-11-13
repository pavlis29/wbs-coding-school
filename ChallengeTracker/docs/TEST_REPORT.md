# Test report — 9. listopadu 2026

## Výsledek

**43 automatických backendových testů prošlo: 20 unit + 23 integračních. 0 chyb, 0 přeskočených testů.** Frontend prošel strict TypeScript kontrolou a produkčním Vite buildem. Lokální aplikace byla spuštěna přes start.sh a ověřena v prohlížeči.

Strojový záznam: [results.trx](test-results/results.trx). V publikované kopii byly odstraněny absolutní místní cesty a názvy počítače; výsledky, názvy testů a časy zůstaly zachované. Testy používají samostatné dočasné SQLite databáze a testovací účty; nezasahují do původních uživatelských dat.

## Prostředí

macOS / Apple Silicon; .NET SDK 10.0.401, .NET runtime 10.0.12; Node.js 22.23.1, npm 10.9.8. Lokální frontend Vite 6.4.5. Nové závislosti jsou uložené v package-lock.json a verzované .NET PackageReference. Původní MVP zůstává ve vlastní záloze.

## Provedené kontroly

| Oblast | Skutečný výsledek |
| --- | --- |
| dotnet restore/build | Úspěch; poslední sestavení API 0 chyb / 0 varování |
| dotnet test | 43/43; výstup results.trx |
| npm install / audit | Závislosti nainstalované; audit frontendu bez nalezených zranitelností při instalaci |
| npm run build | Úspěch; strict TypeScript + Vite; neblokující upozornění Rollupu na komentáře zod |
| SQLite migrace | Skutečně aplikované v testu i vývojovém startu |
| Persistence | Nová instance testovacího serveru načetla uloženou výzvu a pokrok ze stejného DB souboru |
| Production --migrate | Dvakrát úspěšně proti dočasné SQLite DB, bez spuštění serveru a bez seed uživatelů |
| Startup skript | Testovací běh API5082/frontend5174; finální instalace ve WBS/ChallengeTracker také úspěšně spuštěna na API5080/frontend5173 |
| Docker Compose | YAML načten a zkontrolována struktura služeb; žádný skutečný kontejnerový běh |
| SQL Server | Provider sestaven, samostatné migrace a SQL skript v api/SQLServerMigration.sql; skutečný server se nespouštěl |

## Automatické scénáře

- Všechny platné a zakázané přechody stavů, datumové hranice a přesná hranice 24 hodin.
- Registrace, přihlášení a me; chybná hesla, anonymní 401, cizí vlastník 403 a oprávněný úspěch.
- CRUD výzev a pokroku; vlastnictví, smazání souvisejících dat, filtrování.
- Veřejné Active a soukromé Pending členství, schvalování pouze vlastníkem, odchod a izolace soukromých dat.
- Nula/záporné množství, neplatné datum, duplicitní den i souběžné požadavky, editace po 24 hodinách a mazání bez limitu.
- Malá kladná hodnota 0.001 a přesný vypočtený součet. Validace funguje v de-DE, cs-CZ i en-US.
- Souběh změny období výzvy a vložení pokroku neporuší datumovou podmínku.
- Leaderboard počítaný z aktuálních zápisů.
- Oddělené limity 5/10 POST za minutu a uživatele; 429, Retry-After; health bez limitu.
- CORS pro povolený origin a odmítnutí cizího originu; jednotné ProblemDetails.

## Průchod v prohlížeči

Na pracovní databázi byly vytvořeny dva testovací účty. Ověřeno:

1. Přesměrování nepřihlášeného uživatele, registrace a dashboard.
2. Vytvoření veřejné výzvy a přesměrování na její detail.
3. Úprava názvu a cíle ze 100 na 120; stav Open -> Running.
4. Zápis 0.001, editace na 25; správný dnešní součet, total a leaderboard.
5. Odmítnutí druhého zápisu téhož dne se srozumitelnou zprávou; po chybě stále 25, žádný duplicitní zápis ani chybný součet.
6. Vytvoření soukromé výzvy a zobrazení pozvánky.
7. Druhý účet vidí nejprve Request access; po žádosti pouze Pending a omezené údaje.
8. Vlastník schválí člena. Druhý účet po přihlášení vidí členy a může zapsat 1.5 hodiny; nemá ovládání cizí výzvy.
9. Odhlášení a opětovné přihlášení správně oddělují účty.
10. Finální instalace: přihlášení ukázkovým účtem, dashboard a původní importovaná výzva ověřeny na localhost:5173. [Náhled](screenshots/final-overview.png).

[Snímek detailu po editaci pokroku](screenshots/challenge-detail.png). Mazání a odchod členství byly pokryty integračními API testy; tento report je nevydává za ručně dokončené klikací scénáře.

## Opravené chyby odhalené ověřením

- Nepoužívaný import Status bránil původnímu frontend buildu.
- Desetinné validační meze závisely na německém nastavení počítače; jsou nyní invariantní.
- Původní minimum 0.01 zbytečně odmítalo menší kladné hodnoty; byly upraveny DTO, formuláře a SQL přesnost.
- Transakce chrání vztahy při souběžných změnách dat a členství.
- Nastavení Urls v appsettings blokovalo změnu portu přes prostředí; bylo odstraněno.
- Zranitelná tranzitivní závislost Pkcs byla aktualizována na 10.0.14.
- Po přesunu projektu byly závislosti obnoveny přes npm ci; start.sh kontroluje také přítomnost spouštěcích souborů Vite a TypeScriptu.

## Neověřeno / mimo rozsah

**Docker ani skutečný SQL Server se nespouštěly: na Macu chybí Docker.** FR026 je implementovaný v konfiguraci a migracích, ale jeho runtime se nesmí označit za ověřený. Stejně tak nebylo provedeno veřejné nasazení na Azure/GitHub, zátěžový test ani bezpečnostní audit produkční služby. Nejde o tvrzení 26/26 požadavků otestovaných ve všech prostředích.

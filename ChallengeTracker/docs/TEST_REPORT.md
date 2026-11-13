# Výsledky testování

Datum ověření: **9. 11. 2026**.

## Automatické kontroly

| Kontrola | Výsledek |
| --- | --- |
| Backend build | 0 chyb, 0 varování |
| Unit testy | 20 úspěšných |
| Integrační testy | 23 úspěšných |
| Frontend build | TypeScript a Vite bez chyb |
| SQLite migrace | Úspěšně aplikované |
| Uložení dat | Data zůstala dostupná po restartu serveru |

Celkem **43 úspěšných testů**, žádný selhaný ani přeskočený. [Výstup testovacího nástroje](test-results/results.trx) má odstraněné názvy počítače a absolutní lokální cesty.

Testy používají oddělené dočasné databáze. Pokrývají:

- Registraci, přihlášení a oprávnění: 401 bez tokenu, 403 u cizího vlastníka.
- CRUD výzev a pokroku, členství a schvalování soukromých žádostí.
- Přechody stavů a datumové hranice.
- Denní duplicity, souběžné zápisy a limit 24 hodin pro editaci.
- Desetinné hodnoty včetně 0.001 a validaci v de-DE, cs-CZ a en-US.
- Výpočet žebříčku, CORS a jednotné chybové odpovědi.
- Limity 5/10 POST za minutu, odpověď 429 a hlavičku Retry-After.
- Souběh úpravy období výzvy se zápisem pokroku.

## Kontrola v prohlížeči

Ověřena registrace, přihlášení, vytvoření a úprava výzvy, její spuštění a zápis pokroku. Po editaci zápisu se aktualizoval dnešní součet i žebříček. Duplicitní denní zápis byl odmítnut a zobrazené hodnoty se vrátily do původního stavu.

U soukromé výzvy druhý účet požádal o přístup. Do schválení viděl pouze omezené údaje; po schválení mohl zapisovat pokrok. Odhlášení a přepnutí účtů fungovalo.

Mazání a odchod z členství byly ověřeny integračními API testy.

- [Detail výzvy](screenshots/challenge-detail.png)
- [Přehled výzev](screenshots/final-overview.png)

## Prostředí

macOS / Apple Silicon, .NET SDK 10.0.401, runtime 10.0.12, Node.js 22.23.1, npm 10.9.8 a Vite 6.4.5.

Spuštění pomocí `start.sh` bylo ověřeno na výchozích i alternativních portech. Příkaz `--migrate` byl dvakrát úspěšně spuštěn proti SQLite v režimu Production, bez vytvoření demo účtů.

## Zbývající ověření

Docker Compose konfigurace a SQL Server migrace jsou připravené. Jejich běh proti skutečnému SQL Serveru **nebyl otestován**. Nebyly provedeny zátěžové testy ani veřejné nasazení.

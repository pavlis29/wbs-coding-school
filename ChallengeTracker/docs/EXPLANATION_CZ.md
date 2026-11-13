# Jak ChallengeTracker funguje

## Cesta požadavku

React zkontroluje formulář pomocí zod a odešle JSON. API ověří přihlášení a DTO. Endpoint zavolá službu. Ta ověří pravidla a přes DbContext uloží data. API vrátí odpověď a React Query obnoví související údaje na obrazovce.

Při vytvoření výzvy klient neposílá vlastníka. Backend jej přečte z ověřeného tokenu; přepsáním JSON nelze vytvořit výzvu jménem jiného účtu.

## Entity a vztahy

- **User**: účet, email, jméno; Identity bezpečně ukládá a ověřuje hesla.
- **Challenge**: cíl, období, jednotka, viditelnost, stav a vlastník.
- **Membership**: spojení uživatele a výzvy se stavem Pending nebo Active.
- **ProgressEntry**: konkrétní množství, poznámka, datum a autor.

Uživatel může být členem více výzev a výzva má více členů, proto potřebujeme Membership. Pokrok patří účtu i výzvě. Žebříček se počítá ze zápisů pomocí GroupBy a Sum; žádný další uložený součet se nemusí ručně synchronizovat.

## Rozdělení kódu

Endpoint řeší URL, HTTP status a přístup. Služba řeší pravidla. DbContext ukládání a vztahy. DTO určuje, co smí API přijmout a vrátit; nevracíme interní Identity data. Interface říká, jaké operace služba nabízí, implementace říká, jak je provede.

## Pravidla

1. Stavy jdou Open -> Running -> Completed. Přepíná je vlastník. Noví členové se připojují ve stavu Open.
2. Veřejné členství je Active, soukromé Pending do schválení vlastníkem.
3. Pokrok zapisuje aktivní člen; množství je kladné a datum patří do období včetně hranic.
4. Nejvýše jeden zápis na uživatele, výzvu a UTC den. Databázový unikátní index chrání i před souběžnými požadavky.
5. Editace pouze autorem v prvních 24 hodinách od vytvoření. Mazání pouze autorem, bez časového limitu.
6. Změna období výzvy nesmí zneplatnit existující zápisy.

`LoggedAt` je datum pokroku. `CreatedAt` je okamžik vložení a rozhoduje o limitu editace. Přepsáním data pokroku nelze prodloužit editaci. Dny jsou počítané v UTC, což uvádí i formulář.

## Přihlášení a oprávnění

Identity řeší registraci a hesla. Login vrátí časově omezený JWT, frontend jej přidává jako `Authorization: Bearer ...`. API kontroluje podpis a platnost. Skrytí tlačítka není zabezpečení: vlastnictví se vždy znovu ověřuje na serveru.

Školní verze ukládá token v localStorage. Odhlášení odstraní token a query cache. Projekt nemá refresh tokeny ani obnovu zapomenutého hesla.

## React Query

Queries čtou data; mutations je mění. Po změně se příslušná cache zneplatní. U zápisu pokroku se nejdřív uschová původní cache a přidá dočasný zápis, takže se dnešní stav změní hned. Při chybě se provede rollback. Po dokončení se údaje srovnají se serverem.

## Chyby

400 neplatný vstup; 401 chybějící přihlášení; 403 nedostatečné oprávnění; 404 chybějící záznam; 409 konflikt se stavem; 429 příliš mnoho požadavků. ProblemDetails sjednocuje odpověď. Frontend zobrazuje také chyby polí a dobu čekání při omezení počtu požadavků.

## Otázky k prezentaci

- Proč nestačí validace v Reactu? API lze volat bez prohlížeče.
- Proč potřebujeme databázový index pro duplicity? Dva požadavky mohou současně projít kontrolou v C#.
- Proč je ID lepší než jméno? Jména nejsou unikátní ani neměnná.
- Co je migrace? Verzovaná změna databázového schématu.
- Autentizace versus autorizace? Kdo jsi versus co smíš udělat.

## Reference

- [Microsoft: validace ASP.NET Core 10](https://learn.microsoft.com/en-us/aspnet/core/fundamentals/validation?view=aspnetcore-10.0)
- [Microsoft: EF Core SQLite](https://learn.microsoft.com/en-us/ef/core/providers/sqlite/limitations)
- [Microsoft: SQL Server kontejnery](https://learn.microsoft.com/en-us/sql/linux/sql-server-linux-docker-container-deployment?view=sql-server-ver16)

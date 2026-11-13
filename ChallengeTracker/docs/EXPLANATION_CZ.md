# Jak aplikace funguje

## Průchod požadavku

React odešle data formuláře jako JSON. API ověří vstup a přihlášení, endpoint zavolá službu a ta zkontroluje pravidla. DbContext uloží změny do databáze. Po odpovědi serveru React Query obnoví související data.

Formuláře výzvy a pokroku používají zod. Server provádí vlastní validaci, protože API lze volat i mimo prohlížeč.

## Datový model

| Entita | Účel |
| --- | --- |
| User | Účet, email a zobrazované jméno |
| Challenge | Výzva, cíl, jednotka, období, stav a vlastník |
| Membership | Členství uživatele ve výzvě: Pending nebo Active |
| ProgressEntry | Množství, poznámka, datum a autor pokroku |

Uživatel může patřit do více výzev a výzva může mít více členů. Tuto vazbu ukládá Membership. Žebříček se počítá pomocí GroupBy a Sum z jednotlivých zápisů.

## Rozdělení backendu

Endpointy řeší HTTP cesty a odpovědi. Služby obsahují pravidla aplikace. DTO určují přijímaná a vracená data. DbContext nastavuje tabulky, vztahy a indexy; migrace zachycují změny schématu.

## Pravidla

1. Stav výzvy mění vlastník: Open → Running → Completed.
2. Do Open výzvy se mohou připojit další uživatelé. Soukromé členství musí schválit vlastník.
3. Pokrok smí zapisovat aktivní člen. Množství je kladné a datum patří do období výzvy.
4. Unikátní index zajišťuje nejvýše jeden zápis na uživatele, výzvu a UTC den.
5. Autor může zápis upravit před uplynutím 24 hodin. Smazání časově omezené není.
6. Úprava období výzvy nesmí vyloučit už existující zápisy.

`LoggedAt` je datum pokroku, `CreatedAt` čas jeho vložení. Limit editace se počítá z CreatedAt, takže změna data pokroku jej neprodlouží.

## Přihlášení

ASP.NET Core Identity ukládá a kontroluje hesla. Po přihlášení API vrátí JWT a frontend jej posílá v hlavičce `Authorization: Bearer ...`. Vlastník nové výzvy se určuje z tokenu.

Oprávnění kontroluje server. Frontend pouze zobrazuje dostupná tlačítka. Token je uložený v localStorage; při odhlášení se odstraní spolu s query cache.

## Aktualizace obrazovky

Queries načítají data a mutations odesílají změny. Při zápisu pokroku se do cache vloží dočasný zápis. Pokud server požadavek odmítne, obnoví se předchozí hodnoty. Po dokončení se data znovu načtou ze serveru.

## Chyby

API vrací ProblemDetails. Kód 400 označuje neplatný vstup, 401 chybějící přihlášení, 403 nedostatečné oprávnění, 404 chybějící záznam, 409 konflikt a 429 překročení limitu požadavků.

## Dokumentace technologií

- [Validace ASP.NET Core 10](https://learn.microsoft.com/en-us/aspnet/core/fundamentals/validation?view=aspnetcore-10.0)
- [EF Core SQLite](https://learn.microsoft.com/en-us/ef/core/providers/sqlite/limitations)
- [SQL Server v Dockeru](https://learn.microsoft.com/en-us/sql/linux/sql-server-linux-docker-container-deployment?view=sql-server-ver16)

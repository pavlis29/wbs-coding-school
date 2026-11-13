# Ruční testování

Spusť `./start.sh` a připrav dva testovací účty. Použij oddělené profily prohlížeče nebo se mezi účty odhlašuj.

## CRUD v rozhraní

1. Registrace prvního účtu -> dashboard.
2. Vytvoření veřejné výzvy se startem dnes a koncem za týden -> detail nové výzvy.
3. Edit challenge -> změna názvu a cíle -> změny jsou na detailu.
4. Druhý účet: Discover -> Join challenge -> výzva také v My Challenges.
5. První účet: Start challenge -> Running; znovu spustit už nejde.
6. Log progress -> kladná hodnota dnes -> dnešní součet a leaderboard se změní.
7. Editace zápisu -> součty se přepočítají. Delete progress -> zápis zmizí.
8. Obnovit stránku -> data se znovu načtou.
9. Ukončit a znovu spustit aplikaci -> uložené údaje zůstanou.
10. Delete challenge -> odstranění výzvy i souvisejících údajů.

## Soukromá výzva

Vlastník vytvoří Private výzvu a zkopíruje její odkaz. Druhý přihlášený účet otevře odkaz a použije Request access. Vidí Pending, ne cizí zápisy. Vlastník v Members schválí žádost. Druhý účet obnoví detail, může zapisovat. Leave challenge zruší přístup; staré zápisy zůstávají, dokud autor nesmaže zápis nebo vlastník celou výzvu.

## Chybové scénáře

- Prázdný název, nulový cíl a konec před začátkem: formulář neodešle neplatný vstup.
- Pokrok mimo období: formulář jej nepustí; serverovou kontrolu ověřují API testy. Druhý zápis stejného dne odmítne server a optimisticky zobrazená hodnota se vrátí zpět.
- Cizí účet nemá ovládání úpravy/smazání výzvy nebo cizího pokroku; API vrátí 403 při ručním pokusu.
- Po více než 24 hodinách nejde zápis upravit, ale autor jej může smazat. Automatický test posouvá hodiny bez skutečného čekání.
- Vyčerpaný limit POST vrátí 429 se zprávou a Retry-After. Health zůstává dostupný. Počet se počítá pro účet, ne pro všechny uživatele dohromady.
- Odhlášení vyprázdní soukromou cache a chráněné URL přesměruje na přihlášení.

## Rychlá kontrola API

```bash
curl -i http://localhost:5080/health/live
curl -i http://localhost:5080/health/ready
curl -i http://localhost:5080/challenges
curl -i http://localhost:5080/auth/me
```

První tři odpovědi mají být 200. Poslední bez tokenu 401 s ProblemDetails. Kompletní autorizované scénáře provádí `dotnet test api/ChallengeTrackerApi.slnx`.

## Co dělat při problému

- Obsazený port: ukonči předchozí běh nebo použij jiné porty podle README.
- Prázdná stránka/chyba připojení: zkontroluj .local/api.log a adresu API; nový klient nepoužívá /api prefix.
- JWT chyba při ručním spuštění: vlastní Jwt__Key musí mít alespoň 32 bajtů. Doporučený start.sh jej vytváří automaticky.
- Soukromá výzva není v Discover: to je záměr; vstupuje se přes odkaz a schválení.

Tento postup slouží k opakovanému ručnímu testování. Výsledky posledního ověření jsou v [TEST_REPORT.md](TEST_REPORT.md).

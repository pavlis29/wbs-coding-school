# Endpointy

Základ: `http://localhost:5080`, bez starého `/api`. Autorizace: `Authorization: Bearer TOKEN`.

| Metoda a cesta | Význam |
| --- | --- |
| POST /auth/register | email, displayName, password |
| POST /auth/login | email, password; vrací token, expiresAt, user |
| GET /auth/me | Přihlášený účet |
| GET /challenges | Filtry visibility, status, mine |
| POST /challenges | Nová výzva, vlastník z JWT a aktivní členství |
| GET /challenges/{id} | Detail podle přístupových práv |
| PUT /challenges/{id} | Úprava vlastníkem |
| DELETE /challenges/{id} | Smazání vlastníkem, včetně souvisejících dat |
| POST /challenges/{id}/start | Open -> Running |
| POST /challenges/{id}/complete | Running -> Completed |
| GET /challenges/{id}/memberships | Členové podle přístupových práv |
| POST /memberships | Připojení pomocí challengeId |
| DELETE /memberships/{id} | Opuštění členství |
| POST /memberships/{id}/approve | Vlastník schválí žádost |
| GET /challenges/{id}/progress-entries | Zápisy podle přístupových práv |
| POST /progress-entries | challengeId, amount, volitelně note a loggedAt |
| PUT /progress-entries/{id} | amount, note, loggedAt; autor do 24 h |
| DELETE /progress-entries/{id} | Autor zápisu |
| GET /leaderboards/challenges/{id}?period=total | Vypočtené součty podle účtů |
| GET /health | Zdraví aplikace a databáze |
| GET /health/ready | Dostupnost databáze |
| GET /health/live | Běh procesu bez DB |
| GET /scalar/v1 | Dokumentace v Development |
| GET /openapi/v1.json | OpenAPI v Development |

Výzva přijímá title, description, category, target, unit, startDate, endDate, visibility. Visibility: Public/Private, status: Open/Running/Completed. Datumy: YYYY-MM-DD. Čas loggedAt: ISO 8601, například 2026-11-09T12:00:00Z; den se odvozuje po převodu na UTC.

POST pokroku: 10/min/účet; ostatní POST: 5/min/účet. Nepřihlášené auth požadavky se omezují podle IP. 429 obsahuje ProblemDetails a Retry-After. Health není omezený.

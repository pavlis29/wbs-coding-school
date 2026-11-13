# ChallengeTracker

## Idee

ChallengeTracker hilft dabei, kleine Ziele zu verfolgen, zum Beispiel 100 Seiten zu lesen. Nutzer erstellen eine Challenge, nehmen daran teil und tragen ihren täglichen Fortschritt ein.

## Aufbau

Das Frontend verwendet React und TypeScript. Das Backend ist eine ASP.NET Core Minimal API. Die Daten werden mit Entity Framework Core in SQLite gespeichert. ASP.NET Core Identity verwaltet die Konten; nach der Anmeldung wird ein JWT verwendet.

## Demo

1. Ein Konto registrieren.
2. Eine öffentliche Challenge erstellen und den Titel bearbeiten.
3. Mit einem zweiten Konto teilnehmen.
4. Die Challenge starten und Fortschritt eintragen.
5. Den Eintrag ändern und die aktualisierte Rangliste zeigen.
6. Den Eintrag löschen.
7. Bei einer privaten Challenge eine Anfrage stellen und als Eigentümer bestätigen.

## Wichtige Regeln

Pro Nutzer, Challenge und UTC-Tag gibt es höchstens einen Fortschrittseintrag. Ein eindeutiger Datenbankindex verhindert doppelte Einträge auch bei gleichzeitigen Anfragen.

Nur der Autor darf seinen Eintrag bearbeiten. Das ist bis 24 Stunden nach der Erstellung möglich. Das Eintragungsdatum und die Erstellungszeit werden getrennt gespeichert.

## Tests und offene Punkte

Bei der Prüfung am 9. November 2026 waren alle 43 automatischen Tests erfolgreich. Sie prüfen unter anderem Zustandswechsel, Berechtigungen, Datumsgrenzen und doppelte Tages-Einträge.

Die Docker-Konfiguration für SQL Server ist vorbereitet, wurde aber noch nicht mit einem laufenden SQL Server getestet. Passwort-Wiederherstellung, Refresh-Tokens und eine öffentliche Bereitstellung sind nicht enthalten.

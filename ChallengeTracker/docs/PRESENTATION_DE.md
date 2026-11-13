# Präsentation: ChallengeTracker (ca. 5 Minuten)

## 1. Idee

ChallengeTracker hilft dabei, kleine Ziele zu verfolgen, zum Beispiel 100 Seiten zu lesen. Nutzer können Challenges erstellen, teilnehmen und ihren täglichen Fortschritt dokumentieren.

## 2. Technologien

Das Frontend verwendet React und TypeScript. Das Backend ist eine ASP.NET Core Minimal API. Entity Framework Core speichert die Daten lokal in SQLite. Für die Benutzerkonten wird ASP.NET Core Identity verwendet; die Anmeldung liefert ein JWT.

## 3. Live-Demo

Registrierung -> Challenge erstellen -> Titel bearbeiten -> zweiten Nutzer teilnehmen lassen -> Challenge starten -> Fortschritt hinzufügen -> Fortschritt bearbeiten -> Rangliste ansehen -> Eintrag löschen.

Danach eine private Challenge zeigen: Ein zweiter Nutzer stellt eine Anfrage, der Eigentümer bestätigt die Mitgliedschaft. Andere Nutzer dürfen fremde Einträge nicht bearbeiten.

## 4. Eine wichtige Regel erklären

Pro Nutzer, Challenge und UTC-Tag gibt es höchstens einen Fortschrittseintrag. Die Anwendung prüft diese Regel, und ein eindeutiger Datenbankindex verhindert doppelte Einträge auch bei gleichzeitigen Anfragen. Eintragungsdatum und Erstellungszeit sind getrennt: Das Bearbeitungsfenster von 24 Stunden hängt von der Erstellungszeit ab.

## 5. Struktur und Tests

Endpoints behandeln HTTP. Services enthalten die Regeln. DTOs definieren die API-Verträge. Der DbContext verwaltet Beziehungen und Speicherung. Tests prüfen unter anderem Zustandswechsel, Berechtigungen und doppelte Tages-Einträge. Die tatsächlich ausgeführten Prüfungen stehen im Testbericht.

## 6. Grenzen offen benennen

Die Docker-Konfiguration mit SQL Server ist vorbereitet. Ob sie in dieser Umgebung ausgeführt wurde, steht ausdrücklich im Testbericht. Öffentliche Bereitstellung, Passwort-Wiederherstellung und Refresh-Tokens gehören nicht zu diesem Demo-Umfang.

## Vorbereitung

Vor der Präsentation die Anwendung starten, zwei Konten bereithalten und alle Schritte selbst durchgehen. Bei Fragen lieber einen konkreten Ablauf erklären, als unbekannte Fachbegriffe aufzuzählen. Unterstützung durch Werkzeuge entsprechend den Regeln der Schule angeben.

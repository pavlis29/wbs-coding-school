#!/usr/bin/env bash
set -euo pipefail

PROJECT_DIR="$(cd "$(dirname "$0")" && pwd)"
API_PORT="${API_PORT:-5080}"
FRONTEND_PORT="${FRONTEND_PORT:-5173}"
cd "$PROJECT_DIR"
command -v dotnet >/dev/null || { echo 'Nainstaluj .NET 10 SDK.'; exit 1; }
command -v npm >/dev/null || { echo 'Nainstaluj Node.js 22 nebo novejsi LTS.'; exit 1; }
command -v openssl >/dev/null || { echo 'Chybi openssl pro vytvoreni lokalniho klice.'; exit 1; }

for port in "$API_PORT" "$FRONTEND_PORT"; do
  if command -v lsof >/dev/null && lsof -iTCP:"$port" -sTCP:LISTEN -t >/dev/null 2>&1; then
    echo "Port $port uz pouziva jiny proces. Ukonci predchozi beh aplikace a spust znovu."
    exit 1
  fi
done

umask 077
mkdir -p .local
if [ ! -f .local/backend.env ]; then
  printf 'Jwt__Key=%s\n' "$(openssl rand -hex 32)" > .local/backend.env
  printf 'Seed__Password=Aa1!%s\n' "$(openssl rand -hex 12)" >> .local/backend.env
fi
set -a
source .local/backend.env
set +a
export ASPNETCORE_ENVIRONMENT=Development
export ASPNETCORE_URLS="http://localhost:$API_PORT"
export Cors__AllowedOrigins__0="http://localhost:$FRONTEND_PORT"
export Cors__AllowedOrigins__1="http://127.0.0.1:$FRONTEND_PORT"
export Database__Provider=Sqlite
export ConnectionStrings__DefaultConnection="Data Source=$PROJECT_DIR/.local/challenge.db"
export VITE_API_BASE_URL="http://localhost:$API_PORT"

if [ ! -f frontend/node_modules/vite/bin/vite.js ] || [ ! -f frontend/node_modules/typescript/bin/tsc ]; then
  (cd frontend && npm ci)
fi
dotnet build api/ChallengeTrackerApi/ChallengeTrackerApi.csproj --nologo

API_PID=''
FRONTEND_PID=''
cleanup() {
  trap - EXIT INT TERM
  [ -z "$FRONTEND_PID" ] || kill "$FRONTEND_PID" 2>/dev/null || true
  [ -z "$API_PID" ] || kill "$API_PID" 2>/dev/null || true
  wait 2>/dev/null || true
}
trap cleanup EXIT INT TERM

(cd api/ChallengeTrackerApi && exec dotnet bin/Debug/net10.0/ChallengeTrackerApi.dll) > .local/api.log 2>&1 &
API_PID=$!
for attempt in {1..60}; do
  if curl --fail --silent "http://localhost:$API_PORT/health/ready" >/dev/null; then break; fi
  if ! kill -0 "$API_PID" 2>/dev/null; then
    echo 'API se nepodarilo spustit. Podrobnosti jsou v .local/api.log.'
    tail -n 20 .local/api.log
    exit 1
  fi
  if [ "$attempt" -eq 60 ]; then
    echo 'API neni pripravene. Podivej se do .local/api.log.'
    exit 1
  fi
  sleep 1
done

(cd frontend && exec node node_modules/vite/bin/vite.js --host 127.0.0.1 --port "$FRONTEND_PORT") &
FRONTEND_PID=$!
echo "ChallengeTracker: http://localhost:$FRONTEND_PORT"
echo 'V prohlizeci si vytvor vlastni ucet. Ukonceni: Ctrl+C.'
echo 'Data zustanou v .local/challenge.db. Log API: .local/api.log.'
while kill -0 "$API_PID" 2>/dev/null && kill -0 "$FRONTEND_PID" 2>/dev/null; do sleep 1; done
echo 'Jeden ze serveru skoncil. Ukoncuji i druhy.'
exit 1

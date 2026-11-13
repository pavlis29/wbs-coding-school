#!/usr/bin/env bash
set -euo pipefail
PROJECT_DIR="$(cd "$(dirname "$0")/.." && pwd)"
cd "$PROJECT_DIR"
if [ -f .env ]; then
  echo 'Soubor .env uz existuje; ponechavam jej beze zmeny.'
  exit 0
fi
umask 077
printf 'JWT_KEY=%s\n' "$(openssl rand -hex 32)" > .env
printf 'MSSQL_SA_PASSWORD=Aa1!%s\n' "$(openssl rand -hex 24)" >> .env
printf 'FRONTEND_ORIGIN=http://localhost:5173\n' >> .env
echo 'Vytvoren lokalni .env s nahodnymi klici. Neposilej jej do Gitu.'

#!/usr/bin/env bash
set -euo pipefail
PROJECT_DIR="$(cd "$(dirname "$0")/.." && pwd)"
cd "$PROJECT_DIR"
dotnet test api/ChallengeTrackerApi.slnx --nologo
(cd frontend && npm run build)

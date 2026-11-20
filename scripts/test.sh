#!/usr/bin/env bash
set -euo pipefail
root="$(cd "$(dirname "$0")/.." && pwd)"
cd "$root"
projects=(
  tests/UnitTests
  tests/IntegrationTests
  tests/PlatformTests
  tests/PerformanceTests
  tests/SecurityTests
  tests/ArchitectureTests
  tests/UITests
)
for project in "${projects[@]}"; do
  echo "== $project =="
  dotnet test "$project" "$@"
done

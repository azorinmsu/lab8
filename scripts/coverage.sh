#!/usr/bin/env bash
set -euo pipefail
root="$(cd "$(dirname "$0")/.." && pwd)"
cd "$root"
output="${1:-$root/coverage}"
rm -rf "$output"
mkdir -p "$output"
settings=()
if [[ -f tests/coverlet.runsettings ]]; then
  settings=(--settings tests/coverlet.runsettings)
fi
projects=(
  tests/UnitTests
  tests/IntegrationTests
  tests/PlatformTests
  tests/PerformanceTests
  tests/SecurityTests
  tests/ArchitectureTests
)
for project in "${projects[@]}"; do
  name="$(basename "$project")"
  dotnet test "$project" \
    --collect:"XPlat Code Coverage" \
    --results-directory "$output/$name" \
    "${settings[@]}"
done
echo "Отчёты Cobertura: $output"
echo "UI-тесты на эмуляторе в покрытие не входят: сценарий Appium в этой среде не запускается."

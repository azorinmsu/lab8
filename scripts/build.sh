#!/usr/bin/env bash
set -euo pipefail
root="$(cd "$(dirname "$0")/.." && pwd)"
cd "$root"
dotnet build src/Phonebook.Core/Phonebook.Core.csproj "$@"
echo "Сборка Phonebook.Core завершена."
echo "Сборка MAUI-приложения требует workload maui-android и в этой команде не выполняется."
echo "Команда приложения: dotnet build src/Phonebook/Phonebook.csproj -f net9.0-android"

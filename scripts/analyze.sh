#!/usr/bin/env bash
set -euo pipefail
root="$(cd "$(dirname "$0")/.." && pwd)"
cd "$root"
dotnet build src/Phonebook.Core/Phonebook.Core.csproj --configuration Release /p:TreatWarningsAsErrors=true /p:AnalysisLevel=latest /p:EnforceCodeStyleInBuild=true
echo "Статический анализ Phonebook.Core выполнен вместе со сборкой Release."
echo "Предупреждения MAUI-проекта этой командой не проверяются."

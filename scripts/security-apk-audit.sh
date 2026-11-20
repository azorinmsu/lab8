#!/usr/bin/env bash
# Статический просмотр APK. Скрипт не подменяет отчёт MobSF и завершается с ошибкой,
# если обязательный инструмент не установлен.
set -euo pipefail

APK="${1:-}"
if [[ -z "$APK" ]]; then
  echo "Использование: scripts/security-apk-audit.sh path/to/app.apk" >&2
  echo "MobSF в этом скрипте не запускается. Откройте APK в MobSF отдельно и просмотрите отчёт вручную." >&2
  exit 2
fi

if [[ ! -f "$APK" ]]; then
  echo "APK не найден: $APK" >&2
  exit 2
fi

echo "Файл: $APK"
echo "MobSF: запустите вручную, этот скрипт только готовит локальные команды."

if command -v apkanalyzer >/dev/null 2>&1; then
  echo "## manifest permissions"
  apkanalyzer manifest permissions "$APK" || true
  echo "## manifest application"
  apkanalyzer manifest application-id "$APK" || true
else
  echo "apkanalyzer не найден. Установите Android SDK command-line tools." >&2
fi

if command -v aapt >/dev/null 2>&1; then
  aapt dump badging "$APK" | head -n 40 || true
else
  echo "aapt не найден." >&2
fi

echo "Проверка строк APK на маркеры закрытого ключа (не Access Key Unsplash):"
if command -v zipgrep >/dev/null 2>&1; then
  if zipgrep -n "BEGIN PRIVATE KEY" "$APK" >/dev/null 2>&1; then
    echo "Найден маркер закрытого ключа" >&2
    exit 1
  fi
  echo "Маркер закрытого ключа в APK не найден."
else
  echo "zipgrep не найден, проверка строк пропущена." >&2
fi

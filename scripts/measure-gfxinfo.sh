#!/usr/bin/env bash
set -euo pipefail
package="${1:-ContactFlow}"
out="${2:-measurements/gfxinfo.txt}"
if ! command -v adb >/dev/null 2>&1; then
  echo "adb не найден. Нужны эмулятор или устройство и запущенное приложение." >&2
  exit 2
fi
mkdir -p "$(dirname "$out")"
{
  echo "# $(date -Iseconds)"
  adb shell dumpsys gfxinfo "$package" || true
} | tee "$out"
echo "Сохранено: $out"
echo "Ищите Janky frames и percentile. Скрипт не вычисляет синтетическую плавность."

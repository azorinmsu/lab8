#!/usr/bin/env bash
# Разбор вывода `adb shell am start -W`. Скрипт не подставляет чужие цифры вместо измерения.
set -euo pipefail
package="${1:-ContactFlow}"
runs="${2:-5}"
out="${3:-measurements/startup.csv}"

if ! command -v adb >/dev/null 2>&1; then
  echo "adb не найден. Установите Android SDK Platform-Tools и запустите эмулятор или подключите устройство." >&2
  exit 2
fi

component="$(adb shell cmd package resolve-activity --brief "$package" | tr -d '\r' | tail -n 1)"
if [[ -z "$component" || "$component" != */* ]]; then
  echo "Не удалось определить Activity для пакета $package. Проверьте установку приложения: adb shell pm path $package" >&2
  exit 2
fi

mkdir -p "$(dirname "$out")"
echo "scenario,iteration,status,launch_state,total_time_ms,wait_time_ms" > "$out"

measure() {
  local scenario="$1"
  local iteration="$2"
  local output
  output="$(adb shell am start -W -n "$component" 2>&1 || true)"
  printf '%s\n' "$output"
  python3 - "$scenario" "$iteration" "$out" "$output" << 'PY'
import sys
scenario, iteration, path, output = sys.argv[1], sys.argv[2], sys.argv[3], sys.argv[4]
fields = {}
for line in output.splitlines():
    if ":" not in line:
        continue
    key, value = line.split(":", 1)
    fields[key.strip()] = value.strip()
row = [
    scenario,
    iteration,
    fields.get("Status", ""),
    fields.get("LaunchState", ""),
    fields.get("TotalTime", ""),
    fields.get("WaitTime", ""),
]
with open(path, "a", encoding="utf-8") as handle:
    handle.write(",".join(row) + "\n")
PY
}

echo "Cold start: остановите процесс перед каждой итерацией."
for ((i=1; i<=runs; i++)); do
  adb shell am force-stop "$package" || true
  sleep 1
  measure cold "$i"
done

echo "Warm start: процесс остановлен недавно, Activity создаётся заново."
for ((i=1; i<=runs; i++)); do
  adb shell input keyevent KEYCODE_HOME || true
  sleep 1
  adb shell am force-stop "$package" || true
  sleep 1
  measure warm "$i"
done

echo "Возврат из фона. Это не отдельный Hot Start, если am start -W не вернул LaunchState: HOT."
for ((i=1; i<=runs; i++)); do
  adb shell input keyevent KEYCODE_HOME || true
  sleep 1
  measure background "$i"
done

echo "CSV: $out"
echo "Поле launch_state берётся из adb. Не называйте строку hot, если LaunchState другой."

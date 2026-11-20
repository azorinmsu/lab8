#!/usr/bin/env bash
set -euo pipefail
echo "Локальные временные базы тестов:"
rm -rf "${TMPDIR:-/tmp}/phonebook-tests" || true
echo "Каталог phonebook-tests удалён, если он существовал."
echo "Данные приложения на устройстве эта команда не трогает."
echo "Для Debug-сборки на устройстве, если пакет ContactFlow доступен run-as:"
echo "  adb shell run-as ContactFlow rm -f files/contacts.db3"
echo "Окружение Testing при следующем запуске само удаляет свою базу в CacheDirectory/phonebook-testing."

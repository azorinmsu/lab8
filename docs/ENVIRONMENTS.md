# Окружения

Конфигурация сборки и окружение приложения — разные оси.

| | Development | Testing |
| --- | --- | --- |
| Назначение | Работа с устройством и, при наличии ключа, с Unsplash | Учебный прогон без сети и без чужих контактов |
| Выбор | `PHONEBOOK_ENVIRONMENT` не задана или равна `Development` | `PHONEBOOK_ENVIRONMENT=Testing` |
| Сборка | `Debug` или `Release`, на окружение не влияет | То же |

Отдельного Production нет.

## Команды

Development:

```bash
export PHONEBOOK_ENVIRONMENT=Development
dotnet build src/Phonebook/Phonebook.csproj -f net9.0-android -t:Run
```

Testing:

```bash
export PHONEBOOK_ENVIRONMENT=Testing
dotnet build src/Phonebook/Phonebook.csproj -f net9.0-android -t:Run
```

Обе команды запуска приложения требуют workload MAUI и устройство или эмулятор. Переменная читается в `PhonebookEnvironmentSelector`.

Тесты библиотеки не читают пользовательскую базу. Они создают файл в временном каталоге `phonebook-tests`.

## Файлы и источники настроек

- Переменная `PHONEBOOK_ENVIRONMENT`.
- Переменная `UNSPLASH_ACCESS_KEY`.
- Необязательный `appsettings.local.json` с полем `UnsplashAccessKey`. Образец: `src/Phonebook/appsettings.local.example.json`. Локальный файл игнорируется git.
- Регистрация сервисов: `src/Phonebook/MauiProgram.cs`.

## SQLite

Development: `FileSystem.AppDataDirectory/contacts.db3`.

Testing: `FileSystem.CacheDirectory/phonebook-testing/contacts.db3`. Файл удаляется при создании `DatabaseService`, поэтому старые учебные записи не смешиваются с новым запуском.

Тесты используют свой путь и этот файл не открывают.

## Unsplash

Development: `UnsplashService` и публичный Access Key, если он задан. Без ключа запросы не получат заголовок `Authorization`, сервис вернёт пустой список.

Testing: `FakeUnsplashClient`. Сеть не используется. Картинка — учебный URL `example.test`.

## Контакты

Development на Android: `DeviceContactsService` читает ContactsContract после разрешения READ_CONTACTS.

Development не на Android: источник возвращает пустой список.

Testing: `FakeDeviceContactSource` возвращает две вымышленные записи, `AllowAllContactPermissionGate` не показывает системный диалог.

## Какие тесты запускать

Все проекты в `tests/` запускаются без выбора окружения: они создают собственные подмены. Для ручной проверки приложения в Testing не нужен ни Unsplash, ни реальные контакты.

## Сброс данных

```bash
bash scripts/reset-testing-data.sh
```

Скрипт удаляет локальный каталог временных тестов. Команда `adb shell run-as` в нём напечатана и требует устройство; здесь она не выполнялась.

Повторный запуск приложения в Testing сам очищает учебную базу.

## Как понять активное окружение

Значение возвращает `PhonebookEnvironmentSelector.Current`. В Testing импорт не спрашивает разрешение и добавляет контакты «Учебный контакт Альфа» и «Учебный контакт Бета», если база была пустой. В Development на Android импорт запрашивает READ_CONTACTS.

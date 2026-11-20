# Лабораторная работа 5. Безопасность

## 1. Название и цель задания

Проверить, где лежит книга контактов, что делает пользовательская строка с SQLite, не попал ли в репозиторий секрет и как смотреть APK без личных данных.

## 2. Какие навыки проверяются

- Модель изоляции Android и флаги открытия SQLite.
- Безопасная проверка внедрения SQL на учебной базе.
- Различие публичного Access Key Unsplash и секретного ключа.
- Статический просмотр манифеста и границы автоматической проверки.

## 3. Что требуется реализовать или исследовать

Создайте `tests/SecurityTests/Assignments`, база — `TempSqlite`, признак `[Trait("Category", "Security")]`. См. [docs/TESTING.md](../../docs/TESTING.md). Строку `Robert'); DROP TABLE Contacts;--` используйте только на временной базе теста.

### 5.1. Данные

`LocalDataPlacement`, `ContactRepository`, `Platforms/Android/AndroidManifest.xml`. Файл `tests/SecurityTests/Assignments/DataProtectionTests.cs`.

Покажите, что файл создаётся по переданному пути. Разберите флаги `Create`, `ReadWrite` и `SharedCache`: последний относится к кэшу SQLite в процессе, а не к доступу других приложений. В манифесте `android:allowBackup="true"`. Не делайте вывода, что одного флага SQLite достаточно для приватности. Каталог приложения на Android — внутреннее хранилище пакета; резервная копия при `allowBackup` может забрать файл.

### 5.2. SQL

`ContactRepository`. Файл `tests/SecurityTests/Assignments/SqlInjectionTests.cs`.

Сохраните учебную строку `Robert'); DROP TABLE Contacts;--` в имя, телефон и остальные текстовые поля. Таблица `Contact` должна остаться, строка должна читаться обратно. Поиск по этой строке не должен выполнять её как SQL: `SearchContactsAsync` фильтрует память. `GetContactAsync` подставляет целочисленный идентификатор в текст запроса. Проверьте сохранение текста, а не подстановку этой фразы вместо числа.

### 5.3. Unsplash

`UnsplashService` и дерево исходников. Файл `tests/SecurityTests/Assignments/UnsplashSecretTests.cs`.

В репозитории не должно быть закрытого ключа и готового Access Key в исходнике сервиса. Публичный Access Key допустим в клиенте, если он приходит из `UNSPLASH_ACCESS_KEY` или локального файла, а не зашит в git. Проверьте, что тестовый ключ попадает в заголовок `Authorization` и не попадает в URL. Не пишите реальный ключ в тест.

### 5.4. APK

Скрипт `scripts/security-apk-audit.sh` и манифест. Файл `tests/SecurityTests/Assignments/ApkStaticChecklistTests.cs`.

Автоматически проверьте, что манифест объявляет `INTERNET`, `READ_CONTACTS` и `ACCESS_NETWORK_STATE`, а скрипт не сообщает, будто MobSF уже выполнен. Просмотр APK в MobSF, Android Studio APK Analyzer и `adb` выполняется вручную на собранном пакете без персональных контактов. Команды `apkanalyzer` требуют SDK и здесь не запускались.

## 4. Как запустить приложение для выполнения задания

Для тестов приложение не нужно. Сборка APK для MobSF описана в [docs/RUNNING.md](../../docs/RUNNING.md) и требует workload. Учебные данные окружения Testing не являются персональными.

## 5. Как запустить соответствующий тест

```bash
dotnet test tests/SecurityTests/SecurityTests.csproj --filter "FullyQualifiedName~DataProtectionTests|FullyQualifiedName~SqlInjectionTests|FullyQualifiedName~UnsplashSecretTests|FullyQualifiedName~ApkStaticChecklistTests"
```

Проект SecurityTests собирается. Классы задания добавляете вы.

Ручной просмотр APK, когда пакет уже собран:

```bash
bash scripts/security-apk-audit.sh path/to/app.apk
```

Без пути скрипт завершается кодом 2. Это проверено только как контракт файла; сам APK в среде подготовки не собирался.

## 6. Как проверить результат

- Временная база после опасной строки всё ещё содержит таблицу `Contact`.
- Поиск исходников не находит маркер закрытого ключа.
- URL запроса в тесте не содержит ключ.
- Отчёт не утверждает, что `SharedCache` делает базу доступной другим приложениям или, наоборот, заменяет модель Android.
- Пропуск MobSF не записан как успешный аудит APK.
- Для 5.3 при обходе файлов не сканируйте `bin`, `obj`, `.nuget`, `instructor-private` — см. [docs/TESTING.md](../../docs/TESTING.md).
- Ручная часть 5.4: файл `security/apk-audit-notes.md` (или раздел отчёта) — APK собран в Testing или пустой книге, без личных контактов; что смотрели в APK Analyzer или MobSF (разрешения, сетевой трафик, отсутствие секрета); если MobSF не запускали — напишите «MobSF не выполнялся», не «аудит пройден».

## 7. Критерии успешного выполнения

- Четыре группы тестов существуют и проходят с `[Trait("Category", "Security")]`.
- SQL-проверка идёт на временном файле.
- Секрет не добавлен в репозиторий ради теста.
- Описаны путь базы, `allowBackup` и смысл `SharedCache`.
- Заметки по APK приложены или явно помечены как не выполнявшиеся.

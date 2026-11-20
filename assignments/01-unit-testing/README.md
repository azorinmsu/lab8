# Лабораторная работа 1. Unit-тестирование

## 1. Название и цель задания

Исследовать изолированную логику ContactFlow: SQLite-репозиторий, преобразование контактов устройства, проверки полей и клиент Unsplash. Цель — написать детерминированные тесты без Android и без сети.

## 2. Какие навыки проверяются

- Проектирование модульных тестов и подмена HTTP.
- Различение фактического поведения и предполагаемой спецификации.
- Работа с граничными строками, Unicode и ограничениями SQLite.
- Чтение кода, который не вынесен в ViewModel.

## 3. Что требуется реализовать или исследовать

Создайте каталог `tests/UnitTests/Assignments` и файлы из подпунктов ниже. Общие правила: каталог, `TempSqlite`, `RepoPaths` и признак `[Trait("Category", "Unit")]` — в [docs/TESTING.md](../../docs/TESTING.md), раздел «Тесты заданий». Используйте xUnit. Для HTTP берите `StubHttpHandler` или NSubstitute. Не вызывайте `https://api.unsplash.com`.

### 1.1. SQLite и репозиторий

Класс `Phonebook.Data.ContactRepository`. Файл `tests/UnitTests/Assignments/SqliteCrudTests.cs`.

Покройте создание, чтение, обновление и удаление; пустые строки; null; два контакта с одним номером; имя длиннее 255 символов; конфликт первичного ключа; поиск; добавление колонок Email и Address в таблицу старой схемы. База — файл через `TempSqlite`, не in-memory SQLite и не `contacts.db3` приложения.

### 1.2. Преобразование контактов Android

Классы `AndroidContactMapper`, `RawDeviceContact`, `PhoneNumberNormalizer`. Файл `tests/UnitTests/Assignments/AndroidContactMapperTests.cs`.

Покройте отсутствующие имя и телефон, специальные символы и Unicode в имени и адресе, несколько телефонов одного идентификатора, дубликаты номеров, присоединение email. Учтите, что `AndroidContactsContractMap.IncludesPostalAddress` описывает платформенный читатель: почтовый адрес из ContactsContract сейчас не запрашивается. Есть две нормализации: `NormalizeStored` и `NormalizeKey`.

### 1.3. Проверки полей

Классы `ContactFieldValidator` и `PhoneNumberNormalizer`. Файл `tests/UnitTests/Assignments/ContactFieldValidatorTests.cs`.

Покройте номера с `+7` и `8`, пробелы и дефисы, десятизначный номер, строки без цифр. Для email проверьте пустое значение, обычный адрес, адрес с отображаемым именем. Валидатор использует `System.Net.Mail.MailAddress` и сравнение `Address` с исходной строкой. Не пишите, что это RFC 5322. Сохранение карточки не требует заполненных полей: это видно по `SaveEnforcesRequiredFields`. Звонок проверяет только непустую строку.

### 1.4. Unsplash

Классы `UnsplashUrlBuilder`, `UnsplashJsonParser`, `UnsplashService`. Файл `tests/UnitTests/Assignments/UnsplashClientTests.cs`.

Проверьте состав URL поиска и случайных фото. Ключ не входит в query: в поиске нет `client_id`, параметр `orientation` есть у случайных фото и отсутствует у поиска. Покройте разбор JSON, пустой список, некорректный JSON, ответ HTTP с ошибкой. В сервис передавайте `readEnvironment: false`, чтобы тест не зависел от переменной окружения. Загрузка файла не должна отправлять `If-None-Match` и `If-Modified-Since`.

## 4. Как запустить приложение для выполнения задания

Приложение для этих тестов не требуется. Контекст экранов описан в [docs/RUNNING.md](../../docs/RUNNING.md). Сборка логики:

```bash
dotnet build src/Phonebook.Core/Phonebook.Core.csproj
```

## 5. Как запустить соответствующий тест

```bash
dotnet test tests/UnitTests/UnitTests.csproj --filter "FullyQualifiedName~SqliteCrudTests|FullyQualifiedName~AndroidContactMapperTests|FullyQualifiedName~ContactFieldValidatorTests|FullyQualifiedName~UnsplashClientTests"
```

Команда проверена на проекте UnitTests. Пока классов нет, фильтр ничего не выполняет: это не сдача работы.

## 6. Как проверить результат

- `dotnet test` для перечисленных классов завершается без ошибок.
- Временные файлы базы и картинок удаляются в конце теста.
- Повторный запуск даёт тот же результат.
- В журнале теста нет обращения к сети: обработчик HTTP локальный.

## 7. Критерии успешного выполнения

- Есть отдельные тесты на все перечисленные случаи четырёх частей.
- У методов задания стоит `[Trait("Category", "Unit")]`, фильтр `Category=Unit` их находит.
- Утверждения описывают наблюдаемое поведение классов, а не желательную спецификацию, которой в коде нет.
- Для SQLite используется `TempSqlite` и `ContactRepository`.
- Email не назван соответствующим RFC 5322.
- Тесты не зависят от порядка и не используют реальный Access Key.

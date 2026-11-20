# Тестирование

Автоматические тесты ходят в `Phonebook.Core`. Они не поднимают Android и не вызывают Unsplash. Команды ниже проверены локально, кроме отдельно помеченных.

## Тесты заданий

Классы лабораторных работ кладите в каталог `Assignments` внутри нужного проекта, например `tests/UnitTests/Assignments/SqliteCrudTests.cs`. В базовом репозитории этого каталога нет: создайте его сами. Пространство имён: `Phonebook.<ИмяПроекта>.Assignments`, по образцу `InfrastructureSmokeTests` в корне того же проекта.

Для SQLite и путей к исходникам используйте `Phonebook.TestSupport`:

- `TempSqlite` — изолированный файл базы и готовый `ContactRepository`; освобождается через `await using`.
- `RepoPaths.SourceFile(...)` — путь к файлу в репозитории от корня solution (манифест, XAML, csproj).
- `StubHttpHandler` — локальный ответ HTTP без сети.

На каждый тестовый метод задания повесьте признак категории, как у инфраструктурного теста:

```csharp
[Trait("Category", "Unit")]
```

Категория совпадает с типом лабораторной: `Unit`, `Integration`, `Platform`, `Performance`, `Security`, `Architecture`, `UI`, для покрытия в лаб. 7 — `Coverage`. Фильтр `Category=UI` в задании 6 относится только к вашим тестам с этим признаком, не к `InfrastructureSmokeTests`.

## Все тестовые проекты

```bash
bash scripts/test.sh
```

Скрипт последовательно вызывает `dotnet test` для UnitTests, IntegrationTests, PlatformTests, PerformanceTests, SecurityTests, ArchitectureTests и UITests.

## Один проект

```bash
dotnet test tests/UnitTests/UnitTests.csproj
dotnet test tests/IntegrationTests/IntegrationTests.csproj
dotnet test tests/PlatformTests/PlatformTests.csproj
dotnet test tests/PerformanceTests/PerformanceTests.csproj
dotnet test tests/SecurityTests/SecurityTests.csproj
dotnet test tests/ArchitectureTests/ArchitectureTests.csproj
dotnet test tests/UITests/UITests.csproj
```

## Один тест

```bash
dotnet test tests/UnitTests/UnitTests.csproj --filter FullyQualifiedName~InfrastructureSmokeTests
```

Подставьте имя своего класса вместо `InfrastructureSmokeTests`.

## Категории

У тестов есть признак `Category`: `Infrastructure`, `Unit`, `Integration`, `Platform`, `Performance`, `Security`, `Architecture`, `UI`, `Coverage`.

```bash
dotnet test tests/UnitTests/UnitTests.csproj --filter Category=Unit
dotnet test tests/UITests/UITests.csproj --filter Category=UI
```

## Отчёт

```bash
dotnet test tests/UnitTests/UnitTests.csproj --logger "trx;LogFileName=unit.trx" --results-directory TestResults
```

Файл появится в `TestResults`. Каталог не коммитится.

## Покрытие

```bash
bash scripts/coverage.sh
```

Скрипт собирает Cobertura по проектам, кроме UITests. Открытие отчёта: файл `coverage.cobertura.xml` внутри `coverage/`. Порог 80–85% относится к бизнес-логике `Phonebook.Core`, не к страницам MAUI.

## Платформенные и UI-тесты

`tests/PlatformTests` — обычные тесты .NET. Они проверяют решения, которые приложение принимает по разрешениям, Intent и жизненному циклу. Они не ставят APK и не нажимают системные диалоги.

`tests/UITests` читает XAML и исходники. Тест сценария эмулятора помечен `Skip`: без эмулятора, Appium и `PHONEBOOK_RUN_UI=1` он не выполняется и не считается пройденным сценарием.

Запуск на эмуляторе требует отдельной установки Appium, Android driver и образа системы. В CI этот сценарий не запускается.

## Какие тесты требуют Android

| Проект | Android |
| --- | --- |
| UnitTests, IntegrationTests, SecurityTests, ArchitectureTests, PerformanceTests | Нет |
| PlatformTests | Нет, это модель поведения Android на .NET |
| UITests, кроме пропущенного сценария эмулятора | Нет |
| Пропущенный тест эмулятора, `scripts/measure-android-startup.sh`, `scripts/measure-gfxinfo.sh` | Да |

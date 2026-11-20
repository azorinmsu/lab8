# Лабораторная работа 7. Качество кода и архитектура

## 1. Название и цель задания

Настроить покрытие бизнес-логики, включить анализатор компилятора и выделить поставщик изображений за интерфейсом, не меняя поведение экранов.

## 2. Какие навыки проверяются

- Покрытие как инженерный ориентир, а не как самоцель.
- Чтение предупреждений Roslyn.
- Выделение интерфейса и регистрация в DI.
- Сохранение поведения при замене зависимости.

## 3. Что требуется реализовать или исследовать

Каталоги `tests/UnitTests/Assignments` и `tests/ArchitectureTests/Assignments`. Правила путей и признаков — [docs/TESTING.md](../../docs/TESTING.md). На `CoreCoverageTests` — `Category=Coverage`, на остальные тесты этой лаборатории в ArchitectureTests — `Category=Architecture`.

### 7.1. Покрытие

Файл `tests/UnitTests/Assignments/CoreCoverageTests.cs` и при необходимости `tests/coverlet.runsettings`.

Покройте тестами публичное поведение `Phonebook.Core`, которого ещё не касаются ваши предыдущие работы: ветки окружения, нормализации, импорта, кэша, разбора ответа и записи CSV. Ориентир — 80–85% строк `Phonebook.Core`. Страницы MAUI, Android и сгенерированный код в порог не входят. Запуск:

```bash
bash scripts/coverage.sh
```

Скрипт проверен: он создаёт каталог `coverage` и вызывает `dotnet test --collect:"XPlat Code Coverage"`. Если файла `tests/coverlet.runsettings` нет, сбор идёт без него.

### 7.2. Статический анализ

В `src/Phonebook.Core/Phonebook.Core.csproj` включите `TreatWarningsAsErrors` со значением `true`, `AnalysisLevel` со значением `latest` и `EnforceCodeStyleInBuild` со значением `true`. Файл проверки: `tests/ArchitectureTests/Assignments/StaticAnalysisConfigTests.cs`. Он должен прочитать csproj и увидеть эти три свойства.

Сборка с предупреждениями как ошибками для текущего Core проходит. MAUI-проект в этот порог не включайте: его предупреждения платформы не относятся к заданию. Команда:

```bash
bash scripts/analyze.sh
```

Дубли, сложность и неиспользуемый код смотрите по предупреждениям сборки. Отдельный SonarQube в репозитории не поднимается.

### 7.3. Поставщик изображений

Сейчас страницы получают `IUnsplashClient`. Выделите `Phonebook.Imaging.IImageProvider` с поиском, случайной выборкой и загрузкой файла. Реализацию `UnsplashImageProvider` постройте на `IUnsplashClient`. Зарегистрируйте её в `MauiProgram` для обоих окружений. Переведите `ContactDetailPage` и `UnsplashImagePickerPage` на интерфейс. Файл теста `tests/ArchitectureTests/Assignments/ImageProviderTests.cs`: подставьте свою реализацию `IUnsplashClient` и проверьте поиск, загрузку и отказ при пустом URL. Сборка `Phonebook.Core` не должна получить ссылку на MAUI.

## 4. Как запустить приложение для выполнения задания

После смены DI приложение должно открываться как раньше. Команда запуска требует workload и устройство, см. [docs/RUNNING.md](../../docs/RUNNING.md). В Testing по-прежнему используется `FakeUnsplashClient`, и поставщик изображений обязан работать поверх него.

## 5. Как запустить соответствующий тест

```bash
dotnet test tests/UnitTests/UnitTests.csproj --filter FullyQualifiedName~CoreCoverageTests
dotnet test tests/ArchitectureTests/ArchitectureTests.csproj --filter "FullyQualifiedName~StaticAnalysisConfigTests|FullyQualifiedName~ImageProviderTests"
bash scripts/coverage.sh
bash scripts/analyze.sh
```

`dotnet test tests/ArchitectureTests` и `scripts/analyze.sh` проверены на базовом репозитории. Новые классы задания появляются после вашей реализации. Покрытие смотрите по свежему XML, а не по старому файлу.

## 6. Как проверить результат

- В отчёте покрытия включена сборка `Phonebook.Core`, страницы MAUI не требуются для порога.
- `dotnet build src/Phonebook.Core/Phonebook.Core.csproj` с включёнными свойствами не пишет предупреждения.
- Тест поставщика проходит на подмене и не открывает сеть.
- Регистрация `IImageProvider` есть и в Development, и в Testing.
- Поиск изображений и сохранение файла на карточке вызывают тот же клиент, что и раньше.

## 7. Критерии успешного выполнения

- Тесты помечены `Coverage` и `Architecture` там, где указано в §3.
- Покрытие бизнес-логики не ниже 80% по вашему отчёту, без требования такого же процента для MAUI.
- Анализатор включён свойствами проекта, и Core собирается без предупреждений.
- Интерфейс поставщика изображений внедряется через DI.
- Поведение загрузки файла сохранено: пустой URL не приводит к запросу.
- В отчёте указана команда, которой получен процент покрытия.

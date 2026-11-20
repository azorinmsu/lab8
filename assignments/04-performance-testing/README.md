# Лабораторная работа 4. Производительность

## 1. Название и цель задания

Собрать воспроизводимые измерения запуска, списка контактов, вставки SQLite и влияния задержки сети на клиент изображений. Числа, снятые не на вашем стенде, нельзя подписывать как результат ContactFlow.

## 2. Какие навыки проверяются

- Отделение прогрева от измеряемых запусков.
- Сохранение сырых данных в CSV.
- Использование подмены сети вместо боевого API.
- Понимание, какой метрики нет без устройства.

## 3. Что требуется реализовать или исследовать

Каталог `tests/PerformanceTests/Assignments`, признак `[Trait("Category", "Performance")]` — [docs/TESTING.md](../../docs/TESTING.md).

### 4.1. Запуск

Классы `AdbStartWindowParser` и `MeasurementCsvWriter`, скрипт `scripts/measure-android-startup.sh`. Файл `tests/PerformanceTests/Assignments/StartupMeasurementTests.cs`.

Напишите тесты разбора готового текста `adb shell am start -W` для строк `COLD`, `WARM` и `HOT`. Это проверка разборщика, а не замер приложения. На устройстве выполните скрипт. Если в выводе нет `LaunchState: HOT`, не называйте сценарий горячим стартом. Команда скрипта требует `adb` и установленное приложение; без них скрипт завершается с кодом 2. В среде подготовки репозитория `adb` не был установлен, скрипт до измерений не доходил.

### 4.2. Список

Класс `ContactDataSetFactory`, скрипт `scripts/measure-gfxinfo.sh`. Файл `tests/PerformanceTests/Assignments/CollectionViewDataSetTests.cs`.

Проверьте наборы 10, 100, 1000, 5000 и 10000 контактов, уникальность номеров и вариант с путём к изображению и без него. Время кадра, RAM и пропущенные кадры снимаются на устройстве через `dumpsys gfxinfo` после прокрутки. Скрипт требует `adb` и здесь не выполнялся. Не заполняйте отчёт выдуманными FPS.

### 4.3. SQLite

Класс `SqliteInsertExperiment`. Файл `tests/PerformanceTests/Assignments/SqliteInsertExperimentTests.cs`.

Сравните вставку 500 контактов по одной строке, в явной транзакции и пакетом `InsertAll`. Сделайте прогрев и отдельные измеряемые повторы. Запишите CSV колонками `strategy,contacts,phase,iteration,elapsed_ms`. Прогрев помечен `warmup` и не смешивается с `measured`.

### 4.4. Сеть

`UnsplashService` и локальный `HttpMessageHandler` с `Task.Delay`. Файл `tests/PerformanceTests/Assignments/NetworkLatencyTests.cs`.

Сравните нулевую задержку и задержку не меньше 80 мс. Сохраните CSV. Хост запроса может быть `api.unsplash.com`, но ответ обязан отдать обработчик теста. Колонку источника подпишите `stub-handler`.

## 4. Как запустить приложение для выполнения задания

Генераторы и вставка SQLite не требуют приложения. Запуск и gfxinfo требуют установленный APK. Сборка APK описана в [docs/RUNNING.md](../../docs/RUNNING.md) и требует workload, который в среде подготовки не был установлен.

## 5. Как запустить соответствующий тест

```bash
dotnet test tests/PerformanceTests/PerformanceTests.csproj --filter "FullyQualifiedName~StartupMeasurementTests|FullyQualifiedName~CollectionViewDataSetTests|FullyQualifiedName~SqliteInsertExperimentTests|FullyQualifiedName~NetworkLatencyTests"
```

Проект PerformanceTests уже собирается и выполняет инфраструктурный тест. Классы задания добавляете вы. Вставка 500 контактов занимает несколько секунд.

На устройстве, когда оно есть:

```bash
bash scripts/measure-android-startup.sh
bash scripts/measure-gfxinfo.sh
```

## 6. Как проверить результат

- CSV вставки содержит три стратегии, фазы `warmup` и `measured`, 500 контактов.
- Разборщик `adb` проверяется на фикстуре, и фикстура не названа замером приложения.
- Набор данных имеет ровно запрошенный размер.
- Задержанный HTTP-вызов длится не меньше заданной паузы.
- В репозитории нет файла, который притворяется результатом чужого устройства.
- CSV кладите в каталог `measurements/` в корне репозитория (он в `.gitignore`). В отчёте укажите имя файла и команду, которой получили данные.
- Если есть устройство: приложите `measurements/device-report.md` — модель, дата, для 4.1 путь к CSV из `scripts/measure-android-startup.sh` или пометка «adb недоступен»; для 4.2 — что сняли через `measure-gfxinfo.sh` или «не снималось». Можно одну таблицу или простой график по своему CSV (ось времени или размера набора). Чужие цифры не подставляйте.

## 7. Критерии успешного выполнения

- Есть четыре группы проверок с `[Trait("Category", "Performance")]`.
- Прогрев отделён от измеряемых запусков.
- Результаты вставки и задержки записаны в CSV в `measurements/`.
- Эксперимент сети не использует боевой Unsplash.
- Отчёт по устройству (4.1 и 4.2) приложен или явно отмечен как не выполнявшийся.

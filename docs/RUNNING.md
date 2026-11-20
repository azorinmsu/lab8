# Запуск ContactFlow

Команды `dotnet build` и `dotnet test` для `Phonebook.Core` и тестовых проектов проверены на .NET SDK 9.0.123 в Linux. Команды эмулятора, `adb` и сборки MAUI требуют Android SDK и workload; в среде подготовки репозитория они не выполнялись до успешного запуска приложения.

## Инструменты

| Инструмент | Версия в проекте |
| --- | --- |
| .NET SDK | 9.0 |
| MAUI | `Microsoft.Maui.Controls` 9.0.60 |
| Android min SDK | 21 |
| Целевые платформы проекта | `net9.0-android`, `net9.0-ios`, `net9.0-maccatalyst`; Windows только при сборке на Windows |
| JDK | 17 |

Установка workload, если его нет:

```bash
dotnet workload install maui-android
```

Эта команда требует сеть и не проверялась до конца в среде подготовки репозитория.

## Зависимости

```bash
dotnet restore src/Phonebook.Core/Phonebook.Core.csproj
dotnet restore src/Phonebook/Phonebook.csproj
```

Вторая команда требует workload MAUI. Первая проверена.

## Сборка

```bash
dotnet build src/Phonebook.Core/Phonebook.Core.csproj
dotnet build src/Phonebook/Phonebook.csproj -f net9.0-android
```

Проверена только сборка `Phonebook.Core`. Сборка Android завершится ошибкой `NETSDK1147`, пока не установлен workload.

## Запуск приложения

Команды ниже проверены на Linux с workload `maui-android`, JDK 17, Android SDK (API 35) и запущенным эмулятором или устройством (`adb devices` показывает `device`).

На Linux в одной команде собирается и ставится только Android. iOS и Mac Catalyst из `Phonebook.csproj` здесь не участвуют:

```bash
export ANDROID_HOME=/путь/к/android-sdk   # обязательно, иначе XA5300
export JAVA_HOME=/путь/к/jdk-17
export PATH="$PATH:$ANDROID_HOME/platform-tools"

# по желанию: см. docs/ENVIRONMENTS.md
export PHONEBOOK_ENVIRONMENT=Testing       # фиктивные контакты и Unsplash
# export PHONEBOOK_ENVIRONMENT=Development # реальный импорт на Android после READ_CONTACTS

cd /путь/к/репозиторию

dotnet restore src/Phonebook.Core/Phonebook.Core.csproj
dotnet restore src/Phonebook/Phonebook.csproj -p:TargetFrameworks=net9.0-android
dotnet restore src/Phonebook.Core/Phonebook.Core.csproj

dotnet build src/Phonebook/Phonebook.csproj \
  -f net9.0-android \
  -p:TargetFrameworks=net9.0-android \
  -p:AndroidSdkDirectory="$ANDROID_HOME" \
  -t:Run \
  --no-restore
```

Повторный `restore` для `Phonebook.Core` нужен потому, что после restore MAUI-проекта у общей библиотеки в `obj/project.assets.json` может остаться только `net9.0-android`, и сборка упадёт с `NETSDK1005` по `net9.0`. Если SDK ставился впервые:

```bash
dotnet build src/Phonebook/Phonebook.csproj -t:InstallAndroidDependencies \
  -f net9.0-android -p:TargetFrameworks=net9.0-android \
  -p:AndroidSdkDirectory="$ANDROID_HOME" \
  -p:AcceptAndroidSdkLicenses=True
```

На Windows и macOS можно вызывать `dotnet build ... -f net9.0-android -t:Run` без ограничения `TargetFrameworks`, если в системе установлены workload для всех целевых платформ проекта.

Ключ Unsplash не хранится в репозитории. Для реальных запросов в Development задайте публичный Access Key:

```bash
export UNSPLASH_ACCESS_KEY="ваш_публичный_access_key"
```

Либо скопируйте `src/Phonebook/appsettings.local.example.json` в `appsettings.local.json` рядом с собранным приложением и подставьте ключ. Файл `appsettings.local.json` не коммитится. Секретный ключ Unsplash в клиент помещать нельзя. Подробнее об окружениях — [ENVIRONMENTS.md](ENVIRONMENTS.md).

## Эмулятор Android

Нужны Android SDK и образ системы API не ниже 21. Создание и запуск эмулятора выполняются в Android Studio или `avdmanager`/`emulator`. Эти команды требуют установленный SDK и здесь не запускались:

```bash
emulator -list-avds
emulator -avd <имя_образа>
adb devices
```

## Физическое устройство

1. Включите отладку по USB.
2. Подключите устройство и подтвердите отпечаток RSA.
3. Проверьте `adb devices`.
4. Запустите `dotnet build src/Phonebook/Phonebook.csproj -f net9.0-android -t:Run`.

## Проверка успешного запуска

На экране виден заголовок ContactFlow, строка «0 контактов» или число контактов и кнопка «+». В окружении Testing импорт подставляет две учебные записи и не запрашивает разрешение READ_CONTACTS.

## Типовые ошибки

- `NETSDK1147` и упоминание `maui` или `wasi-experimental`: не установлен workload `maui-android`.
- Сборка Android без SDK: установите Android SDK и примите лицензии `sdkmanager --licenses`.
- Пустые изображения Unsplash в Development: не задан `UNSPLASH_ACCESS_KEY`. Приложение при этом открывается, список изображений остаётся пустым.
- Идентификатор пакета в проекте — `ContactFlow`. Если установщик Android отклонит имя без точки, это ограничение исходного проекта.

# ContactFlow

Учебное приложение телефонной книги на .NET MAUI. В нём есть локальная книга контактов SQLite, импорт контактов Android, карточка контакта и подбор изображения через Unsplash.

## Системные требования

- .NET SDK 9.0
- Для приложения Android: workload `maui-android`, Android SDK с API 21 и выше, эмулятор или устройство
- JDK 17 для Android-сборки

## Запуск

Библиотека логики:

```bash
dotnet build src/Phonebook.Core/Phonebook.Core.csproj
```

Приложение Android (нужны workload и SDK):

```bash
dotnet build src/Phonebook/Phonebook.csproj -f net9.0-android
dotnet build src/Phonebook/Phonebook.csproj -f net9.0-android -t:Run
```

Подробности: [docs/RUNNING.md](docs/RUNNING.md).

## Тесты

```bash
bash scripts/test.sh
```

Подробности: [docs/TESTING.md](docs/TESTING.md).

## Задания

- [Лабораторная работа 1. Unit-тестирование](assignments/01-unit-testing/README.md)
- [Лабораторная работа 2. Интеграционное тестирование](assignments/02-integration-testing/README.md)
- [Лабораторная работа 3. Платформенное тестирование Android](assignments/03-platform-testing/README.md)
- [Лабораторная работа 4. Производительность](assignments/04-performance-testing/README.md)
- [Лабораторная работа 5. Безопасность](assignments/05-security-testing/README.md)
- [Лабораторная работа 6. UI и доступность](assignments/06-ui-accessibility/README.md)
- [Лабораторная работа 7. Качество кода и архитектура](assignments/07-code-quality/README.md)

## Окружения

[docs/ENVIRONMENTS.md](docs/ENVIRONMENTS.md)
# lab8
# lab8
# lab8
# lab8

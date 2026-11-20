# Лабораторная работа 3. Платформенное тестирование Android

## 1. Название и цель задания

Проверить решения приложения для Android без запуска эмулятора: разрешение READ_CONTACTS, вызовы связи и жизненный цикл Activity. Минимальный API проекта — 21, он задан в `Phonebook.csproj` и `AndroidActivityPolicy.MinSdkVersion`.

## 2. Какие навыки проверяются

- Сопоставление кода с моделью разрешений Android.
- Отличие поддерживаемого сценария от желаемого.
- Чтение `MainActivity` и манифеста.
- Честная граница между модульным тестом и проверкой на устройстве.

## 3. Что требуется реализовать или исследовать

Создайте `tests/PlatformTests/Assignments`. Пути к манифесту и `MainActivity.cs` — через `RepoPaths`. Признак `[Trait("Category", "Platform")]`. См. [docs/TESTING.md](../../docs/TESTING.md). APK не устанавливается.

### 3.1. Разрешения

Класс `ContactsPermissionPolicy`. Файл `tests/PlatformTests/Assignments/ContactsPermissionPolicyTests.cs`.

Проверьте состояния выдано, отклонено и отозвано; повторный запрос; API 21–22 и API 23 и новее. Для READ_CONTACTS нет сценария «только в этот раз»: `SupportsOneTimeGrant` должен остаться ложным. Не придумывайте одноразовое разрешение для контактов.

### 3.2. Intent

Класс `CommunicationIntents`. Файл `tests/PlatformTests/Assignments/CommunicationIntentTests.cs`.

В приложении есть набор номера через телефонное приложение и письмо через `ACTION_SENDTO` с `mailto:`. Отдельного SMS нет. Проверьте пустой номер, неподдерживаемый набор, отсутствие почтового клиента и сообщение `ActivityNotFoundMessage`. Наличие SIM-карты код не проверяет: зафиксируйте `SimCardIsChecked`.

### 3.3. Жизненный цикл

Классы `AndroidActivityPolicy` и `ContactFormSnapshot`, файл `Platforms/Android/MainActivity.cs`. Файл теста `tests/PlatformTests/Assignments/ActivityLifecycleTests.cs`.

`MainActivity` перечисляет `ConfigurationChanges`, включая ориентацию и размер экрана, поэтому поворот не пересоздаёт Activity. Отдельного сохранения формы при уничтожении процесса нет. `ContactFormSnapshot` описывает поля карточки, но страница сама снимок не пишет. Проверьте круг чтения и записи снимка и флаг `PersistsFormAcrossProcessDeath`. Не утверждайте, что MAUI сохраняет текст полей само.

## 4. Как запустить приложение для выполнения задания

Модульные тесты запускаются без приложения. Наблюдение поворота и диалога разрешения на устройстве требует эмулятор или телефон и команду из [docs/RUNNING.md](../../docs/RUNNING.md). Эта команда в среде подготовки репозитория не выполнялась.

## 5. Как запустить соответствующий тест

```bash
dotnet test tests/PlatformTests/PlatformTests.csproj --filter "FullyQualifiedName~ContactsPermissionPolicyTests|FullyQualifiedName~CommunicationIntentTests|FullyQualifiedName~ActivityLifecycleTests"
```

`dotnet test tests/PlatformTests` проверен. Сами классы задания нужно добавить.

## 6. Как проверить результат

- Тесты зелёные и не требуют `adb`.
- В утверждениях видны API 21 и 23.
- Сценарий SMS и проверка SIM не выданы за реализованные.
- Текст `MainActivity.cs` действительно содержит `ConfigChanges.Orientation`.

## 7. Критерии успешного выполнения

- Три группы тестов с `Category=Platform` покрывают разрешение, Intent и жизненный цикл.
- Одноразовое разрешение для READ_CONTACTS не смоделировано как поддерживаемое.
- Для поворота указано, что Activity не пересоздаётся из-за `ConfigurationChanges`.
- Для смерти процесса указано, что форма не сохраняется.
- Нет зависимости от контактов устройства.

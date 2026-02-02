# NOAA Client Sample

Консольное приложение для демонстрации работы с NOAA API клиентами с использованием System.CommandLine.

## Описание

Это приложение демонстрирует использование всех доступных методов клиентов NOAA через мощный CLI интерфейс:

### ACE (Advanced Composition Explorer) Spacecraft
- **Magnetometer Data** - данные магнитометра (1-минутное усреднение)
- **SWEPAM Data** - данные Solar Wind Electron Proton Alpha Monitor

### DSCOVR (Deep Space Climate Observatory) Spacecraft
- **Magnetometer Data** - данные магнитометра с различными временными диапазонами:
  - 2 часа
  - 1 день
  - 3 дня
  - 7 дней
- **Solar Wind Plasma Data** - данные солнечного ветра с различными временными диапазонами:
  - 2 часа
  - 1 день
  - 3 дня
  - 7 дней

### KP Index
- **27-Day Forecast** - прогноз KP-индекса на 27 дней
- **3-Day Forecast** - прогноз KP-индекса на 3 дня
- **Nowcast** - текущие данные KP-индекса

## Возможности

✨ **Команды CLI** - мощный интерфейс командной строки с использованием System.CommandLine

📊 **Табличный вывод** - форматированное отображение данных в виде таблиц

🎨 **Уровни активности** - KP-индекс отображается с текстовыми метками уровня активности:
- Низкая (0-2)
- Умеренная (3-4)
- Повышенная (5-6)
- Высокая (7-8)
- Экстремальная (9+)

⚙️ **Опции командной строки**:
- `--limit` / `-l` - количество записей для отображения
- `--verbose` / `-v` - полный вывод всех данных
- `--period` / `-p` - временной диапазон для DSCOVR (2h, 1d, 3d, 7d)

⚡ **Массовое выполнение** - команда `all` для выполнения всех запросов последовательно

🔍 **Встроенная справка** - используйте `--help` для любой команды

## Требования

- .NET 8.0 или выше
- Доступ к интернету (для подключения к NOAA API)

## Конфигурация

Приложение использует файл `appsettings.json` для настройки:

```json
{
  "Noaa": {
    "ServerUrl": "https://services.swpc.noaa.gov"
  }
}
```

## Запуск

### Общая справка

```bash
dotnet run -- --help
```

### ACE Commands

Получить данные магнитометра ACE:
```bash
dotnet run -- ace magnetometer
dotnet run -- ace magnetometer --limit 20
dotnet run -- ace magnetometer --verbose
```

Получить данные SWEPAM ACE:
```bash
dotnet run -- ace swepam
dotnet run -- ace swepam -l 15
```

### DSCOVR Commands

Получить данные магнитометра DSCOVR:
```bash
dotnet run -- dscovr magnetometer --period 2h
dotnet run -- dscovr magnetometer -p 1d
dotnet run -- dscovr magnetometer -p 7d --verbose
```

Получить данные солнечного ветра DSCOVR:
```bash
dotnet run -- dscovr plasma --period 2h
dotnet run -- dscovr plasma -p 3d
```

### KP Index Commands

Получить 27-дневный прогноз:
```bash
dotnet run -- kp 27day
dotnet run -- kp 27day --limit 15
```

Получить 3-дневный прогноз:
```bash
dotnet run -- kp 3day
dotnet run -- kp 3day -v
```

Получить текущие данные (nowcast):
```bash
dotnet run -- kp nowcast
dotnet run -- kp nowcast -l 20
```

### Выполнить все запросы

```bash
dotnet run -- all
```

## Примеры использования

### Быстрый старт

```bash
# Получить последние данные ACE
dotnet run -- ace magnetometer

# Получить данные DSCOVR за последний день
dotnet run -- dscovr magnetometer -p 1d

# Посмотреть текущий KP-индекс
dotnet run -- kp nowcast
```

### Подробный анализ

```bash
# Посмотреть все данные магнитометра ACE
dotnet run -- ace magnetometer --verbose

# Получить последние 30 записей прогноза KP
dotnet run -- kp 3day --limit 30

# Выполнить все запросы сразу
dotnet run -- all
```

## Структура команд

```
noaa-client-sample
├── ace
│   ├── magnetometer [--limit] [--verbose]
│   └── swepam [--limit] [--verbose]
├── dscovr
│   ├── magnetometer --period {2h|1d|3d|7d} [--verbose]
│   └── plasma --period {2h|1d|3d|7d} [--verbose]
├── kp
│   ├── 27day [--limit] [--verbose]
│   ├── 3day [--limit] [--verbose]
│   └── nowcast [--limit] [--verbose]
└── all
```

## Архитектура

Приложение построено с использованием лучших практик:

- **System.CommandLine** - современная библиотека для создания CLI приложений
- **Dependency Injection** - использование Microsoft.Extensions.DependencyInjection
- **Configuration Management** - использование Microsoft.Extensions.Configuration
- **Typed HTTP Clients** - регистрация через AddHttpClient
- **Clean Architecture** - разделение на слои и использование интерфейсов
- **Async/Await** - асинхронная работа с API
- **Error Handling** - обработка ошибок и информативные сообщения

## Преимущества System.CommandLine

✅ **Автоматическая генерация справки** - встроенная поддержка `--help`

✅ **Валидация аргументов** - автоматическая проверка типов и значений

✅ **Автодополнение** - поддержка shell completion для bash, zsh, powershell

✅ **Иерархия команд** - логическая структура команд и подкоманд

✅ **Парсинг опций** - мощный парсер с поддержкой алиасов и значений по умолчанию

✅ **Расширяемость** - легко добавлять новые команды и опции

## Зависимости

- **AuroraScienceHub.Integrations.Noaa** - основная библиотека клиентов NOAA
- **Microsoft.Extensions.Hosting** - для DI и конфигурации
- **System.CommandLine** - для создания CLI интерфейса

## Формат вывода

Все данные выводятся в виде текстовых таблиц с выравниванием колонок:

```
ACE Magnetometer Data (всего записей: 120)
------------------------------------------------------------------------------------------------------------------------
DateTime             Status   Bx (nT)    By (nT)    Bz (nT)    Bt (nT)    Lat        Lon
------------------------------------------------------------------------------------------------------------------------
2026-02-02 10:00:00  0        2.30       -1.45      3.20       4.12       -0.50      1.20
2026-02-02 10:01:00  0        2.35       -1.40      3.25       4.15       -0.48      1.22
...
```

## Обработка ошибок

Приложение корректно обрабатывает:
- Ошибки сети (HttpRequestException)
- Ошибки конфигурации
- Неверные параметры команд
- Общие исключения

Все ошибки выводятся красным цветом с понятными сообщениями.

## Лицензия

См. файл LICENSE в корне репозитория.



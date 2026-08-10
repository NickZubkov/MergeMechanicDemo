# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Состояние проекта

Unity-проект `MergeMechanicDemo` на **Unity 2022.3.62f3** (LTS), Built-in Render Pipeline. Это гринфилд: своего кода в `Assets/` нет (лежит только импортированный редактором `Assets/TextMesh Pro/` — TMP Essentials), `ProjectSettings/EditorBuildSettings.asset` не содержит ни одной сцены, в `Library/ScriptAssemblies` собраны лишь сборки пакетов (TextMeshPro, uGUI, TestRunner, MCPForUnity). Архитектуры кода ещё нет — она создаётся с нуля.

## Окружение

- Редактор: `C:\Program Files\Unity\Hub\Editor\2022.3.62f3\Editor\Unity.exe` (в Hub стоят и другие версии — использовать строго ту, что в `ProjectSettings/ProjectVersion.txt`).
- Оболочка — PowerShell 5.1. Пути с пробелами вызывать через `&`.

## Команды

Основной путь работы — **MCP for Unity при открытом редакторе** (см. «Автоматизация редактора»): компиляция, консоль, тесты и Play Mode доступны без перезапуска Unity. CLI ниже — запасной вариант на случай, когда редактор закрыт или MCP-сервер не отвечает.

Батч-режим требует, чтобы **редактор с этим проектом был закрыт** — иначе Unity падает с «Multiple Unity instances cannot open the same project»; при живом MCP-подключении редактор как раз открыт, так что эти два пути взаимоисключающие. Ключ `-logFile -` выводит лог в stdout, без него вывода не будет вообще.

Импорт ассетов и компиляция скриптов (проверка, что код собирается):

```powershell
& "C:\Program Files\Unity\Hub\Editor\2022.3.62f3\Editor\Unity.exe" -quit -batchmode -nographics -projectPath "D:\UnityProjects\MergeMechanicDemo" -logFile -
```

Тесты (test-framework уже в проекте, см. ниже):

```powershell
# все EditMode-тесты
& "C:\Program Files\Unity\Hub\Editor\2022.3.62f3\Editor\Unity.exe" -runTests -batchmode -nographics -projectPath "D:\UnityProjects\MergeMechanicDemo" -testPlatform EditMode -testResults "$env:TEMP\results.xml" -logFile -

# один тест / один класс — фильтр по полному имени
... -testPlatform EditMode -testFilter "MergeMechanic.Tests.BoardTests.Merge_TwoEqualTiles_ProducesNextRank"
```

`-testPlatform PlayMode` — для PlayMode-тестов. Код возврата: 0 — все тесты прошли, 2 — есть падения, 3 — запуск не удался; детали смотреть в XML из `-testResults`.

Сборки собственной команды нет: чтобы билдить из CLI, придётся написать editor-скрипт с `BuildPipeline.BuildPlayer` и вызывать его через `-executeMethod`.

## Набор пакетов урезан — что придётся доставить

Помимо встроенных модулей (`com.unity.modules.*`) в `Packages/manifest.json` есть `com.unity.ugui` (`Canvas`, `Image`, `Button`), `com.unity.textmeshpro` 3.0.7 (`TMP_Text`), `com.unity.device-simulator.devices` и `com.coplaydev.unity-mcp` (git-зависимость, см. ниже).

Важно: `com.coplaydev.unity-mcp` тянет за собой **`com.unity.test-framework` 1.1.33** (и `com.unity.ext.nunit` 1.0.6, `com.unity.nuget.newtonsoft-json` 3.0.2) — они видны в `packages-lock.json` с `depth: 1..2`, но не в `manifest.json`. То есть NUnit, `[Test]` и `-runTests` работают из коробки, отдельно добавлять ничего не нужно. Обратная сторона: тестовый фреймворк держится только на транзитивной зависимости MCP-пакета — если он когда-нибудь будет удалён, тесты отвалятся, и `com.unity.test-framework` придётся вписать в `manifest.json` явно.

Реально отсутствуют:

| Нужно для | Пакет | Последствие отсутствия |
|---|---|---|
| Генерация `.csproj`/`.sln`, IntelliSense | `com.unity.ide.visualstudio` или `com.unity.ide.rider` | файлы проектов не генерируются |
| Новый Input System | `com.unity.inputsystem` | доступен только legacy `Input.*` |

`Assets/**/*.csproj` и `*.sln` в `.gitignore`, так что их отсутствие в репозитории — норма, а не признак поломки.

## Настройки, влияющие на код

- `activeInputHandler: 0` — legacy Input Manager.
- Color space — Gamma; API Compatibility Level — .NET Standard 2.1.
- Asset Serialization: Force Text — все `.unity`/`.prefab`/`.asset` являются YAML и читаемо диффаются.
- Enter Play Mode Options выключены (домены перезагружаются) — статические поля обнуляются при входе в Play Mode штатно.

## Ассеты и Git

- У каждого файла в `Assets/` есть парный `.meta` с GUID. Создавая, перемещая или удаляя ассеты в обход редактора, всегда обрабатывай `.meta` вместе с файлом — иначе рвутся ссылки в сценах и префабах. Скрипты, созданные текстовыми инструментами, получат `.meta` при следующем импорте редактором; сгенерированный `.meta` нужно коммитить.
- `.gitattributes` задаёт `eol=lf` для всего текста; новые скрипты Unity тоже создаёт с LF.
- `Library/`, `Temp/`, `Logs/`, `UserSettings/` игнорируются — не коммитить и не чинить их содержимое руками.
- Незакоммиченные изменения в `Packages/manifest.json`, `packages-lock.json`, `ProjectSettings/ProjectSettings.asset` и новый `PackageManagerSettings.asset` — след первого открытия проекта в редакторе, а не чья-то незавершённая работа.

## Автоматизация редактора (MCP for Unity)

В проект добавлен пакет `com.coplaydev.unity-mcp` (git-зависимость с ветки `main` — версия не запинена, при переразрешении может подтянуться новый коммит). Со стороны Claude Code сервер **подключён и работает** — это основной способ управлять редактором. Есть и skill `unity-mcp-skill` с подробными паттернами работы.

### Как он подключён

- Регистрация лежит **не в репозитории**: `.mcp.json` здесь нет, сервер прописан в пользовательском `~/.claude.json` в секции проекта `D:/UnityProjects/MergeMechanicDemo` → `mcpServers.UnityMCP`. То есть подключение локальное: на другой машине или у другого разработчика его придётся поднимать заново через Window → MCP for Unity в редакторе.
- Транспорт stdio, сервер ставится через uv: `uvx --from mcpforunityserver==10.1.2 mcp-for-unity` (версия сервера запинена, версия Unity-пакета — нет).
- Проверить состояние подключения — команда `/mcp`.
- Инстанс редактора — `MergeMechanicDemo@d4bac111`. Пока открыт один редактор, ничего указывать не нужно; если открыто несколько, сервер потребует `set_active_instance` (или параметр `unity_instance` на конкретном вызове).

### Что помнить при работе

- **Ресурсы адресуются по URI**, а не по имени: `mcpforunity://editor/state`, `mcpforunity://project/info`, `mcpforunity://project/tags`, `mcpforunity://project/layers`, `mcpforunity://tests`, `mcpforunity://menu-items`, `mcpforunity://editor/selection`. Полезная нагрузка лежит под `data`, например `data.advice.ready_for_tools` — его стоит проверять перед серией мутаций (там же `blocking_reasons` и флаги компиляции/домейн-релоуда).
- **По умолчанию активна только группа `core`** (25 инструментов: сцены, GameObject'ы, ассеты, префабы, скрипты, консоль, `batch_execute`). Остальные группы выключены и включаются на сессию через `manage_tools(action='activate', group=...)`:
  - `testing` — `run_tests`, `get_test_job` (нужна перед любым прогоном тестов через MCP);
  - `docs` — `unity_docs`, `unity_reflect`;
  - `scripting_ext` — `execute_code`, `manage_scriptable_object`;
  - плюс `ui`, `vfx`, `animation`, `profiling`, `asset_gen`, `probuilder`.
- `batch_execute` ограничен 25 командами за вызов (`data.settings.batch_execute_max_commands`).
- Скрипты, созданные/изменённые **мимо** MCP (обычными Write/Edit), редактор не увидит сам — после правок вызывать `refresh_unity`, затем `read_console` для ошибок компиляции. Для правок через MCP есть `create_script` / `apply_text_edits` / `script_apply_edits` / `validate_script`.
- Play Mode гоняется через `manage_editor` (`play`/`pause`/`stop`), тесты — через `run_tests` (test-framework в проекте уже есть, см. выше); в `mcpforunity://tests` сейчас видны только заглушки сборок `MergeMechanicDemo` для EditMode/PlayMode, потому что собственных тестов ещё не написано.
- Пока MCP-сессия жива, редактор открыт — значит CLI-команды из раздела «Команды» в этот момент запускать нельзя.

# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Состояние проекта

Unity-проект `MergeMechanicDemo` на **Unity 2022.3.62f3** (LTS), Built-in Render Pipeline. Кор-механика мерджа реализована и живёт в `Assets/_Project/`; единственная сцена `Assets/_Project/Scenes/Game.unity` стоит в `ProjectSettings/EditorBuildSettings.asset` под индексом 0. Сторонние ассеты — `TextMesh Pro/` (TMP Essentials) и `Plugins/Zenject/` (Extenject, DI-контейнер со своими asmdef: `Zenject`, `Zenject-Editor`, `Zenject-TestFramework` и тестовые).

## Структура своего кода

Корневой namespace `MergeMechanic`, asmdef нет — всё компилируется в `Assembly-CSharp`.

```
Assets/_Project/
├─ Configs/       Items, Spawners, Junk, BoardConfig, SpawnerTimerConfig, GameConfig (.asset)
├─ Prefabs/       Cell, BoardObjectView
├─ Scenes/        Game.unity
└─ Scripts/
   ├─ Configs/       ScriptableObject-данные, проверки данных в OnValidate
   ├─ Domain/        Board, BoardObject, MergeRule, InteractionResult,
   │                 IRandomProvider, UnityRandomProvider, WeightedPicker
   ├─ Services/      IGameBoard/GameBoardService, ISpawnerTimer/SpawnerTimerService, TimerState
   ├─ Signals/       BoardObjectSpawned / BoardObjectMoved / BoardObjectsMerged
   ├─ Presentation/  IBoardLayout + IBoardBuilder/BoardView, BoardObjectView (+Factory),
   │                 IBoardObjectViews/BoardPresenter, DragInputController,
   │                 SpawnerTimerButtonView, DefaultSprite
   ├─ Installers/    GameInstaller
   └─ Editor/        BuildScript
```

Инварианты, на которых держится вся конструкция, — ломать их правкой «по месту» нельзя:

- **Спавнер — не отдельная сущность, а уровень цепочки с непустой таблицей спавна.** Поэтому мердж, перемещение и потолок цепочки работают для предметов и спавнеров одним кодом.
- **Тап по спавнеру делает два независимых броска: основной по `SpawnTable` и дополнительный по `ExtraSpawn`.** Порядок между ними — часть правил, а не деталь реализации: основной идёт первым, поэтому последнюю свободную клетку получает обычный предмет. Дополнительный бросок при полном поле молча пропадает, результат тапа остаётся `Spawned` — отдельного значения в `InteractionResult` под него нет.
- **Немерджибельность цепочки `Junk` держится на её длине, а не на флаге.** `MergeRule` требует `!target.IsMaxLevel`, а у цепочки из одного уровня единственный уровень сразу является потолком; с чужими предметами она не сходится по `source.Chain == target.Chain`. Дописать в `Junk` второй уровень — значит разрешить мусору мерджиться самому с собой.
- **`IGameBoard.TryInteract(from, to)` — единственная точка входа для любого ввода.** Тап это `from == to`; отдельного распознавания тапа нет. Input-слой правил не знает.
- **Состояние поля меняется только в `GameBoardService`.** Сцена — отражение модели: `BoardPresenter` подписан на три сигнала и создаёт, двигает и удаляет вьюхи.
- `GameBoardService` строит доску в конструкторе и намеренно **не** `IInitializable` — иначе корректность зависела бы от порядка `Initialize`. Стартовая раскладка сигналов не публикует: презентер читает `Objects` при своей инициализации.
- Перетаскиваемая вьюха всегда возвращается на позицию из модели — и при `Moved`, и при `Rejected`; при `Merged` её уже уничтожил презентер.
- **Целевая клетка хода берётся из позиции вьюхи, а не курсора** (`TryWorldToCell(view.transform.position)`). Перетаскиваемый предмет намеренно сохраняет захват `_grabOffset` и потому смещён относительно курсора — разрешать ход по `Input.mousePosition` значит расходиться с тем, что видит игрок.
- **Ветки в `DragInputController.Tick` независимы, а не `else if`.** Нажатие и отпускание могут прийти в одном кадре: связанные ветки теряли тап и оставляли драг залипшим.
- **`Board.Place`/`Remove` бросают исключения** на выходе за поле и на записи в занятую клетку и сами ведут счётчик занятых клеток, на котором держится `HasFreeCell`. Обходить их прямой записью в `_cells` нельзя — счётчик разъедется.
- **`BoardView.FitCamera` резервирует снизу полосу под UI и пересчитывается при смене размера экрана.** Доля берётся из `RectTransform` кнопки (`_bottomUiArea`): у Screen Space Overlay канваса `GetWorldCorners` возвращает экранные пиксели, поэтому подгонять константы под разрешение не нужно. Поле вписывается не во весь экран, а в его часть над кнопкой, и камера опускается на `orthographicSize * reserved` — вниз, потому что содержимое от этого поднимается вверх. Убрать резерв, не перенеся кнопку с поля, значит вернуть наложение кнопки на нижний ряд.

## Текущая работа

Проект — тестовое задание Unity Developer для Team Planet: кор-механика мерджа из Gossip Harbor (поле 7×9, спавнеры, цепочки уровней, кнопка с таймером). Задания 1 и 2 сданы WebGL-билдом и описанием архитектуры; Задание 3 — доп. фича плюс APK.

Все документы лежат **вне репозитория** — папка `docs/` в `.gitignore` (строка `/[Dd]ocs/`), так что через git их не видно:

- ТЗ: `Docs/Тестовое Задание Unity Developer (Team Planet) 2026 Август.md`
- Задания 1–2: спека `docs/superpowers/specs/2026-08-10-merge-mechanic-design.md`, план на 14 задач `docs/superpowers/plans/2026-08-10-merge-mechanic.md`
- Задание 3: спека `docs/superpowers/specs/2026-08-11-extra-spawn-design.md`, план на 8 задач `docs/superpowers/plans/2026-08-11-extra-spawn.md`

**Начиная работу над механикой, прочитать сначала спеку, затем план.** Прогресс отмечен чекбоксами `- [ ]` в плане; сверять с историей коммитов.

### Что сделано (2026-08-10)

Задачи 1–13: UniTask в зависимостях, конфиги, домен, сервисы, сигналы, слой представления, инсталлер, ассеты, префабы, сцена и прогон в Play Mode. Механика проверена по чек-листу ТЗ прямо через контейнер: тап по спавнеру раздаёт предметы по весам (90/10 у S-1, 50/50 у S-2), мердж поднимает уровень, потолок цепочки и чужая цепочка дают отказ, таймер при полном поле уходит в `WaitingForSpace` и доставляет спавнер, как только клетка освободится. Расхождений модели и сцены нет.

**WebGL-билд собран** (задача 14 закрыта): `Succeeded, 38 459 354 bytes`, платформа уже стояла на WebGL, сборка ~4 минуты. Проверен в браузере — клики, драги и мердж работают. По замечанию заказчика работы исправлено наложение кнопки спавнера на нижний ряд поля (см. инвариант про `FitCamera` выше); проверено на трёх аспектах канваса, включая узкий вертикальный и широкий низкий.

Затем проведено код-ревью всего `Scripts/` и рефакторинг по его итогам — отчёт с находками, обоснованиями и решениями: `docs/reviews/2026-08-10-code-review.md` (тоже вне репозитория). Применены все находки, кроме `LOW-1`–`LOW-3` (косметика) и раздела «Риски для будущего APK» — оба тач-риска там описаны и ждут APK-задания. Правки прогнаны в Play Mode через контейнер: спавн, мердж, перемещение, полный цикл таймера, сверка модели со сценой — консоль чистая.

### Что сделано (2026-08-11, Задание 3)

Спавнер спавнит дополнительно с шансом 30% предмет, который не мерджится. Реализовано вторым, независимым броском в `TrySpawnFrom`: шанс и таблица лежат в `ChainLevel.ExtraSpawn`, то есть настраиваются отдельно для каждого уровня спавнера. Немерджибельность выражена данными — цепочка `Junk` из одного уровня, `MergeRule` не тронут.

Проверено в Play Mode через контейнер: `junk+junk`, `junk+item` и `item+junk` дают `Rejected`, `item+item` по-прежнему `Merged`, мусор перетаскивается; при единственной свободной клетке её получает основной бросок, тап остаётся `Spawned`, поле закрывается. Статистика на временно увеличенной доске 20×20: S-1 — 58 мусора из 200 тапов (29.0%) при 179/21 обычных, S-2 — 51 из 150 (34.0%) при 69/81. Заодно закрыт тач-риск `IsPointerOverGameObject()` без `fingerId`.

### Что осталось

Оба плана выполнены целиком. Остаются вещи, сознательно вынесенные за скоуп (тесты, сохранение состояния, анимации, звук), способ убирать мусор с поля (по ТЗ его нет — поле со временем забивается) и один незакрытый пункт к APK:

- Проверить компоновку на реальном вертикальном экране: резерв под UI считается от кнопки и должен отработать сам, но глазами это ещё не смотрели. APK собирает заказчик работы, `BuildScript` под Android намеренно не расширяли.

Договорённости, которые нельзя пересматривать молча:

- Стек: Zenject + UniTask 2.5.11. **UniRx не используем** — архивирован, а `SignalBus` уже покрывает события.
- Слой представления — world-space 2D в плоскости XY на `SpriteRenderer`; из UI только кнопка спавнера. Ввод — drag & drop на legacy `Input.*`.
- В биндингах Zenject **`BindInterfacesTo` по умолчанию**. `BindInterfacesAndSelfTo` не применять: потребность инжектить конкретный класс означает, что типу не хватает интерфейса. Исключения — `BindInstance` для ScriptableObject-данных без поведения и `BindFactory` для фабрик.
- Делаем **строго по ТЗ**. Тесты, сохранение состояния, анимации и разрезка по asmdef сознательно вынесены за скоуп — они перечислены в разделах «Вне скоупа» спеки и плана.

## Code style

- **Порядок членов в типе строго один: поля → свойства → конструктор → методы.** Разрывать группы нельзя: свойство после конструктора или поле между методами — ошибка стиля, даже если так «ближе по смыслу». Вложенные типы (`BoardConfig.StartingObject`, `BoardObjectView.Factory`) идут последними, после методов.
- **Комментариев в коде нет** — ни `///`-сводок, ни пояснений в теле. Решение заказчика работы. Обоснования архитектуры живут в этом файле и в спеке; добавляя нетривиальный код, объяснять его там, а не комментарием.

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

WebGL-сборка — `MergeMechanic.Editor.BuildScript.BuildWebGL` (`Assets/_Project/Scripts/Editor/BuildScript.cs`). Из открытого редактора вызывается пунктом меню `MergeMechanic → Build WebGL`, из CLI при закрытом редакторе:

```powershell
& "C:\Program Files\Unity\Hub\Editor\2022.3.62f3\Editor\Unity.exe" -quit -batchmode -nographics -projectPath "D:\UnityProjects\MergeMechanicDemo" -executeMethod MergeMechanic.Editor.BuildScript.BuildWebGL -logFile -
```

Результат кладётся в `Builds/WebGL` — папка в `.gitignore` и в репозиторий не попадает. Первая сборка занимает 10–20 минут, ей предшествует переключение платформы с переимпортом ассетов.

## Набор пакетов урезан — что придётся доставить

Помимо встроенных модулей (`com.unity.modules.*`) в `Packages/manifest.json` есть `com.unity.ugui` (`Canvas`, `Image`, `Button`), `com.unity.textmeshpro` 3.0.7 (`TMP_Text`), `com.cysharp.unitask` (git-зависимость, **запинена тегом 2.5.11** — незапиненную при переразрешении унесёт на произвольный коммит), `com.unity.device-simulator.devices` и `com.coplaydev.unity-mcp` (git-зависимость с незапиненной ветки, см. ниже).

Важно: `com.coplaydev.unity-mcp` тянет за собой **`com.unity.test-framework` 1.1.33** (и `com.unity.ext.nunit` 1.0.6, `com.unity.nuget.newtonsoft-json` 3.0.2) — они видны в `packages-lock.json` с `depth: 1..2`, но не в `manifest.json`. То есть NUnit, `[Test]` и `-runTests` работают из коробки, отдельно добавлять ничего не нужно. Обратная сторона: тестовый фреймворк держится только на транзитивной зависимости MCP-пакета — если он когда-нибудь будет удалён, тесты отвалятся, и `com.unity.test-framework` придётся вписать в `manifest.json` явно.

Для IDE установлен `com.unity.ide.rider` 3.0.40 — см. раздел «Интеграция с Rider».

Реально отсутствуют:

| Нужно для | Пакет | Последствие отсутствия |
|---|---|---|
| Новый Input System | `com.unity.inputsystem` | доступен только legacy `Input.*` |

`Assets/**/*.csproj` и `*.sln` в `.gitignore`, так что их отсутствие в репозитории — норма, а не признак поломки.

## Интеграция с Rider

- Пакет `com.unity.ide.rider` 3.0.40 установлен; активный редактор — `Packages.Rider.Editor.RiderScriptEditor`, External Script Editor указывает на `C:\Program Files\JetBrains\JetBrains Rider 2022.3.1\bin\rider64.exe` (на машине есть также Rider 2025.2.3 и Toolbox-сборка 2024.2 — переключается в Edit → Preferences → External Tools).
- В корне генерируются `MergeMechanicDemo.sln` и по `.csproj` на сборку (`Assembly-CSharp` + Zenject'овские). Все они в `.gitignore` — это артефакты, а не часть репозитория.
- Пересоздать файлы проектов без открытия IDE: Edit → Preferences → External Tools → Regenerate project files, либо через MCP `execute_code`:

  ```csharp
  Unity.CodeEditor.CodeEditor.CurrentEditor.SyncAll();
  ```

- По умолчанию `.csproj` создаются только для сборок из `Assets/`; чтобы в Rider были видны исходники пакетов (MCPForUnity, TMP), нужно отметить соответствующие галочки «Generate .csproj files for…» там же в External Tools.

## Настройки, влияющие на код

- `activeInputHandler: 0` — legacy Input Manager.
- Color space — Gamma; API Compatibility Level — .NET Standard 2.1.
- Asset Serialization: Force Text — все `.unity`/`.prefab`/`.asset` являются YAML и читаемо диффаются.
- Enter Play Mode Options выключены (домены перезагружаются) — статические поля обнуляются при входе в Play Mode штатно.

## Ассеты и Git

- У каждого файла в `Assets/` есть парный `.meta` с GUID. Создавая, перемещая или удаляя ассеты в обход редактора, всегда обрабатывай `.meta` вместе с файлом — иначе рвутся ссылки в сценах и префабах. Скрипты, созданные текстовыми инструментами, получат `.meta` при следующем импорте редактором; сгенерированный `.meta` нужно коммитить.
- `.gitattributes` задаёт `eol=lf` для всего текста; новые скрипты Unity тоже создаёт с LF.
- `Library/`, `Temp/`, `Logs/`, `UserSettings/` игнорируются — не коммитить и не чинить их содержимое руками.
- Редактор сам переписывает `Packages/packages-lock.json`, `ProjectSettings/ProjectSettings.asset` и `PackageManagerSettings.asset` при открытии проекта и переключении платформы — такие изменения в `git status` обычно след работы редактора, а не чья-то незавершённая правка. Смотреть диф перед тем, как откатывать или коммитить.

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
- **Пока окно редактора не в фокусе, игровой цикл в Play Mode не крутится вообще** — `Time.frameCount` остаётся 1, `Time.time` — 0. Отложенный `Object.Destroy` не выполняется, UniTask не продвигается, и это выглядит как дефект: вьюхи съеденных объектов висят на сцене, таймер стоит на стартовом значении. Первым делом в проверочном скрипте выставлять `Application.runInBackground = true` и сверять состояние только после того, как `Time.frameCount` растёт.
- Все поля конфигов, вьюх и инсталлера приватные (`[SerializeField]`), поэтому ассеты, префабы и сцена собирались через `execute_code` (группа `scripting_ext`) с `SerializedObject` / `FindProperty`. Типы своего кода в этом контексте достаются через `System.Type.GetType("MergeMechanic.…, Assembly-CSharp")`, TMP — через `…, Unity.TextMeshPro`. Тем же способом правятся данные в существующих ассетах.
- Пока MCP-сессия жива, редактор открыт — значит CLI-команды из раздела «Команды» в этот момент запускать нельзя.

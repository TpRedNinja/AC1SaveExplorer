# AC1 Save Object Explorer

A dark-themed WPF tool for digging through Assassin's Creed 1 save files. It reads the object-graph JSON dumps produced by [AC1SaveTool](https://github.com/bloxtbc/AC1SaveTool), sorts every object into **Save File**, **Mission**, or **Uncategorized**, and lets you build up a human-readable hash dictionary as you identify more of them (cross-referencing against ACViewer, IDA, or wherever you're pulling names from).

## Why this exists

AC1's save format identifies almost everything by hash — `classID`s for objects, property IDs for their fields. AC1SaveTool can dump a save into structured JSON, but the raw dump is a wall of `prop_882350721`-style unresolved IDs. This tool turns that into something browsable: named objects where known, a searchable list, and inline editing so unresolved hashes get a name that sticks around.

## Features

- **Load any AC1SaveTool-style dump** (`.json`) and browse it as a categorized, searchable, expandable list.
- **Save File / Mission / Uncategorized / Candidates tabs** — Candidates automatically surfaces objects that share the exact property signature of a confirmed mission object (`MissionStatus` / `IsCompleted` / `Unknown`) but haven't been named yet, so you can work through likely-mission objects one by one instead of hunting blind.
- **Inline naming** — type a name and pick a category for any object; it applies to every object sharing that `classID` and persists automatically.
- **Growable hash dictionaries** — a property-hash dictionary (seeded from AC1SaveTool's own `hashes.json`) and an object/class-name dictionary, both editable, exportable, and auto-saved to `%AppData%\AC1SaveExplorer\` between sessions.
- **Copy hex** button on every object row, for pasting a `classID`'s hex hash straight into ACViewer or IDA.
- Dark UI throughout, including native dropdowns/popups.

## Getting started

1. Open `AC1SaveExplorer.sln` in Visual Studio (requires the **.NET desktop development** workload and the **.NET 8 SDK**).
2. Build and run (F5).
3. Click **Load dump.json** and point it at a save dump from AC1SaveTool.
4. Optionally load a property-hash or object-name dictionary you've built up previously via the buttons in the sidebar.

## Credits

- [AC1SaveTool](https://github.com/bloxtbc/AC1SaveTool) (MIT) — the save-dumping tool this app's JSON format and default property-hash dictionary come from.
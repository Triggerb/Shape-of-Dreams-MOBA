# Known facts

Last verified locally: 2026-07-20.

## Installed game

- Steam app ID: `2444750`.
- Install path: `C:\Program Files (x86)\Steam\steamapps\common\Shape of Dreams`.
- Installed version: `r.1.3.1.3_s` (from `version.txt`).
- Engine: Unity, 64-bit Windows player.
- Scripting backend: managed Mono/.NET, evidenced by `MonoBleedingEdge` and the
  managed DLL set under `Shape of Dreams_Data/Managed`.
- Important managed assemblies include `Assembly-CSharp.dll`, `Dew.Core.dll`,
  `Dew.Contents.dll`, `Dew.External.dll`, and `Dew.UI.dll`.
- Harmony (`0Harmony.dll`) is bundled and exposed to mods.
- Several game assemblies include PDB files. This should make ordinary managed
  inspection much more useful than Ghidra for the current build.

## Official mod surface

- The game root contains `Mods/Documentation.txt`, linking to the official docs.
- `Mods/ModTemplate` is a compilable .NET Standard 2.1 C# project.
- Assemblies listed in a mod's `about/metadata.json` are loaded by the game.
- Classes deriving from `ModBehaviour` are attached to a per-mod Unity container
  GameObject and are the primary lifecycle entry points.
- Mods can use the supplied Harmony instance; patches must be removed on unload
  to support live reload.
- Developer Mode is available in gameplay settings. F1 controls console mode and
  the backtick key opens/closes the console when enabled.
- The title screen includes a mod manager. Developer Mode also exposes it from the
  console. Loading/unloading mid-run can be unsafe.
- Workshop upload is available through the mod manager in Developer Mode. New and
  updated uploads default to hidden visibility.
- Gameplay-changing server mods must identify themselves as gameplay-altering.

## Declarative data

- `RawData/!ModResources/overrides` currently contains 1,854 JSON files describing
  parameters that mods can override.
- JSON overrides are automatically treated as gameplay-altering and synchronized
  from host to clients, but can only change exposed static parameters.
- The catalogue contains trees for all eight shipped Travelers: Aurena, Bismuth,
  Husk, Lacerta, Mist, Nachia, Vesper, and Yubar.
- Traveler data exposes base/scaling stats, movement/rotation parameters, skill
  configs, casting geometry, cooldowns, charges, damage factors, effects, attacks,
  and constellation-related values.
- Husk is the first likely behavioral reference for Master Wu because it already
  has a sword basic attack, a flash-step movement skill, melee/dash actions, and a
  stance/status-effect structure.
- `RawData` is licensed for community resources, guides, personal reference, and
  non-commercial fan content. It must not be reused commercially or in ways that
  infringe third-party rights. The raw game data will not be vendored here.

## What is not known yet

- Whether the supported API can register a genuinely new Traveler without a
  Harmony patch, or whether it mainly supports modifying existing resources.
- The canonical registration path for Traveler selection, localization, portraits,
  prefabs, network spawning, progression, save data, and constellations.
- Whether a new Traveler can be backed by an external AssetBundle and which exact
  Unity editor version/build settings are required.
- The minimum set of network identities and content keys needed for co-op.
- Whether “shards” in the desired final guide maps to the game's current Essence,
  Memory, constellation, or another content concept. Terminology will be resolved
  against the current API and game UI.

## Current hypothesis

The safest path is a vertical slice: first prove local mod loading, then inject or
clone one Traveler registration while reusing runtime game assets, then replace
one component at a time. Native disassembly should be unnecessary unless a future
build moves to IL2CPP or a native-only boundary becomes relevant.

## Traveler registration trace

- The lobby iterates `Dew.allHeroes`, resolves each entry through `DewResources`,
  filters it through the active `DewGameContentSettings.availableHeroes`, and
  creates a selection item from the resulting `Hero` prefab.
- Runtime resources are backed by `DewResourceDatabase` mappings for type/GUID,
  name/GUID, network asset ID/GUID, dependencies, and light/heavy asset variants.
- Selection and progression additionally assume a matching profile hero entry,
  mastery data, localization keys, loadout skills, constellation data, and valid
  Mirror network prefab identity.
- Consequently, a separate ninth Traveler requires a registered serialized prefab
  and coordinated resource/network/save entries. A C# subclass plus UI insertion
  alone would produce an incomplete or unsafe character.
- The `0.2.0` greybox therefore converts Shell reversibly while we establish the
  supported external prefab/AssetBundle route.

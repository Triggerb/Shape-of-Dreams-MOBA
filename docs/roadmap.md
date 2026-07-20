# Roadmap

## Phase 0 — Reproducible workspace

- Initialize Git once Git for Windows is available.
- Record game version and hash only the referenced managed assemblies.
- Copy/customize the official template into `src/MasterWu` without copying game
  assemblies into the repository.
- Add build/deploy scripts whose target is explicit and safely validated.

Exit criterion: a “hello world” Master Wu mod builds, loads, logs, unloads, and
reloads without modifying the base installation.

## Phase 1 — Registration spike

- Read official example mod source and API reference.
- Inspect `ModBehaviour`, content registries, Traveler selection UI, localization,
  network prefab registration, and save/progression identifiers in managed code.
- Write a minimal runtime probe that logs relevant registry contents.
- Test whether cloning an existing Traveler resource produces a ninth selection.

Exit criterion: a uniquely named placeholder Traveler can be selected and spawned
in a solo test room.

## Phase 2 — Master Wu greybox

Use game-native mechanics and placeholder visuals:

- melee basic attack;
- targeted or short-range blink/slash inspired by precise repositioning;
- defensive channel or timed mitigation using an existing supported status model;
- temporary speed/attack cadence stance;
- the game's existing movement/dodge slot as required by its control scheme.

Start by composing existing behavior, likely from Husk and other shipped skills.
Do not force a League-style passive or slot layout if it conflicts with Shape of
Dreams' systems.

Exit criterion: a full solo run can begin and complete without critical errors.

## Phase 3 — Original assets

- Establish the exact Unity version and AssetBundle loading path.
- Produce an original silhouette, model, rig, animations, portrait, icons, VFX,
  SFX, names, and descriptions—or use assets with compatible licenses.
- Maintain an asset provenance/license manifest.

Exit criterion: no Riot assets, names, audio, or extracted game assets are shipped
inside the mod package.

## Phase 4 — Progression and content

- Add/associate Memories and Essences.
- Resolve the user's “shards” terminology against current game concepts.
- Add constellation/progression integration only where the existing structure
  supports it cleanly.
- Test save migration and uninstall behavior.

## Phase 5 — Multiplayer and release

- Verify host/client content synchronization and network prefab identity.
- Test mixed mod states only as allowed by the official system.
- Add compatibility/version checks, diagnostics, and a clean unload path.
- Publish a hidden Workshop build, then review packaging and terms before public
  release.

## Documentation end goal

Produce a versioned guide with four independent recipes:

1. add a playable Traveler;
2. add a Memory/skill;
3. add an Essence/modifier;
4. add the content category meant by “shard.”

Each recipe must distinguish official API, JSON override, Harmony patch, and asset
pipeline steps, and list update-sensitive assumptions.

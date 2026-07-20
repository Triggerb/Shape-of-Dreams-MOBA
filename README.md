# Shape of Dreams — Master Wu Modding Project

Research and implementation workspace for adding an original playable Traveler,
**Master Wu**, to *Shape of Dreams*, then turning the work into a reproducible
guide for adding Travelers, Memories (skills), Essences (skill modifiers), and
related content.

Master Wu is inspired by the fast melee, timing, and repositioning practice the
project owner enjoys in MOBA games. Names, writing, visuals, audio, and shipped
assets must be original or appropriately licensed; this repository will not
redistribute Riot Games or extracted Shape of Dreams assets.

## Current finding

The project no longer needs to begin with native reverse engineering. Game build
`r.1.3.1.3_s` has official mod support, a built-in mod manager, Steam Workshop
integration, a bundled C# template, Harmony, managed Mono assemblies with PDBs,
and an official JSON override catalogue. See [Known facts](docs/known-facts.md).

## First playable milestone

The first milestone is intentionally small:

1. Load a local `Master Wu` mod through the official mod manager.
2. Make Master Wu selectable as a distinct Traveler, initially reusing a legal
   placeholder/model already available to the game at runtime.
3. Give him a coherent melee kit using existing game-native mechanics.
4. Replace placeholders with original assets later.
5. Document every stable step and turn the result into a general authoring guide.

Exact parity with any League of Legends champion is not a requirement. Preserving
Shape of Dreams' slot model, networking model, balance hooks, and overall gameplay
structure takes priority.

Version `0.2.0` is the first playable greybox. While the separate-Traveler
registration path is under research, enabling the mod converts **Shell** into
Master Wu with original names and a rebalanced fast-melee kit. Select Shell in the
lobby to test it.

## Repository map

- `docs/known-facts.md` — verified local and online facts
- `docs/tooling.md` — what is installed and what may be needed
- `docs/roadmap.md` — staged research and implementation plan
- `docs/research-log.md` — dated observations and experiments
- `src/` — mod source code (added after the API/registration spike)
- `tools/` — repeatable inspection and development helpers

## Safety rules

- Do not edit or commit files inside the Steam installation.
- Test via a copied/deployed mod directory and keep generated output ignored.
- Do not commit game DLLs, PDBs, extracted assets, Steam Workshop content, logs,
  or proprietary third-party assets.
- Expect every game update to invalidate assumptions; record the tested game
  version with each result.
- Mark gameplay-changing server mods appropriately and test multiplayer only with
  informed participants.

## Upstream references

- [Official mod documentation](https://github.com/LizardSmoothie/ShapeOfDreamsModDocs)
- [Official API reference](https://lizardsmoothie.com/sod/moddoc/api/Global.html)
- [Steam Workshop](https://steamcommunity.com/app/2444750/workshop/)
- [Shape of Dreams on Steam](https://store.steampowered.com/app/2444750/Shape_of_Dream/)

## Build and local deployment

```powershell
dotnet build .\src\MasterWu\MasterWu.csproj --configuration Debug
powershell.exe -NoProfile -ExecutionPolicy Bypass -File .\tools\Deploy-LocalMod.ps1
```

Close the game before deployment; the deployment script refuses to overwrite a
loaded mod. Then launch it, open **Mods**, enable **Master Wu**, and use Developer
Mode's log viewer to verify the `[Master Wu]` load/unload messages. After changing
or disabling a gameplay-override mod, restart the game before opening Traveler
loadouts or constellations; live reload can leave stale Addressable/UI references.

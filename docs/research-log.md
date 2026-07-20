# Research log

## 2026-07-20 — Project reset

- Created a clean documentation-first workspace outside the Steam installation.
- Verified installed game version `r.1.3.1.3_s`.
- Confirmed official mod support, local template, mod manager, Workshop workflow,
  Harmony, JSON overrides, managed assemblies, and PDBs.
- Found one locally subscribed Workshop mod (Camera Unlock, item `3623360027`),
  packaged as a managed DLL; it does not include source code.
- Confirmed .NET SDK is available. Installed Git for Windows 2.55.0.3 and
  initialized the repository on branch `main`; Git identity remains unconfigured.
- Chose supported mod APIs plus managed inspection as the primary approach.
- Selected Husk as the initial behavioral reference, not as an asset to redistribute.

### Next investigation

Inspect official example sources and the Dew.Core API to identify the exact
Traveler/content registration surface before writing the first gameplay code.

## 2026-07-20 — First loadable assembly

- Verified ILSpy at the user's default per-user installation path.
- Created initial Git commit `6610c09` after the user configured Git identity.
- Located the developer's SUPER LACERTA Workshop example (item `3618193300`). It
  demonstrates host-synchronized JSON overrides, but its current Steam listing is
  flagged inconsistently as removed/incompatible.
- Scaffolded `src/MasterWu` from the installed official C# template.
- Built the .NET Standard 2.1 assembly with zero warnings and zero errors.
- Added a path-validated local deployment script and deployed version `0.1.0` to
  the game's `Mods/MasterWu` directory.
- The remaining verification is in-game: enable Master Wu and confirm its lifecycle
  log messages appear and no loader error is produced.

## 2026-07-20 — Playable greybox 0.2.0

- User confirmed version 0.1 loads and unloads correctly.
- Verified the repository is synchronized with `origin/main` over SSH.
- Installed `ilspycmd` 10.1.1 in a user-local research tools directory and traced
  the Traveler selection/resource path without adding decompiled code to the repo.
- Established that a ninth Traveler needs prefab, GUID, network asset, content,
  profile, localization, loadout, and progression registration.
- Implemented a reversible Shell-to-Master-Wu conversion as the first playable
  vertical slice.
- Added original names: Focused Rhythm, Wind Step, Vanishing Cut, and Unbound
  Tempo. The greybox deliberately omits a Meditate analogue to respect the game's
  existing slot structure.
- Added nine official JSON override targets (51 validated keys) covering Traveler
  stats, sword cadence, dash, line/circle strike, and ultimate stance.
- Built with zero warnings/errors and deployed version 0.2.0 locally.

## 2026-07-20 — Greybox 0.2.1 hotfix

- User confirmed the conversion is playable and the hero display name changes.
- Fixed boolean override values: the exported examples use title-case values, but
  the runtime JSON deserializer requires lowercase JSON boolean literals.
- Changed skill localization replacement to cover every config index instead of
  only config zero.
- Confirmed one reported missing Addressable GUID exists inside a base-game bundle;
  it is not supplied by this mod. The other was not a literal installed key.
- Added a deployment guard that refuses to overwrite the mod while the game is
  running. Clean restarts are now required around gameplay-override changes to
  avoid stale resource and constellation UI state.

## 2026-07-20 — Addressables hotfix 0.2.2

- Mapped missing key `f35cb58ffe62fd14889b14de623aa3c4` to the base game's
  `St_Q_Laceration` resource inside its serialized resource database.
- Mapped the related identity-key failure to Shell's `St_D_TheKillingFlow`.
- Confirmed that direct JSON variants of `St_*` SkillTrigger resources can be
  registered by name but cannot be reloaded by GUID from the runtime Addressables
  catalog in this build.
- Removed direct overrides for movement, Q, and R SkillTriggers. Underlying ability
  instances, status effects, hero stats, and attack settings remain overridden.
- Custom skill cooldowns/charges are deferred to a runtime C# implementation that
  does not ask Addressables to rebuild SkillTrigger resources.

## 2026-07-20 — Custom combat mechanics 0.3.0

- Confirmed Shape of Dreams internally supports Q/W/E/R plus identity and movement
  slots, although shipped Travelers begin runs with only Q/R/identity/movement.
- Added a server-authoritative combat controller to Shell/Master Wu instances.
- Implemented Double Strike: the fourth primary attack produces a 50%-damage bonus
  basic hit, and that bonus hit counts as stack one of the next cycle.
- Added a prototype Wuju-style pure on-hit component (`8 + 15% AD`) to all primary
  and Double Strike hits. A pressable starting E awaits a safe custom SkillTrigger
  asset; the combat mechanic is active continuously in this version.
- Replaces the starting Q at server spawn with the game's Chain Lightning Memory,
  renamed Vanishing Cut and overridden to deal physical AD-scaling damage across
  up to five targets.
- Reworked the ultimate status toward Unbound Tempo: +65% attack speed, +55%
  movement speed, no shockwave, and no main-skill lock for eight seconds.
- JSON validation passes for five targets and 40 keys; C# builds with zero warnings
  and errors.

## 2026-07-20 - Full starting kit prototype 0.4.0

- Identified `St_R_LightningDance` / `Se_R_LightningDance` as the native skill
  closest to Alpha Strike: the caster becomes invulnerable, disappears, travels
  between nearby targets, damages them, and reappears when the sequence ends.
- Equipped runtime-created skills into the otherwise unused W and E ability slots:
  Tranquility supplies Meditation's channel and Incendiary Rounds supplies Wuju
  Style's activation, duration, cooldown, and UI lifecycle.
- Added server-side Meditation healing, 90% incoming damage reduction, and movement
  cancellation. Added Wuju true damage only while its timed buff is active.
- Disabled Shell's Killing Flow status behavior while retaining its identity slot
  as the visible home for Double Strike. Double Strike remains implemented by the
  Master Wu controller.
- Suppressed Annihilation Stance's enlarged attack visuals, compensated its range
  bonus, and retained only the configured attack-speed and added movement-speed
  effects for Highlander.
- Removed the `Hero_Husk` JSON variant. It was the remaining likely source of lobby
  Addressables reconstruction attempts for base character skill references.

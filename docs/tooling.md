# Tooling and setup

## Already present

- .NET SDK (`dotnet`)
- The game's official mod template and managed references
- Harmony supplied by the game
- Unity Mono runtime

## Installed during project setup

- **Git for Windows 2.55.0.3** — installed through Windows Package Manager. The
  repository is initialized on branch `main`. A first commit still requires the
  user's preferred `user.name` and `user.email`; the project does not invent a
  Git identity.

## Missing on this machine

### Required for the next research step

- **A .NET decompiler/assembly browser** — install one of:
  - ILSpy: <https://github.com/icsharpcode/ILSpy/releases>
  - dnSpyEx: <https://github.com/dnSpyEx/dnSpy>

ILSpy is the conservative default for reading assemblies. dnSpyEx is convenient
for debugging and deeper interactive inspection. Do not edit the game's managed
DLLs as the development workflow.

### Likely useful soon

- **Visual Studio 2022 Community** with the “.NET desktop development” workload,
  or Rider. VS Code plus C# tooling can also build the template, but a full IDE
  makes assembly navigation easier.
- **Unity Hub/editor**, but only after the build's exact Unity version and external
  AssetBundle contract are confirmed. Installing a guessed Unity version now may
  waste time.
- **AssetRipper** or **AssetStudio** for research-only inspection of prefab/model/
  material relationships if the official examples and API are insufficient.

### Not needed at the start

- **Ghidra**: the current game is a Mono build with managed assemblies and PDBs.
  Ghidra is useful for native binaries, not the fastest route to C# gameplay code.
- **Il2CppDumper/Cpp2IL**: there is no evidence this installed build uses IL2CPP.
- **BepInEx/MelonLoader**: the game already has its own loader and Harmony. Adding
  a second loader would create needless compatibility and distribution problems.

## User-side preparation for the first test

1. In the game, enable Developer Mode in gameplay settings.
2. Open the title-screen Mods menu and confirm `ModTemplate` is visible (do not
   enable it if it has not been built).
3. Keep Steam Workshop test uploads hidden until local loading and licensing are
   settled.
4. Tell the project log about any subscribed official example mods; source-bearing
   examples are especially valuable.

## Inspection boundary

We may read public APIs and decompile managed assemblies for interoperability and
learning, subject to the game's EULA/modding terms. We will not redistribute game
code/assets or bypass access controls. Before publishing, review the current
modding Terms of Service and licenses for every included asset.

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

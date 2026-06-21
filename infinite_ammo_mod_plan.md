# Infinite Ammo — BepInEx 5 port

A native BepInEx 5 reimplementation of the MelonLoader "Infinite Ammo" mod
(original by DaBlackPantha). It rearms the active weapon station and
countermeasure when they hit zero, and refuels the aircraft at <= 10% fuel.

**Singleplayer only.** The original author warns it may crash or get you banned
on multiplayer servers. Same risk applies here — the cheat logic is unchanged.

---

## Why a rewrite instead of a loader shim

The original is a MelonLoader 0.7.1 mod. NOMM runs BepInEx 5 (mono). The
`BepInEx.MelonLoader.Loader` shim only targets MelonLoader 0.5.7, so it would
reject this DLL. Since Nuclear Option is a Mono game, the cleaner path is a
direct BepInEx plugin — the only real difference between the two is the loader
entry wrapper; the game-side calls are identical.

---

## Build

Requires the .NET SDK (8.x is fine; the project targets `netstandard2.0`, which
needs no .NET Framework targeting pack).

1. Open `InfiniteAmmoBepInEx.csproj` and set `<GamePath>` to your install folder
   (the one containing `NuclearOption.exe`). The default is the usual Steam path.
   You must have launched the game at least once with BepInEx installed so that
   `BepInEx\core\BepInEx.dll` exists.
2. From this folder:
   ```
   dotnet build -c Release
   ```
3. Output: `bin\Release\NuclearOption-InfiniteAmmo.dll`

---

## Install

**Via NOMM (recommended):** use NOMM's "Add Mods from Files" to add
`NuclearOption-InfiniteAmmo.dll`, then enable it.

**Manual:** drop the DLL into `<GamePath>\BepInEx\plugins\` and launch the game.

A config file is generated at `BepInEx\config\com.dablackpantha.infiniteammo.cfg`
after the first launch, with toggles for weapons / countermeasures / fuel, a
`FuelThreshold`, and `VerboseLogging` (off by default).

---

## Faithfulness notes & known caveats

- **Active station only.** Like the original, weapon rearm targets
  `weaponManager.currentWeaponStation`, not every hardpoint. Switching weapons
  refills whichever is currently selected.
- **Field types to sanity-check if it fails to compile.** `WeaponStation.Ammo`
  and `Countermeasure.ammo` are compared with `== 0`. The decompiled IL treated
  them as truthy/zero (numeric counts), which is almost certainly right, but if
  the game has updated since the original (April 2026) and changed a field to a
  property or renamed it, the compiler will point straight at the line.
- **Game updates** can rename/move these members in `Assembly-CSharp.dll`. If a
  future patch breaks it, rebuild against the updated assembly; the fix is
  usually a one-line member rename.
- Deviations from the original, all harmless: the instigator `Unit` is created
  once up front (the original could pass `null`), each subsystem has its own null
  guard, and the empty debug-log stubs are replaced by an opt-in verbose log.

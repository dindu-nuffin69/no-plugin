# Infinite Ammo — Nuclear Option

A **singleplayer-only** [Nuclear Option](https://store.steampowered.com/app/2168680/Nuclear_Option/)
mod that rearms **all** weapon stations and **all** countermeasure dispensers when they run empty,
and refuels the aircraft when fuel runs low. BepInEx 5 / Mono plugin.

Originally created by **DaBlackPantha** as a MelonLoader mod. This is a BepInEx 5 port with fixes
and extended coverage (all stations, all countermeasures, AI-gunner stations, linked guns).

---

## ⚠ Singleplayer only

**Do not use this mod in multiplayer.**

Rearm and refuel writes are server-authoritative in Nuclear Option — the server validates and
applies them. On a real multiplayer server the writes are silently ignored, the mod does nothing
useful, and you risk crashes or bans depending on the server's anti-cheat posture. The original
mod author explicitly warned against MP use.

**This mod is intended for singleplayer missions and sandbox play only.**

---

## What it does

- **Rearms all weapon stations** — not just the one you currently have selected. Linked guns, wing
  pylons, and AI-gunner stations on multi-crew aircraft all refill when they hit zero. This fixes
  the original mod's limitation where unselected stations and AI gunner guns would run dry and
  never recover.
- **Rearms all countermeasure dispensers** — every dispenser on the aircraft refills when empty,
  not just the active one.
- **Refuels at low fuel** — when the actual fuel ratio drops to or below `FuelThreshold` (default
  5 %), the tanks are topped up to `FuelRefillTarget` (default 10 %). Set `FuelRefillTarget` to
  `1.0` for full tanks.
- **KeepConsumablesTopped mode** (off by default) — instead of waiting for stations to hit zero,
  tops them up the moment any ammo or countermeasure count drops below full. Useful for sustained
  fire drills without ever seeing a reload.

---

## Install

### Option A — via NOMM (recommended, easiest)

[**NOMM** (Nuclear Option Mod Manager)](https://github.com/Combat787/NOMM) installs and updates
this mod for you, including BepInEx.

1. Download and run [NOMM](https://github.com/Combat787/NOMM/releases/latest).
2. Point it at your Nuclear Option install if it doesn't auto-detect it.
3. Search for **Infinite Ammo**, click install. Done.

NOMM sources its mod list from the community [**NOMNOM**](https://github.com/KopterBuzz/NOMNOM)
registry, where this mod is listed.

### Option B — manual

1. Install [BepInEx 5](https://github.com/BepInEx/BepInEx/releases) (the **Mono x64** build) into
   your Nuclear Option folder and run the game once so it generates its folders.
2. Drop `NuclearOption-InfiniteAmmo.dll` (from the
   [latest release](https://github.com/cosistra/nuclear-option-infinite-ammo/releases/latest)) into
   `<game>\BepInEx\plugins\InfiniteAmmo\`.

First launch writes the config to `<game>\BepInEx\config\com.dablackpantha.infiniteammo.cfg`.

---

## Configuration

All settings live in `com.dablackpantha.infiniteammo.cfg` (edit the file, or use
BepInEx.ConfigurationManager if you have it installed).

| Key | Default | Description |
|---|---|---|
| `InfiniteWeapons` | `true` | Rearm all weapon stations (including linked guns and AI-gunner stations) when they hit zero. |
| `InfiniteCountermeasures` | `true` | Rearm all countermeasure dispensers when they hit zero. |
| `InfiniteFuel` | `true` | Refuel when fuel drops to or below `FuelThreshold`. |
| `FuelThreshold` | `0.05` | Refuel trigger: actual fuel ratio (0–1). Default 0.05 = 5 %. |
| `FuelRefillTarget` | `0.10` | Top tanks up to this ratio when refueling. Set to `1.0` for full tanks. Must be above `FuelThreshold`. |
| `KeepConsumablesTopped` | `false` | When `true`, top up stations/countermeasures the moment any ammo drops below full instead of waiting for zero. |
| `VerboseLogging` | `false` | Log every rearm/refuel action to the BepInEx log. |

---

## Build from source

Requires the .NET SDK. Point `<GamePath>` in `InfiniteAmmoBepInEx.csproj` at your install (the
folder containing `NuclearOption.exe`, with BepInEx 5 installed into it):

```
dotnet build InfiniteAmmoBepInEx.csproj -c Release
```

Then copy `bin\Release\NuclearOption-InfiniteAmmo.dll` into `<game>\BepInEx\plugins\InfiniteAmmo\`.

Maintainers: [`release.ps1`](release.ps1) builds, tags, and publishes a GitHub release in one step.

---

## Requirements

- Nuclear Option
- BepInEx 5 (Mono x64)

## Credits

Original mod concept and MelonLoader implementation by **DaBlackPantha**.
BepInEx port and extended station/countermeasure coverage by **cosistra**.

## License

MIT — see [LICENSE](LICENSE). Game code is **not** included or redistributed.



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

-
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



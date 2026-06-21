# Changelog

All notable changes to Infinite Ammo. Versions are the `PluginVersion` in `InfiniteAmmoPlugin.cs`
(the single source of truth); each release is published via `release.ps1`.

## 1.1.0

- **Rearms ALL weapon stations, not just the currently selected one.** Previously, linked guns on
  multi-weapon mounts and any station not currently selected by the player would run dry and never
  refill. Now every station on `aircraft.weaponStations` is checked and rearmed when empty. This
  also covers AI-gunner stations on multi-crew aircraft — the gunner's gun now refills even though
  the player never selects it.
- **Rearms ALL countermeasure dispensers.** The original implementation only rearmed the active
  countermeasure. All dispensers (via `GetComponentsInChildren<Countermeasure>`) now refill
  individually when empty.
- **Switched entry point to `GameManager.GetLocalAircraft`.** Replaced the brittle
  `GameObject.Find("Player(Clone)")` + `GetComponent<Player>()` chain with the MP-aware
  `GameManager.GetLocalAircraft(out Aircraft aircraft)` call — this is the correct API for
  accessing the local player's aircraft and is null-safe across scene transitions.
- **Added `KeepConsumablesTopped` toggle** (`General/KeepConsumablesTopped`, default `false`).
  When enabled, all stations and countermeasures are topped up the moment any ammo drops below
  full, rather than waiting for zero. Useful for sustained fire drills.
- **Louder singleplayer-only messaging.** The load log now reads:
  `"Infinite Ammo v1.1.0 loaded — SINGLEPLAYER ONLY. Rearm/refuel writes are ignored on multiplayer servers."`

## 1.0.0

- Initial faithful BepInEx 5 port of DaBlackPantha's MelonLoader "Infinite Ammo" mod.
- Config: `InfiniteWeapons`, `InfiniteCountermeasures`, `InfiniteFuel`, `FuelThreshold`,
  `FuelRefillTarget`, `VerboseLogging`.
- Fixed the fuel-refill trigger: the original compared the wrong value (`fuelLevel` setpoint
  instead of `GetFuelLevel()` actual ratio), so tanks never topped up after spawn. This port
  uses `GetFuelLevel()` as the trigger and writes `fuelLevel` as the refill target.

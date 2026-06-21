using BepInEx;
using BepInEx.Configuration;
using UnityEngine;
using UnityEngine.SceneManagement;
using NuclearOption.Networking;

namespace InfiniteAmmoBepInEx
{
    // Faithful BepInEx 5 port of "Infinite Ammo" by DaBlackPantha (originally a MelonLoader mod).
    // Singleplayer only — rearm/refuel writes are server-authoritative and ignored on multiplayer servers.
    [BepInPlugin(PluginGuid, PluginName, PluginVersion)]
    public class InfiniteAmmoPlugin : BaseUnityPlugin
    {
        public const string PluginGuid    = "com.dablackpantha.infiniteammo";
        public const string PluginName    = "Infinite Ammo";
        public const string PluginVersion = "1.1.0";

        // Original mod only acted while this scene was active.
        private const string TargetScene = "GameWorld";
        private bool _sceneLoaded;

        private ConfigEntry<bool>  _enableWeapons;
        private ConfigEntry<bool>  _enableCountermeasures;
        private ConfigEntry<bool>  _enableFuel;
        private ConfigEntry<float> _fuelThreshold;
        private ConfigEntry<float> _fuelRefillTarget;
        private ConfigEntry<bool>  _keepTopped;
        private ConfigEntry<bool>  _verbose;

        private void Awake()
        {
            _enableWeapons         = Config.Bind("General", "InfiniteWeapons",         true,  "Rearm ALL weapon stations (including linked guns and AI-gunner stations) when they run dry. Singleplayer only.");
            _enableCountermeasures = Config.Bind("General", "InfiniteCountermeasures", true,  "Rearm ALL countermeasure dispensers (flares, chaff, etc.) when they run dry. Singleplayer only.");
            _enableFuel            = Config.Bind("General", "InfiniteFuel",            true,  "Refuel when fuel reaches the threshold.");
            _fuelThreshold         = Config.Bind("General", "FuelThreshold",           0.05f, "Refuel when the ACTUAL fuel ratio (0..1) drops to/below this. Default 0.05 (5%).");
            _fuelRefillTarget      = Config.Bind("General", "FuelRefillTarget",        0.10f, "Top the tanks back up to this ratio (0..1) when refueling. Default 0.10 (10%). Set to 1.0 for full tanks. Keep this above FuelThreshold.");
            _keepTopped            = Config.Bind("General", "KeepConsumablesTopped",   false, "When true, top up ANY station/countermeasure that is below full every frame (counters never appear to drop). When false (default), only refill when fully empty. Singleplayer only.");
            _verbose               = Config.Bind("Debug",   "VerboseLogging",          false, "Log every rearm/refuel action (the original's debug logs were empty stubs).");

            SceneManager.sceneLoaded += OnSceneLoaded;
            Logger.LogInfo($"{PluginName} v{PluginVersion} loaded — SINGLEPLAYER ONLY. Rearm/refuel writes are server-authoritative and are ignored on multiplayer servers.");
        }

        private void OnDestroy() => SceneManager.sceneLoaded -= OnSceneLoaded;

        // Mirrors the original OnSceneWasLoaded(buildIndex, sceneName) gate.
        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
            => _sceneLoaded = scene.name == TargetScene;

        private void Update()
        {
            if (!_sceneLoaded) return;

            if (!GameManager.GetLocalAircraft(out Aircraft aircraft) || aircraft == null) return;

            // Original constructed this lazily inside the weapon branch, which left it null when
            // ammo wasn't depleted. Hoisting it here keeps Rearm/Refuel from ever receiving null.
            var instigator = new Unit { unitName = PluginName };

            // --- Weapons: loop ALL stations (covers linked guns and AI-gunner stations) ---
            if (_enableWeapons.Value)
            {
                foreach (var ws in aircraft.weaponStations)
                {
                    if (ws == null || ws.Cargo) continue;

                    // WeaponStation.Ammo/FullAmmo are CACHED ints. The game only recomputes them
                    // (via AccountAmmo) for a station that is actively firing or selected — and on
                    // the client path UpdateLastFired just does "Ammo -= roundsFired", which can drift.
                    // So a linked / non-selected gun stays at a stale, non-zero Ammo even when its
                    // real per-weapon ammo is empty, and an "== 0" test never fires for it (the bug:
                    // only the selected gun refilled). Recompute from the live per-weapon ammo first
                    // so empty detection is accurate for EVERY station.
                    ws.AccountAmmo();
                    if (ws.FullAmmo <= 0) continue;

                    bool needs = _keepTopped.Value ? ws.Ammo < ws.FullAmmo : ws.Ammo <= 0;
                    if (needs)
                    {
                        int before = ws.Ammo;
                        ws.Rearm();
                        Verbose($"Rearmed {aircraft.definition.name}'s {ws.WeaponInfo.weaponName} ({before}/{ws.FullAmmo} -> {ws.Ammo})");
                    }
                }
            }

            // --- Countermeasures: enumerate ALL dispensers on the aircraft ---
            // Note: Countermeasure.maxAmmo is private in subclasses (set from initial ammo in Awake).
            // Rearm() is virtual and already no-ops when already full, so calling it when
            // KeepConsumablesTopped is true is safe and correct even without a public FullAmmo.
            if (_enableCountermeasures.Value)
            {
                foreach (var cm in aircraft.GetComponentsInChildren<Countermeasure>(true))
                {
                    if (cm == null) continue;
                    // When topping: call Rearm() unconditionally (it no-ops when already full).
                    // When not topping: only call when fully empty.
                    bool needs = _keepTopped.Value ? true : cm.ammo == 0;
                    if (needs)
                    {
                        cm.Rearm(aircraft, instigator);
                        Verbose($"Rearmed {aircraft.definition.name}'s {cm.displayName} countermeasure.");
                    }
                }
            }

            // --- Fuel ---
            // IMPORTANT: aircraft.fuelLevel is NOT the current fuel gauge. It is the target fill ratio
            // that Refuel() tops the tanks up to (SetFuelLevel calls fuelTank.Refuel(fuelLevel)), and it
            // is set once at spawn — it does not drop as you burn fuel. The original mod compared this
            // setpoint to the threshold, so it never fired once the plane spawned with more than the
            // threshold's worth of fuel — that's the "won't refill" bug. The real current ratio is
            // GetFuelLevel() (sum of tank levels / capacity). We trigger off that, then write our desired
            // target into fuelLevel so Refuel fills the tanks back up to it.
            if (_enableFuel.Value)
            {
                float currentFuel = aircraft.GetFuelLevel();
                if (currentFuel <= _fuelThreshold.Value)
                {
                    aircraft.fuelLevel = _fuelRefillTarget.Value;
                    aircraft.Refuel(instigator);
                    Verbose($"Refueled {aircraft.definition.name} from {currentFuel:P0} up to {_fuelRefillTarget.Value:P0}");
                }
            }
        }

        private void Verbose(string msg)
        {
            if (_verbose.Value) Logger.LogInfo(msg);
        }
    }
}

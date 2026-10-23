// =============================================================================
// TimeFreezeManager.cs — RULE//BREAK
// Implements Time Freeze WITHOUT using Time.timeScale = 0.
// Registers/deregisters FreezableObject instances and pauses them individually.
// Player, UI, camera, and managers are never affected.
// =============================================================================
using UnityEngine;
using System.Collections.Generic;
using RuleBreak.Level;
using RuleBreak.Audio;

namespace RuleBreak.Time
{
    public class TimeFreezeManager : MonoBehaviour
    {
        public static TimeFreezeManager Instance { get; private set; }

        [SerializeField] private float freezeDuration = 0f; // 0 = toggle (no auto-release)

        private bool _frozen = false;
        private readonly HashSet<FreezableObject> _freezables = new HashSet<FreezableObject>();

        public bool IsFrozen => _frozen;

        // Event for HUD / VFX to subscribe
        public event System.Action<bool> OnFreezeStateChanged;

        private void Awake()
        {
            if (Instance != null && Instance != this) { Destroy(gameObject); return; }
            Instance = this;
        }

        // ── Called by PlayerController on T key ───────────────────────────────
        public void ToggleFreeze()
        {
            var energy = GlitchEnergyManager.Instance;

            if (!_frozen)
            {
                // Activating freeze
                if (energy != null)
                {
                    if (!energy.CanAfford(energy.CostTimeFreeze))
                    {
                        AudioManager.Instance?.PlayEnergyDenied();
                        return;
                    }
                    energy.Spend(energy.CostTimeFreeze);
                }
                Freeze();
                GameProgressManager.Instance?.RegisterFreezeUse();
            }
            else
            {
                Unfreeze();
            }
        }

        private void Freeze()
        {
            _frozen = true;
            foreach (var f in _freezables) f?.DoFreeze();
            AudioManager.Instance?.PlayTimeFreeze();
            OnFreezeStateChanged?.Invoke(true);

            if (freezeDuration > 0f)
            {
                CancelInvoke(nameof(Unfreeze));
                Invoke(nameof(Unfreeze), freezeDuration);
            }

            Debug.Log("Time Freeze: ON");
        }

        private void Unfreeze()
        {
            CancelInvoke(nameof(Unfreeze));
            _frozen = false;
            foreach (var f in _freezables) f?.DoUnfreeze();
            AudioManager.Instance?.PlayTimeUnfreeze();
            OnFreezeStateChanged?.Invoke(false);
            Debug.Log("Time Freeze: OFF");
        }

        // ── Registration ──────────────────────────────────────────────────────
        public void Register(FreezableObject obj)   { if (obj != null) _freezables.Add(obj); }
        public void Unregister(FreezableObject obj) { _freezables.Remove(obj); }

        private void OnDestroy()
        {
            // Safety: unfreeze everything if this manager is destroyed
            if (_frozen) Unfreeze();
        }
    }
}

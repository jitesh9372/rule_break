// =============================================================================
// DimensionManager.cs — RULE//BREAK
// Manages two reality states: REALITY_A (default) and REALITY_B.
// Objects tagged or using DimensionObject component are shown/hidden per reality.
// =============================================================================
using UnityEngine;
using System.Collections.Generic;
using RuleBreak.Level;
using RuleBreak.Audio;

namespace RuleBreak.Dimension
{
    public enum Reality { A, B }

    public class DimensionManager : MonoBehaviour
    {
        public static DimensionManager Instance { get; private set; }

        [SerializeField] private Reality startReality = Reality.A;

        private Reality _currentReality;
        private readonly List<DimensionObject> _objects = new List<DimensionObject>();

        public Reality CurrentReality => _currentReality;

        public event System.Action<Reality> OnDimensionChanged;

        private void Awake()
        {
            if (Instance != null && Instance != this) { Destroy(gameObject); return; }
            Instance = this;
            _currentReality = startReality;
        }

        private void Start()
        {
            // Apply initial state after all DimensionObjects have registered
            ApplyCurrentReality();
        }

        // ── Called by PlayerController on E key ───────────────────────────────
        public void ToggleDimension()
        {
            var energy = GlitchEnergyManager.Instance;
            if (energy != null)
            {
                if (!energy.CanAfford(energy.CostDimension))
                {
                    AudioManager.Instance?.PlayEnergyDenied();
                    return;
                }
                energy.Spend(energy.CostDimension);
            }

            _currentReality = (_currentReality == Reality.A) ? Reality.B : Reality.A;
            ApplyCurrentReality();
            AudioManager.Instance?.PlayDimensionShift();
            GameProgressManager.Instance?.RegisterDimensionUse();
            OnDimensionChanged?.Invoke(_currentReality);
            Debug.Log($"Dimension shifted to {_currentReality}");
        }

        private void ApplyCurrentReality()
        {
            foreach (var obj in _objects) obj?.Apply(_currentReality);
        }

        // ── Registration ──────────────────────────────────────────────────────
        public void Register(DimensionObject obj)   { if (obj != null && !_objects.Contains(obj)) _objects.Add(obj); }
        public void Unregister(DimensionObject obj) { _objects.Remove(obj); }
    }
}

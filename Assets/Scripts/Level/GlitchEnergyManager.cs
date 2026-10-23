// =============================================================================
// GlitchEnergyManager.cs — RULE//BREAK
// Singleton that tracks the player's Glitch Energy (0-100).
// Abilities query CanAfford() and call Spend() before activating.
// Energy recharges slowly over time and from energy pickups.
// =============================================================================
using UnityEngine;

namespace RuleBreak.Level
{
    public class GlitchEnergyManager : MonoBehaviour
    {
        // ── Singleton ─────────────────────────────────────────────────────────
        public static GlitchEnergyManager Instance { get; private set; }

        // ── Inspector ─────────────────────────────────────────────────────────
        [Header("Energy")]
        [SerializeField] private float maxEnergy        = 100f;
        [SerializeField] private float startEnergy      = 100f;
        [SerializeField] private float rechargePerSecond = 5f;

        [Header("Ability Costs")]
        [SerializeField] private float costGravity    = 20f;
        [SerializeField] private float costTimeFreeze = 30f;
        [SerializeField] private float costDimension  = 25f;
        [SerializeField] private float costCorruption = 40f;

        // ── State ─────────────────────────────────────────────────────────────
        private float _energy;

        // ── Events ────────────────────────────────────────────────────────────
        public event System.Action<float, float> OnEnergyChanged; // current, max

        // ── Properties ────────────────────────────────────────────────────────
        public float Energy    => _energy;
        public float MaxEnergy => maxEnergy;

        public float CostGravity    => costGravity;
        public float CostTimeFreeze => costTimeFreeze;
        public float CostDimension  => costDimension;
        public float CostCorruption => costCorruption;

        // ── Unity ─────────────────────────────────────────────────────────────
        private void Awake()
        {
            if (Instance != null && Instance != this) { Destroy(gameObject); return; }
            Instance = this;
            DontDestroyOnLoad(gameObject);
            _energy = startEnergy;
        }

        private void Update()
        {
            float prev = _energy;
            _energy = Mathf.MoveTowards(_energy, maxEnergy, rechargePerSecond * UnityEngine.Time.deltaTime);
            if (!Mathf.Approximately(_energy, prev))
                OnEnergyChanged?.Invoke(_energy, maxEnergy);
        }

        // ── Public API ────────────────────────────────────────────────────────
        public bool CanAfford(float cost) => _energy >= cost;

        /// <summary>Deducts cost. Returns false if insufficient energy.</summary>
        public bool Spend(float cost)
        {
            if (!CanAfford(cost)) return false;
            _energy = Mathf.Max(0f, _energy - cost);
            OnEnergyChanged?.Invoke(_energy, maxEnergy);
            return true;
        }

        public void Restore(float amount)
        {
            _energy = Mathf.Min(maxEnergy, _energy + amount);
            OnEnergyChanged?.Invoke(_energy, maxEnergy);
        }

        public void ResetEnergy()
        {
            _energy = startEnergy;
            OnEnergyChanged?.Invoke(_energy, maxEnergy);
        }
    }
}

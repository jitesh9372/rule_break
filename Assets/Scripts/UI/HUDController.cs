// =============================================================================
// HUDController.cs — RULE//BREAK
// Controls the in-game HUD: dynamic energy bar, reality integrity gauge,
// contextual system alerts, ability status modules (G, T, E, R),
// memory fragment counter, and interactive prompt overlays.
// =============================================================================
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using RuleBreak.Level;
using RuleBreak.Dimension;
using RuleBreak.Time;

namespace RuleBreak.UI
{
    public class HUDController : MonoBehaviour
    {
        public static HUDController Instance { get; private set; }

        [Header("Energy Gauge")]
        [SerializeField] private Slider energySlider;
        [SerializeField] private Text   energyText;
        [SerializeField] private CircularGauge energyGauge;

        [Header("Reality Integrity")]
        [SerializeField] private Text integrityText;
        [SerializeField] private CircularGauge integrityGauge;

        [Header("Radar Objective Display")]
        [SerializeField] private RadarObjectiveDisplay radarDisplay;

        [Header("Memory Fragments")]
        [SerializeField] private Text fragmentText;

        [Header("Current Level / Objective")]
        [SerializeField] private Text levelText;
        [SerializeField] private Text objectiveText;

        [Header("System Message Toast")]
        [SerializeField] private GameObject systemMsgPanel;
        [SerializeField] private Text       systemMsgText;

        [Header("Interaction Prompt")]
        [SerializeField] private GameObject interactionPromptPanel;
        [SerializeField] private Text       interactionPromptText;

        [Header("Ability Modules")]
        [SerializeField] private Text dimensionText;
        [SerializeField] private Text abilityGText;
        [SerializeField] private Text abilityTText;
        [SerializeField] private Text abilityEText;
        [SerializeField] private Text abilityRText;
        [SerializeField] private RadialAbilityIndicator abilityGIndicator;
        [SerializeField] private RadialAbilityIndicator abilityTIndicator;
        [SerializeField] private RadialAbilityIndicator abilityEIndicator;
        [SerializeField] private RadialAbilityIndicator abilityRIndicator;

        [Header("Glitch Effect")]
        [SerializeField] private float glitchIntensity = 0f;
        public float GlitchIntensity { get => glitchIntensity; set => glitchIntensity = value; }

        private Coroutine _msgCoroutine;
        private int _fragTotal = 8;
        private float _lastEnergy = 100f;

        private static readonly Color ColCyan     = new Color(0f, 0.9f, 1f);
        private static readonly Color ColDim      = new Color(0.35f, 0.42f, 0.52f);
        private static readonly Color ColViolet   = new Color(0.61f, 0.36f, 1f);
        private static readonly Color ColGreen    = new Color(0.38f, 0.95f, 0.64f);
        private static readonly Color ColWarning  = new Color(1f, 0.71f, 0.28f);
        private static readonly Color ColCritical = new Color(1f, 0.25f, 0.34f);

        private void Awake()
        {
            if (Instance != null && Instance != this) { Destroy(gameObject); return; }
            Instance = this;
        }

        private void OnEnable()
        {
            if (GlitchEnergyManager.Instance != null)
                GlitchEnergyManager.Instance.OnEnergyChanged += UpdateEnergy;
            if (RealityIntegrityManager.Instance != null)
                RealityIntegrityManager.Instance.OnIntegrityChanged += UpdateIntegrity;
            if (DimensionManager.Instance != null)
                DimensionManager.Instance.OnDimensionChanged += UpdateDimension;
        }

        private void OnDisable()
        {
            if (GlitchEnergyManager.Instance != null)
                GlitchEnergyManager.Instance.OnEnergyChanged -= UpdateEnergy;
            if (RealityIntegrityManager.Instance != null)
                RealityIntegrityManager.Instance.OnIntegrityChanged -= UpdateIntegrity;
            if (DimensionManager.Instance != null)
                DimensionManager.Instance.OnDimensionChanged -= UpdateDimension;
        }

        private void Start()
        {
            var energy = GlitchEnergyManager.Instance;
            if (energy != null) UpdateEnergy(energy.Energy, energy.MaxEnergy);

            var integrity = RealityIntegrityManager.Instance;
            if (integrity != null) UpdateIntegrity(integrity.Integrity);

            var lvlMgr = LevelManager.Instance;
            if (lvlMgr != null && levelText != null)
                levelText.text = $"LEVEL 0{lvlMgr.LevelIndex}";

            UpdateFragmentDisplay(0);
            HideInteractionPrompt();
            if (systemMsgPanel != null) systemMsgPanel.SetActive(false);
        }

        private void Update()
        {
            RefreshAbilityIndicators();
        }

        // ── Energy ────────────────────────────────────────────────────────────
        private void UpdateEnergy(float current, float max)
        {
            _lastEnergy = current;
            if (energySlider != null) energySlider.value = current / max;
            if (energyText   != null) energyText.text    = $"ENERGY: {Mathf.RoundToInt(current)} / {Mathf.RoundToInt(max)}";
            if (energyGauge  != null) energyGauge.SetValue(current, max);
            RefreshAbilityIndicators();
        }

        // ── Integrity ─────────────────────────────────────────────────────────
        private void UpdateIntegrity(float value)
        {
            if (integrityGauge != null) integrityGauge.SetValue(value, 100f);
            if (integrityText == null) return;
            integrityText.text = $"REALITY INTEGRITY: {Mathf.RoundToInt(value)}%";

            if (value > 60f)
                integrityText.color = ColGreen;
            else if (value > 30f)
                integrityText.color = ColWarning;
            else
                integrityText.color = ColCritical;
        }

        // ── Dimension ─────────────────────────────────────────────────────────
        private void UpdateDimension(Reality r)
        {
            if (dimensionText != null)
            {
                dimensionText.text = r == Reality.A ? "REALITY: ALPHA [E]" : "REALITY: BETA [E]";
                dimensionText.color = r == Reality.A ? ColCyan : ColViolet;
            }
            abilityEIndicator?.TriggerActivationPulse();
        }

        // ── Ability Modules Highlight ─────────────────────────────────────────
        private void RefreshAbilityIndicators()
        {
            // [G] Gravity (cost 20)
            if (abilityGText != null)
            {
                bool canAfford = _lastEnergy >= 20f;
                abilityGText.color = canAfford ? ColCyan : ColDim;
            }
            if (abilityGIndicator != null) abilityGIndicator.UpdateEnergyStatus(_lastEnergy);

            // [T] Time Freeze (cost 25)
            bool isFrozen = TimeFreezeManager.Instance != null && TimeFreezeManager.Instance.IsFrozen;
            if (abilityTText != null)
            {
                if (isFrozen)
                {
                    abilityTText.color = Color.white;
                    abilityTText.text = "[T] TIME: FROZEN";
                }
                else
                {
                    bool canAfford = _lastEnergy >= 25f;
                    abilityTText.color = canAfford ? ColCyan : ColDim;
                    abilityTText.text = "[T] FREEZE (25)";
                }
            }
            if (abilityTIndicator != null)
            {
                abilityTIndicator.UpdateEnergyStatus(_lastEnergy);
                if (isFrozen) abilityTIndicator.SetState(true);
            }

            // [E] Dimension (cost 15)
            bool isDimB = DimensionManager.Instance != null && DimensionManager.Instance.CurrentReality == Reality.B;
            if (abilityEText != null)
            {
                if (isDimB)
                {
                    abilityEText.color = ColViolet;
                    abilityEText.text = "[E] REALITY: BETA";
                }
                else
                {
                    bool canAfford = _lastEnergy >= 15f;
                    abilityEText.color = canAfford ? ColCyan : ColDim;
                    abilityEText.text = "[E] REALITY: ALPHA";
                }
            }
            if (abilityEIndicator != null)
            {
                abilityEIndicator.UpdateEnergyStatus(_lastEnergy);
                if (isDimB) abilityEIndicator.SetState(true);
            }

            // [R] Rule Corruption (cost 30)
            if (abilityRText != null)
            {
                bool canAfford = _lastEnergy >= 30f;
                abilityRText.color = canAfford ? new Color(1f, 0.24f, 0.6f) : ColDim;
            }
            if (abilityRIndicator != null) abilityRIndicator.UpdateEnergyStatus(_lastEnergy);
        }

        // ── Fragments ─────────────────────────────────────────────────────────
        public void UpdateFragmentDisplay(int count)
        {
            if (fragmentText != null)
                fragmentText.text = $"FRAGMENTS: {count} / {_fragTotal}";
        }

        public void ShowFragmentNotification(int count, int total)
        {
            _fragTotal = total;
            UpdateFragmentDisplay(count);
            ShowSystemMessage($"MEMORY FRAGMENT ACQUIRED [{count}/{total}]", 3.5f);
        }

        // ── Objective ─────────────────────────────────────────────────────────
        public void SetObjective(string obj)
        {
            if (objectiveText != null) objectiveText.text = $"OBJECTIVE: {obj}";
        }

        // ── System Message ────────────────────────────────────────────────────
        public void ShowSystemMessage(string msg, float duration = 4f)
        {
            if (systemMsgPanel == null) return;
            if (_msgCoroutine != null) StopCoroutine(_msgCoroutine);
            _msgCoroutine = StartCoroutine(ShowMsgCoroutine(msg, duration));
        }

        private IEnumerator ShowMsgCoroutine(string msg, float duration)
        {
            systemMsgPanel.SetActive(true);
            if (systemMsgText != null) systemMsgText.text = msg;
            yield return new WaitForSeconds(duration);
            systemMsgPanel.SetActive(false);
        }

        // ── Interaction Prompt ────────────────────────────────────────────────
        public void ShowInteractionPrompt(string msg)
        {
            if (interactionPromptPanel != null) interactionPromptPanel.SetActive(true);
            if (interactionPromptText  != null) interactionPromptText.text = msg;
        }

        public void HideInteractionPrompt()
        {
            if (interactionPromptPanel != null) interactionPromptPanel.SetActive(false);
        }

        // ── Checkpoint Feedback ───────────────────────────────────────────────
        public void ShowCheckpointActivated()
        {
            ShowSystemMessage("► CHECKPOINT SYNCHRONIZED — RESPAWN LOGGED", 2.5f);
        }

        // ── Direct Ability Pulse Feedback ─────────────────────────────────────
        public void PulseAbilityIndicator(string key)
        {
            if (key == "G") abilityGIndicator?.TriggerActivationPulse();
            else if (key == "T") abilityTIndicator?.TriggerActivationPulse();
            else if (key == "E") abilityEIndicator?.TriggerActivationPulse();
            else if (key == "R") abilityRIndicator?.TriggerActivationPulse();
        }
    }
}

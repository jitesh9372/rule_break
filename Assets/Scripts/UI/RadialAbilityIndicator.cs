// =============================================================================
// RadialAbilityIndicator.cs — RULE//BREAK
// Circular ability indicator module with holographic outer ring,
// energy sufficiency tracking, dynamic dimming, and activation pulse glow.
// =============================================================================
using UnityEngine;
using UnityEngine.UI;

namespace RuleBreak.UI
{
    public class RadialAbilityIndicator : MonoBehaviour
    {
        [Header("UI References")]
        [SerializeField] private Image outerRing;
        [SerializeField] private Image iconOrFill;
        [SerializeField] private Text keyText;
        [SerializeField] private Text nameText;
        [SerializeField] private Text costText;

        [Header("Settings")]
        [SerializeField] private float energyCost = 15f;
        [SerializeField] private Color activeColor = new Color(0f, 0.9f, 1f, 1f);
        [SerializeField] private Color dimmedColor = new Color(0.25f, 0.32f, 0.42f, 0.5f);

        private float _pulseTimer = 0f;
        private bool _isPulsing = false;
        private Vector3 _baseScale = Vector3.one;

        private void Awake()
        {
            _baseScale = transform.localScale;
        }

        private void Update()
        {
            if (_isPulsing)
            {
                _pulseTimer += UnityEngine.Time.deltaTime * 6f;
                float scale = 1f + Mathf.Sin(_pulseTimer) * 0.15f;
                transform.localScale = _baseScale * scale;

                if (_pulseTimer >= Mathf.PI)
                {
                    _isPulsing = false;
                    _pulseTimer = 0f;
                    transform.localScale = _baseScale;
                }
            }
        }

        public void Setup(string key, string name, float cost, Color color)
        {
            energyCost = cost;
            activeColor = color;

            if (keyText != null) keyText.text = key;
            if (nameText != null) nameText.text = name;
            if (costText != null) costText.text = $"{cost}E";

            SetState(true);
        }

        public void UpdateEnergyStatus(float currentEnergy)
        {
            bool canAfford = currentEnergy >= energyCost;
            SetState(canAfford);
        }

        public void SetState(bool isReady)
        {
            Color targetCol = isReady ? activeColor : dimmedColor;

            if (outerRing != null) outerRing.color = targetCol;
            if (keyText != null) keyText.color = targetCol;
            if (nameText != null) nameText.color = isReady ? Color.white : dimmedColor;
            if (costText != null) costText.color = isReady ? new Color(0.7f, 0.85f, 1f, 0.9f) : dimmedColor;
        }

        public void TriggerActivationPulse()
        {
            _isPulsing = true;
            _pulseTimer = 0f;
            if (outerRing != null) outerRing.color = Color.white;
        }
    }
}

// =============================================================================
// CircularGauge.cs — RULE//BREAK
// Circular holographic gauge with radial 360 fill, smooth tweening,
// outer rotating decorative ring, and dynamic color state transitions.
// =============================================================================
using UnityEngine;
using UnityEngine.UI;

namespace RuleBreak.UI
{
    public class CircularGauge : MonoBehaviour
    {
        [Header("UI Elements")]
        [SerializeField] private Image radialFillImage;
        [SerializeField] private RectTransform outerRingTransform;
        [SerializeField] private Text valueText;
        [SerializeField] private Text labelText;

        [Header("Animation")]
        [SerializeField] private float smoothSpeed = 8f;
        [SerializeField] private float ringRotationSpeed = 20f;

        [Header("Colors")]
        [SerializeField] private Color normalColor = new Color(0f, 0.9f, 1f, 1f);
        [SerializeField] private Color warningColor = new Color(1f, 0.72f, 0.28f, 1f);
        [SerializeField] private Color criticalColor = new Color(1f, 0.25f, 0.35f, 1f);

        private float _currentFill = 1f;
        private float _targetFill = 1f;
        private float _currentNumeric = 100f;
        private float _maxNumeric = 100f;

        private void Update()
        {
            // Smooth lerp fill amount
            _currentFill = Mathf.Lerp(_currentFill, _targetFill, UnityEngine.Time.deltaTime * smoothSpeed);
            if (radialFillImage != null)
            {
                radialFillImage.fillAmount = _currentFill;
                radialFillImage.color = GetColorForFill(_currentFill);
            }

            // Rotate outer decorative ring
            if (outerRingTransform != null)
            {
                outerRingTransform.Rotate(Vector3.forward, -ringRotationSpeed * UnityEngine.Time.deltaTime);
            }
        }

        private Color GetColorForFill(float fill)
        {
            if (fill < 0.25f) return criticalColor;
            if (fill < 0.5f) return warningColor;
            return normalColor;
        }

        public void SetValue(float current, float max)
        {
            _currentNumeric = current;
            _maxNumeric = Mathf.Max(1f, max);
            _targetFill = Mathf.Clamp01(current / _maxNumeric);

            if (valueText != null)
            {
                valueText.text = $"{Mathf.RoundToInt(current)}";
            }
        }

        public void SetLabel(string label)
        {
            if (labelText != null) labelText.text = label;
        }
    }
}

// =============================================================================
// SciFiButton.cs — RULE//BREAK
// Enhances standard Unity Button with futuristic holographic interactions:
// - Smooth scale tweening on hover/focus (1.0 -> 1.04)
// - Glow border brightening and subtle corner accent shifts
// - Audio cues on pointer enter and pointer click
// - Full gamepad/keyboard selection support
// =============================================================================
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using RuleBreak.Audio;

namespace RuleBreak.UI
{
    [RequireComponent(typeof(Button))]
    public class SciFiButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler,
                                               IPointerDownHandler, IPointerUpHandler,
                                               ISelectHandler, IDeselectHandler
    {
        [Header("Animation Settings")]
        [SerializeField] private float hoverScale = 1.04f;
        [SerializeField] private float pressScale = 0.97f;
        [SerializeField] private float tweenSpeed = 12f;

        [Header("Colors")]
        [SerializeField] private Color normalBorderColor = new Color(0f, 0.85f, 1f, 0.6f);
        [SerializeField] private Color hoverBorderColor = new Color(0.2f, 1f, 0.9f, 1f);
        [SerializeField] private Image targetBorderImage;

        private Button _button;
        private Vector3 _originalScale;
        private Vector3 _targetScale;
        private bool _isHovered = false;

        private void Awake()
        {
            _button = GetComponent<Button>();
            _originalScale = transform.localScale;
            _targetScale = _originalScale;

            if (targetBorderImage == null)
            {
                targetBorderImage = GetComponent<Image>();
            }
        }

        private void Update()
        {
            if (transform.localScale != _targetScale)
            {
                transform.localScale = Vector3.Lerp(transform.localScale, _targetScale, UnityEngine.Time.unscaledDeltaTime * tweenSpeed);
            }
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (_button != null && !_button.interactable) return;
            SetHover(true);
            AudioManager.Instance?.PlayUIHover();
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            SetHover(false);
        }

        public void OnSelect(BaseEventData eventData)
        {
            if (_button != null && !_button.interactable) return;
            SetHover(true);
            AudioManager.Instance?.PlayUIHover();
        }

        public void OnDeselect(BaseEventData eventData)
        {
            SetHover(false);
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            if (_button != null && !_button.interactable) return;
            _targetScale = _originalScale * pressScale;
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            _targetScale = _isHovered ? _originalScale * hoverScale : _originalScale;
            AudioManager.Instance?.PlayUIClick();
        }

        private void SetHover(bool hover)
        {
            _isHovered = hover;
            _targetScale = hover ? _originalScale * hoverScale : _originalScale;

            if (targetBorderImage != null)
            {
                targetBorderImage.color = hover ? hoverBorderColor : normalBorderColor;
            }
        }
    }
}

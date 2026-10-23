// =============================================================================
// ScanlineOverlay.cs — RULE//BREAK
// Subtle animated holographic scanline overlay for futuristic screens and HUD.
// =============================================================================
using UnityEngine;
using UnityEngine.UI;

namespace RuleBreak.UI
{
    [RequireComponent(typeof(RawImage))]
    public class ScanlineOverlay : MonoBehaviour
    {
        [SerializeField] private float scrollSpeed = 0.05f;
        [SerializeField] private float scanlineDensity = 20f;
        [SerializeField] private float alpha = 0.06f;

        private RawImage _rawImage;
        private Rect _uvRect;

        private void Awake()
        {
            _rawImage = GetComponent<RawImage>();
            _rawImage.texture = SciFiProceduralTextures.GetScanlineTexture();
            _rawImage.color = new Color(0f, 0.9f, 1f, alpha);
            _uvRect = new Rect(0, 0, 1, scanlineDensity);
            _rawImage.uvRect = _uvRect;
        }

        private void Update()
        {
            _uvRect.y += scrollSpeed * UnityEngine.Time.unscaledDeltaTime;
            if (_rawImage != null) _rawImage.uvRect = _uvRect;
        }
    }
}

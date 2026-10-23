// =============================================================================
// RealityIntegrityManager.cs — RULE//BREAK
// Tracks narrative Reality Integrity (0-100%). Drives visual and audio changes.
// =============================================================================
using UnityEngine;

namespace RuleBreak.Level
{
    public class RealityIntegrityManager : MonoBehaviour
    {
        public static RealityIntegrityManager Instance { get; private set; }

        [SerializeField] private float startIntegrity = 100f;
        private float _integrity;

        public float Integrity => _integrity;
        public float IntegrityNormalized => _integrity / 100f;

        public event System.Action<float> OnIntegrityChanged;

        private void Awake()
        {
            if (Instance != null && Instance != this) { Destroy(gameObject); return; }
            Instance = this;
            DontDestroyOnLoad(gameObject);
            _integrity = startIntegrity;
        }

        public void SetIntegrity(float value)
        {
            _integrity = Mathf.Clamp(value, 0f, 100f);
            OnIntegrityChanged?.Invoke(_integrity);
        }

        public void ReduceIntegrity(float amount)
        {
            SetIntegrity(_integrity - amount);
        }

        /// <summary>Integrity tier: 0=high, 1=medium, 2=low, 3=critical</summary>
        public int GetTier()
        {
            if (_integrity > 70f) return 0;
            if (_integrity > 40f) return 1;
            if (_integrity > 15f) return 2;
            return 3;
        }
    }
}

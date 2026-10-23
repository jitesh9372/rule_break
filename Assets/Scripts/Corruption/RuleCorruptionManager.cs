// =============================================================================
// RuleCorruptionManager.cs — RULE//BREAK
// Detects corruptible objects in the player's crosshair and applies Rule Corruption.
// Called by PlayerController on R key press.
// =============================================================================
using UnityEngine;
using RuleBreak.Level;
using RuleBreak.Audio;

namespace RuleBreak.Corruption
{
    public class RuleCorruptionManager : MonoBehaviour
    {
        public static RuleCorruptionManager Instance { get; private set; }

        [SerializeField] private float detectRange = 5f;
        [SerializeField] private LayerMask detectMask = ~0;

        // Currently targeted object (for HUD prompt)
        public CorruptibleObject CurrentTarget { get; private set; }

        public event System.Action<CorruptibleObject> OnTargetChanged;

        private void Awake()
        {
            if (Instance != null && Instance != this) { Destroy(gameObject); return; }
            Instance = this;
        }

        private void Update()
        {
            UpdateTargetDetection();
        }

        private void UpdateTargetDetection()
        {
            var cam = Camera.main;
            if (cam == null) return;

            CorruptibleObject found = null;
            if (Physics.Raycast(cam.transform.position, cam.transform.forward,
                                out RaycastHit hit, detectRange, detectMask))
            {
                found = hit.collider.GetComponentInParent<CorruptibleObject>();
            }

            if (found != CurrentTarget)
            {
                CurrentTarget = found;
                OnTargetChanged?.Invoke(CurrentTarget);
            }
        }

        // Called by PlayerController on R key
        public void TryCorrupt(Transform playerTransform, Transform cameraTransform)
        {
            if (CurrentTarget == null) return;

            var energy = GlitchEnergyManager.Instance;
            if (energy != null)
            {
                if (!energy.CanAfford(energy.CostCorruption))
                {
                    AudioManager.Instance?.PlayEnergyDenied();
                    Debug.Log("Rule Corruption: Insufficient energy!");
                    return;
                }
                energy.Spend(energy.CostCorruption);
            }

            bool success = CurrentTarget.TryCorrupt();
            if (success)
            {
                RuleBreak.Effects.SciFiEffectSystem.Instance?.PlayCorruptionGlitch(CurrentTarget.transform.position);
                RuleBreak.UI.HUDController.Instance?.PulseAbilityIndicator("R");
                Debug.Log($"Rule Corruption applied to: {CurrentTarget.name} → {CurrentTarget.Type}");
            }
            else
            {
                Debug.Log("Rule Corruption: Object cannot be further corrupted.");
            }
        }
    }
}

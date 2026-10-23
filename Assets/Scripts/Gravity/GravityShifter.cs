// =============================================================================
// GravityShifter.cs — RULE//BREAK (Updated with energy cost)
// Attach to the same GameObject as PlayerController and Rigidbody.
// Integrates with GlitchEnergyManager for the 20-energy cost.
// =============================================================================
using UnityEngine;
using RuleBreak.Player;
using RuleBreak.Level;
using RuleBreak.Audio;
using RuleBreak.Effects;
using RuleBreak.UI;

namespace RuleBreak.Gravity
{
    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(PlayerController))]
    public class GravityShifter : MonoBehaviour
    {
        [Header("Gravity")]
        [SerializeField] private float gravityStrength = 20f;
        [SerializeField] private float rotateSpeed     = 5f;

        [Header("Shift Ability")]
        [SerializeField] private KeyCode shiftKey    = KeyCode.G;
        [SerializeField] private float   cooldown    = 1f;
        [SerializeField] private float   launchImpulse = 4f;

        [Header("Debug")]
        [SerializeField] private bool debugLog = false;

        private Rigidbody        _rb;
        private PlayerController _controller;

        private int   _gravitySign   = 1;
        private float _cooldownTimer = 0f;

        private Vector3    _targetUp  = Vector3.up;
        private Quaternion _targetRot = Quaternion.identity;

        private void Awake()
        {
            _rb         = GetComponent<Rigidbody>();
            _controller = GetComponent<PlayerController>();
            _rb.useGravity = false;
            _targetRot = transform.rotation;
            _targetUp  = Vector3.up;
        }

        private void Update()
        {
            if (_cooldownTimer > 0f) _cooldownTimer -= UnityEngine.Time.deltaTime;

            if (Input.GetKeyDown(shiftKey) && _cooldownTimer <= 0f)
            {
                // Check controller input is enabled
                if (_controller != null && !_controller.InputEnabled) return;

                // Energy check
                var energy = GlitchEnergyManager.Instance;
                if (energy != null)
                {
                    if (!energy.CanAfford(energy.CostGravity))
                    {
                        AudioManager.Instance?.PlayEnergyDenied();
                        Debug.Log("Gravity Shift: Insufficient energy!");
                        return;
                    }
                    energy.Spend(energy.CostGravity);
                }

                GameProgressManager.Instance?.RegisterGravityUse();
                ToggleGravity();
            }
        }

        private void FixedUpdate()
        {
            ApplyCustomGravity();
            SmoothRotateBody();
        }

        private void ToggleGravity()
        {
            _gravitySign *= -1;
            _cooldownTimer = cooldown;

            Vector3 newDown = _gravitySign > 0 ? Vector3.down : Vector3.up;
            Vector3 newUp   = -newDown;

            _controller.SetGravityDirection(newDown);
            _targetUp  = newUp;
            _targetRot = Quaternion.FromToRotation(transform.up, newUp) * transform.rotation;

            _rb.linearVelocity = Vector3.zero;
            _rb.AddForce(newUp * launchImpulse, ForceMode.Impulse);

            AudioManager.Instance?.PlayGravityShift();
            GetComponent<CapsuleProceduralAnimator>()?.TriggerAbilityPulse();
            SciFiEffectSystem.Instance?.PlayGravityPulse(transform.position, newUp);
            HUDController.Instance?.PulseAbilityIndicator("G");

            if (debugLog) Debug.Log($"GravityShifter: sign={_gravitySign}, down={newDown}");
        }

        private void ApplyCustomGravity()
        {
            Vector3 gravDir = _gravitySign > 0 ? Vector3.down : Vector3.up;
            _rb.AddForce(gravDir * gravityStrength, ForceMode.Acceleration);
        }

        private void SmoothRotateBody()
        {
            Quaternion tiltCorr = Quaternion.FromToRotation(transform.up, _targetUp);
            Quaternion desired  = tiltCorr * transform.rotation;
            transform.rotation  = Quaternion.Slerp(transform.rotation, desired,
                                                   UnityEngine.Time.fixedDeltaTime * rotateSpeed * 10f);
        }

        // ── Public API ────────────────────────────────────────────────────────
        public bool IsGravityReversed => _gravitySign < 0;

        public void SetGravityReversed(bool reversed)
        {
            int desired = reversed ? -1 : 1;
            if (desired == _gravitySign) return;

            // Force a flip without energy cost (used by respawn)
            _gravitySign = desired;
            Vector3 newDown = reversed ? Vector3.up : Vector3.down;
            Vector3 newUp   = -newDown;
            _controller.SetGravityDirection(newDown);
            _targetUp = newUp;
            _rb.linearVelocity = Vector3.zero;
        }

        // ── Editor ────────────────────────────────────────────────────────────
        private void OnDrawGizmosSelected()
        {
            Gizmos.color = IsGravityReversed ? Color.cyan : Color.yellow;
            Gizmos.DrawRay(transform.position, (IsGravityReversed ? Vector3.up : Vector3.down) * 2f);
        }
    }
}

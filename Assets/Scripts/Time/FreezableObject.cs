// =============================================================================
// FreezableObject.cs — RULE//BREAK
// Add to any moving/rotating object that should stop during Time Freeze.
// Stores velocity, angular velocity, and animator state before freezing.
// =============================================================================
using UnityEngine;
// Do NOT import using RuleBreak.Time here — this file IS in that namespace.
// Self-referencing usings cause CS0246 on some Unity versions.

namespace RuleBreak.Time
{
    public class FreezableObject : MonoBehaviour
    {
        private Rigidbody                   _rb;
        private Animator                    _anim;
        private MovingPlatformFreezeAdapter _movingPlatform;

        // Saved state
        private Vector3 _savedVelocity;
        private Vector3 _savedAngularVelocity;
        private float   _savedAnimSpeed;
        private bool    _frozen;

        private void Awake()
        {
            _rb             = GetComponent<Rigidbody>();
            _anim           = GetComponent<Animator>();
            _movingPlatform = GetComponent<MovingPlatformFreezeAdapter>();
        }

        private void OnEnable()  => TimeFreezeManager.Instance?.Register(this);
        private void OnDisable() => TimeFreezeManager.Instance?.Unregister(this);

        public void DoFreeze()
        {
            if (_frozen) return;
            _frozen = true;

            if (_rb != null)
            {
                _savedVelocity        = _rb.linearVelocity;
                _savedAngularVelocity = _rb.angularVelocity;
                _rb.linearVelocity    = Vector3.zero;
                _rb.angularVelocity   = Vector3.zero;
                _rb.isKinematic       = true;
            }

            if (_anim != null)
            {
                _savedAnimSpeed = _anim.speed;
                _anim.speed     = 0f;
            }

            _movingPlatform?.SetFrozen(true);
        }

        public void DoUnfreeze()
        {
            if (!_frozen) return;
            _frozen = false;

            if (_rb != null)
            {
                _rb.isKinematic     = false;
                _rb.linearVelocity  = _savedVelocity;
                _rb.angularVelocity = _savedAngularVelocity;
            }

            if (_anim != null)
                _anim.speed = _savedAnimSpeed;

            _movingPlatform?.SetFrozen(false);
        }
    }

    // ── Adapter to pause script-driven platforms ───────────────────────────────
    public class MovingPlatformFreezeAdapter : MonoBehaviour
    {
        private bool _frozen;
        public bool IsFrozen  => _frozen;
        public void SetFrozen(bool v) => _frozen = v;
    }
}

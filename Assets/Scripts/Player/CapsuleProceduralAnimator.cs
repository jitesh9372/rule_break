// =============================================================================
// CapsuleProceduralAnimator.cs — RULE//BREAK
// Provides rich procedural animation for the player capsule:
// - Idle breathing and subtle bobbing
// - Running forward-tilt and lateral stride bob
// - Jump anticipation and airborne stretch
// - High-impact landing squash and elastic spring recovery
// - Ability activation flash reaction
// Does not alter the Rigidbody / CapsuleCollider bounds.
// =============================================================================
using UnityEngine;

namespace RuleBreak.Player
{
    public class CapsuleProceduralAnimator : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private Transform visualTransform;
        [SerializeField] private PlayerController controller;
        [SerializeField] private Rigidbody rb;

        [Header("Idle Bob")]
        [SerializeField] private float idleBobSpeed = 2.5f;
        [SerializeField] private float idleBobAmount = 0.035f;

        [Header("Movement Bobbing & Lean")]
        [SerializeField] private float walkBobSpeed = 10f;
        [SerializeField] private float sprintBobSpeed = 16f;
        [SerializeField] private float walkBobAmount = 0.07f;
        [SerializeField] private float maxLeanAngle = 10f;

        [Header("Squash and Stretch")]
        [SerializeField] private float jumpStretchY = 1.25f;
        [SerializeField] private float landSquashY = 0.75f;
        [SerializeField] private float springSpeed = 12f;

        private Vector3 _initialVisualLocalPos;
        private Vector3 _currentScale = Vector3.one;
        private Vector3 _targetScale = Vector3.one;
        private float _animTime;
        private bool _wasGrounded = true;
        private float _lastVerticalVelocity;

        private void Awake()
        {
            if (controller == null) controller = GetComponent<PlayerController>();
            if (rb == null) rb = GetComponent<Rigidbody>();

            SetupVisualChild();
        }

        private void SetupVisualChild()
        {
            if (visualTransform != null)
            {
                _initialVisualLocalPos = visualTransform.localPosition;
                return;
            }

            // Check if there is already a child named "Visual"
            Transform existing = transform.Find("Visual");
            if (existing != null)
            {
                visualTransform = existing;
                _initialVisualLocalPos = visualTransform.localPosition;
                return;
            }

            // Move root MeshFilter and MeshRenderer to child "Visual"
            var rootMf = GetComponent<MeshFilter>();
            var rootMr = GetComponent<MeshRenderer>();

            if (rootMf != null && rootMr != null)
            {
                var visGo = new GameObject("Visual");
                visGo.transform.SetParent(transform, false);
                visGo.transform.localPosition = Vector3.zero;
                visGo.transform.localRotation = Quaternion.identity;
                visGo.transform.localScale = Vector3.one;

                var childMf = visGo.AddComponent<MeshFilter>();
                childMf.sharedMesh = rootMf.sharedMesh;

                var childMr = visGo.AddComponent<MeshRenderer>();
                childMr.sharedMaterials = rootMr.sharedMaterials;

                Destroy(rootMr);
                Destroy(rootMf);

                visualTransform = visGo.transform;
            }
            else
            {
                // Fallback: visualTransform is just transform
                visualTransform = transform;
            }

            _initialVisualLocalPos = visualTransform.localPosition;
        }

        private void Update()
        {
            if (visualTransform == null) return;

            bool isGrounded = controller != null ? controller.IsGrounded : true;
            Vector3 vel = rb != null ? rb.linearVelocity : Vector3.zero;
            Vector3 gravDown = controller != null ? controller.GravityDown : Vector3.down;
            Vector3 verticalVel = Vector3.Project(vel, gravDown);
            Vector3 lateralVel = vel - verticalVel;
            float horizontalSpeed = lateralVel.magnitude;

            // ── Jump Takeoff & Landing Impact ──────────────────────────────────
            if (_wasGrounded && !isGrounded && verticalVel.sqrMagnitude > 1f)
            {
                // Took off
                _targetScale = new Vector3(1f / Mathf.Sqrt(jumpStretchY), jumpStretchY, 1f / Mathf.Sqrt(jumpStretchY));
            }
            else if (!_wasGrounded && isGrounded)
            {
                // Landed: impact squash proportional to fall velocity
                float impact = Mathf.Clamp01(Mathf.Abs(_lastVerticalVelocity) / 15f);
                float squashY = Mathf.Lerp(0.85f, landSquashY, impact);
                float spreadXZ = 1f / Mathf.Sqrt(squashY);
                _targetScale = new Vector3(spreadXZ, squashY, spreadXZ);
            }
            else
            {
                // Smoothly return target to neutral (1, 1, 1)
                _targetScale = Vector3.MoveTowards(_targetScale, Vector3.one, UnityEngine.Time.deltaTime * springSpeed * 0.5f);
            }

            _wasGrounded = isGrounded;
            _lastVerticalVelocity = Vector3.Dot(vel, gravDown);

            // Interpolate current scale toward target with spring
            _currentScale = Vector3.Lerp(_currentScale, _targetScale, UnityEngine.Time.deltaTime * springSpeed);

            // ── Idle breathing and Movement stride bob ────────────────────────
            float bobOffset = 0f;
            if (isGrounded)
            {
                if (horizontalSpeed < 0.2f)
                {
                    // Idle breathing
                    _animTime += UnityEngine.Time.deltaTime * idleBobSpeed;
                    bobOffset = Mathf.Sin(_animTime) * idleBobAmount;
                }
                else
                {
                    // Stride bobbing
                    float strideRate = (horizontalSpeed > 7f) ? sprintBobSpeed : walkBobSpeed;
                    _animTime += UnityEngine.Time.deltaTime * strideRate;
                    bobOffset = Mathf.Abs(Mathf.Sin(_animTime)) * walkBobAmount;
                }
            }
            else
            {
                // Airborne gentle drift
                _animTime += UnityEngine.Time.deltaTime * 3f;
                bobOffset = Mathf.Sin(_animTime) * 0.02f;
            }

            visualTransform.localPosition = _initialVisualLocalPos + Vector3.up * bobOffset;

            // Apply squash/stretch scale (only if child is separate, or relative)
            if (visualTransform != transform)
            {
                visualTransform.localScale = _currentScale;

                // ── Movement lean ──────────────────────────────────────────────
                if (horizontalSpeed > 0.5f && isGrounded)
                {
                    float lean = Mathf.Clamp(horizontalSpeed / 10f, 0f, 1f) * maxLeanAngle;
                    visualTransform.localRotation = Quaternion.AngleAxis(lean, Vector3.right);
                }
                else
                {
                    visualTransform.localRotation = Quaternion.Slerp(visualTransform.localRotation, Quaternion.identity, UnityEngine.Time.deltaTime * 8f);
                }
            }
        }

        /// <summary>Call on ability trigger for a responsive visual pulse</summary>
        public void TriggerAbilityPulse()
        {
            _targetScale = new Vector3(1.2f, 0.85f, 1.2f);
        }
    }
}

// =============================================================================
// PlayerController.cs — RULE//BREAK (Complete)
// Rigidbody-based third-person movement using Unity's legacy Input Manager.
// Supports gravity direction changes, sprint, pause, and input enable/disable.
//
// Controls:
//   W/A/S/D        – Move
//   Left Shift     – Sprint
//   Space          – Jump
//   G              – Gravity Shift  (GravityShifter handles the flip)
//   T              – Time Freeze    (TimeFreezeManager)
//   E              – Dimension Shift(DimensionManager)
//   R              – Rule Corruption(RuleCorruptionManager)
//   Esc            – Pause
//   F              – Interact
// =============================================================================

using UnityEngine;
using RuleBreak.Level;

namespace RuleBreak.Player
{
    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(CapsuleCollider))]
    public class PlayerController : MonoBehaviour
    {
        // ── Inspector settings ────────────────────────────────────────────────
        [Header("Movement")]
        [SerializeField] private float walkSpeed       = 5f;
        [SerializeField] private float sprintMultiplier = 1.8f;
        [SerializeField] private float acceleration    = 12f;

        [Header("Jumping")]
        [SerializeField] private float jumpForce       = 7f;
        [SerializeField] private float gravityMultiplier = 2.5f;

        [Header("Ground Detection")]
        [SerializeField] private Transform groundCheck;
        [SerializeField] private float groundCheckRadius = 0.3f;
        [SerializeField] private LayerMask groundMask = ~0;

        [Header("Interaction")]
        [SerializeField] private float interactRange   = 3f;

        // ── Private references ────────────────────────────────────────────────
        private Rigidbody  _rb;
        private Transform  _cameraTransform;

        // ── Runtime state ─────────────────────────────────────────────────────
        private Vector2 _moveInput;
        private bool    _jumpRequested;
        private bool    _isSprinting;
        private bool    _isGrounded;
        private bool    _inputEnabled = true;
        private bool    _isPaused     = false;

        // ── Gravity direction (set by GravityShifter) ─────────────────────────
        private Vector3 _gravityDown = Vector3.down;

        public Vector3 GravityDown => _gravityDown;

        /// <summary>Called by GravityShifter. Pass Vector3.down or Vector3.up.</summary>
        public void SetGravityDirection(Vector3 down) => _gravityDown = down.normalized;

        // ── Input enable/disable (used by LevelManager on completion) ─────────
        public void SetInputEnabled(bool enabled) => _inputEnabled = enabled;
        public bool InputEnabled => _inputEnabled;

        // ── Unity lifecycle ───────────────────────────────────────────────────
        private void Awake()
        {
            _rb = GetComponent<Rigidbody>();
            _rb.freezeRotation = true;
            if (Camera.main != null) _cameraTransform = Camera.main.transform;
        }

        private void Update()
        {
            if (!_inputEnabled || _isPaused) return;

            // Basic movement input
            _moveInput.x = Input.GetAxisRaw("Horizontal");
            _moveInput.y = Input.GetAxisRaw("Vertical");
            if (Input.GetButtonDown("Jump")) _jumpRequested = true;
            _isSprinting = Input.GetKey(KeyCode.LeftShift);

            // Ability keys — handled here so they work even during time freeze
            HandleAbilityInput();
        }

        private void FixedUpdate()
        {
            if (_isPaused) return;
            CheckGround();
            ApplyExtraGravity();
            if (_inputEnabled) { MovePlayer(); HandleJump(); }
        }

        // ── Ground detection ──────────────────────────────────────────────────
        private void CheckGround()
        {
            Vector3 checkPos = groundCheck != null
                ? groundCheck.position
                : transform.position + _gravityDown * 0.9f;
            _isGrounded = Physics.CheckSphere(checkPos, groundCheckRadius, groundMask,
                                              QueryTriggerInteraction.Ignore);
        }

        // ── Extra gravity ─────────────────────────────────────────────────────
        private void ApplyExtraGravity()
        {
            if (!_isGrounded)
            {
                float gravMag = Physics.gravity.magnitude;
                _rb.AddForce(_gravityDown * gravMag * (gravityMultiplier - 1f),
                             ForceMode.Acceleration);
            }
        }

        // ── WASD movement ─────────────────────────────────────────────────────
        private void MovePlayer()
        {
            Vector3 playerUp = -_gravityDown;

            Vector3 camForward = _cameraTransform != null
                ? Vector3.ProjectOnPlane(_cameraTransform.forward, playerUp).normalized
                : Vector3.ProjectOnPlane(transform.forward, playerUp).normalized;

            Vector3 camRight = _cameraTransform != null
                ? Vector3.ProjectOnPlane(_cameraTransform.right, playerUp).normalized
                : Vector3.ProjectOnPlane(transform.right, playerUp).normalized;

            Vector3 desiredMove = camForward * _moveInput.y + camRight * _moveInput.x;
            float currentSpeed  = walkSpeed * ((_isSprinting && _moveInput.sqrMagnitude > 0.01f)
                                               ? sprintMultiplier : 1f);

            Vector3 currentVel    = _rb.linearVelocity;
            Vector3 gravAxisVel   = Vector3.Project(currentVel, _gravityDown);
            Vector3 lateralVel    = currentVel - gravAxisVel;
            Vector3 targetVel     = desiredMove * currentSpeed;
            Vector3 velChange     = Vector3.ClampMagnitude(targetVel - lateralVel, currentSpeed);

            _rb.AddForce(velChange * acceleration, ForceMode.Acceleration);

            if (desiredMove.sqrMagnitude > 0.01f)
            {
                Quaternion targetRot = Quaternion.LookRotation(desiredMove, playerUp);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRot,
                                                      UnityEngine.Time.fixedDeltaTime * 10f);
            }
        }

        // ── Jump ──────────────────────────────────────────────────────────────
        private void HandleJump()
        {
            if (_jumpRequested && _isGrounded)
            {
                Vector3 jumpDir = -_gravityDown;
                Vector3 currentVel = _rb.linearVelocity;
                _rb.linearVelocity = currentVel - Vector3.Project(currentVel, _gravityDown);
                _rb.AddForce(jumpDir * jumpForce, ForceMode.Impulse);
            }
            _jumpRequested = false;
        }

        // ── Ability input (delegated to managers) ─────────────────────────────
        private void HandleAbilityInput()
        {
            // G – Gravity Shift (handled by GravityShifter directly, but track usage)
            // T – Time Freeze
            if (Input.GetKeyDown(KeyCode.T))
            {
                RuleBreak.Time.TimeFreezeManager.Instance?.ToggleFreeze();
                GetComponent<CapsuleProceduralAnimator>()?.TriggerAbilityPulse();
            }

            // E – Dimension Shift
            if (Input.GetKeyDown(KeyCode.E))
            {
                RuleBreak.Dimension.DimensionManager.Instance?.ToggleDimension();
                GetComponent<CapsuleProceduralAnimator>()?.TriggerAbilityPulse();
            }

            // R – Rule Corruption
            if (Input.GetKeyDown(KeyCode.R))
            {
                RuleBreak.Corruption.RuleCorruptionManager.Instance?.TryCorrupt(transform, _cameraTransform);
                GetComponent<CapsuleProceduralAnimator>()?.TriggerAbilityPulse();
            }

            // F – Interact
            if (Input.GetKeyDown(KeyCode.F))
                TryInteract();
        }

        private void TryInteract()
        {
            if (_cameraTransform == null) return;
            if (Physics.Raycast(_cameraTransform.position, _cameraTransform.forward,
                                out RaycastHit hit, interactRange))
            {
                var interactable = hit.collider.GetComponent<IInteractable>();
                interactable?.Interact();
            }
        }

        // ── Pause ─────────────────────────────────────────────────────────────
        public void SetPaused(bool paused)
        {
            _isPaused = paused;
            if (paused) _rb.linearVelocity = Vector3.zero;
        }

        // ── Public accessors ──────────────────────────────────────────────────
        public bool IsGrounded => _isGrounded;

        // ── Editor helpers ────────────────────────────────────────────────────
        private void OnDrawGizmosSelected()
        {
            Vector3 checkPos = groundCheck != null
                ? groundCheck.position
                : transform.position + _gravityDown * 0.9f;
            Gizmos.color = _isGrounded ? Color.green : Color.red;
            Gizmos.DrawWireSphere(checkPos, groundCheckRadius);
        }
    }

    // ── Interactable interface ─────────────────────────────────────────────────
    public interface IInteractable
    {
        void Interact();
    }
}

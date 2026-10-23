// =============================================================================
// PlayerCamera.cs — RULE//BREAK (Updated with sensitivity API)
// Third-person orbit camera driven by legacy Input Manager mouse axes.
// =============================================================================
using UnityEngine;
using RuleBreak.Save;

namespace RuleBreak.Player
{
    public class PlayerCamera : MonoBehaviour
    {
        [Header("Target")]
        [SerializeField] private Transform target;
        [SerializeField] private float targetHeightOffset = 1.6f;

        [Header("Orbit")]
        [SerializeField] private float sensitivityX  = 3f;
        [SerializeField] private float sensitivityY  = 2f;
        [SerializeField] private float minPitchAngle = -20f;
        [SerializeField] private float maxPitchAngle = 60f;

        [Header("Distance")]
        [SerializeField] private float distance          = 5f;
        [SerializeField] private float minDistance       = 1f;
        [SerializeField] private float collisionSmoothing = 10f;

        [Header("Follow Smoothing")]
        [SerializeField] private float followSpeed = 10f;

        private float   _yaw;
        private float   _pitch;
        private Vector3 _currentFollowPos;
        private bool    _cursorLocked = true;

        private void Awake()
        {
            if (target == null)
            {
                var playerObj = GameObject.FindGameObjectWithTag("Player") ?? GameObject.Find("Player");
                if (playerObj != null) target = playerObj.transform;
            }

            if (target != null)
                _currentFollowPos = target.position + Vector3.up * targetHeightOffset;

            // Load saved sensitivity
            float saved = SaveManager.GetSensitivity();
            ApplySensitivity(saved);

            LockCursor(true);
        }

        private void LateUpdate()
        {
            if (target == null) return;

            if (_cursorLocked)
            {
                _yaw   += Input.GetAxis("Mouse X") * sensitivityX;
                _pitch -= Input.GetAxis("Mouse Y") * sensitivityY;
                _pitch  = Mathf.Clamp(_pitch, minPitchAngle, maxPitchAngle);
            }

            Vector3 desiredFocus = target.position + Vector3.up * targetHeightOffset;
            _currentFollowPos = Vector3.Lerp(_currentFollowPos, desiredFocus,
                                             UnityEngine.Time.deltaTime * followSpeed);

            Quaternion rotation   = Quaternion.Euler(_pitch, _yaw, 0f);
            Vector3    desiredPos = _currentFollowPos - rotation * Vector3.forward * distance;

            float   actualDistance = distance;
            Vector3 dirFromPivot   = (desiredPos - _currentFollowPos).normalized;

            if (Physics.SphereCast(_currentFollowPos, 0.2f, dirFromPivot,
                                   out RaycastHit hit, distance))
            {
                actualDistance = Mathf.Clamp(hit.distance - 0.1f, minDistance, distance);
            }

            float smoothDist = Mathf.Lerp(
                Vector3.Distance(transform.position, _currentFollowPos),
                actualDistance,
                UnityEngine.Time.deltaTime * collisionSmoothing);

            transform.position = _currentFollowPos - rotation * Vector3.forward * smoothDist;
            transform.rotation = rotation;
        }

        // ── Public API ────────────────────────────────────────────────────────
        public float Yaw => _yaw;

        public void SetTarget(Transform t)
        {
            target = t;
            if (t != null) _currentFollowPos = t.position + Vector3.up * targetHeightOffset;
        }

        public void ApplySensitivity(float value)
        {
            sensitivityX = value;
            sensitivityY = value * 0.67f;
        }

        public void LockCursor(bool locked)
        {
            _cursorLocked        = locked;
            Cursor.lockState     = locked ? CursorLockMode.Locked : CursorLockMode.None;
            Cursor.visible       = !locked;
        }
    }
}

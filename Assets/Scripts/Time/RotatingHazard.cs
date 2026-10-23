// =============================================================================
// RotatingHazard.cs — RULE//BREAK
// A hazard that rotates continuously. Stops during Time Freeze.
// Kills the player on contact (triggers PlayerRespawn.Die()).
// =============================================================================
using UnityEngine;
// NOTE: Use UnityEngine.Time explicitly to avoid ambiguity with the
// RuleBreak.Time namespace this file lives in.

namespace RuleBreak.Time
{
    public class RotatingHazard : MonoBehaviour
    {
        [SerializeField] private Vector3 rotationAxis  = Vector3.up;
        [SerializeField] private float   rotationSpeed = 90f;

        private MovingPlatformFreezeAdapter _freezeAdapter;

        private void Awake()
        {
            _freezeAdapter = gameObject.AddComponent<MovingPlatformFreezeAdapter>();
            // Add FreezableObject to register with TimeFreezeManager
            if (GetComponent<FreezableObject>() == null)
                gameObject.AddComponent<FreezableObject>();
        }

        private void Update()
        {
            if (_freezeAdapter != null && _freezeAdapter.IsFrozen) return;
            // Use UnityEngine.Time explicitly — plain "Time" resolves to this namespace
            transform.Rotate(rotationAxis, rotationSpeed * UnityEngine.Time.deltaTime, Space.Self);
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!other.CompareTag("Player")) return;
            RuleBreak.Player.PlayerRespawn.Instance?.Die();
        }
    }
}

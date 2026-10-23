// =============================================================================
// PlayerRespawn.cs — RULE//BREAK
// Manages respawning: tracks checkpoints, death count, gravity reset.
// Replaces the simple static-variable approach in KillZone.cs.
// =============================================================================
using UnityEngine;
using RuleBreak.Level;
using RuleBreak.Audio;
using RuleBreak.Story;

namespace RuleBreak.Player
{
    public class PlayerRespawn : MonoBehaviour
    {
        public static PlayerRespawn Instance { get; private set; }

        [SerializeField] private Vector3 defaultSpawnPoint = new Vector3(0f, 2f, 0f);
        [SerializeField] private float   respawnDelay      = 0.5f;

        private Vector3 _currentCheckpoint;
        private bool    _isDying = false;

        private void Awake()
        {
            if (Instance != null && Instance != this) { Destroy(gameObject); return; }
            Instance = this;
            _currentCheckpoint = defaultSpawnPoint;

            // Also update the legacy static RespawnPoint for backward compatibility
            RuleBreak.Gameplay.KillZone.RespawnPoint = _currentCheckpoint;
        }

        public void SetCheckpoint(Vector3 pos)
        {
            _currentCheckpoint = pos;
            RuleBreak.Gameplay.KillZone.RespawnPoint = pos;
            AudioManager.Instance?.PlayCheckpoint();
        }

        public void Die()
        {
            if (_isDying) return;
            _isDying = true;

            GameProgressManager.Instance?.RegisterDeath();
            AudioManager.Instance?.PlayDeath();

            // Notify death-reality system
            DeathRealityManager.Instance?.OnPlayerDied();

            Invoke(nameof(DoRespawn), respawnDelay);
        }

        private void DoRespawn()
        {
            _isDying = false;

            var rb = GetComponent<Rigidbody>();
            if (rb != null) rb.linearVelocity = Vector3.zero;

            transform.position = _currentCheckpoint;

            // Reset gravity to normal
            var gravity = GetComponent<RuleBreak.Gravity.GravityShifter>();
            if (gravity != null && gravity.IsGravityReversed)
                gravity.SetGravityReversed(false);

            var ctrl = GetComponent<PlayerController>();
            if (ctrl != null) ctrl.SetGravityDirection(Vector3.down);

            Debug.Log($"Player respawned at {_currentCheckpoint}. Deaths: {GameProgressManager.Instance?.Deaths}");
        }
    }
}

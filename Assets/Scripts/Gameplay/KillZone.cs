// =============================================================================
// KillZone.cs — RULE//BREAK (Updated)
// Now also calls PlayerRespawn.Die() for proper death tracking.
// Keeps the static RespawnPoint for backwards compatibility.
// =============================================================================
using UnityEngine;

namespace RuleBreak.Gameplay
{
    public class KillZone : MonoBehaviour
    {
        public static Vector3 RespawnPoint = new Vector3(0f, 1.5f, 0f);

        private void OnTriggerEnter(Collider other)
        {
            if (!other.CompareTag("Player")) return;

            // Prefer the new respawn system
            var respawn = other.GetComponent<RuleBreak.Player.PlayerRespawn>();
            if (respawn != null)
            {
                respawn.Die();
                return;
            }

            // Fallback: legacy teleport
            other.transform.position = RespawnPoint;
            var rb = other.GetComponent<Rigidbody>();
            if (rb != null) rb.linearVelocity = Vector3.zero;
            Debug.Log("KillZone: legacy respawn to " + RespawnPoint);
        }
    }
}

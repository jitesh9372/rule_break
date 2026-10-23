// =============================================================================
// Checkpoint.cs — RULE//BREAK
// Placed in scene as a trigger. Updates PlayerRespawn when the player enters.
// Backwards-compatible with legacy KillZone.RespawnPoint.
// =============================================================================
using UnityEngine;

namespace RuleBreak.Player
{
    public class Checkpoint : MonoBehaviour
    {
        [SerializeField] private GameObject activeVisual;
        [SerializeField] private Vector3    spawnOffset = new Vector3(0f, 1.5f, 0f);

        private bool _activated = false;

        private void OnTriggerEnter(Collider other)
        {
            if (_activated) return;
            if (!other.CompareTag("Player")) return;

            _activated = true;
            Vector3 spawnPos = transform.position + spawnOffset;

            // Update new system
            PlayerRespawn.Instance?.SetCheckpoint(spawnPos);

            // Also keep legacy static alive
            RuleBreak.Gameplay.KillZone.RespawnPoint = spawnPos;

            if (activeVisual != null) activeVisual.SetActive(true);

            RuleBreak.Effects.SciFiEffectSystem.Instance?.PlayCheckpointBeacon(spawnPos);
            RuleBreak.UI.HUDController.Instance?.ShowCheckpointActivated();
            Debug.Log("Checkpoint activated at " + spawnPos);
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = _activated ? Color.cyan : new Color(0f, 1f, 0.5f, 0.4f);
            Gizmos.DrawWireSphere(transform.position, 1.2f);
        }
    }
}

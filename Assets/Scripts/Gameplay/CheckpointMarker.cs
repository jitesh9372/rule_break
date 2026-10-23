// =============================================================================
// CheckpointMarker.cs — RULE//BREAK (Updated – kept in Gameplay namespace)
// Works with both legacy KillZone.RespawnPoint and new PlayerRespawn.
// =============================================================================
using UnityEngine;

namespace RuleBreak.Gameplay
{
    public class CheckpointMarker : MonoBehaviour
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

            // New system
            RuleBreak.Player.PlayerRespawn.Instance?.SetCheckpoint(spawnPos);
            // Legacy static
            KillZone.RespawnPoint = spawnPos;

            if (activeVisual != null) activeVisual.SetActive(true);

            RuleBreak.UI.HUDController.Instance?.ShowCheckpointActivated();
            Debug.Log("Checkpoint reached at " + spawnPos);
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = _activated ? Color.cyan : new Color(0f, 1f, 1f, 0.3f);
            Gizmos.DrawWireSphere(transform.position, 1.2f);
        }
    }
}

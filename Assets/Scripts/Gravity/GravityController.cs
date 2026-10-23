// =============================================================================
// GravityController.cs — RULE//BREAK
// Global gravity management helper (not used for player-specific gravity).
// Manages world-space gravity settings and surface orientation cues.
// =============================================================================
using UnityEngine;

namespace RuleBreak.Gravity
{
    public class GravityController : MonoBehaviour
    {
        [SerializeField] private float defaultGravity = -20f;

        private void Awake()
        {
            // Set world gravity strength (not direction — GravityShifter handles per-player)
            Physics.gravity = new Vector3(0f, defaultGravity, 0f);
        }

        public void SetGravityStrength(float magnitude)
        {
            Physics.gravity = new Vector3(0f, -Mathf.Abs(magnitude), 0f);
        }
    }
}

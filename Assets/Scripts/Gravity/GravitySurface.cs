// =============================================================================
// GravitySurface.cs — RULE//BREAK
// Tag a surface as a "gravity surface" so the player can interact with it.
// Used as a marker for the gravity puzzle system.
// =============================================================================
using UnityEngine;

namespace RuleBreak.Gravity
{
    public class GravitySurface : MonoBehaviour
    {
        [SerializeField] private bool isFloor   = true;
        [SerializeField] private bool isCeiling = false;

        public bool IsFloor   => isFloor;
        public bool IsCeiling => isCeiling;

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = isCeiling ? new Color(0.6f, 0.2f, 1f, 0.4f) : new Color(0f, 0.9f, 1f, 0.4f);
            Gizmos.DrawWireCube(transform.position, transform.lossyScale * 1.01f);
        }
    }
}

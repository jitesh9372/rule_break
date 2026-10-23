// =============================================================================
// GravityPuzzle.cs — RULE//BREAK
// Trigger zone that forces a specific gravity direction on the player.
// Useful for scripted puzzle transitions or introduction areas.
// =============================================================================
using UnityEngine;

namespace RuleBreak.Gravity
{
    public class GravityPuzzle : MonoBehaviour
    {
        public enum GravityForce { Normal, Reversed }

        [SerializeField] private GravityForce forceGravity = GravityForce.Normal;
        [SerializeField] private bool onlyOnFirstEnter = false;

        private bool _triggered = false;

        private void OnTriggerEnter(Collider other)
        {
            if (onlyOnFirstEnter && _triggered) return;
            if (!other.CompareTag("Player")) return;

            _triggered = true;
            var shifter = other.GetComponent<GravityShifter>();
            if (shifter == null) return;

            bool wantReversed = forceGravity == GravityForce.Reversed;
            if (shifter.IsGravityReversed != wantReversed)
                shifter.SetGravityReversed(wantReversed);
        }
    }
}

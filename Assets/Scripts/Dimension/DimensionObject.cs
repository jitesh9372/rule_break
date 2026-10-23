// =============================================================================
// DimensionObject.cs — RULE//BREAK
// Attach to any object that belongs to only one reality or should behave
// differently per reality. Registers with DimensionManager.
// =============================================================================
using UnityEngine;

namespace RuleBreak.Dimension
{
    public enum DimensionOwnership
    {
        SharedBoth,   // Always present
        OnlyRealityA, // Only visible/active in Reality A
        OnlyRealityB  // Only visible/active in Reality B
    }

    public class DimensionObject : MonoBehaviour
    {
        [Tooltip("Which reality this object belongs to.")]
        [SerializeField] private DimensionOwnership ownership = DimensionOwnership.SharedBoth;

        [Tooltip("If true, the object is deactivated. Otherwise only its renderer is hidden.")]
        [SerializeField] private bool deactivateCompletely = false;

        private Renderer[]  _renderers;
        private Collider[]  _colliders;

        private void Awake()
        {
            _renderers = GetComponentsInChildren<Renderer>();
            _colliders = GetComponentsInChildren<Collider>();
        }

        private void OnEnable()  => DimensionManager.Instance?.Register(this);
        private void OnDisable() => DimensionManager.Instance?.Unregister(this);

        /// <summary>Called by DimensionManager. Shows/hides this object for the given reality.</summary>
        public void Apply(Reality reality)
        {
            bool shouldBeActive = ShouldBeActiveIn(reality);

            if (deactivateCompletely)
            {
                gameObject.SetActive(shouldBeActive);
                return;
            }

            // Toggle renderers and colliders without deactivating the whole object
            foreach (var r in _renderers)
                if (r != null) r.enabled = shouldBeActive;

            foreach (var c in _colliders)
                if (c != null) c.enabled = shouldBeActive;
        }

        private bool ShouldBeActiveIn(Reality r) => ownership switch
        {
            DimensionOwnership.OnlyRealityA => r == Reality.A,
            DimensionOwnership.OnlyRealityB => r == Reality.B,
            _                               => true
        };
    }
}

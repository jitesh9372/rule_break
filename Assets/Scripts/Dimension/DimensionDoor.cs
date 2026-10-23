// =============================================================================
// DimensionDoor.cs — RULE//BREAK
// A door that is only passable in one dimension.
// =============================================================================
using UnityEngine;

namespace RuleBreak.Dimension
{
    public class DimensionDoor : MonoBehaviour
    {
        [SerializeField] private Reality openInReality = Reality.B;

        private Collider[] _colliders;

        private void Awake() => _colliders = GetComponentsInChildren<Collider>();

        private void OnEnable()
        {
            if (DimensionManager.Instance != null)
                DimensionManager.Instance.OnDimensionChanged += OnDimensionChanged;
        }

        private void OnDisable()
        {
            if (DimensionManager.Instance != null)
                DimensionManager.Instance.OnDimensionChanged -= OnDimensionChanged;
        }

        private void Start()
        {
            // Apply current state
            if (DimensionManager.Instance != null)
                OnDimensionChanged(DimensionManager.Instance.CurrentReality);
        }

        private void OnDimensionChanged(Reality reality)
        {
            bool blocked = (reality != openInReality);
            foreach (var c in _colliders)
                if (c != null && !c.isTrigger) c.enabled = blocked;
        }
    }
}

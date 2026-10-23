// =============================================================================
// MovingPlatform.cs — RULE//BREAK (Updated: Freeze-aware)
// Oscillates between two world-space points. Respects TimeFreezeManager.
// =============================================================================
using UnityEngine;

namespace RuleBreak.Gameplay
{
    public class MovingPlatform : MonoBehaviour
    {
        [SerializeField] private Vector3 pointA;
        [SerializeField] private Vector3 pointB = new Vector3(6f, 0f, 0f);
        [SerializeField] private float   speed  = 2f;

        private Vector3 _target;
        private RuleBreak.Time.MovingPlatformFreezeAdapter _freezeAdapter;

        private void Awake()
        {
            if (pointA == Vector3.zero) pointA = transform.position;
            _target = pointB;

            // Add freeze adapter so FreezableObject can pause this script
            _freezeAdapter = gameObject.GetComponent<RuleBreak.Time.MovingPlatformFreezeAdapter>();
            if (_freezeAdapter == null)
                _freezeAdapter = gameObject.AddComponent<RuleBreak.Time.MovingPlatformFreezeAdapter>();
        }

        private void Update()
        {
            // Respect time freeze
            if (_freezeAdapter != null && _freezeAdapter.IsFrozen) return;

            transform.position = Vector3.MoveTowards(transform.position, _target, speed * UnityEngine.Time.deltaTime);
            if (Vector3.Distance(transform.position, _target) < 0.05f)
                _target = (_target == pointB) ? pointA : pointB;
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.cyan;
            Vector3 a = (pointA == Vector3.zero) ? transform.position : pointA;
            Gizmos.DrawLine(a, pointB);
            Gizmos.DrawWireSphere(a, 0.2f);
            Gizmos.DrawWireSphere(pointB, 0.2f);
        }
    }
}

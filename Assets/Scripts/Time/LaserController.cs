// =============================================================================
// LaserController.cs — RULE//BREAK
// A moving or static laser hazard. Stops during Time Freeze.
// The laser itself is a LineRenderer between two transforms.
// =============================================================================
using UnityEngine;
// NOTE: Use UnityEngine.Time explicitly — plain "Time" would be ambiguous
// with the enclosing RuleBreak.Time namespace.

namespace RuleBreak.Time
{
    [RequireComponent(typeof(LineRenderer))]
    public class LaserController : MonoBehaviour
    {
        [Header("Laser Endpoints")]
        [SerializeField] private Transform startPoint;
        [SerializeField] private Transform endPoint;

        [Header("Movement (optional)")]
        [SerializeField] private bool  moves     = false;
        [SerializeField] private float moveSpeed = 1f;
        [SerializeField] private float moveRange = 3f;

        private LineRenderer                _lr;
        private MovingPlatformFreezeAdapter _freezeAdapter;
        private Vector3                     _originPos;

        private void Awake()
        {
            _lr = GetComponent<LineRenderer>();
            _lr.positionCount = 2;
            _lr.startWidth    = 0.05f;
            _lr.endWidth      = 0.05f;
            _lr.material      = new Material(Shader.Find("Unlit/Color"));
            _lr.material.color = new Color(1f, 0.1f, 0.1f);

            _freezeAdapter = gameObject.AddComponent<MovingPlatformFreezeAdapter>();
            if (GetComponent<FreezableObject>() == null)
                gameObject.AddComponent<FreezableObject>();

            _originPos = transform.position;
        }

        private void Update()
        {
            bool frozen = _freezeAdapter != null && _freezeAdapter.IsFrozen;

            if (!frozen && moves)
            {
                // Use UnityEngine.Time explicitly to avoid namespace ambiguity
                float offset = Mathf.Sin(UnityEngine.Time.time * moveSpeed) * moveRange;
                transform.position = _originPos + transform.right * offset;
            }

            // Update line renderer
            if (startPoint != null && endPoint != null)
            {
                _lr.SetPosition(0, startPoint.position);
                _lr.SetPosition(1, endPoint.position);
            }
            else
            {
                _lr.SetPosition(0, transform.position);
                _lr.SetPosition(1, transform.position + transform.forward * 10f);
            }
        }

        // Laser kill zone — handled by a separate trigger child called "LaserHitbox"
    }
}

// =============================================================================
// CorruptibleObject.cs — RULE//BREAK
// Attach to any object that can be transformed by Rule Corruption (R key).
// Defines what the object becomes when corrupted and handles state changes.
// =============================================================================
using UnityEngine;

namespace RuleBreak.Corruption
{
    public class CorruptibleObject : MonoBehaviour
    {
        [Header("Corruption Config")]
        [SerializeField] private CorruptionType corruptionType = CorruptionType.LaserToPlatform;
        [SerializeField] private bool           isCorrupted    = false;
        [SerializeField] private bool           canRevert      = false;

        [Header("Visuals")]
        [SerializeField] private Color normalColor    = Color.red;
        [SerializeField] private Color corruptedColor = Color.magenta;

        [Header("Bounce (SpikeToBouncePad)")]
        [SerializeField] private float bounceForce = 12f;

        [Header("Portal (DoorToPortal)")]
        [SerializeField] private string portalTargetScene = "";
        [SerializeField] private Transform portalTeleportTarget;

        // Internal components
        private Collider[]  _colliders;
        private Renderer[]  _renderers;
        private MonoBehaviour _laserCtrl;
        private MonoBehaviour _movingPlatform;

        public bool IsCorrupted => isCorrupted;
        public CorruptionType Type => corruptionType;

        private void Awake()
        {
            _colliders     = GetComponentsInChildren<Collider>(true);
            _renderers     = GetComponentsInChildren<Renderer>(true);
            _laserCtrl     = GetComponent<RuleBreak.Time.LaserController>();
            _movingPlatform = GetComponent<RuleBreak.Gameplay.MovingPlatform>();
            ApplyState(isCorrupted);
        }

        // ── Called by RuleCorruptionManager ───────────────────────────────────
        public bool TryCorrupt()
        {
            if (isCorrupted && !canRevert) return false;
            isCorrupted = !isCorrupted;
            ApplyState(isCorrupted);
            RuleBreak.Audio.AudioManager.Instance?.PlayCorruption();
            RuleBreak.Level.GameProgressManager.Instance?.RegisterCorruption();
            return true;
        }

        private void ApplyState(bool corrupted)
        {
            SetColor(corrupted ? corruptedColor : normalColor);

            switch (corruptionType)
            {
                case CorruptionType.LaserToPlatform:
                    ApplyLaserToPlatform(corrupted); break;

                case CorruptionType.SpikeToBouncePad:
                    // Spikes become non-lethal and bounce; handled in OnTriggerEnter below
                    // Colliders stay – but now trigger → solid for bounce, not death
                    foreach (var c in _colliders)
                        if (c != null) c.isTrigger = corrupted;
                    break;

                case CorruptionType.WallToPassable:
                    foreach (var c in _colliders)
                        if (c != null) c.enabled = !corrupted;
                    break;

                case CorruptionType.MovingToStatic:
                    if (_movingPlatform != null) _movingPlatform.enabled = !corrupted;
                    break;

                case CorruptionType.DoorToPortal:
                    foreach (var c in _colliders)
                        if (c != null) c.isTrigger = corrupted;
                    break;
            }

            // Laser: disable when turned into platform
            if (_laserCtrl != null) _laserCtrl.enabled = !corrupted;
        }

        private void ApplyLaserToPlatform(bool corrupted)
        {
            foreach (var c in _colliders)
            {
                if (c == null) continue;
                c.isTrigger = !corrupted; // when platform, make solid
                c.enabled   = true;
            }
        }

        private void SetColor(Color col)
        {
            foreach (var r in _renderers)
                if (r != null && r.material != null)
                    r.material.color = col;
        }

        // ── Bounce pad & portal trigger ───────────────────────────────────────
        private void OnTriggerEnter(Collider other)
        {
            if (!isCorrupted) return;
            if (!other.CompareTag("Player")) return;

            if (corruptionType == CorruptionType.SpikeToBouncePad)
            {
                var rb = other.GetComponent<Rigidbody>();
                if (rb != null)
                {
                    var ctrl = other.GetComponent<RuleBreak.Player.PlayerController>();
                    Vector3 up = ctrl != null ? -ctrl.GravityDown : Vector3.up;
                    rb.linearVelocity = Vector3.zero;
                    rb.AddForce(up * bounceForce, ForceMode.Impulse);
                }
            }
            else if (corruptionType == CorruptionType.DoorToPortal)
            {
                if (!string.IsNullOrEmpty(portalTargetScene))
                    UnityEngine.SceneManagement.SceneManager.LoadScene(portalTargetScene);
                else if (portalTeleportTarget != null)
                    other.transform.position = portalTeleportTarget.position;
            }
        }

        // ── Gizmo ─────────────────────────────────────────────────────────────
        private void OnDrawGizmos()
        {
            Gizmos.color = isCorrupted ? new Color(1f, 0f, 1f, 0.5f) : new Color(1f, 0.5f, 0f, 0.3f);
            Gizmos.DrawWireCube(transform.position, transform.lossyScale);
        }
    }
}

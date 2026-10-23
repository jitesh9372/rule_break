// =============================================================================
// LevelExit.cs — RULE//BREAK
// Triggers level completion when the player enters the exit portal.
// Delegates to LevelManager so completion logic is centralised.
// =============================================================================
using UnityEngine;
using UnityEngine.SceneManagement;
using RuleBreak.Level;
using RuleBreak.Audio;

namespace RuleBreak.Gameplay
{
    public class ExitPortal : MonoBehaviour
    {
        [SerializeField] private string nextSceneName = "Level02";
        [SerializeField] private GameObject portalVisual;

        private bool _triggered = false;

        private void Update()
        {
            if (portalVisual != null)
            {
                float pulse = 1f + Mathf.Sin(UnityEngine.Time.time * 2f) * 0.05f;
                portalVisual.transform.localScale = Vector3.one * pulse;
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (_triggered) return;
            if (!other.CompareTag("Player")) return;
            _triggered = true;

            // Level 04 ending cinematic
            if (RuleBreak.Story.FinalRevealManager.Instance != null)
            {
                RuleBreak.Story.FinalRevealManager.Instance.TriggerReveal();
                return;
            }

            // Prefer LevelManager completion flow
            if (LevelManager.Instance != null)
            {
                LevelManager.Instance.CompleteLevel();
            }
            else
            {
                // Direct load fallback
                AudioManager.Instance?.PlayPortal();
                SceneManager.LoadScene(nextSceneName);
            }
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = new Color(0f, 1f, 1f, 0.4f);
            Gizmos.DrawWireSphere(transform.position, 1.5f);
        }
    }
}

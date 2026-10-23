// =============================================================================
// FinalRevealManager.cs — RULE//BREAK
// Triggers the final reveal sequence after the player completes Level 04.
// =============================================================================
using UnityEngine;
using System.Collections;
using RuleBreak.Level;
using RuleBreak.Save;
using RuleBreak.Audio;
using RuleBreak.UI;
using UnityEngine.SceneManagement;

namespace RuleBreak.Story
{
    public class FinalRevealManager : MonoBehaviour
    {
        public static FinalRevealManager Instance { get; private set; }

        [SerializeField] private GameObject revealPanel;
        [SerializeField] private FinalRevealUI revealUI;

        private void Awake()
        {
            if (Instance != null && Instance != this) { Destroy(gameObject); return; }
            Instance = this;
        }

        public void TriggerReveal()
        {
            StartCoroutine(RevealSequence());
        }

        private IEnumerator RevealSequence()
        {
            // Disable player input
            var player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                var ctrl = player.GetComponent<RuleBreak.Player.PlayerController>();
                if (ctrl != null) ctrl.SetInputEnabled(false);
            }

            // Play music
            AudioManager.Instance?.PlayLevelMusic(99);

            yield return new WaitForSeconds(1f);

            // Set integrity to 0
            RealityIntegrityManager.Instance?.SetIntegrity(0f);

            // Save completion
            int score = GameProgressManager.Instance?.CalculateFinalScore() ?? 0;
            int frags = SaveManager.GetFragments();
            int secrets = SaveManager.GetSecrets();

            SaveManager.SetGameComplete();
            SaveManager.TrySetBestScore(4, score);

            // Show reveal UI
            if (revealPanel != null) revealPanel.SetActive(true);
            revealUI?.ShowReveal(score, frags, secrets);

            Debug.Log($"Final Reveal triggered. Score: {score}, Fragments: {frags}, Secrets: {secrets}");
        }
    }
}

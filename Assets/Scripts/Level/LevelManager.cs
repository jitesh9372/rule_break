// =============================================================================
// LevelManager.cs — RULE//BREAK
// Coordinates level start, completion, and scene transitions.
// One LevelManager exists per gameplay scene.
// =============================================================================
using UnityEngine;
using UnityEngine.SceneManagement;
using RuleBreak.Save;
using RuleBreak.Audio;

namespace RuleBreak.Level
{
    public class LevelManager : MonoBehaviour
    {
        public static LevelManager Instance { get; private set; }

        [Header("Level Identity")]
        [SerializeField] private int   levelIndex = 1;       // 1-4
        [SerializeField] private float realityIntegrityTarget = 82f; // shown at completion

        [Header("Completion Screen")]
        [SerializeField] private GameObject levelCompletePanel;

        // Prevent double-completion
        private bool _completed = false;

        public int LevelIndex => levelIndex;
        public bool IsCompleted => _completed;

        // Event that UI can subscribe to
        public event System.Action OnLevelComplete;

        private void Awake()
        {
            if (Instance != null && Instance != this) { Destroy(gameObject); return; }
            Instance = this;
        }

        private void Start()
        {
            // Notify audio manager which level music to play
            AudioManager.Instance?.PlayLevelMusic(levelIndex);

            // Set integrity target for this level
            RealityIntegrityManager.Instance?.SetIntegrity(realityIntegrityTarget + 18f); // slight buffer
        }

        // ── Called by LevelExit ───────────────────────────────────────────────
        public void CompleteLevel()
        {
            if (_completed) return;
            _completed = true;

            // Unlock next level
            SaveManager.UnlockLevel(levelIndex + 1);

            // Score
            int score = GameProgressManager.Instance?.CalculateFinalScore() ?? 0;
            SaveManager.TrySetBestScore(levelIndex, score);

            // SFX
            AudioManager.Instance?.PlayLevelComplete();

            // Set integrity milestone
            RealityIntegrityManager.Instance?.SetIntegrity(realityIntegrityTarget);

            // Show UI
            if (levelCompletePanel != null)
                levelCompletePanel.SetActive(true);

            // Disable player input
            var player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                var ctrl = player.GetComponent<RuleBreak.Player.PlayerController>();
                if (ctrl != null) ctrl.SetInputEnabled(false);
            }

            OnLevelComplete?.Invoke();
            Debug.Log($"Level {levelIndex} complete. Score: {score}");
        }

        // ── Scene Loading ─────────────────────────────────────────────────────
        public void LoadNextLevel()
        {
            int next = levelIndex + 1;
            string sceneName = next <= 4 ? $"Level0{next}" : "MainMenu";
            SceneManager.LoadScene(sceneName);
        }

        public void LoadMainMenu() => SceneManager.LoadScene("MainMenu");

        public void RestartLevel() => SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}

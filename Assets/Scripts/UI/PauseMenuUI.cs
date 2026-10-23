// =============================================================================
// PauseMenuUI.cs — RULE//BREAK
// Handles ESC pause/resume. Does NOT use Time.timeScale alone (breaks Freeze).
// Uses a dedicated paused flag that the player controller respects.
// =============================================================================
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

namespace RuleBreak.UI
{
    public class PauseMenuUI : MonoBehaviour
    {
        [Header("Panel")]
        [SerializeField] private GameObject pausePanel;

        [Header("Buttons")]
        [SerializeField] private Button resumeButton;
        [SerializeField] private Button restartButton;
        [SerializeField] private Button restartLevelButton;
        [SerializeField] private Button settingsButton;
        [SerializeField] private Button mainMenuButton;

        [Header("Settings Panel")]
        [SerializeField] private SettingsUI settingsUI;

        private bool _paused = false;

        private void Awake()
        {
            resumeButton?.onClick.AddListener(Resume);
            restartButton?.onClick.AddListener(RestartCheckpoint);
            restartLevelButton?.onClick.AddListener(RestartLevel);
            settingsButton?.onClick.AddListener(OpenSettings);
            mainMenuButton?.onClick.AddListener(GoToMainMenu);
            pausePanel?.SetActive(false);
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                if (settingsUI != null && settingsUI.IsOpen)
                    settingsUI.Close();
                else
                    TogglePause();
            }
        }

        public void TogglePause()
        {
            _paused = !_paused;

            pausePanel?.SetActive(_paused);
            Cursor.lockState = _paused ? CursorLockMode.None : CursorLockMode.Locked;
            Cursor.visible   = _paused;

            // Tell the player controller to halt (keeps physics running so freeze works)
            var player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                var ctrl = player.GetComponent<RuleBreak.Player.PlayerController>();
                ctrl?.SetPaused(_paused);
            }
        }

        private void Resume()
        {
            if (_paused) TogglePause();
        }

        private void RestartCheckpoint()
        {
            Resume();
            RuleBreak.Player.PlayerRespawn.Instance?.Die();
        }

        private void RestartLevel()
        {
            Resume();
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }

        private void OpenSettings()
        {
            settingsUI?.Open();
        }

        private void GoToMainMenu()
        {
            Resume();
            SceneManager.LoadScene("MainMenu");
        }
    }
}

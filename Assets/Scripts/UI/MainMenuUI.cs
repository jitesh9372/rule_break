// =============================================================================
// MainMenuUI.cs — RULE//BREAK
// Cinematic main menu manager with New Game, Continue, Level Select,
// Settings, Controls Guide, Credits, and Quit functionality.
// =============================================================================
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using RuleBreak.Save;
using RuleBreak.Audio;

namespace RuleBreak.UI
{
    public class MainMenuUI : MonoBehaviour
    {
        [Header("Main Buttons")]
        [SerializeField] private Button playButton;
        [SerializeField] private Button continueButton;
        [SerializeField] private Button levelSelectButton;
        [SerializeField] private Button settingsButton;
        [SerializeField] private Button controlsButton;
        [SerializeField] private Button creditsButton;
        [SerializeField] private Button quitButton;

        [Header("Modals & Sub-Panels")]
        [SerializeField] private GameObject levelSelectPanel;
        [SerializeField] private SettingsUI settingsUI;
        [SerializeField] private GameObject controlsPanel;
        [SerializeField] private GameObject creditsPanel;
        [SerializeField] private GameObject confirmNewGamePanel;

        [Header("Level Select Buttons")]
        [SerializeField] private Button level1Button;
        [SerializeField] private Button level2Button;
        [SerializeField] private Button level3Button;
        [SerializeField] private Button level4Button;
        [SerializeField] private Button backFromLevelSelectButton;

        [Header("Sub-Panel Close Buttons")]
        [SerializeField] private Button closeControlsButton;
        [SerializeField] private Button closeCreditsButton;

        [Header("New Game Confirm")]
        [SerializeField] private Button confirmYesButton;
        [SerializeField] private Button confirmNoButton;

        [Header("Atmosphere / Camera Rig")]
        [SerializeField] private Transform cameraTransform;

        private void Awake()
        {
            playButton?.onClick.AddListener(OnPlay);
            continueButton?.onClick.AddListener(OnContinue);
            levelSelectButton?.onClick.AddListener(OnLevelSelect);
            settingsButton?.onClick.AddListener(OnSettings);
            controlsButton?.onClick.AddListener(OnControls);
            creditsButton?.onClick.AddListener(OnCredits);
            quitButton?.onClick.AddListener(OnQuit);

            closeControlsButton?.onClick.AddListener(CloseControls);
            closeCreditsButton?.onClick.AddListener(CloseCredits);

            level1Button?.onClick.AddListener(() => LoadLevel(1));
            level2Button?.onClick.AddListener(() => LoadLevel(2));
            level3Button?.onClick.AddListener(() => LoadLevel(3));
            level4Button?.onClick.AddListener(() => LoadLevel(4));
            backFromLevelSelectButton?.onClick.AddListener(CloseLevelSelect);

            confirmYesButton?.onClick.AddListener(ConfirmNewGame);
            confirmNoButton?.onClick.AddListener(CancelNewGame);

            if (levelSelectPanel != null) levelSelectPanel.SetActive(false);
            if (controlsPanel != null) controlsPanel.SetActive(false);
            if (creditsPanel != null) creditsPanel.SetActive(false);
            if (confirmNewGamePanel != null) confirmNewGamePanel.SetActive(false);

            UpdateContinueButton();
            UpdateLevelSelectUI();
        }

        private void Start()
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;

            AudioManager.Instance?.PlayLevelMusic(0);
        }

        private void Update()
        {
            // Gentle ambient camera motion
            if (cameraTransform != null)
            {
                float yaw = Mathf.Sin(UnityEngine.Time.time * 0.3f) * 1.5f;
                float pitch = Mathf.Cos(UnityEngine.Time.time * 0.2f) * 0.8f;
                cameraTransform.localRotation = Quaternion.Euler(pitch, yaw, 0f);
            }
        }

        private void UpdateContinueButton()
        {
            if (continueButton != null)
            {
                bool hasSave = SaveManager.GetUnlockedLevel() > 1 || SaveManager.IsGameComplete();
                continueButton.interactable = hasSave;
                var canvasGroup = continueButton.GetComponent<CanvasGroup>();
                if (canvasGroup != null) canvasGroup.alpha = hasSave ? 1f : 0.4f;
            }
        }

        private void UpdateLevelSelectUI()
        {
            if (level1Button != null) level1Button.interactable = true;
            if (level2Button != null) level2Button.interactable = SaveManager.IsLevelUnlocked(2);
            if (level3Button != null) level3Button.interactable = SaveManager.IsLevelUnlocked(3);
            if (level4Button != null) level4Button.interactable = SaveManager.IsLevelUnlocked(4);
        }

        private void OnPlay()
        {
            AudioManager.Instance?.PlayUIClick();
            if (SaveManager.GetUnlockedLevel() > 1)
            {
                confirmNewGamePanel?.SetActive(true);
            }
            else
            {
                StartNewGame();
            }
        }

        private void ConfirmNewGame()
        {
            AudioManager.Instance?.PlayUIClick();
            confirmNewGamePanel?.SetActive(false);
            SaveManager.ResetAll();
            StartNewGame();
        }

        private void CancelNewGame()
        {
            AudioManager.Instance?.PlayUIClick();
            confirmNewGamePanel?.SetActive(false);
        }

        private void StartNewGame()
        {
            SceneManager.LoadScene("Level01");
        }

        private void OnContinue()
        {
            AudioManager.Instance?.PlayUIClick();
            int level = SaveManager.GetUnlockedLevel();
            string scene = level <= 4 ? $"Level0{level}" : "MainMenu";
            SceneManager.LoadScene(scene);
        }

        private void OnLevelSelect()
        {
            AudioManager.Instance?.PlayUIClick();
            levelSelectPanel?.SetActive(true);
            UpdateLevelSelectUI();
        }

        private void CloseLevelSelect()
        {
            AudioManager.Instance?.PlayUIClick();
            levelSelectPanel?.SetActive(false);
        }

        private void LoadLevel(int index)
        {
            if (!SaveManager.IsLevelUnlocked(index))
            {
                AudioManager.Instance?.PlayEnergyDenied();
                return;
            }
            AudioManager.Instance?.PlayUIClick();
            SceneManager.LoadScene($"Level0{index}");
        }

        private void OnSettings()
        {
            AudioManager.Instance?.PlayUIClick();
            settingsUI?.Open();
        }

        private void OnControls()
        {
            AudioManager.Instance?.PlayUIClick();
            controlsPanel?.SetActive(true);
        }

        private void CloseControls()
        {
            AudioManager.Instance?.PlayUIClick();
            controlsPanel?.SetActive(false);
        }

        private void OnCredits()
        {
            AudioManager.Instance?.PlayUIClick();
            creditsPanel?.SetActive(true);
        }

        private void CloseCredits()
        {
            AudioManager.Instance?.PlayUIClick();
            creditsPanel?.SetActive(false);
        }

        private void OnQuit()
        {
            AudioManager.Instance?.PlayUIClick();
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }
    }
}

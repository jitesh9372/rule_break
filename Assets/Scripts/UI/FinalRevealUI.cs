// =============================================================================
// FinalRevealUI.cs — RULE//BREAK
// Cinematic final-chapter results scene and story reveal sequence for Level 04.
// - Parallax camera drift and rotating holographic energy emblem
// - Glitch text reveal: "SYSTEM BREACHED" & "LEVEL 04 — THE FINAL PROTOCOL"
// - Iconic story reveal: "YOU ARE THE GLITCH. / The system was never designed to contain you."
// - Genuine saved gameplay statistics (Time, Fragments, Deaths, Score, Primary Ability)
// - Polished buttons: Level Select, Restart Level, Main Menu, Credits
// =============================================================================
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;
using RuleBreak.Save;
using RuleBreak.Level;
using RuleBreak.Audio;

namespace RuleBreak.UI
{
    public class FinalRevealUI : MonoBehaviour
    {
        [Header("Cinematic Panels")]
        [SerializeField] private GameObject revealPanel;
        [SerializeField] private GameObject resultsPanel;
        [SerializeField] private GameObject creditsPanel;
        [SerializeField] private GameObject levelSelectPanel;

        [Header("Holographic Emblem / Parallax")]
        [SerializeField] private RectTransform emblemTransform;
        [SerializeField] private Transform cameraRigTransform;

        [Header("Title & Reveal Text Elements")]
        [SerializeField] private Text titleText;
        [SerializeField] private Text subtitleText;
        [SerializeField] private Text storyGlitchText;
        [SerializeField] private Text storySubText;

        [Header("Statistics Readouts")]
        [SerializeField] private Text statusValueText;
        [SerializeField] private Text timeValueText;
        [SerializeField] private Text fragmentsValueText;
        [SerializeField] private Text deathsValueText;
        [SerializeField] private Text scoreValueText;
        [SerializeField] private Text primaryVectorValueText;
        [SerializeField] private Text unlockedValueText;

        [Header("Results Action Buttons")]
        [SerializeField] private Button levelSelectButton;
        [SerializeField] private Button restartLevelButton;
        [SerializeField] private Button mainMenuButton;
        [SerializeField] private Button creditsButton;
        [SerializeField] private Button closeCreditsButton;
        [SerializeField] private Button closeLevelSelectButton;

        [Header("Level Select Direct Buttons")]
        [SerializeField] private Button selectL1Button;
        [SerializeField] private Button selectL2Button;
        [SerializeField] private Button selectL3Button;
        [SerializeField] private Button selectL4Button;

        private bool _isSequenceRunning = false;
        private float _timeCounter = 0f;

        private void Awake()
        {
            levelSelectButton?.onClick.AddListener(OpenLevelSelect);
            restartLevelButton?.onClick.AddListener(RestartCurrentLevel);
            mainMenuButton?.onClick.AddListener(GoToMainMenu);
            creditsButton?.onClick.AddListener(OpenCredits);
            closeCreditsButton?.onClick.AddListener(CloseCredits);
            closeLevelSelectButton?.onClick.AddListener(CloseLevelSelect);

            selectL1Button?.onClick.AddListener(() => LoadLevel(1));
            selectL2Button?.onClick.AddListener(() => LoadLevel(2));
            selectL3Button?.onClick.AddListener(() => LoadLevel(3));
            selectL4Button?.onClick.AddListener(() => LoadLevel(4));

            if (revealPanel != null) revealPanel.SetActive(false);
            if (resultsPanel != null) resultsPanel.SetActive(false);
            if (creditsPanel != null) creditsPanel.SetActive(false);
            if (levelSelectPanel != null) levelSelectPanel.SetActive(false);
        }

        private void Update()
        {
            _timeCounter += UnityEngine.Time.deltaTime;

            // Rotate holographic emblem
            if (emblemTransform != null)
            {
                emblemTransform.Rotate(Vector3.forward, 25f * UnityEngine.Time.deltaTime);
            }

            // Slow ambient camera drift if a camera rig is assigned
            if (cameraRigTransform != null)
            {
                cameraRigTransform.Rotate(Vector3.up, 2f * UnityEngine.Time.deltaTime, Space.World);
            }
        }

        public void ShowReveal(int score, int fragments, int secrets)
        {
            if (_isSequenceRunning) return;
            StartCoroutine(CinematicRevealRoutine(score, fragments, secrets));
        }

        private IEnumerator CinematicRevealRoutine(int finalScore, int totalFragments, int totalSecrets)
        {
            _isSequenceRunning = true;
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;

            if (revealPanel != null) revealPanel.SetActive(true);
            if (resultsPanel != null) resultsPanel.SetActive(false);

            // Audio: play final reveal track
            AudioManager.Instance?.PlayLevelMusic(99);

            // Phase 1: Glitch title reveal
            if (titleText != null)
            {
                string[] glitchFrames = { "S¥ST3M BR3@CH3D", "$Y$T#M BR3ACHED", "SYSTEM BREACHED" };
                for (int i = 0; i < glitchFrames.Length; i++)
                {
                    titleText.text = glitchFrames[i];
                    AudioManager.Instance?.PlayCorruption();
                    yield return new WaitForSeconds(0.18f);
                }
            }

            if (subtitleText != null)
            {
                subtitleText.text = "LEVEL 04 — THE FINAL PROTOCOL";
            }

            yield return new WaitForSeconds(1.2f);

            // Phase 2: Iconic story reveal
            if (storyGlitchText != null)
            {
                storyGlitchText.gameObject.SetActive(true);
                storyGlitchText.text = "YOU ARE THE GLITCH.";
                AudioManager.Instance?.PlayCorruption();
            }

            yield return new WaitForSeconds(1.8f);

            if (storySubText != null)
            {
                storySubText.gameObject.SetActive(true);
                storySubText.text = "The system was never designed to contain you.";
                AudioManager.Instance?.PlayDimensionShift();
            }

            yield return new WaitForSeconds(2.8f);

            // Phase 3: Transition to Results Panel
            if (revealPanel != null) revealPanel.SetActive(false);
            if (resultsPanel != null) resultsPanel.SetActive(true);

            PopulateResults(finalScore, totalFragments, totalSecrets);
            AudioManager.Instance?.PlayLevelComplete();
            _isSequenceRunning = false;
        }

        private void PopulateResults(int score, int fragments, int secrets)
        {
            var gpm = GameProgressManager.Instance;

            // Status
            if (statusValueText != null)
                statusValueText.text = "PROTOCOL STATUS: BREACH COMPLETE [ALL RULES BROKEN]";

            // Real Time Elapsed
            if (timeValueText != null)
            {
                float totalSeconds = gpm != null ? gpm.TotalTime : 0f;
                int mins = Mathf.FloorToInt(totalSeconds / 60f);
                int secs = Mathf.FloorToInt(totalSeconds % 60f);
                timeValueText.text = $"{mins:00}:{secs:00}";
            }

            // Real Collectibles (Fragments)
            if (fragmentsValueText != null)
            {
                int fragsFound = gpm != null ? gpm.FragmentsFound : SaveManager.GetFragments();
                fragmentsValueText.text = $"{fragsFound} / 9 RECOVERED";
            }

            // Real Deaths / Retries
            if (deathsValueText != null)
            {
                int deaths = gpm != null ? gpm.Deaths : SaveManager.GetDeaths();
                deathsValueText.text = $"{deaths} SYSTEM RETRIES";
            }

            // Real Final Score
            if (scoreValueText != null)
            {
                scoreValueText.text = $"{score:N0} PTS";
            }

            // Real Primary Ability Vector
            if (primaryVectorValueText != null)
            {
                string vector = gpm != null ? gpm.GetPrimaryDependency() : "RULE CORRUPTION";
                primaryVectorValueText.text = $"PRIMARY VECTOR: {vector}";
            }

            // Unlocked Content
            if (unlockedValueText != null)
            {
                unlockedValueText.text = "ALL LEVELS UNLOCKED // ARCHIVE PROTOCOLS ACCESSIBLE";
            }

            // Save game completion flag
            SaveManager.SetGameComplete();
            SaveManager.UnlockLevel(4);
        }

        // ── Action Handlers ───────────────────────────────────────────────────
        private void OpenLevelSelect()
        {
            AudioManager.Instance?.PlayUIClick();
            if (levelSelectPanel != null) levelSelectPanel.SetActive(true);
        }

        private void CloseLevelSelect()
        {
            AudioManager.Instance?.PlayUIClick();
            if (levelSelectPanel != null) levelSelectPanel.SetActive(false);
        }

        private void LoadLevel(int level)
        {
            AudioManager.Instance?.PlayUIClick();
            SceneManager.LoadScene($"Level0{level}");
        }

        private void RestartCurrentLevel()
        {
            AudioManager.Instance?.PlayUIClick();
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }

        private void GoToMainMenu()
        {
            AudioManager.Instance?.PlayUIClick();
            SceneManager.LoadScene("MainMenu");
        }

        private void OpenCredits()
        {
            AudioManager.Instance?.PlayUIClick();
            if (creditsPanel != null) creditsPanel.SetActive(true);
        }

        private void CloseCredits()
        {
            AudioManager.Instance?.PlayUIClick();
            if (creditsPanel != null) creditsPanel.SetActive(false);
        }
    }
}

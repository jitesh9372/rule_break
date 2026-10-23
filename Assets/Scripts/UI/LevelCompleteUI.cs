// =============================================================================
// LevelCompleteUI.cs — RULE//BREAK
// Shows the level completion screen with story message, integrity reading,
// and a Continue button that loads the next level.
// =============================================================================
using UnityEngine;
using UnityEngine.UI;
using RuleBreak.Level;

namespace RuleBreak.UI
{
    public class LevelCompleteUI : MonoBehaviour
    {
        [Header("Text Elements")]
        [SerializeField] private Text levelCompleteText;
        [SerializeField] private Text systemStatusText;
        [SerializeField] private Text realityIntegrityText;
        [SerializeField] private Text newAbilityText;
        [SerializeField] private Text storyMessageText;

        [Header("Buttons")]
        [SerializeField] private Button continueButton;
        [SerializeField] private Button mainMenuButton;

        // Level-specific data — set these in the Inspector per level
        [Header("Level Data")]
        [SerializeField] private string completionTitle    = "LEVEL 01 COMPLETE";
        [SerializeField] private string systemStatus       = "GRAVITY SYSTEM: UNSTABLE";
        [SerializeField] private float  integrityDisplay   = 82f;
        [SerializeField] private string newAbilityUnlocked = "NEW ABILITY UNLOCKED\nTIME FREEZE [T]";
        [SerializeField] private string storyMessage       = "\"Something stopped time... but you didn't.\"";

        private void Awake()
        {
            continueButton?.onClick.AddListener(OnContinue);
            mainMenuButton?.onClick.AddListener(OnMainMenu);
        }

        private void OnEnable()
        {
            // Populate UI when panel becomes active
            if (levelCompleteText)    levelCompleteText.text    = completionTitle;
            if (systemStatusText)     systemStatusText.text     = systemStatus;
            if (realityIntegrityText) realityIntegrityText.text = $"REALITY INTEGRITY: {integrityDisplay}%";
            if (newAbilityText)       newAbilityText.text       = newAbilityUnlocked;
            if (storyMessageText)     storyMessageText.text     = storyMessage;

            // Unlock cursor for menu interaction
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible   = true;
        }

        private void OnContinue()
        {
            LevelManager.Instance?.LoadNextLevel();
        }

        private void OnMainMenu()
        {
            LevelManager.Instance?.LoadMainMenu();
        }
    }
}

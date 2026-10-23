// =============================================================================
// InteractionPromptUI.cs — RULE//BREAK
// Updates the interaction/corruption prompt based on what the player is aiming at.
// Works with RuleCorruptionManager for corruption prompts.
// =============================================================================
using UnityEngine;
using UnityEngine.UI;
using RuleBreak.Corruption;

namespace RuleBreak.UI
{
    public class InteractionPromptUI : MonoBehaviour
    {
        [SerializeField] private GameObject panel;
        [SerializeField] private Text       promptText;

        private void Update()
        {
            var mgr = RuleCorruptionManager.Instance;
            if (mgr == null) { Hide(); return; }

            var target = mgr.CurrentTarget;
            if (target == null) { Hide(); return; }

            string state = target.IsCorrupted ? "REVERT" : "CORRUPT";
            string label = target.Type switch
            {
                CorruptionType.LaserToPlatform  => "LASER → PLATFORM",
                CorruptionType.SpikeToBouncePad => "SPIKE → BOUNCE PAD",
                CorruptionType.WallToPassable   => "WALL → PASSABLE",
                CorruptionType.DoorToPortal     => "DOOR → PORTAL",
                CorruptionType.MovingToStatic   => "PLATFORM → STATIC",
                _                               => "OBJECT"
            };

            Show($"[R] {state}: {label}");
        }

        private void Show(string msg)
        {
            if (panel && !panel.activeSelf) panel.SetActive(true);
            if (promptText) promptText.text = msg;
        }

        private void Hide()
        {
            if (panel && panel.activeSelf) panel.SetActive(false);
        }
    }
}

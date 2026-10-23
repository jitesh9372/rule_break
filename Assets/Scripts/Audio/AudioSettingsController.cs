// =============================================================================
// AudioSettingsController.cs — RULE//BREAK
// Runtime controller for applying saved audio settings on scene load.
// Attach to the AudioManager GameObject or a scene initializer.
// =============================================================================
using UnityEngine;
using RuleBreak.Save;

namespace RuleBreak.Audio
{
    public class AudioSettingsController : MonoBehaviour
    {
        private void Start()
        {
            // Apply saved settings on startup
            QualitySettings.SetQualityLevel(SaveManager.GetQuality());
            Screen.fullScreen = SaveManager.GetFullscreen();
            AudioManager.Instance?.ApplyVolumeSettings();
        }
    }
}

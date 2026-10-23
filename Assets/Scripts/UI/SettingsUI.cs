// =============================================================================
// SettingsUI.cs — RULE//BREAK
// Settings panel: volume, sensitivity, fullscreen, quality.
// Persists via SaveManager. Works in both main menu and pause menu contexts.
// =============================================================================
using UnityEngine;
using UnityEngine.UI;
using RuleBreak.Save;
using RuleBreak.Audio;

namespace RuleBreak.UI
{
    public class SettingsUI : MonoBehaviour
    {
        [Header("Panel")]
        [SerializeField] private GameObject settingsPanel;

        [Header("Volume")]
        [SerializeField] private Slider masterVolSlider;
        [SerializeField] private Slider musicVolSlider;
        [SerializeField] private Slider sfxVolSlider;

        [Header("Sensitivity")]
        [SerializeField] private Slider sensitivitySlider;

        [Header("Display")]
        [SerializeField] private Toggle fullscreenToggle;
        [SerializeField] private Dropdown qualityDropdown;

        [Header("Buttons")]
        [SerializeField] private Button closeButton;
        [SerializeField] private Button applyButton;

        public bool IsOpen => settingsPanel != null && settingsPanel.activeSelf;

        private void Awake()
        {
            closeButton?.onClick.AddListener(Close);
            applyButton?.onClick.AddListener(ApplySettings);
            settingsPanel?.SetActive(false);
        }

        public void Open()
        {
            settingsPanel?.SetActive(true);
            LoadCurrentSettings();
        }

        public void Close()
        {
            settingsPanel?.SetActive(false);
        }

        private void LoadCurrentSettings()
        {
            if (masterVolSlider)    masterVolSlider.value    = SaveManager.GetMasterVolume();
            if (musicVolSlider)     musicVolSlider.value     = SaveManager.GetMusicVolume();
            if (sfxVolSlider)       sfxVolSlider.value       = SaveManager.GetSFXVolume();
            if (sensitivitySlider)  sensitivitySlider.value  = SaveManager.GetSensitivity();
            if (fullscreenToggle)   fullscreenToggle.isOn    = SaveManager.GetFullscreen();
            if (qualityDropdown)    qualityDropdown.value    = SaveManager.GetQuality();
        }

        private void ApplySettings()
        {
            if (masterVolSlider)   SaveManager.SetMasterVolume(masterVolSlider.value);
            if (musicVolSlider)    SaveManager.SetMusicVolume(musicVolSlider.value);
            if (sfxVolSlider)      SaveManager.SetSFXVolume(sfxVolSlider.value);
            if (sensitivitySlider) SaveManager.SetSensitivity(sensitivitySlider.value);
            if (fullscreenToggle)  SaveManager.SetFullscreen(fullscreenToggle.isOn);
            if (qualityDropdown)   SaveManager.SetQuality(qualityDropdown.value);

            // Apply immediately
            AudioManager.Instance?.ApplyVolumeSettings();
            QualitySettings.SetQualityLevel(SaveManager.GetQuality());
            Screen.fullScreen = SaveManager.GetFullscreen();

            // Apply camera sensitivity
            var cam = Object.FindAnyObjectByType<RuleBreak.Player.PlayerCamera>();
            if (cam != null) cam.ApplySensitivity(SaveManager.GetSensitivity());

            Close();
        }
    }
}

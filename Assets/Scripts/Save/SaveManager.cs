// =============================================================================
// SaveManager.cs — RULE//BREAK
// Centralised save/load using PlayerPrefs. All keys are prefixed "RB_".
// =============================================================================
using UnityEngine;

namespace RuleBreak.Save
{
    public static class SaveManager
    {
        // ── Keys ──────────────────────────────────────────────────────────────
        const string K_LEVEL_UNLOCKED   = "RB_LevelUnlocked";
        const string K_DEATHS           = "RB_Deaths";
        const string K_FRAGMENTS        = "RB_Fragments";
        const string K_SECRETS          = "RB_Secrets";
        const string K_SCORE_PREFIX     = "RB_BestScore_L";
        const string K_SENSITIVITY      = "RB_Sensitivity";
        const string K_MASTER_VOL       = "RB_VolMaster";
        const string K_MUSIC_VOL        = "RB_VolMusic";
        const string K_SFX_VOL          = "RB_VolSFX";
        const string K_FULLSCREEN       = "RB_Fullscreen";
        const string K_QUALITY          = "RB_Quality";
        const string K_GAME_COMPLETE    = "RB_GameComplete";

        // ── Levels ────────────────────────────────────────────────────────────
        /// <summary>Highest level index unlocked (1-based). Always at least 1.</summary>
        public static int GetUnlockedLevel()  => Mathf.Max(1, PlayerPrefs.GetInt(K_LEVEL_UNLOCKED, 1));
        public static void UnlockLevel(int lvl)
        {
            if (lvl > GetUnlockedLevel())
                PlayerPrefs.SetInt(K_LEVEL_UNLOCKED, lvl);
            PlayerPrefs.Save();
        }
        public static bool IsLevelUnlocked(int lvl) => lvl <= GetUnlockedLevel();

        // ── Score ─────────────────────────────────────────────────────────────
        public static int GetBestScore(int level)  => PlayerPrefs.GetInt(K_SCORE_PREFIX + level, 0);
        public static void TrySetBestScore(int level, int score)
        {
            if (score > GetBestScore(level))
                PlayerPrefs.SetInt(K_SCORE_PREFIX + level, score);
            PlayerPrefs.Save();
        }

        // ── Stats ─────────────────────────────────────────────────────────────
        public static int GetDeaths()    => PlayerPrefs.GetInt(K_DEATHS, 0);
        public static void AddDeath()    { PlayerPrefs.SetInt(K_DEATHS, GetDeaths() + 1); PlayerPrefs.Save(); }

        public static int GetFragments() => PlayerPrefs.GetInt(K_FRAGMENTS, 0);
        public static void AddFragment() { PlayerPrefs.SetInt(K_FRAGMENTS, GetFragments() + 1); PlayerPrefs.Save(); }
        public static void SetFragments(int v) { PlayerPrefs.SetInt(K_FRAGMENTS, v); PlayerPrefs.Save(); }

        public static int GetSecrets()   => PlayerPrefs.GetInt(K_SECRETS, 0);
        public static void AddSecret()   { PlayerPrefs.SetInt(K_SECRETS, GetSecrets() + 1); PlayerPrefs.Save(); }

        public static bool IsGameComplete() => PlayerPrefs.GetInt(K_GAME_COMPLETE, 0) == 1;
        public static void SetGameComplete() { PlayerPrefs.SetInt(K_GAME_COMPLETE, 1); PlayerPrefs.Save(); }

        // ── Settings ──────────────────────────────────────────────────────────
        public static float GetSensitivity() => PlayerPrefs.GetFloat(K_SENSITIVITY, 3f);
        public static void  SetSensitivity(float v) { PlayerPrefs.SetFloat(K_SENSITIVITY, v); PlayerPrefs.Save(); }

        public static float GetMasterVolume() => PlayerPrefs.GetFloat(K_MASTER_VOL, 1f);
        public static void  SetMasterVolume(float v) { PlayerPrefs.SetFloat(K_MASTER_VOL, v); PlayerPrefs.Save(); }

        public static float GetMusicVolume()  => PlayerPrefs.GetFloat(K_MUSIC_VOL, 0.7f);
        public static void  SetMusicVolume(float v) { PlayerPrefs.SetFloat(K_MUSIC_VOL, v); PlayerPrefs.Save(); }

        public static float GetSFXVolume()    => PlayerPrefs.GetFloat(K_SFX_VOL, 1f);
        public static void  SetSFXVolume(float v) { PlayerPrefs.SetFloat(K_SFX_VOL, v); PlayerPrefs.Save(); }

        public static bool  GetFullscreen()   => PlayerPrefs.GetInt(K_FULLSCREEN, 1) == 1;
        public static void  SetFullscreen(bool v) { PlayerPrefs.SetInt(K_FULLSCREEN, v ? 1 : 0); PlayerPrefs.Save(); }

        public static int   GetQuality()      => PlayerPrefs.GetInt(K_QUALITY, 2);
        public static void  SetQuality(int v) { PlayerPrefs.SetInt(K_QUALITY, v); PlayerPrefs.Save(); }

        // ── New Game ──────────────────────────────────────────────────────────
        public static void ResetAll()
        {
            PlayerPrefs.DeleteKey(K_LEVEL_UNLOCKED);
            PlayerPrefs.DeleteKey(K_DEATHS);
            PlayerPrefs.DeleteKey(K_FRAGMENTS);
            PlayerPrefs.DeleteKey(K_SECRETS);
            PlayerPrefs.DeleteKey(K_GAME_COMPLETE);
            for (int i = 1; i <= 4; i++)
                PlayerPrefs.DeleteKey(K_SCORE_PREFIX + i);
            PlayerPrefs.Save();
        }
    }
}

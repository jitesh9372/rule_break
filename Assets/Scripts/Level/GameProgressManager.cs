// =============================================================================
// GameProgressManager.cs — RULE//BREAK
// Singleton that persists run statistics (deaths, time, ability uses).
// Survives scene loads. Delegates saving to SaveManager.
// =============================================================================
using UnityEngine;
using RuleBreak.Save;

namespace RuleBreak.Level
{
    public class GameProgressManager : MonoBehaviour
    {
        public static GameProgressManager Instance { get; private set; }

        // ── Session stats ─────────────────────────────────────────────────────
        public int   Deaths         { get; private set; }
        public float TotalTime      { get; private set; }
        public int   GravityUses    { get; private set; }
        public int   FreezeUses     { get; private set; }
        public int   DimensionUses  { get; private set; }
        public int   CorruptionUses { get; private set; }
        public int   SecretsFound   { get; private set; }
        public int   FragmentsFound { get; private set; }
        public int   CurrentLevel   { get; set; } = 1;

        // ── Scoring ───────────────────────────────────────────────────────────
        [SerializeField] private int   baseScore           = 10000;
        [SerializeField] private int   deathPenalty        = 200;
        [SerializeField] private float timePenaltyPerMin   = 100f;
        [SerializeField] private int   abilityUsePenalty   = 10;
        [SerializeField] private int   fragmentBonus       = 500;
        [SerializeField] private int   secretBonus         = 1000;
        [SerializeField] private int   minScore            = 0;

        private void Awake()
        {
            if (Instance != null && Instance != this) { Destroy(gameObject); return; }
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        private void Update() { TotalTime += UnityEngine.Time.deltaTime; }

        // ── Trackers ──────────────────────────────────────────────────────────
        public void RegisterDeath()        { Deaths++;         SaveManager.AddDeath(); }
        public void RegisterGravityUse()   { GravityUses++;    }
        public void RegisterFreezeUse()    { FreezeUses++;     }
        public void RegisterDimensionUse() { DimensionUses++;  }
        public void RegisterCorruption()   { CorruptionUses++; }
        public void RegisterSecret()       { SecretsFound++;   SaveManager.AddSecret(); }
        public void RegisterFragment()     { FragmentsFound++; SaveManager.AddFragment(); }

        // ── Score ─────────────────────────────────────────────────────────────
        public int CalculateFinalScore()
        {
            int totalAbilityUses = GravityUses + FreezeUses + DimensionUses + CorruptionUses;
            int score = baseScore
                - Deaths * deathPenalty
                - (int)(TotalTime / 60f * timePenaltyPerMin)
                - totalAbilityUses * abilityUsePenalty
                + FragmentsFound * fragmentBonus
                + SecretsFound * secretBonus;
            return Mathf.Max(minScore, score);
        }

        // ── Adaptive profile ──────────────────────────────────────────────────
        /// <summary>Returns the player's primary ability (the one used most).</summary>
        public string GetPrimaryDependency()
        {
            int max = Mathf.Max(GravityUses, FreezeUses, DimensionUses, CorruptionUses);
            if (max == 0) return "NONE";
            if (GravityUses   == max) return "GRAVITY";
            if (FreezeUses    == max) return "TIME FREEZE";
            if (DimensionUses == max) return "DIMENSION";
            return "RULE CORRUPTION";
        }

        public bool IsLowExperimentation()
        {
            // Low if one ability used ≥3x more than others combined
            int total = GravityUses + FreezeUses + DimensionUses + CorruptionUses;
            int maxUse = Mathf.Max(GravityUses, FreezeUses, DimensionUses, CorruptionUses);
            return total > 0 && maxUse > total * 0.6f;
        }
    }
}

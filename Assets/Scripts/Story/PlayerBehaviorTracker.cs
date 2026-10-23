// =============================================================================
// PlayerBehaviorTracker.cs — RULE//BREAK
// Tracks player behaviour over the session for adaptive puzzle hinting.
// Periodically emits system messages if player relies on one ability too much.
// =============================================================================
using UnityEngine;
using RuleBreak.Level;
using RuleBreak.UI;

namespace RuleBreak.Story
{
    public class PlayerBehaviorTracker : MonoBehaviour
    {
        [SerializeField] private float profileMessageInterval = 60f; // seconds between profile messages
        [SerializeField] private int   minUsesBeforeMessage   = 5;

        private float _timer;
        private bool  _messageSent;

        private void Update()
        {
            var progress = GameProgressManager.Instance;
            if (progress == null) return;

            _timer += UnityEngine.Time.deltaTime;
            if (_timer < profileMessageInterval) return;
            _timer = 0f;

            int total = progress.GravityUses + progress.FreezeUses +
                        progress.DimensionUses + progress.CorruptionUses;

            if (total < minUsesBeforeMessage) return;

            string dep = progress.GetPrimaryDependency();
            bool  low  = progress.IsLowExperimentation();

            if (low && !_messageSent)
            {
                _messageSent = true;
                string msg = $"PLAYER PROFILE GENERATED\nPRIMARY DEPENDENCY: {dep}\nEXPERIMENTATION: LOW";
                HUDController.Instance?.ShowSystemMessage(msg, 5f);
                Debug.Log("Behavior Tracker: " + msg);
            }
        }
    }
}

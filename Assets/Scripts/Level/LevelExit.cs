// =============================================================================
// LevelExit.cs — RULE//BREAK
// Semantic alias — ExitPortal handles the actual exit logic.
// This class is a named placeholder referenced in the project structure.
// =============================================================================
// Note: Actual implementation is in ExitPortal.cs (RuleBreak.Gameplay namespace).
// LevelExit.cs is kept as an alias to satisfy the required project structure.
using UnityEngine;

namespace RuleBreak.Level
{
    /// <summary>
    /// Alias marker. Actual exit logic is in RuleBreak.Gameplay.ExitPortal.
    /// Add ExitPortal component to the exit trigger GameObject.
    /// </summary>
    public class LevelExit : MonoBehaviour
    {
        private void Awake()
        {
            // If this GameObject doesn't already have an ExitPortal, add one
            if (GetComponent<RuleBreak.Gameplay.ExitPortal>() == null)
                gameObject.AddComponent<RuleBreak.Gameplay.ExitPortal>();

            // Self-remove — ExitPortal handles everything
            Destroy(this);
        }
    }
}

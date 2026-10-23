// =============================================================================
// DeathRealityManager.cs — RULE//BREAK
// Controls deterministic world changes triggered by player deaths.
// Changes are specific, limited, and reversible where needed.
// =============================================================================
using UnityEngine;
using System.Collections.Generic;
using RuleBreak.UI;

namespace RuleBreak.Story
{
    [System.Serializable]
    public class DeathChange
    {
        [Tooltip("Death count at which this change activates (1-based).")]
        public int   deathThreshold = 1;
        [Tooltip("GameObject to reposition or activate.")]
        public GameObject target;
        [Tooltip("New world position (if repositioning).")]
        public Vector3    newPosition;
        [Tooltip("Activate instead of reposition.")]
        public bool       activateObject = false;
        [Tooltip("HUD message shown when this change occurs.")]
        public string     hudMessage = "SYSTEM: Something changed...";

        [HideInInspector] public bool applied = false;
    }

    public class DeathRealityManager : MonoBehaviour
    {
        public static DeathRealityManager Instance { get; private set; }

        [SerializeField] private List<DeathChange> changes = new List<DeathChange>();

        private int _deathCount = 0;

        private void Awake()
        {
            if (Instance != null && Instance != this) { Destroy(gameObject); return; }
            Instance = this;
        }

        public void OnPlayerDied()
        {
            _deathCount++;

            foreach (var change in changes)
            {
                if (change.applied) continue;
                if (_deathCount < change.deathThreshold) continue;

                ApplyChange(change);
                change.applied = true;
            }
        }

        private void ApplyChange(DeathChange c)
        {
            if (c.target == null) return;

            if (c.activateObject)
                c.target.SetActive(true);
            else
                c.target.transform.position = c.newPosition;

            HUDController.Instance?.ShowSystemMessage(c.hudMessage, 4f);
            Debug.Log($"DeathRealityManager: change applied at death #{_deathCount}: {c.hudMessage}");
        }
    }
}

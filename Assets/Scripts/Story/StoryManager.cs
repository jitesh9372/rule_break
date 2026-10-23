// =============================================================================
// StoryManager.cs — RULE//BREAK
// Central story message sequencer. Manages system "lie" objectives and
// narrative beats. System messages are displayed through HUDController.
// =============================================================================
using UnityEngine;
using System.Collections;
using RuleBreak.UI;

namespace RuleBreak.Story
{
    [System.Serializable]
    public class StoryBeat
    {
        public string message;
        public float  delay    = 0f;  // seconds after beat trigger before showing
        public float  duration = 4f;
    }

    public class StoryManager : MonoBehaviour
    {
        public static StoryManager Instance { get; private set; }

        [Header("Level Intro")]
        [SerializeField] private StoryBeat[] introBeats;

        [Header("System Lies")]
        [SerializeField] private StoryBeat[] lieBeats;

        private int _lieIndex = 0;

        private void Awake()
        {
            if (Instance != null && Instance != this) { Destroy(gameObject); return; }
            Instance = this;
        }

        private void Start()
        {
            if (introBeats != null && introBeats.Length > 0)
                StartCoroutine(PlayBeats(introBeats));
        }

        // ── Public API ────────────────────────────────────────────────────────
        public void ShowNextLie()
        {
            if (lieBeats == null || _lieIndex >= lieBeats.Length) return;
            var beat = lieBeats[_lieIndex++];
            StartCoroutine(DelayedMessage(beat.message, beat.delay, beat.duration));
        }

        public void ShowMessage(string msg, float duration = 4f)
        {
            HUDController.Instance?.ShowSystemMessage(msg, duration);
        }

        // ── Coroutines ────────────────────────────────────────────────────────
        private IEnumerator PlayBeats(StoryBeat[] beats)
        {
            foreach (var beat in beats)
            {
                yield return new WaitForSeconds(beat.delay);
                HUDController.Instance?.ShowSystemMessage(beat.message, beat.duration);
            }
        }

        private IEnumerator DelayedMessage(string msg, float delay, float dur)
        {
            yield return new WaitForSeconds(delay);
            HUDController.Instance?.ShowSystemMessage(msg, dur);
        }
    }
}

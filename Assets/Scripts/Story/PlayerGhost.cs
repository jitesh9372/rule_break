// =============================================================================
// PlayerGhost.cs — RULE//BREAK
// Replays recorded GhostFrames as a translucent ghost. No physics involved.
// =============================================================================
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace RuleBreak.Story
{
    public class PlayerGhost : MonoBehaviour
    {
        private List<GhostFrame> _frames;
        private int              _index;
        private bool             _playing;

        public void Init(List<GhostFrame> frames)
        {
            _frames = frames;
            _index  = 0;
            _playing = frames != null && frames.Count > 0;

            if (_playing)
                StartCoroutine(Replay());
        }

        private IEnumerator Replay()
        {
            float startTime = UnityEngine.Time.time;
            float recordStart = _frames[0].timestamp;

            while (_index < _frames.Count)
            {
                var frame = _frames[_index];
                float elapsed = UnityEngine.Time.time - startTime;
                float frameTime = frame.timestamp - recordStart;

                if (elapsed >= frameTime)
                {
                    transform.position = frame.position;
                    transform.rotation = frame.rotation;
                    _index++;
                }
                yield return null;
            }

            // Ghost finished its run — fade it out
            yield return FadeOut(1.5f);
            Destroy(gameObject);
        }

        private IEnumerator FadeOut(float duration)
        {
            var r = GetComponent<Renderer>();
            if (r == null) yield break;

            float elapsed = 0f;
            Color c = r.material.color;
            while (elapsed < duration)
            {
                elapsed += UnityEngine.Time.deltaTime;
                c.a = Mathf.Lerp(0.25f, 0f, elapsed / duration);
                r.material.color = c;
                yield return null;
            }
        }
    }
}

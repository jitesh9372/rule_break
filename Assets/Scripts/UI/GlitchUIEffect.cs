// =============================================================================
// GlitchUIEffect.cs — RULE//BREAK
// Applies text glitch effects to UI Text components based on Reality Integrity.
// Randomly replaces characters with '?' or '█' on low integrity.
// =============================================================================
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using RuleBreak.Level;

namespace RuleBreak.UI
{
    [RequireComponent(typeof(Text))]
    public class GlitchUIEffect : MonoBehaviour
    {
        [SerializeField] private float glitchInterval  = 2f;
        [SerializeField] private float glitchDuration  = 0.3f;
        [SerializeField] private int   glitchCharCount = 2;
        [SerializeField] private bool  alwaysGlitch    = false;

        private Text     _text;
        private string   _original;
        private Coroutine _glitchCoroutine;

        private void Awake()
        {
            _text     = GetComponent<Text>();
            _original = _text.text;
        }

        private void Update()
        {
            var integrity = RealityIntegrityManager.Instance;
            bool shouldGlitch = alwaysGlitch ||
                                (integrity != null && integrity.Integrity < 50f);

            if (shouldGlitch && _glitchCoroutine == null)
                _glitchCoroutine = StartCoroutine(GlitchLoop());
            else if (!shouldGlitch && _glitchCoroutine != null)
            {
                StopCoroutine(_glitchCoroutine);
                _glitchCoroutine = null;
                _text.text = _original;
            }
        }

        private IEnumerator GlitchLoop()
        {
            while (true)
            {
                yield return new WaitForSeconds(glitchInterval);
                _original = _text.text;
                GlitchOnce();
                yield return new WaitForSeconds(glitchDuration);
                _text.text = _original;
            }
        }

        private void GlitchOnce()
        {
            if (string.IsNullOrEmpty(_original)) return;
            char[] chars = _original.ToCharArray();
            char[] glitchChars = { '?', '█', '▓', '░', '▒' };
            for (int i = 0; i < glitchCharCount; i++)
            {
                int idx = Random.Range(0, chars.Length);
                if (chars[idx] != ' ' && chars[idx] != '\n')
                    chars[idx] = glitchChars[Random.Range(0, glitchChars.Length)];
            }
            _text.text = new string(chars);
        }

        public void UpdateOriginal(string newText)
        {
            _original  = newText;
            _text.text = newText;
        }
    }
}

// =============================================================================
// SystemMessageUI.cs — RULE//BREAK
// Standalone system-message popup (complements HUDController for
// non-gameplay scenes like MainMenu).
// =============================================================================
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace RuleBreak.UI
{
    public class SystemMessageUI : MonoBehaviour
    {
        [SerializeField] private GameObject panel;
        [SerializeField] private Text       messageText;

        private Coroutine _current;

        public void Show(string msg, float duration = 3f)
        {
            if (_current != null) StopCoroutine(_current);
            _current = StartCoroutine(ShowCoroutine(msg, duration));
        }

        private IEnumerator ShowCoroutine(string msg, float dur)
        {
            if (panel) panel.SetActive(true);
            if (messageText) messageText.text = msg;
            yield return new WaitForSeconds(dur);
            if (panel) panel.SetActive(false);
        }
    }
}

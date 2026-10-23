// =============================================================================
// MemoryFragment.cs — RULE//BREAK (Updated)
// Collectible pickup. Uses SaveManager and GameProgressManager for persistence.
// =============================================================================
using UnityEngine;
using RuleBreak.Level;
using RuleBreak.Audio;
using RuleBreak.UI;

namespace RuleBreak.Gameplay
{
    public class MemoryFragment : MonoBehaviour
    {
        public static int CollectedCount = 0;
        public static int TotalInScene   = 0;

        [SerializeField] private float  rotateSpeed   = 90f;
        [SerializeField] private float  bobAmplitude  = 0.2f;
        [SerializeField] private float  bobSpeed      = 1.5f;
        [SerializeField] private string storyMessage  = "SYSTEM LOG: Reality integrity nominal.";
        [SerializeField] private int    fragmentIndex = 0; // unique ID (0-7)

        private Vector3 _startPos;
        private bool    _collected = false;

        private void Awake()
        {
            _startPos = transform.position;
            TotalInScene++;
        }

        private void Update()
        {
            if (_collected) return;
            transform.Rotate(Vector3.up, rotateSpeed * UnityEngine.Time.deltaTime, Space.World);
            float newY = _startPos.y + Mathf.Sin(UnityEngine.Time.time * bobSpeed) * bobAmplitude;
            transform.position = new Vector3(_startPos.x, newY, _startPos.z);
        }

        private void OnTriggerEnter(Collider other)
        {
            if (_collected) return;
            if (!other.CompareTag("Player")) return;

            _collected = true;
            CollectedCount++;

            GameProgressManager.Instance?.RegisterFragment();
            AudioManager.Instance?.PlayFragment();

            // Show story message in HUD
            HUDController.Instance?.ShowSystemMessage(storyMessage, 4f);
            HUDController.Instance?.ShowFragmentNotification(CollectedCount, TotalInScene);
            RuleBreak.Effects.SciFiEffectSystem.Instance?.PlayFragmentPickup(transform.position);

            Debug.Log($"Memory Fragment [{fragmentIndex}] collected! {CollectedCount}/{TotalInScene}");
            Destroy(gameObject);
        }
    }
}

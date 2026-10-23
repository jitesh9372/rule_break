// =============================================================================
// RadarObjectiveDisplay.cs — RULE//BREAK
// Circular radar-style holographic objective scanner.
// - Real-time tracking of player position and heading.
// - Dynamic blips for Exit Portal (Cyan/Green), Memory Fragments (Amber), Checkpoints (Blue).
// - Continuous rotating sweep beam with fade trail.
// - Compact target distance readout.
// =============================================================================
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using RuleBreak.Gameplay;
using RuleBreak.Player;

namespace RuleBreak.UI
{
    public class RadarObjectiveDisplay : MonoBehaviour
    {
        [Header("Radar Configuration")]
        [SerializeField] private float radarWorldRange = 45f;
        [SerializeField] private float radarUIRadius = 46f;
        [SerializeField] private float sweepSpeed = 140f;

        [Header("UI References")]
        [SerializeField] private RectTransform sweepBeamTransform;
        [SerializeField] private RectTransform blipContainer;
        [SerializeField] private Text distanceReadoutText;

        private Transform _playerTransform;
        private readonly List<RadarTarget> _activeTargets = new List<RadarTarget>();
        private readonly List<GameObject> _blipPool = new List<GameObject>();

        private struct RadarTarget
        {
            public Transform transform;
            public Color color;
            public string label;
        }

        private void Start()
        {
            FindPlayer();
            RefreshTargets();
        }

        private void FindPlayer()
        {
            var p = GameObject.FindGameObjectWithTag("Player");
            if (p != null) _playerTransform = p.transform;
        }

        public void RefreshTargets()
        {
            _activeTargets.Clear();

            // 1. Exit Portal
            var portal = Object.FindAnyObjectByType<ExitPortal>();
            if (portal != null)
            {
                _activeTargets.Add(new RadarTarget
                {
                    transform = portal.transform,
                    color = new Color(0.1f, 1f, 0.7f, 1f),
                    label = "GATEWAY"
                });
            }

            // 2. Memory Fragments
            var fragments = Object.FindObjectsByType<MemoryFragment>(FindObjectsInactive.Exclude);
            foreach (var frag in fragments)
            {
                if (frag != null && frag.gameObject.activeInHierarchy)
                {
                    _activeTargets.Add(new RadarTarget
                    {
                        transform = frag.transform,
                        color = new Color(1f, 0.75f, 0.2f, 1f),
                        label = "DATA FRAG"
                    });
                }
            }

            // 3. Checkpoints
            var checkpoints = Object.FindObjectsByType<Checkpoint>(FindObjectsInactive.Exclude);
            foreach (var cp in checkpoints)
            {
                if (cp != null && cp.gameObject.activeInHierarchy)
                {
                    _activeTargets.Add(new RadarTarget
                    {
                        transform = cp.transform,
                        color = new Color(0.2f, 0.6f, 1f, 0.8f),
                        label = "BEACON"
                    });
                }
            }
        }

        private void Update()
        {
            if (_playerTransform == null)
            {
                FindPlayer();
                if (_playerTransform == null) return;
            }

            // Rotate sweep beam
            if (sweepBeamTransform != null)
            {
                sweepBeamTransform.Rotate(Vector3.forward, -sweepSpeed * UnityEngine.Time.deltaTime);
            }

            // Update radar blips
            UpdateBlips();
        }

        private void UpdateBlips()
        {
            if (blipContainer == null) return;

            Vector3 playerPos = _playerTransform.position;
            Vector3 playerForward = _playerTransform.forward;
            playerForward.y = 0f;
            if (playerForward.sqrMagnitude < 0.001f) playerForward = Vector3.forward;
            playerForward.Normalize();

            float minDistance = float.MaxValue;
            string closestLabel = "NO SIGNAL";

            int targetIndex = 0;

            for (int i = 0; i < _activeTargets.Count; i++)
            {
                var target = _activeTargets[i];
                if (target.transform == null || !target.transform.gameObject.activeInHierarchy) continue;

                Vector3 toTarget = target.transform.position - playerPos;
                toTarget.y = 0f;
                float dist = toTarget.magnitude;

                if (dist < minDistance)
                {
                    minDistance = dist;
                    closestLabel = $"{target.label}: {dist:F1}m";
                }

                // Map to 2D polar space on the radar
                float angle = Vector3.SignedAngle(playerForward, toTarget.normalized, Vector3.up);
                float normalizedDist = Mathf.Clamp01(dist / radarWorldRange);
                float uiDist = normalizedDist * radarUIRadius;

                float rad = (-angle + 90f) * Mathf.Deg2Rad;
                Vector2 blipPos = new Vector2(Mathf.Cos(rad) * uiDist, Mathf.Sin(rad) * uiDist);

                GameObject blipGO = GetOrCreateBlip(targetIndex);
                blipGO.SetActive(true);
                var rt = blipGO.GetComponent<RectTransform>();
                rt.anchoredPosition = blipPos;

                var img = blipGO.GetComponent<Image>();
                if (img != null) img.color = target.color;

                targetIndex++;
            }

            // Hide unused blips
            for (int i = targetIndex; i < _blipPool.Count; i++)
            {
                _blipPool[i].SetActive(false);
            }

            if (distanceReadoutText != null)
            {
                distanceReadoutText.text = closestLabel;
            }
        }

        private GameObject GetOrCreateBlip(int index)
        {
            while (index >= _blipPool.Count)
            {
                var go = new GameObject($"Blip_{_blipPool.Count}");
                go.transform.SetParent(blipContainer, false);
                var rt = go.AddComponent<RectTransform>();
                rt.sizeDelta = new Vector2(8f, 8f);
                var img = go.AddComponent<Image>();
                img.sprite = SciFiProceduralTextures.GetBlip(16, Color.white);
                img.raycastTarget = false;
                _blipPool.Add(go);
            }
            return _blipPool[index];
        }
    }
}

// =============================================================================
// PlayerGhostRecorder.cs — RULE//BREAK
// Records simplified player positions each frame (capped to maxDuration).
// On demand, spawns a PlayerGhost to replay the recorded positions.
// =============================================================================
using UnityEngine;
using System.Collections.Generic;

namespace RuleBreak.Story
{
    [System.Serializable]
    public struct GhostFrame
    {
        public Vector3    position;
        public Quaternion rotation;
        public float      timestamp;
    }

    public class PlayerGhostRecorder : MonoBehaviour
    {
        public static PlayerGhostRecorder Instance { get; private set; }

        [SerializeField] private float       maxDuration  = 120f;  // max seconds to record
        [SerializeField] private float       recordRate   = 10f;   // frames per second
        [SerializeField] private GameObject  ghostPrefab;          // assign PlayerGhost prefab

        private readonly List<GhostFrame> _frames    = new List<GhostFrame>();
        private float                     _nextRecord = 0f;
        private PlayerGhost               _activeGhost;

        public IReadOnlyList<GhostFrame> Frames => _frames;

        private void Awake()
        {
            if (Instance != null && Instance != this) { Destroy(gameObject); return; }
            Instance = this;
        }

        private void Update()
        {
            if (UnityEngine.Time.time < _nextRecord) return;
            _nextRecord = UnityEngine.Time.time + 1f / recordRate;

            _frames.Add(new GhostFrame
            {
                position  = transform.position,
                rotation  = transform.rotation,
                timestamp = UnityEngine.Time.time
            });

            // Trim if exceeds max duration
            float cutoff = UnityEngine.Time.time - maxDuration;
            while (_frames.Count > 0 && _frames[0].timestamp < cutoff)
                _frames.RemoveAt(0);
        }

        /// <summary>Spawn the ghost to replay the last run.</summary>
        public void SpawnGhost()
        {
            if (_frames.Count == 0) return;

            if (_activeGhost != null) Destroy(_activeGhost.gameObject);

            GameObject go = ghostPrefab != null
                ? Instantiate(ghostPrefab)
                : CreateDefaultGhost();

            _activeGhost = go.GetComponent<PlayerGhost>();
            if (_activeGhost == null) _activeGhost = go.AddComponent<PlayerGhost>();
            _activeGhost.Init(new List<GhostFrame>(_frames));
        }

        private GameObject CreateDefaultGhost()
        {
            var go  = GameObject.CreatePrimitive(PrimitiveType.Capsule);
            go.name = "PlayerGhost";
            // Remove collider so ghost doesn't physically interact
            var col = go.GetComponent<Collider>();
            if (col != null) Destroy(col);

            var r = go.GetComponent<Renderer>();
            if (r != null)
            {
                r.material.color = new Color(0f, 0.9f, 1f, 0.25f);
                r.material.SetFloat("_Mode", 3); // transparent (standard shader)
            }
            return go;
        }

        public void ClearRecording() => _frames.Clear();
    }
}

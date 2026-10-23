// =============================================================================
// SciFiEffectSystem.cs — RULE//BREAK
// High-performance procedural visual effect manager.
// Generates and manages runtime Particle Systems, holographic shockwaves,
// quantum phase bursts, and dynamic lights without requiring external asset bundles.
// =============================================================================
using UnityEngine;
using System.Collections;
using RuleBreak.Dimension;

namespace RuleBreak.Effects
{
    public class SciFiEffectSystem : MonoBehaviour
    {
        public static SciFiEffectSystem Instance { get; private set; }

        private Material _particleAdditiveMat;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;

            InitParticleMaterial();
        }

        private void InitParticleMaterial()
        {
            var shader = Shader.Find("Universal Render Pipeline/Particles/Unlit")
                      ?? Shader.Find("Particles/Standard Unlit")
                      ?? Shader.Find("Mobile/Particles/Additive")
                      ?? Shader.Find("Sprites/Default");

            _particleAdditiveMat = new Material(shader);
        }

        // ── 1. Gravity Shift Shockwave ─────────────────────────────────────────
        public void PlayGravityPulse(Vector3 position, Vector3 direction)
        {
            var go = new GameObject("VFX_GravityPulse");
            go.transform.position = position;
            go.transform.up = direction;

            var ps = go.AddComponent<ParticleSystem>();
            var pRenderer = go.GetComponent<ParticleSystemRenderer>();
            pRenderer.material = _particleAdditiveMat;

            var main = ps.main;
            main.duration = 0.6f;
            main.loop = false;
            main.startLifetime = 0.5f;
            main.startSpeed = 8f;
            main.startSize = 0.25f;
            main.startColor = new Color(0f, 0.95f, 1f, 0.9f);
            main.playOnAwake = true;

            var emission = ps.emission;
            emission.rateOverTime = 0;
            emission.SetBursts(new ParticleSystem.Burst[] { new ParticleSystem.Burst(0f, 35) });

            var shape = ps.shape;
            shape.shapeType = ParticleSystemShapeType.Circle;
            shape.radius = 0.5f;

            // Flash light
            var light = go.AddComponent<Light>();
            light.type = LightType.Point;
            light.color = new Color(0f, 0.95f, 1f);
            light.range = 8f;
            light.intensity = 3f;

            StartCoroutine(FadeLightAndDestroy(go, light, 0.55f));
        }

        // ── 2. Time Freeze Cryo Burst ──────────────────────────────────────────
        public void PlayTimeFreeze(bool isFrozen)
        {
            var cam = Camera.main;
            Vector3 spawnPos = cam != null ? cam.transform.position + cam.transform.forward * 2f : Vector3.zero;

            var go = new GameObject("VFX_TimeFreeze");
            go.transform.position = spawnPos;

            var ps = go.AddComponent<ParticleSystem>();
            var pRenderer = go.GetComponent<ParticleSystemRenderer>();
            pRenderer.material = _particleAdditiveMat;

            var main = ps.main;
            main.duration = 0.8f;
            main.loop = false;
            main.startLifetime = 0.7f;
            main.startSpeed = isFrozen ? 4f : 6f;
            main.startSize = 0.3f;
            main.startColor = isFrozen ? new Color(0.2f, 0.65f, 1f, 0.95f) : new Color(0.8f, 0.95f, 1f, 0.8f);
            main.playOnAwake = true;

            var emission = ps.emission;
            emission.rateOverTime = 0;
            emission.SetBursts(new ParticleSystem.Burst[] { new ParticleSystem.Burst(0f, 40) });

            var shape = ps.shape;
            shape.shapeType = ParticleSystemShapeType.Sphere;
            shape.radius = 1.2f;

            var light = go.AddComponent<Light>();
            light.type = LightType.Point;
            light.color = new Color(0.25f, 0.6f, 1f);
            light.range = 12f;
            light.intensity = 2.5f;

            StartCoroutine(FadeLightAndDestroy(go, light, 0.75f));
        }

        // ── 3. Dimensional Shift Quantum Flash ─────────────────────────────────
        public void PlayDimensionShift(Reality newReality, Vector3 position)
        {
            var go = new GameObject("VFX_DimensionShift");
            go.transform.position = position;

            var ps = go.AddComponent<ParticleSystem>();
            var pRenderer = go.GetComponent<ParticleSystemRenderer>();
            pRenderer.material = _particleAdditiveMat;

            Color shiftColor = newReality == Reality.A ? new Color(0f, 0.9f, 1f, 0.9f) : new Color(0.7f, 0.3f, 1f, 0.95f);

            var main = ps.main;
            main.duration = 0.7f;
            main.loop = false;
            main.startLifetime = 0.6f;
            main.startSpeed = 9f;
            main.startSize = 0.35f;
            main.startColor = shiftColor;
            main.playOnAwake = true;

            var emission = ps.emission;
            emission.rateOverTime = 0;
            emission.SetBursts(new ParticleSystem.Burst[] { new ParticleSystem.Burst(0f, 50) });

            var shape = ps.shape;
            shape.shapeType = ParticleSystemShapeType.Sphere;
            shape.radius = 0.8f;

            var light = go.AddComponent<Light>();
            light.type = LightType.Point;
            light.color = shiftColor;
            light.range = 10f;
            light.intensity = 3.5f;

            StartCoroutine(FadeLightAndDestroy(go, light, 0.65f));
        }

        // ── 4. Rule Corruption Digital Glitch ──────────────────────────────────
        public void PlayCorruptionGlitch(Vector3 position)
        {
            var go = new GameObject("VFX_CorruptionGlitch");
            go.transform.position = position;

            var ps = go.AddComponent<ParticleSystem>();
            var pRenderer = go.GetComponent<ParticleSystemRenderer>();
            pRenderer.material = _particleAdditiveMat;

            var main = ps.main;
            main.duration = 0.6f;
            main.loop = false;
            main.startLifetime = 0.5f;
            main.startSpeed = 7f;
            main.startSize = 0.3f;
            main.startColor = new Color(1f, 0.15f, 0.65f, 0.95f);
            main.playOnAwake = true;

            var emission = ps.emission;
            emission.rateOverTime = 0;
            emission.SetBursts(new ParticleSystem.Burst[] { new ParticleSystem.Burst(0f, 45) });

            var shape = ps.shape;
            shape.shapeType = ParticleSystemShapeType.Box;
            shape.scale = new Vector3(1.5f, 1.5f, 1.5f);

            var light = go.AddComponent<Light>();
            light.type = LightType.Point;
            light.color = new Color(1f, 0.2f, 0.6f);
            light.range = 8f;
            light.intensity = 3f;

            StartCoroutine(FadeLightAndDestroy(go, light, 0.55f));
        }

        // ── 5. Memory Fragment Pickup Sparkler ─────────────────────────────────
        public void PlayFragmentPickup(Vector3 position)
        {
            var go = new GameObject("VFX_FragmentPickup");
            go.transform.position = position;

            var ps = go.AddComponent<ParticleSystem>();
            var pRenderer = go.GetComponent<ParticleSystemRenderer>();
            pRenderer.material = _particleAdditiveMat;

            var main = ps.main;
            main.duration = 0.8f;
            main.loop = false;
            main.startLifetime = 0.7f;
            main.startSpeed = 5f;
            main.startSize = 0.22f;
            main.startColor = new Color(1f, 0.75f, 0.2f, 0.95f);
            main.playOnAwake = true;

            var emission = ps.emission;
            emission.rateOverTime = 0;
            emission.SetBursts(new ParticleSystem.Burst[] { new ParticleSystem.Burst(0f, 30) });

            var shape = ps.shape;
            shape.shapeType = ParticleSystemShapeType.Sphere;
            shape.radius = 0.5f;

            var light = go.AddComponent<Light>();
            light.type = LightType.Point;
            light.color = new Color(1f, 0.8f, 0.3f);
            light.range = 7f;
            light.intensity = 2.5f;

            StartCoroutine(FadeLightAndDestroy(go, light, 0.75f));
        }

        // ── 6. Checkpoint Activation Beacon ───────────────────────────────────
        public void PlayCheckpointBeacon(Vector3 position)
        {
            var go = new GameObject("VFX_CheckpointBeacon");
            go.transform.position = position;

            var ps = go.AddComponent<ParticleSystem>();
            var pRenderer = go.GetComponent<ParticleSystemRenderer>();
            pRenderer.material = _particleAdditiveMat;

            var main = ps.main;
            main.duration = 1.2f;
            main.loop = false;
            main.startLifetime = 1.0f;
            main.startSpeed = 6f;
            main.startSize = 0.25f;
            main.startColor = new Color(0.2f, 1f, 0.65f, 0.95f);
            main.playOnAwake = true;

            var emission = ps.emission;
            emission.rateOverTime = 0;
            emission.SetBursts(new ParticleSystem.Burst[] { new ParticleSystem.Burst(0f, 40) });

            var shape = ps.shape;
            shape.shapeType = ParticleSystemShapeType.Cone;
            shape.angle = 15f;
            shape.radius = 0.3f;

            var light = go.AddComponent<Light>();
            light.type = LightType.Point;
            light.color = new Color(0.3f, 1f, 0.6f);
            light.range = 9f;
            light.intensity = 3f;

            StartCoroutine(FadeLightAndDestroy(go, light, 1.1f));
        }

        private IEnumerator FadeLightAndDestroy(GameObject root, Light light, float duration)
        {
            float elapsed = 0f;
            float initIntensity = light != null ? light.intensity : 1f;

            while (elapsed < duration)
            {
                elapsed += UnityEngine.Time.deltaTime;
                if (light != null)
                {
                    light.intensity = Mathf.Lerp(initIntensity, 0f, elapsed / duration);
                }
                yield return null;
            }

            Destroy(root);
        }
    }
}

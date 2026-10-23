// =============================================================================
// SciFiProceduralTextures.cs — RULE//BREAK
// Generates procedural, resolution-independent sci-fi UI textures and sprites:
// - Holographic rings (outer glow, segmented ticks, hollow centers)
// - Radial progress fill rings for circular meters
// - Radar grid lines, rotating sweep fans, and blip pings
// - Glassmorphic dark panels with emissive neon borders
// - High-performance memory caching: generated once per session
// =============================================================================
using UnityEngine;
using System.Collections.Generic;

namespace RuleBreak.UI
{
    public static class SciFiProceduralTextures
    {
        private static readonly Dictionary<string, Sprite> _cache = new Dictionary<string, Sprite>();

        // ── 1. Hollow Ring Sprite (for radial meters and HUD decorations) ───────
        public static Sprite GetHollowRing(int size = 128, float innerRadius = 0.72f, Color? tint = null)
        {
            Color baseColor = tint ?? new Color(0f, 0.9f, 1f, 1f);
            string key = $"Ring_{size}_{innerRadius}_{baseColor}";
            if (_cache.TryGetValue(key, out Sprite cached)) return cached;

            var tex = new Texture2D(size, size, TextureFormat.RGBA32, false);
            tex.wrapMode = TextureWrapMode.Clamp;
            tex.filterMode = FilterMode.Bilinear;

            float center = (size - 1) * 0.5f;
            float maxR = center;
            float minR = maxR * innerRadius;

            Color[] pixels = new Color[size * size];

            for (int y = 0; y < size; y++)
            {
                for (int x = 0; x < size; x++)
                {
                    float dx = x - center;
                    float dy = y - center;
                    float dist = Mathf.Sqrt(dx * dx + dy * dy);

                    float alpha = 0f;
                    if (dist >= minR && dist <= maxR)
                    {
                        // Antialias outer and inner edges
                        float outerEdge = Mathf.Clamp01((maxR - dist) * 1.5f);
                        float innerEdge = Mathf.Clamp01((dist - minR) * 1.5f);
                        alpha = Mathf.Min(outerEdge, innerEdge);
                    }

                    pixels[y * size + x] = new Color(baseColor.r, baseColor.g, baseColor.b, baseColor.a * alpha);
                }
            }

            tex.SetPixels(pixels);
            tex.Apply();

            Sprite sprite = Sprite.Create(tex, new Rect(0, 0, size, size), new Vector2(0.5f, 0.5f));
            _cache[key] = sprite;
            return sprite;
        }

        // ── 2. Segmented / Ticked Holographic Ring ─────────────────────────────
        public static Sprite GetSegmentedRing(int size = 128, int segments = 16, float gapAngle = 6f, Color? tint = null)
        {
            Color baseColor = tint ?? new Color(0f, 0.9f, 1f, 0.9f);
            string key = $"SegRing_{size}_{segments}_{gapAngle}_{baseColor}";
            if (_cache.TryGetValue(key, out Sprite cached)) return cached;

            var tex = new Texture2D(size, size, TextureFormat.RGBA32, false);
            tex.wrapMode = TextureWrapMode.Clamp;
            tex.filterMode = FilterMode.Bilinear;

            float center = (size - 1) * 0.5f;
            float maxR = center;
            float minR = maxR * 0.78f;
            float segAngle = 360f / segments;

            Color[] pixels = new Color[size * size];

            for (int y = 0; y < size; y++)
            {
                for (int x = 0; x < size; x++)
                {
                    float dx = x - center;
                    float dy = y - center;
                    float dist = Mathf.Sqrt(dx * dx + dy * dy);

                    float alpha = 0f;
                    if (dist >= minR && dist <= maxR)
                    {
                        float angle = (Mathf.Atan2(dy, dx) * Mathf.Rad2Deg + 360f) % 360f;
                        float modAngle = angle % segAngle;
                        if (modAngle > gapAngle * 0.5f && modAngle < segAngle - (gapAngle * 0.5f))
                        {
                            float outerEdge = Mathf.Clamp01((maxR - dist) * 1.5f);
                            float innerEdge = Mathf.Clamp01((dist - minR) * 1.5f);
                            alpha = Mathf.Min(outerEdge, innerEdge);
                        }
                    }

                    pixels[y * size + x] = new Color(baseColor.r, baseColor.g, baseColor.b, baseColor.a * alpha);
                }
            }

            tex.SetPixels(pixels);
            tex.Apply();

            Sprite sprite = Sprite.Create(tex, new Rect(0, 0, size, size), new Vector2(0.5f, 0.5f));
            _cache[key] = sprite;
            return sprite;
        }

        // ── 3. Radar Grid (Concentric Circles & Crosshairs) ─────────────────────
        public static Sprite GetRadarGrid(int size = 128, Color? tint = null)
        {
            Color baseColor = tint ?? new Color(0f, 0.85f, 1f, 0.45f);
            string key = $"RadarGrid_{size}_{baseColor}";
            if (_cache.TryGetValue(key, out Sprite cached)) return cached;

            var tex = new Texture2D(size, size, TextureFormat.RGBA32, false);
            tex.wrapMode = TextureWrapMode.Clamp;
            tex.filterMode = FilterMode.Bilinear;

            float center = (size - 1) * 0.5f;
            float maxR = center;
            float ring1 = maxR * 0.33f;
            float ring2 = maxR * 0.66f;
            float ring3 = maxR * 0.98f;

            Color[] pixels = new Color[size * size];

            for (int y = 0; y < size; y++)
            {
                for (int x = 0; x < size; x++)
                {
                    float dx = x - center;
                    float dy = y - center;
                    float dist = Mathf.Sqrt(dx * dx + dy * dy);

                    float alpha = 0f;

                    // Range Rings
                    if (Mathf.Abs(dist - ring1) < 1f || Mathf.Abs(dist - ring2) < 1f || Mathf.Abs(dist - ring3) < 1.2f)
                    {
                        alpha = 0.7f;
                    }

                    // Crosshair lines
                    if ((Mathf.Abs(dx) < 0.8f || Mathf.Abs(dy) < 0.8f) && dist <= maxR)
                    {
                        alpha = Mathf.Max(alpha, 0.5f);
                    }

                    // Background glass tint inside radar circle
                    if (dist < maxR)
                    {
                        alpha = Mathf.Max(alpha, 0.12f);
                    }

                    pixels[y * size + x] = new Color(baseColor.r, baseColor.g, baseColor.b, alpha * baseColor.a);
                }
            }

            tex.SetPixels(pixels);
            tex.Apply();

            Sprite sprite = Sprite.Create(tex, new Rect(0, 0, size, size), new Vector2(0.5f, 0.5f));
            _cache[key] = sprite;
            return sprite;
        }

        // ── 4. Radar Sweep Gradient Wedge ──────────────────────────────────────
        public static Sprite GetRadarSweep(int size = 128, float sweepAngle = 55f, Color? tint = null)
        {
            Color baseColor = tint ?? new Color(0f, 0.95f, 1f, 0.75f);
            string key = $"RadarSweep_{size}_{sweepAngle}_{baseColor}";
            if (_cache.TryGetValue(key, out Sprite cached)) return cached;

            var tex = new Texture2D(size, size, TextureFormat.RGBA32, false);
            tex.wrapMode = TextureWrapMode.Clamp;
            tex.filterMode = FilterMode.Bilinear;

            float center = (size - 1) * 0.5f;
            float maxR = center;

            Color[] pixels = new Color[size * size];

            for (int y = 0; y < size; y++)
            {
                for (int x = 0; x < size; x++)
                {
                    float dx = x - center;
                    float dy = y - center;
                    float dist = Mathf.Sqrt(dx * dx + dy * dy);

                    float alpha = 0f;
                    if (dist <= maxR)
                    {
                        float angle = (Mathf.Atan2(dy, dx) * Mathf.Rad2Deg + 360f) % 360f;
                        if (angle >= 0f && angle <= sweepAngle)
                        {
                            // Leading edge is bright, trailing edge fades out
                            float intensity = angle / sweepAngle;
                            float edgeFade = Mathf.Clamp01((maxR - dist) * 1.5f);
                            alpha = Mathf.Pow(intensity, 2.5f) * edgeFade * baseColor.a;
                        }
                    }

                    pixels[y * size + x] = new Color(baseColor.r, baseColor.g, baseColor.b, alpha);
                }
            }

            tex.SetPixels(pixels);
            tex.Apply();

            Sprite sprite = Sprite.Create(tex, new Rect(0, 0, size, size), new Vector2(0.5f, 0.5f));
            _cache[key] = sprite;
            return sprite;
        }

        // ── 5. Radar Glowing Blip Ping ─────────────────────────────────────────
        public static Sprite GetBlip(int size = 16, Color? tint = null)
        {
            Color baseColor = tint ?? new Color(0f, 1f, 0.7f, 1f);
            string key = $"Blip_{size}_{baseColor}";
            if (_cache.TryGetValue(key, out Sprite cached)) return cached;

            var tex = new Texture2D(size, size, TextureFormat.RGBA32, false);
            tex.wrapMode = TextureWrapMode.Clamp;
            tex.filterMode = FilterMode.Bilinear;

            float center = (size - 1) * 0.5f;
            float maxR = center;

            Color[] pixels = new Color[size * size];

            for (int y = 0; y < size; y++)
            {
                for (int x = 0; x < size; x++)
                {
                    float dx = x - center;
                    float dy = y - center;
                    float dist = Mathf.Sqrt(dx * dx + dy * dy);

                    float alpha = Mathf.Clamp01(1f - (dist / maxR));
                    alpha = Mathf.Pow(alpha, 1.8f);

                    pixels[y * size + x] = new Color(baseColor.r, baseColor.g, baseColor.b, baseColor.a * alpha);
                }
            }

            tex.SetPixels(pixels);
            tex.Apply();

            Sprite sprite = Sprite.Create(tex, new Rect(0, 0, size, size), new Vector2(0.5f, 0.5f));
            _cache[key] = sprite;
            return sprite;
        }

        // ── 6. Glassmorphic Panel Sprite with Glowing Neon Border ───────────────
        public static Sprite GetGlassPanel(int width = 64, int height = 64, int borderWidth = 2,
                                           Color? fillColor = null, Color? borderColor = null)
        {
            Color fill = fillColor ?? new Color(0.04f, 0.07f, 0.13f, 0.88f);
            Color border = borderColor ?? new Color(0f, 0.85f, 1f, 0.65f);
            string key = $"Panel_{width}_{height}_{borderWidth}_{fill}_{border}";
            if (_cache.TryGetValue(key, out Sprite cached)) return cached;

            var tex = new Texture2D(width, height, TextureFormat.RGBA32, false);
            tex.wrapMode = TextureWrapMode.Clamp;
            tex.filterMode = FilterMode.Bilinear;

            Color[] pixels = new Color[width * height];

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    bool isBorder = (x < borderWidth || x >= width - borderWidth ||
                                     y < borderWidth || y >= height - borderWidth);

                    // Corner bevel (chamfer 4px)
                    bool isCorner = (x + y < 5) || (x + (height - 1 - y) < 5) ||
                                    ((width - 1 - x) + y < 5) || ((width - 1 - x) + (height - 1 - y) < 5);

                    if (isCorner)
                    {
                        pixels[y * width + x] = Color.clear;
                    }
                    else if (isBorder)
                    {
                        pixels[y * width + x] = border;
                    }
                    else
                    {
                        // Subtle vertical top-to-bottom darkening inside
                        float grad = Mathf.Lerp(1.1f, 0.9f, (float)y / height);
                        pixels[y * width + x] = new Color(fill.r * grad, fill.g * grad, fill.b * grad, fill.a);
                    }
                }
            }

            tex.SetPixels(pixels);
            tex.Apply();

            // 9-slice border setup
            Vector4 borderVector = new Vector4(8, 8, 8, 8);
            Sprite sprite = Sprite.Create(tex, new Rect(0, 0, width, height), new Vector2(0.5f, 0.5f), 100f, 0, SpriteMeshType.FullRect, borderVector);
            _cache[key] = sprite;
            return sprite;
        }

        // ── 7. Scanline Texture ────────────────────────────────────────────────
        public static Texture2D GetScanlineTexture()
        {
            var tex = new Texture2D(4, 4, TextureFormat.RGBA32, false);
            tex.wrapMode = TextureWrapMode.Repeat;
            tex.filterMode = FilterMode.Point;

            Color c1 = new Color(0f, 0f, 0f, 0f);
            Color c2 = new Color(0f, 0f, 0f, 0.18f);

            Color[] pixels = {
                c1, c1, c1, c1,
                c2, c2, c2, c2,
                c1, c1, c1, c1,
                c2, c2, c2, c2,
            };

            tex.SetPixels(pixels);
            tex.Apply();
            return tex;
        }
    }
}

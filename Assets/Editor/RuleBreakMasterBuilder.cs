// =============================================================================
// RuleBreakMasterBuilder.cs — RULE//BREAK
// Master Editor automation: builds all levels, validates project, sets scenes.
// Menu: RULE//BREAK > ...
// =============================================================================
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;
using System.IO;

public static class RuleBreakMasterBuilder
{
    // ── Colour palette constants ───────────────────────────────────────────────
    public static readonly Color ColVoid       = new Color(0.031f, 0.043f, 0.078f);
    public static readonly Color ColDeepNavy   = new Color(0.063f, 0.090f, 0.161f);
    public static readonly Color ColSurface    = new Color(0.090f, 0.122f, 0.196f);
    public static readonly Color ColCyan       = new Color(0f, 0.898f, 1f);
    public static readonly Color ColNeonBlue   = new Color(0.263f, 0.533f, 1f);
    public static readonly Color ColViolet     = new Color(0.608f, 0.361f, 1f);
    public static readonly Color ColMagenta    = new Color(1f, 0.239f, 0.604f);
    public static readonly Color ColAmber      = new Color(1f, 0.710f, 0.278f);
    public static readonly Color ColRed        = new Color(1f, 0.251f, 0.341f);
    public static readonly Color ColGreen      = new Color(0.380f, 0.949f, 0.635f);

    // ── Shared scene validation ────────────────────────────────────────────────
    public static bool EnsureScene(string sceneName)
    {
        if (EditorApplication.isPlaying)
        {
            EditorUtility.DisplayDialog("RULE//BREAK",
                "Cannot build or modify scene geometry while Unity is in Play Mode.\n\nPlease exit Play Mode first (press the Play button in Unity to stop).", "OK");
            return false;
        }

        Scene scene = SceneManager.GetActiveScene();
        if (scene.name != sceneName)
        {
            EditorUtility.DisplayDialog("RULE//BREAK",
                $"Please open the {sceneName} scene first.", "OK");
            return false;
        }
        return true;
    }

    // ── Create or find a root group ────────────────────────────────────────────
    public static GameObject GetOrCreateGroup(string name)
    {
        GameObject existing = GameObject.Find(name);
        if (existing != null) return existing;
        var g = new GameObject(name);
        Undo.RegisterCreatedObjectUndo(g, "Create " + name);
        return g;
    }

    // ── Platform helper ────────────────────────────────────────────────────────
    // ── Platform helper ────────────────────────────────────────────────────────
    public static GameObject MakePlatform(string name, Vector3 pos, Vector3 scale,
                                          GameObject parent, Color? color = null,
                                          bool emissive = false)
    {
        if (parent.transform.Find(name) != null) return parent.transform.Find(name).gameObject;

        var p = GameObject.CreatePrimitive(PrimitiveType.Cube);
        p.name = name;
        p.transform.SetParent(parent.transform);
        p.transform.position   = pos;
        p.transform.localScale = scale;

        var r = p.GetComponent<Renderer>();
        if (r != null)
        {
            var mat = new Material(Shader.Find("Universal Render Pipeline/Lit") ?? r.sharedMaterial.shader);
            mat.color = color ?? ColSurface;
            if (emissive)
            {
                mat.EnableKeyword("_EMISSION");
                mat.SetColor("_EmissionColor", (color ?? ColCyan) * 0.45f);
            }
            r.sharedMaterial = mat;
        }

        // Sci-Fi Emissive Rim Trim on sizeable walking surfaces
        if (scale.x >= 2.5f && scale.z >= 2.5f && !name.Contains("Roof") && !name.Contains("SR_"))
        {
            var trim = GameObject.CreatePrimitive(PrimitiveType.Cube);
            trim.name = "EdgeTrim";
            trim.transform.SetParent(p.transform, false);
            trim.transform.localPosition = new Vector3(0f, 0.42f, 0f);
            trim.transform.localScale = new Vector3(1.01f, 0.12f, 1.01f);
            var trimCol = trim.GetComponent<Collider>();
            if (trimCol != null) Object.DestroyImmediate(trimCol);

            var tr = trim.GetComponent<Renderer>();
            if (tr != null)
            {
                var trimMat = new Material(Shader.Find("Universal Render Pipeline/Lit") ?? tr.sharedMaterial.shader);
                trimMat.color = color ?? ColCyan;
                trimMat.EnableKeyword("_EMISSION");
                trimMat.SetColor("_EmissionColor", (color ?? ColCyan) * 0.8f);
                tr.sharedMaterial = trimMat;
            }
        }

        Undo.RegisterCreatedObjectUndo(p, "Create " + name);
        return p;
    }

    // ── Hazard (spike / kill zone) ─────────────────────────────────────────────
    public static void MakeHazard(string name, Vector3 pos, GameObject parent)
    {
        if (parent.transform.Find(name) != null) return;

        var spike = GameObject.CreatePrimitive(PrimitiveType.Cube);
        spike.name = name;
        spike.transform.SetParent(parent.transform);
        spike.transform.position   = pos;
        spike.transform.localScale = new Vector3(2f, 0.4f, 2f);

        var r = spike.GetComponent<Renderer>();
        if (r != null)
        {
            var mat = new Material(Shader.Find("Universal Render Pipeline/Lit") ??
                                   r.sharedMaterial.shader);
            mat.color = ColRed;
            mat.EnableKeyword("_EMISSION");
            mat.SetColor("_EmissionColor", ColRed * 0.5f);
            r.sharedMaterial = mat;
        }

        // Kill trigger
        var trigger = new GameObject(name + "_Trigger");
        trigger.transform.SetParent(spike.transform);
        trigger.transform.localPosition = new Vector3(0f, 1f, 0f);
        var bc = trigger.AddComponent<BoxCollider>();
        bc.isTrigger = true;
        bc.size = new Vector3(1f, 0.5f, 1f);
        trigger.AddComponent<RuleBreak.Gameplay.KillZone>();

        Undo.RegisterCreatedObjectUndo(spike, "Create " + name);
    }

    // ── Void kill zone ─────────────────────────────────────────────────────────
    public static void MakeVoidKillZone(string name, Vector3 pos, GameObject parent)
    {
        if (parent.transform.Find(name) != null) return;

        var kz = new GameObject(name);
        kz.transform.SetParent(parent.transform);
        kz.transform.position = pos;
        var bc = kz.AddComponent<BoxCollider>();
        bc.isTrigger = true;
        bc.size = new Vector3(120f, 2f, 120f);
        kz.AddComponent<RuleBreak.Gameplay.KillZone>();
        Undo.RegisterCreatedObjectUndo(kz, "Create " + name);
    }

    // ── Moving platform ────────────────────────────────────────────────────────
    public static void MakeMovingPlatform(string name, Vector3 a, Vector3 b, GameObject parent)
    {
        if (parent.transform.Find(name) != null) return;

        var p = GameObject.CreatePrimitive(PrimitiveType.Cube);
        p.name = name;
        p.transform.SetParent(parent.transform);
        p.transform.position   = a;
        p.transform.localScale = new Vector3(3f, 0.3f, 3f);

        var r = p.GetComponent<Renderer>();
        if (r != null)
        {
            var mat = new Material(Shader.Find("Universal Render Pipeline/Lit") ?? r.sharedMaterial.shader);
            mat.color = ColCyan;
            mat.EnableKeyword("_EMISSION");
            mat.SetColor("_EmissionColor", ColCyan * 0.35f);
            r.sharedMaterial = mat;
        }

        var mp = p.AddComponent<RuleBreak.Gameplay.MovingPlatform>();
        var so = new SerializedObject(mp);
        so.FindProperty("pointA").vector3Value = a;
        so.FindProperty("pointB").vector3Value = b;
        so.ApplyModifiedProperties();

        p.AddComponent<RuleBreak.Time.FreezableObject>();
        Undo.RegisterCreatedObjectUndo(p, "Create " + name);
    }

    // ── Memory Fragment ────────────────────────────────────────────────────────
    public static void MakeFragment(string name, Vector3 pos, GameObject parent,
                                    string story = "SYSTEM LOG: Fragment detected.", int index = 0)
    {
        if (parent.transform.Find(name) != null) return;

        var f = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        f.name = name;
        f.transform.SetParent(parent.transform);
        f.transform.position   = pos;
        f.transform.localScale = Vector3.one * 0.5f;

        var col = f.GetComponent<Collider>();
        if (col != null) col.isTrigger = true;

        var r = f.GetComponent<Renderer>();
        if (r != null)
        {
            var mat = new Material(Shader.Find("Universal Render Pipeline/Lit") ?? r.sharedMaterial.shader);
            mat.color = ColCyan;
            mat.EnableKeyword("_EMISSION");
            mat.SetColor("_EmissionColor", ColCyan * 1.5f);
            r.sharedMaterial = mat;
        }

        // Orbiting halo ring around fragment
        var halo = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        halo.name = "HaloRing";
        halo.transform.SetParent(f.transform, false);
        halo.transform.localPosition = Vector3.zero;
        halo.transform.localRotation = Quaternion.Euler(30f, 45f, 0f);
        halo.transform.localScale = new Vector3(1.6f, 0.04f, 1.6f);
        var haloCol = halo.GetComponent<Collider>();
        if (haloCol != null) Object.DestroyImmediate(haloCol);

        var haloR = halo.GetComponent<Renderer>();
        if (haloR != null)
        {
            var mat = new Material(Shader.Find("Universal Render Pipeline/Lit") ?? haloR.sharedMaterial.shader);
            mat.color = ColCyan;
            mat.EnableKeyword("_EMISSION");
            mat.SetColor("_EmissionColor", ColCyan * 0.8f);
            haloR.sharedMaterial = mat;
        }

        var mf = f.AddComponent<RuleBreak.Gameplay.MemoryFragment>();
        var so = new SerializedObject(mf);
        so.FindProperty("storyMessage").stringValue  = story;
        so.FindProperty("fragmentIndex").intValue    = index;
        so.ApplyModifiedProperties();

        Undo.RegisterCreatedObjectUndo(f, "Create " + name);
    }

    // ── Checkpoint ─────────────────────────────────────────────────────────────
    public static void MakeCheckpoint(string name, Vector3 pos, GameObject parent)
    {
        if (parent.transform.Find(name) != null) return;

        var cp = new GameObject(name);
        cp.transform.SetParent(parent.transform);
        cp.transform.position = pos;

        var bc = cp.AddComponent<BoxCollider>();
        bc.isTrigger = true;
        bc.size = new Vector3(3.5f, 3f, 3.5f);

        // Tech base pedestal
        var basePed = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        basePed.name = "BasePedestal";
        basePed.transform.SetParent(cp.transform, false);
        basePed.transform.localPosition = new Vector3(0f, -0.05f, 0f);
        basePed.transform.localScale = new Vector3(2f, 0.08f, 2f);
        var baseCol = basePed.GetComponent<Collider>();
        if (baseCol != null) Object.DestroyImmediate(baseCol);

        var rBase = basePed.GetComponent<Renderer>();
        if (rBase != null)
        {
            var mat = new Material(Shader.Find("Universal Render Pipeline/Lit") ?? rBase.sharedMaterial.shader);
            mat.color = ColSurface;
            rBase.sharedMaterial = mat;
        }

        // Active beacon ring
        var ring = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        ring.name = "BeaconRing";
        ring.transform.SetParent(cp.transform, false);
        ring.transform.localPosition = new Vector3(0f, 0.1f, 0f);
        ring.transform.localScale    = new Vector3(1.6f, 0.04f, 1.6f);

        var rr = ring.GetComponent<Renderer>();
        if (rr != null)
        {
            var mat = new Material(Shader.Find("Universal Render Pipeline/Lit") ?? rr.sharedMaterial.shader);
            mat.color = ColCyan;
            mat.EnableKeyword("_EMISSION");
            mat.SetColor("_EmissionColor", ColCyan * 0.9f);
            rr.sharedMaterial = mat;
        }

        var ringCol = ring.GetComponent<Collider>();
        if (ringCol != null) Object.DestroyImmediate(ringCol);

        // Central floating diamond beacon
        var diamond = GameObject.CreatePrimitive(PrimitiveType.Cube);
        diamond.name = "BeaconDiamond";
        diamond.transform.SetParent(cp.transform, false);
        diamond.transform.localPosition = new Vector3(0f, 1.2f, 0f);
        diamond.transform.localRotation = Quaternion.Euler(45f, 45f, 45f);
        diamond.transform.localScale = new Vector3(0.45f, 0.45f, 0.45f);
        var diaCol = diamond.GetComponent<Collider>();
        if (diaCol != null) Object.DestroyImmediate(diaCol);

        var diaR = diamond.GetComponent<Renderer>();
        if (diaR != null)
        {
            var mat = new Material(Shader.Find("Universal Render Pipeline/Lit") ?? diaR.sharedMaterial.shader);
            mat.color = ColCyan;
            mat.EnableKeyword("_EMISSION");
            mat.SetColor("_EmissionColor", ColCyan * 1.2f);
            diaR.sharedMaterial = mat;
        }

        var cm = cp.AddComponent<RuleBreak.Gameplay.CheckpointMarker>();
        var so = new SerializedObject(cm);
        so.FindProperty("activeVisual").objectReferenceValue = ring;
        so.ApplyModifiedProperties();

        Undo.RegisterCreatedObjectUndo(cp, "Create " + name);
    }

    // ── Exit portal ────────────────────────────────────────────────────────────
    public static void MakeExitPortal(string name, Vector3 pos, string nextScene, GameObject parent)
    {
        if (parent.transform.Find(name) != null) return;

        var ep = new GameObject(name);
        ep.transform.SetParent(parent.transform);
        ep.transform.position = pos;

        var sc = ep.AddComponent<SphereCollider>();
        sc.isTrigger = true;
        sc.radius    = 1.8f;

        // Outer Gateway Ring
        var gateFrame = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        gateFrame.name = "GatewayFrame";
        gateFrame.transform.SetParent(ep.transform, false);
        gateFrame.transform.localPosition = Vector3.zero;
        gateFrame.transform.localRotation = Quaternion.Euler(90f, 0f, 0f);
        gateFrame.transform.localScale = new Vector3(3.2f, 0.15f, 3.2f);
        var gfCol = gateFrame.GetComponent<Collider>();
        if (gfCol != null) Object.DestroyImmediate(gfCol);

        var rFrame = gateFrame.GetComponent<Renderer>();
        if (rFrame != null)
        {
            var mat = new Material(Shader.Find("Universal Render Pipeline/Lit") ?? rFrame.sharedMaterial.shader);
            mat.color = ColDeepNavy;
            mat.EnableKeyword("_EMISSION");
            mat.SetColor("_EmissionColor", ColNeonBlue * 0.4f);
            rFrame.sharedMaterial = mat;
        }

        // Inner glowing portal core
        var visual = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        visual.name = "PortalVisual";
        visual.transform.SetParent(ep.transform, false);
        visual.transform.localPosition = Vector3.zero;
        visual.transform.localScale    = new Vector3(2.4f, 2.4f, 0.6f);

        var r = visual.GetComponent<Renderer>();
        if (r != null)
        {
            var mat = new Material(Shader.Find("Universal Render Pipeline/Lit") ?? r.sharedMaterial.shader);
            mat.color = ColCyan;
            mat.EnableKeyword("_EMISSION");
            mat.SetColor("_EmissionColor", ColCyan * 1.5f);
            r.sharedMaterial = mat;
        }

        var vc = visual.GetComponent<Collider>();
        if (vc != null) Object.DestroyImmediate(vc);

        var portal = ep.AddComponent<RuleBreak.Gameplay.ExitPortal>();
        var so = new SerializedObject(portal);
        so.FindProperty("nextSceneName").stringValue              = nextScene;
        so.FindProperty("portalVisual").objectReferenceValue      = visual;
        so.ApplyModifiedProperties();

        Undo.RegisterCreatedObjectUndo(ep, "Create " + name);
    }

    // ── Atmosphere light ───────────────────────────────────────────────────────
    public static void MakePointLight(string name, Vector3 pos, Color col,
                                      float intensity, float range, GameObject parent)
    {
        if (parent.transform.Find(name) != null) return;

        var lg = new GameObject(name);
        lg.transform.SetParent(parent.transform);
        lg.transform.position = pos;

        var l = lg.AddComponent<Light>();
        l.type      = LightType.Point;
        l.color     = col;
        l.intensity = intensity;
        l.range     = range;

        Undo.RegisterCreatedObjectUndo(lg, "Create " + name);
    }

    // ── Corruptible object ─────────────────────────────────────────────────────
    public static GameObject MakeCorruptible(string name, Vector3 pos, Vector3 scale,
                                             RuleBreak.Corruption.CorruptionType type,
                                             GameObject parent)
    {
        if (parent.transform.Find(name) != null) return parent.transform.Find(name).gameObject;

        var obj = GameObject.CreatePrimitive(PrimitiveType.Cube);
        obj.name = name;
        obj.transform.SetParent(parent.transform);
        obj.transform.position   = pos;
        obj.transform.localScale = scale;

        var r = obj.GetComponent<Renderer>();
        if (r != null)
        {
            var mat = new Material(Shader.Find("Universal Render Pipeline/Lit") ?? r.sharedMaterial.shader);
            mat.color = ColAmber;
            mat.EnableKeyword("_EMISSION");
            mat.SetColor("_EmissionColor", ColAmber * 0.3f);
            r.sharedMaterial = mat;
        }

        var co = obj.AddComponent<RuleBreak.Corruption.CorruptibleObject>();
        var so = new SerializedObject(co);
        so.FindProperty("corruptionType").enumValueIndex = (int)type;
        so.ApplyModifiedProperties();

        Undo.RegisterCreatedObjectUndo(obj, "Create " + name);
        return obj;
    }

    // ── Save modified scene ────────────────────────────────────────────────────
    public static void SaveScene()
    {
        EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
        EditorSceneManager.SaveScene(SceneManager.GetActiveScene());
    }

    // =========================================================================
    // VALIDATE PROJECT
    // =========================================================================
    [MenuItem("RULE//BREAK/Validate Project")]
    public static void ValidateProject()
    {
        var issues = new System.Text.StringBuilder();
        int ok = 0;

        // Check all required scripts exist
        string[] requiredScripts = {
            "Assets/Scripts/Player/PlayerController.cs",
            "Assets/Scripts/Player/PlayerCamera.cs",
            "Assets/Scripts/Player/PlayerRespawn.cs",
            "Assets/Scripts/Player/Checkpoint.cs",
            "Assets/Scripts/Gravity/GravityShifter.cs",
            "Assets/Scripts/Time/TimeFreezeManager.cs",
            "Assets/Scripts/Time/FreezableObject.cs",
            "Assets/Scripts/Dimension/DimensionManager.cs",
            "Assets/Scripts/Dimension/DimensionObject.cs",
            "Assets/Scripts/Corruption/RuleCorruptionManager.cs",
            "Assets/Scripts/Corruption/CorruptibleObject.cs",
            "Assets/Scripts/Level/GlitchEnergyManager.cs",
            "Assets/Scripts/Level/LevelManager.cs",
            "Assets/Scripts/Save/SaveManager.cs",
            "Assets/Scripts/Audio/AudioManager.cs",
            "Assets/Scripts/UI/HUDController.cs",
            "Assets/Scripts/UI/MainMenuUI.cs",
            "Assets/Scripts/UI/PauseMenuUI.cs",
            "Assets/Scripts/UI/FinalRevealUI.cs",
            "Assets/Scripts/Player/CapsuleProceduralAnimator.cs",
            "Assets/Scripts/Story/FinalRevealManager.cs",
        };

        foreach (var s in requiredScripts)
        {
            if (File.Exists(s)) ok++;
            else issues.AppendLine($"MISSING: {s}");
        }

        // Check scenes exist
        string[] scenes = { "Assets/Scenes/MainMenu.unity", "Assets/Scenes/Level01.unity",
                             "Assets/Scenes/Level02.unity", "Assets/Scenes/Level03.unity",
                             "Assets/Scenes/Level04.unity" };
        foreach (var sc in scenes)
        {
            if (File.Exists(sc)) ok++;
            else issues.AppendLine($"MISSING SCENE: {sc}");
        }

        string result = issues.Length == 0
            ? $"✓ All {ok} checks passed. Project looks healthy."
            : $"⚠ {ok} checks passed. Issues found:\n\n{issues}";

        EditorUtility.DisplayDialog("RULE//BREAK – Validation", result, "OK");
    }

    // =========================================================================
    // REGISTER ALL SCENES IN BUILD SETTINGS
    // =========================================================================
    [MenuItem("RULE//BREAK/Register All Scenes in Build Settings")]
    public static void RegisterScenes()
    {
        var scenes = new[]
        {
            "Assets/Scenes/MainMenu.unity",
            "Assets/Scenes/Level01.unity",
            "Assets/Scenes/Level02.unity",
            "Assets/Scenes/Level03.unity",
            "Assets/Scenes/Level04.unity"
        };

        var buildScenes = new EditorBuildSettingsScene[scenes.Length];
        for (int i = 0; i < scenes.Length; i++)
        {
            buildScenes[i] = new EditorBuildSettingsScene(scenes[i], true);
        }

        EditorBuildSettings.scenes = buildScenes;
        EditorUtility.DisplayDialog("RULE//BREAK",
            "All 5 scenes registered in Build Settings:\n" +
            "0: MainMenu\n1: Level01\n2: Level02\n3: Level03\n4: Level04", "OK");
    }
}

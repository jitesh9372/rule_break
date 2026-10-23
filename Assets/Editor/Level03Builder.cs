// =============================================================================
// Level03Builder.cs — RULE//BREAK
// Menu: RULE//BREAK > Build Level 03 Environment
// Dimension-themed level: two distinct reality layouts.
// =============================================================================
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;
using RuleBreak.Dimension;

public static class Level03Builder
{
    [MenuItem("RULE//BREAK/Build Level 03 Environment")]
    public static void BuildLevel03()
    {
        if (!RuleBreakMasterBuilder.EnsureScene("Level03")) return;

        var grpShared   = RuleBreakMasterBuilder.GetOrCreateGroup("L03_Shared");
        var grpRealityA = RuleBreakMasterBuilder.GetOrCreateGroup("L03_RealityA");
        var grpRealityB = RuleBreakMasterBuilder.GetOrCreateGroup("L03_RealityB");
        var grpHazards  = RuleBreakMasterBuilder.GetOrCreateGroup("L03_Hazards");
        var grpCollect  = RuleBreakMasterBuilder.GetOrCreateGroup("L03_Collectibles");
        var grpChk      = RuleBreakMasterBuilder.GetOrCreateGroup("L03_Checkpoints");
        var grpExit     = RuleBreakMasterBuilder.GetOrCreateGroup("L03_Exit");
        var grpLights   = RuleBreakMasterBuilder.GetOrCreateGroup("L03_Lights");

        // ── Shared geometry (exists in both realities) ────────────────────────
        RuleBreakMasterBuilder.MakePlatform("Floor_L03", new Vector3(0f, -0.15f, 0f),
            new Vector3(12f, 0.3f, 8f), grpShared, RuleBreakMasterBuilder.ColDeepNavy);
        RuleBreakMasterBuilder.MakePlatform("Shared_01", new Vector3(0f, 0f, 14f),
            new Vector3(5f, 0.3f, 4f), grpShared, RuleBreakMasterBuilder.ColSurface, true);
        RuleBreakMasterBuilder.MakePlatform("Shared_02", new Vector3(0f, 2f, 40f),
            new Vector3(5f, 0.3f, 5f), grpShared, RuleBreakMasterBuilder.ColSurface, true);
        RuleBreakMasterBuilder.MakePlatform("Shared_03", new Vector3(0f, 4f, 60f),
            new Vector3(6f, 0.3f, 6f), grpShared, RuleBreakMasterBuilder.ColNeonBlue, true);

        // ── Reality A platforms ──────────────────────────────────────────────
        MakeDimensionPlatform("A_Plat_01", new Vector3(-5f, 1f, 20f),  new Vector3(4f, 0.3f, 4f), DimensionOwnership.OnlyRealityA, grpRealityA, RuleBreakMasterBuilder.ColNeonBlue);
        MakeDimensionPlatform("A_Plat_02", new Vector3(-8f, 2f, 28f),  new Vector3(3f, 0.3f, 3f), DimensionOwnership.OnlyRealityA, grpRealityA, RuleBreakMasterBuilder.ColNeonBlue);
        MakeDimensionPlatform("A_Plat_03", new Vector3(-5f, 3f, 36f),  new Vector3(4f, 0.3f, 4f), DimensionOwnership.OnlyRealityA, grpRealityA, RuleBreakMasterBuilder.ColNeonBlue);
        // Reality A wall (blocks passage in A, open in B)
        MakeDimensionPlatform("A_Wall_01", new Vector3(0f, 2f, 50f),   new Vector3(1f, 6f, 0.5f), DimensionOwnership.OnlyRealityA, grpRealityA, RuleBreakMasterBuilder.ColRed);

        // ── Reality B platforms ──────────────────────────────────────────────
        MakeDimensionPlatform("B_Plat_01", new Vector3(5f, 1f, 20f),   new Vector3(4f, 0.3f, 4f), DimensionOwnership.OnlyRealityB, grpRealityB, RuleBreakMasterBuilder.ColViolet);
        MakeDimensionPlatform("B_Plat_02", new Vector3(8f, 2f, 28f),   new Vector3(3f, 0.3f, 3f), DimensionOwnership.OnlyRealityB, grpRealityB, RuleBreakMasterBuilder.ColViolet);
        MakeDimensionPlatform("B_Plat_03", new Vector3(5f, 3f, 36f),   new Vector3(4f, 0.3f, 4f), DimensionOwnership.OnlyRealityB, grpRealityB, RuleBreakMasterBuilder.ColViolet);
        // Hidden B path: bridge that only appears in B
        MakeDimensionPlatform("B_Bridge",  new Vector3(0f, 2f, 52f),   new Vector3(5f, 0.3f, 4f), DimensionOwnership.OnlyRealityB, grpRealityB, RuleBreakMasterBuilder.ColViolet);

        // ── Hazards ──────────────────────────────────────────────────────────
        RuleBreakMasterBuilder.MakeHazard("Spike_L03_01", new Vector3(0f, 1.2f, 24f), grpHazards);
        RuleBreakMasterBuilder.MakeHazard("Spike_L03_02", new Vector3(0f, 3.2f, 44f), grpHazards);
        RuleBreakMasterBuilder.MakeVoidKillZone("VoidKill_L03", new Vector3(0f, -15f, 35f), grpHazards);

        // ── Memory Fragments (3 total in L03) ────────────────────────────────
        RuleBreakMasterBuilder.MakeFragment("Fragment_L03_01",
            new Vector3(-8f, 3f, 28f), grpCollect,
            "SYSTEM LOG 003\n\"Something is changing the world.\"\n\"It adapts. It learns.\"", 4);
        RuleBreakMasterBuilder.MakeFragment("Fragment_L03_02",
            new Vector3(8f, 3f, 28f), grpCollect,
            "SYSTEM LOG 003b\n\"Two realities observed simultaneously.\"\n\"Observer effect confirmed.\"", 5);
        RuleBreakMasterBuilder.MakeFragment("Fragment_L03_03",
            new Vector3(0f, 5f, 60f), grpCollect,
            "SYSTEM LOG 003c\n\"The observer IS the corruption.\"\n\"Query: Is the observer aware?\"", 6);

        // ── Checkpoints ──────────────────────────────────────────────────────
        RuleBreakMasterBuilder.MakeCheckpoint("Checkpoint_L03_A", new Vector3(0f, 2.5f, 40f), grpChk);
        RuleBreakMasterBuilder.MakeCheckpoint("Checkpoint_L03_B", new Vector3(0f, 4.5f, 60f), grpChk);

        // ── Exit portal ──────────────────────────────────────────────────────
        RuleBreakMasterBuilder.MakeExitPortal("Exit_L03", new Vector3(0f, 4.5f, 70f), "Level04", grpExit);

        // ── Lights ───────────────────────────────────────────────────────────
        RuleBreakMasterBuilder.MakePointLight("Light_L03_A", new Vector3(-6f, 4f, 24f), RuleBreakMasterBuilder.ColNeonBlue, 2f, 12f, grpLights);
        RuleBreakMasterBuilder.MakePointLight("Light_L03_B", new Vector3(6f,  4f, 24f), RuleBreakMasterBuilder.ColViolet, 2f, 12f, grpLights);
        RuleBreakMasterBuilder.MakePointLight("Light_L03_X", new Vector3(0f,  6f, 55f), RuleBreakMasterBuilder.ColMagenta, 3f, 15f, grpLights);
        RuleBreakMasterBuilder.MakePointLight("Light_L03_Exit", new Vector3(0f, 6f, 70f), RuleBreakMasterBuilder.ColCyan, 4f, 18f, grpLights);

        // ── Managers ─────────────────────────────────────────────────────────
        SetupLevel03Managers();

        RuleBreakMasterBuilder.SaveScene();

        EditorUtility.DisplayDialog("RULE//BREAK",
            "✓ Level 03 (DIMENSION) built!\n\n" +
            "Layout:\n" +
            "• 3 shared platforms\n" +
            "• 4 Reality A only platforms (blue)\n" +
            "• 4 Reality B only platforms (violet)\n" +
            "• Reality A blocking wall (absent in B)\n" +
            "• Reality B bridge secret path\n" +
            "• 3 Memory Fragments\n" +
            "• 2 Checkpoints\n" +
            "• Exit portal at Z=70", "OK");
    }

    static void MakeDimensionPlatform(string name, Vector3 pos, Vector3 scale,
                                       DimensionOwnership ownership, GameObject parent, Color color)
    {
        if (parent.transform.Find(name) != null) return;

        var p = RuleBreakMasterBuilder.MakePlatform(name, pos, scale, parent, color, true);
        if (p == null) return;

        var dimObj = p.AddComponent<DimensionObject>();
        var so = new SerializedObject(dimObj);
        so.FindProperty("ownership").enumValueIndex = (int)ownership;
        so.ApplyModifiedProperties();
    }

    static void SetupLevel03Managers()
    {
        // DimensionManager
        if (GameObject.Find("DimensionManager") == null)
        {
            var dm = new GameObject("DimensionManager");
            dm.AddComponent<RuleBreak.Dimension.DimensionManager>();
            Undo.RegisterCreatedObjectUndo(dm, "Create DimensionManager");
        }
        // TimeFreezeManager
        if (GameObject.Find("TimeFreezeManager") == null)
        {
            var tfm = new GameObject("TimeFreezeManager");
            tfm.AddComponent<RuleBreak.Time.TimeFreezeManager>();
            Undo.RegisterCreatedObjectUndo(tfm, "Create TimeFreezeManager");
        }
    }
}

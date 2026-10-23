// =============================================================================
// Level04Builder.cs — RULE//BREAK
// Menu: RULE//BREAK > Build Level 04 Environment
// Rule Breaker – uses all four abilities. Includes ERROR 404 secret room.
// =============================================================================
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;
using RuleBreak.Corruption;
using RuleBreak.Dimension;

public static class Level04Builder
{
    [MenuItem("RULE//BREAK/Build Level 04 Environment")]
    public static void BuildLevel04()
    {
        if (!RuleBreakMasterBuilder.EnsureScene("Level04")) return;

        var grpPlatforms = RuleBreakMasterBuilder.GetOrCreateGroup("L04_Platforms");
        var grpHazards   = RuleBreakMasterBuilder.GetOrCreateGroup("L04_Hazards");
        var grpMoving    = RuleBreakMasterBuilder.GetOrCreateGroup("L04_Moving");
        var grpCorrupt   = RuleBreakMasterBuilder.GetOrCreateGroup("L04_Corruptible");
        var grpDimension = RuleBreakMasterBuilder.GetOrCreateGroup("L04_Dimension");
        var grpSecret    = RuleBreakMasterBuilder.GetOrCreateGroup("L04_Secret_ERROR404");
        var grpCollect   = RuleBreakMasterBuilder.GetOrCreateGroup("L04_Collectibles");
        var grpChk       = RuleBreakMasterBuilder.GetOrCreateGroup("L04_Checkpoints");
        var grpFinal     = RuleBreakMasterBuilder.GetOrCreateGroup("L04_FinalPuzzle");
        var grpExit      = RuleBreakMasterBuilder.GetOrCreateGroup("L04_Exit");
        var grpLights    = RuleBreakMasterBuilder.GetOrCreateGroup("L04_Lights");

        // ── Starting area ────────────────────────────────────────────────────
        RuleBreakMasterBuilder.MakePlatform("Floor_L04", new Vector3(0f, -0.15f, 0f),
            new Vector3(12f, 0.3f, 8f), grpPlatforms, RuleBreakMasterBuilder.ColDeepNavy);

        // ── Main path – glitching platform chain ─────────────────────────────
        RuleBreakMasterBuilder.MakePlatform("L4_Plat_01", new Vector3(0f, 0f, 12f),   new Vector3(5f, 0.3f, 4f), grpPlatforms, RuleBreakMasterBuilder.ColSurface, true);
        RuleBreakMasterBuilder.MakePlatform("L4_Plat_02", new Vector3(5f, 1f, 20f),   new Vector3(4f, 0.3f, 4f), grpPlatforms, RuleBreakMasterBuilder.ColSurface, true);
        RuleBreakMasterBuilder.MakePlatform("L4_Plat_03", new Vector3(-5f, 2f, 28f),  new Vector3(4f, 0.3f, 4f), grpPlatforms, RuleBreakMasterBuilder.ColSurface, true);
        RuleBreakMasterBuilder.MakePlatform("L4_Plat_04", new Vector3(0f, 3f, 36f),   new Vector3(5f, 0.3f, 5f), grpPlatforms, RuleBreakMasterBuilder.ColSurface, true);
        RuleBreakMasterBuilder.MakePlatform("L4_Plat_05", new Vector3(0f, 5f, 50f),   new Vector3(5f, 0.3f, 5f), grpPlatforms, RuleBreakMasterBuilder.ColNeonBlue, true);

        // Ceiling platforms for gravity flip in final puzzle
        RuleBreakMasterBuilder.MakePlatform("L4_Ceiling_01", new Vector3(0f, 12f, 60f), new Vector3(6f, 0.3f, 6f), grpPlatforms, RuleBreakMasterBuilder.ColViolet, true);
        RuleBreakMasterBuilder.MakePlatform("CeilingRoof_L04", new Vector3(0f, 13f, 60f), new Vector3(16f, 0.3f, 20f), grpPlatforms, RuleBreakMasterBuilder.ColDeepNavy);

        // ── Moving platforms ─────────────────────────────────────────────────
        RuleBreakMasterBuilder.MakeMovingPlatform("L4_MP_01",
            new Vector3(-5f, 2f, 24f), new Vector3(5f, 2f, 24f), grpMoving);
        RuleBreakMasterBuilder.MakeMovingPlatform("L4_MP_02",
            new Vector3(0f, 4f, 42f), new Vector3(0f, 7f, 42f), grpMoving);

        // ── Corruptible lasers (become platforms) ────────────────────────────
        RuleBreakMasterBuilder.MakeCorruptible("Laser_C01",
            new Vector3(0f, 1f, 16f), new Vector3(10f, 0.2f, 0.2f),
            CorruptionType.LaserToPlatform, grpCorrupt);
        RuleBreakMasterBuilder.MakeCorruptible("Laser_C02",
            new Vector3(0f, 3f, 46f), new Vector3(12f, 0.2f, 0.2f),
            CorruptionType.LaserToPlatform, grpCorrupt);

        // ── Corruptible spikes (become bounce pads) ───────────────────────────
        RuleBreakMasterBuilder.MakeCorruptible("Spike_C01",
            new Vector3(3f, 0.4f, 22f), new Vector3(2f, 0.5f, 2f),
            CorruptionType.SpikeToBouncePad, grpCorrupt);

        // Kill trigger for uncorrupted spike
        var spikeTrigger = new GameObject("Spike_C01_Kill");
        spikeTrigger.transform.position = new Vector3(3f, 0.7f, 22f);
        var sbc = spikeTrigger.AddComponent<BoxCollider>();
        sbc.isTrigger = true;
        sbc.size = Vector3.one;
        spikeTrigger.AddComponent<RuleBreak.Gameplay.KillZone>();
        Undo.RegisterCreatedObjectUndo(spikeTrigger, "Create Spike_C01_Kill");

        // ── Corruptible wall (becomes passable) ───────────────────────────────
        RuleBreakMasterBuilder.MakeCorruptible("Wall_C01",
            new Vector3(0f, 2f, 55f), new Vector3(0.5f, 6f, 8f),
            CorruptionType.WallToPassable, grpCorrupt);

        // ── Dimension objects in L04 ─────────────────────────────────────────
        MakeDimOnly("DimB_Plat_01", new Vector3(0f, 2f, 32f), new Vector3(4f, 0.3f, 4f),
                    DimensionOwnership.OnlyRealityB, grpDimension, RuleBreakMasterBuilder.ColViolet);
        MakeDimOnly("DimB_Plat_02", new Vector3(0f, 4f, 56f), new Vector3(4f, 0.3f, 4f),
                    DimensionOwnership.OnlyRealityB, grpDimension, RuleBreakMasterBuilder.ColViolet);

        // ── Final puzzle sequence geometry ────────────────────────────────────
        // Platform sequence requiring all 4 abilities in order
        RuleBreakMasterBuilder.MakePlatform("FP_00_Entry",  new Vector3(0f, 5f, 62f),  new Vector3(4f, 0.3f, 4f), grpFinal, RuleBreakMasterBuilder.ColSurface, true);
        RuleBreakMasterBuilder.MakePlatform("FP_01_Ceiling", new Vector3(0f, 12f, 68f), new Vector3(4f, 0.3f, 4f), grpFinal, RuleBreakMasterBuilder.ColViolet, true);
        RuleBreakMasterBuilder.MakePlatform("FP_02_DimB",   new Vector3(0f, 12f, 74f), new Vector3(4f, 0.3f, 4f), grpFinal, RuleBreakMasterBuilder.ColViolet, true);

        // Moving frozen laser in final area
        RuleBreakMasterBuilder.MakeCorruptible("FP_Laser_Final",
            new Vector3(0f, 8f, 66f), new Vector3(16f, 0.25f, 0.25f),
            CorruptionType.LaserToPlatform, grpFinal);

        // Final corrupt door
        RuleBreakMasterBuilder.MakeCorruptible("FP_Door_Final",
            new Vector3(0f, 3f, 80f), new Vector3(4f, 6f, 0.5f),
            CorruptionType.WallToPassable, grpFinal);

        // ── ERROR 404 Secret Room ─────────────────────────────────────────────
        BuildSecretRoom(grpSecret);

        // ── Hazards ──────────────────────────────────────────────────────────
        RuleBreakMasterBuilder.MakeHazard("Spike_L04_01", new Vector3(5f, 2.2f, 28f), grpHazards);
        RuleBreakMasterBuilder.MakeHazard("Spike_L04_02", new Vector3(-5f, 3.2f, 38f), grpHazards);
        RuleBreakMasterBuilder.MakeVoidKillZone("VoidKill_L04", new Vector3(0f, -15f, 40f), grpHazards);

        // ── Memory Fragments ─────────────────────────────────────────────────
        RuleBreakMasterBuilder.MakeFragment("Fragment_L04_01",
            new Vector3(5f, 4f, 36f), grpCollect,
            "SYSTEM LOG 004\n\"Who gave you permission to change the rules?\"\n\"Who gave YOU permission?\"", 7);
        RuleBreakMasterBuilder.MakeFragment("Fragment_L04_02",
            new Vector3(-5f, 3f, 50f), grpCollect,
            "FINAL LOG\n\"CORRUPTION SOURCE: YOU.\"\n\"You were the anomaly all along.\"", 8);
        RuleBreakMasterBuilder.MakeFragment("Fragment_L04_SECRET",
            new Vector3(0f, 1f, -40f), grpSecret,    // inside secret room
            "ERROR 404\n\"You were never supposed to reach this place.\"\n\"And yet here you are.\"", 9);

        // ── Checkpoints ──────────────────────────────────────────────────────
        RuleBreakMasterBuilder.MakeCheckpoint("Checkpoint_L04_A", new Vector3(0f, 3.5f, 36f), grpChk);
        RuleBreakMasterBuilder.MakeCheckpoint("Checkpoint_L04_B", new Vector3(0f, 5.5f, 50f), grpChk);
        RuleBreakMasterBuilder.MakeCheckpoint("Checkpoint_L04_C", new Vector3(0f, 5.5f, 62f), grpChk);

        // ── Exit (triggers FinalReveal, not next scene load) ─────────────────
        RuleBreakMasterBuilder.MakeExitPortal("Exit_L04", new Vector3(0f, 3.5f, 85f), "MainMenu", grpExit);

        // ── Lights ───────────────────────────────────────────────────────────
        RuleBreakMasterBuilder.MakePointLight("Light_L04_01", new Vector3(0f, 5f, 25f), RuleBreakMasterBuilder.ColAmber, 3f, 15f, grpLights);
        RuleBreakMasterBuilder.MakePointLight("Light_L04_02", new Vector3(0f, 5f, 45f), RuleBreakMasterBuilder.ColViolet, 2f, 12f, grpLights);
        RuleBreakMasterBuilder.MakePointLight("Light_L04_03", new Vector3(0f, 8f, 65f), RuleBreakMasterBuilder.ColMagenta, 3f, 15f, grpLights);
        RuleBreakMasterBuilder.MakePointLight("Light_L04_Exit", new Vector3(0f, 5f, 85f), RuleBreakMasterBuilder.ColCyan, 4f, 18f, grpLights);

        // ── Setup all required managers ───────────────────────────────────────
        SetupLevel04Managers();

        RuleBreakMasterBuilder.SaveScene();

        EditorUtility.DisplayDialog("RULE//BREAK",
            "✓ Level 04 (RULE BREAKER) built!\n\n" +
            "Layout:\n" +
            "• 5 main platforms + ceiling path\n" +
            "• 2 moving platforms\n" +
            "• 2 corruptible lasers (→ platform)\n" +
            "• 1 corruptible spike (→ bounce pad)\n" +
            "• 1 corruptible wall (→ passable)\n" +
            "• 2 dimension B only platforms\n" +
            "• Final puzzle sequence (all 4 abilities)\n" +
            "• ERROR 404 secret room\n" +
            "• 3 Memory Fragments\n" +
            "• 3 Checkpoints\n" +
            "• Exit portal at Z=85\n\n" +
            "⚠ Assign FinalRevealManager to Exit_L04 manually.", "OK");
    }

    static void BuildSecretRoom(GameObject parent)
    {
        // The secret room is at Z=-30 to -50 (behind the start), entered via
        // a corruptible wall trigger in L04
        var roomCenter = new Vector3(0f, 0f, -40f);

        // Room walls + floor (mostly black)
        RuleBreakMasterBuilder.MakePlatform("SR_Floor",  roomCenter + Vector3.down * 0.15f, new Vector3(16f, 0.3f, 16f), parent, RuleBreakMasterBuilder.ColVoid);
        RuleBreakMasterBuilder.MakePlatform("SR_WallN",  roomCenter + new Vector3(0f, 3f, 8f),  new Vector3(16f, 6f, 0.3f), parent, RuleBreakMasterBuilder.ColVoid);
        RuleBreakMasterBuilder.MakePlatform("SR_WallS",  roomCenter + new Vector3(0f, 3f, -8f), new Vector3(16f, 6f, 0.3f), parent, RuleBreakMasterBuilder.ColVoid);
        RuleBreakMasterBuilder.MakePlatform("SR_WallE",  roomCenter + new Vector3(8f, 3f, 0f),  new Vector3(0.3f, 6f, 16f), parent, RuleBreakMasterBuilder.ColVoid);
        RuleBreakMasterBuilder.MakePlatform("SR_WallW",  roomCenter + new Vector3(-8f, 3f, 0f), new Vector3(0.3f, 6f, 16f), parent, RuleBreakMasterBuilder.ColVoid);
        RuleBreakMasterBuilder.MakePlatform("SR_Ceiling", roomCenter + new Vector3(0f, 6f, 0f),  new Vector3(16f, 0.3f, 16f), parent, RuleBreakMasterBuilder.ColVoid);

        // Sparse magenta + cyan lights
        RuleBreakMasterBuilder.MakePointLight("SR_Light_Magenta", roomCenter + new Vector3(-3f, 4f, 0f),
            RuleBreakMasterBuilder.ColMagenta, 1.5f, 8f, parent);
        RuleBreakMasterBuilder.MakePointLight("SR_Light_Cyan", roomCenter + new Vector3(3f, 4f, 0f),
            RuleBreakMasterBuilder.ColCyan, 1f, 6f, parent);

        // Hidden entrance – corruptible wall connecting main level to secret room
        RuleBreakMasterBuilder.MakeCorruptible("SecretEntrance",
            new Vector3(0f, 2f, -5f), new Vector3(4f, 6f, 0.5f),
            CorruptionType.WallToPassable, parent);

        // Corridor connecting main area to secret room
        RuleBreakMasterBuilder.MakePlatform("SR_Corridor",
            new Vector3(0f, 0f, -20f), new Vector3(4f, 0.3f, 30f), parent, RuleBreakMasterBuilder.ColVoid);
    }

    static void MakeDimOnly(string name, Vector3 pos, Vector3 scale,
                             DimensionOwnership own, GameObject parent, Color col)
    {
        if (parent.transform.Find(name) != null) return;
        var p = RuleBreakMasterBuilder.MakePlatform(name, pos, scale, parent, col, true);
        if (p == null) return;
        var dimObj = p.AddComponent<DimensionObject>();
        var so = new SerializedObject(dimObj);
        so.FindProperty("ownership").enumValueIndex = (int)own;
        so.ApplyModifiedProperties();
    }

    static void SetupLevel04Managers()
    {
        if (GameObject.Find("TimeFreezeManager") == null)
        {
            var go = new GameObject("TimeFreezeManager");
            go.AddComponent<RuleBreak.Time.TimeFreezeManager>();
            Undo.RegisterCreatedObjectUndo(go, "Create TimeFreezeManager");
        }
        if (GameObject.Find("DimensionManager") == null)
        {
            var go = new GameObject("DimensionManager");
            go.AddComponent<RuleBreak.Dimension.DimensionManager>();
            Undo.RegisterCreatedObjectUndo(go, "Create DimensionManager");
        }
        if (GameObject.Find("RuleCorruptionManager") == null)
        {
            var go = new GameObject("RuleCorruptionManager");
            go.AddComponent<RuleBreak.Corruption.RuleCorruptionManager>();
            Undo.RegisterCreatedObjectUndo(go, "Create RuleCorruptionManager");
        }
        if (GameObject.Find("DeathRealityManager") == null)
        {
            var go = new GameObject("DeathRealityManager");
            go.AddComponent<RuleBreak.Story.DeathRealityManager>();
            Undo.RegisterCreatedObjectUndo(go, "Create DeathRealityManager");
        }
        if (GameObject.Find("PlayerBehaviorTracker") == null)
        {
            // Attach to player
            var player = GameObject.FindGameObjectWithTag("Player");
            if (player != null) player.AddComponent<RuleBreak.Story.PlayerBehaviorTracker>();
        }
    }
}

// =============================================================================
// Level02Builder.cs — RULE//BREAK
// Menu: RULE//BREAK > Build Level 02 Environment
// Time-themed level: moving lasers, rotating hazards, timed doors.
// =============================================================================
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;
using RuleBreak.Time;

public static class Level02Builder
{
    [MenuItem("RULE//BREAK/Build Level 02 Environment")]
    public static void BuildLevel02()
    {
        if (!RuleBreakMasterBuilder.EnsureScene("Level02")) return;

        // ── Groups ───────────────────────────────────────────────────────────
        var grpPlatforms = RuleBreakMasterBuilder.GetOrCreateGroup("L02_Platforms");
        var grpHazards   = RuleBreakMasterBuilder.GetOrCreateGroup("L02_Hazards");
        var grpMoving    = RuleBreakMasterBuilder.GetOrCreateGroup("L02_Moving");
        var grpLasers    = RuleBreakMasterBuilder.GetOrCreateGroup("L02_Lasers");
        var grpCollect   = RuleBreakMasterBuilder.GetOrCreateGroup("L02_Collectibles");
        var grpChk       = RuleBreakMasterBuilder.GetOrCreateGroup("L02_Checkpoints");
        var grpExit      = RuleBreakMasterBuilder.GetOrCreateGroup("L02_Exit");
        var grpLights    = RuleBreakMasterBuilder.GetOrCreateGroup("L02_Lights");

        // ── Starting floor ───────────────────────────────────────────────────
        RuleBreakMasterBuilder.MakePlatform("Floor_L02_Start", new Vector3(0f, -0.15f, 0f),
            new Vector3(10f, 0.3f, 8f), grpPlatforms, RuleBreakMasterBuilder.ColDeepNavy);

        // ── Main platform chain ──────────────────────────────────────────────
        float[] zPos = { 12f, 20f, 28f, 36f, 42f, 50f, 58f };
        float[] yPos = { 0f,  1f,  2f,  1f,  3f,  2f,  4f };
        float[] xPos = { 0f,  3f, -3f,  0f,  4f, -4f,  0f };

        for (int i = 0; i < zPos.Length; i++)
        {
            RuleBreakMasterBuilder.MakePlatform($"Plat_{i:D2}",
                new Vector3(xPos[i], yPos[i], zPos[i]),
                new Vector3(4f, 0.3f, 4f), grpPlatforms,
                RuleBreakMasterBuilder.ColSurface, true);
        }

        // ── Moving platforms (T puzzles) ─────────────────────────────────────
        RuleBreakMasterBuilder.MakeMovingPlatform("MP_01",
            new Vector3(-5f, 1f, 16f), new Vector3(5f, 1f, 16f), grpMoving);
        RuleBreakMasterBuilder.MakeMovingPlatform("MP_02",
            new Vector3(0f, 2f, 32f), new Vector3(0f, 5f, 32f), grpMoving);
        RuleBreakMasterBuilder.MakeMovingPlatform("MP_03",
            new Vector3(-6f, 3f, 46f), new Vector3(6f, 3f, 46f), grpMoving);

        // ── Laser hazards ────────────────────────────────────────────────────
        // Lasers are represented as thin cubes (red, kill trigger, FreezableObject)
        MakeLaserHazard("Laser_01", new Vector3(0f, 1f, 18f), new Vector3(12f, 0.2f, 0.2f),
                        new Vector3(-6f, 1f, 18f), new Vector3(6f, 1f, 18f), grpLasers);
        MakeLaserHazard("Laser_02", new Vector3(0f, 2f, 38f), new Vector3(0.2f, 0.2f, 10f),
                        new Vector3(0f, 2f, 33f), new Vector3(0f, 2f, 43f), grpLasers);
        MakeLaserHazard("Laser_03", new Vector3(0f, 3f, 54f), new Vector3(14f, 0.2f, 0.2f),
                        new Vector3(-7f, 3f, 54f), new Vector3(7f, 3f, 54f), grpLasers);

        // ── Rotating hazards ─────────────────────────────────────────────────
        MakeRotator("Rotator_01", new Vector3(3f, 1.5f, 24f), grpHazards);
        MakeRotator("Rotator_02", new Vector3(-3f, 2.5f, 44f), grpHazards);

        // ── Spike hazards ────────────────────────────────────────────────────
        RuleBreakMasterBuilder.MakeHazard("Spikes_L02_01", new Vector3(5f, 1.2f, 22f), grpHazards);
        RuleBreakMasterBuilder.MakeHazard("Spikes_L02_02", new Vector3(-4f, 2.2f, 36f), grpHazards);
        RuleBreakMasterBuilder.MakeVoidKillZone("VoidKill_L02", new Vector3(0f, -15f, 30f), grpHazards);

        // ── Memory Fragments ─────────────────────────────────────────────────
        RuleBreakMasterBuilder.MakeFragment("Fragment_L02_01",
            new Vector3(6f, 2f, 28f), grpCollect,
            "SYSTEM LOG 002\n\"Time stopped.\"\n\"But you didn't.\"", 2);
        RuleBreakMasterBuilder.MakeFragment("Fragment_L02_02",
            new Vector3(-6f, 4f, 50f), grpCollect,
            "SYSTEM LOG 002b\n\"Temporal anomaly: ONGOING.\"\n\"Source: UNKNOWN.\"", 3);

        // ── Checkpoints ──────────────────────────────────────────────────────
        RuleBreakMasterBuilder.MakeCheckpoint("Checkpoint_L02_A", new Vector3(0f, 2.5f, 36f), grpChk);
        RuleBreakMasterBuilder.MakeCheckpoint("Checkpoint_L02_B", new Vector3(0f, 4.5f, 50f), grpChk);

        // ── Exit portal ──────────────────────────────────────────────────────
        RuleBreakMasterBuilder.MakeExitPortal("Exit_L02", new Vector3(0f, 4.5f, 64f), "Level03", grpExit);

        // ── Lights ───────────────────────────────────────────────────────────
        RuleBreakMasterBuilder.MakePointLight("Light_L02_01", new Vector3(0f, 5f, 20f), RuleBreakMasterBuilder.ColAmber, 3f, 15f, grpLights);
        RuleBreakMasterBuilder.MakePointLight("Light_L02_02", new Vector3(0f, 5f, 40f), RuleBreakMasterBuilder.ColNeonBlue, 2f, 12f, grpLights);
        RuleBreakMasterBuilder.MakePointLight("Light_L02_Exit", new Vector3(0f, 6f, 64f), RuleBreakMasterBuilder.ColCyan, 4f, 18f, grpLights);

        // ── Player + managers ─────────────────────────────────────────────────
        SetupLevel02Player();

        RuleBreakMasterBuilder.SaveScene();

        EditorUtility.DisplayDialog("RULE//BREAK",
            "✓ Level 02 (TIME) built!\n\n" +
            "Layout:\n" +
            "• 7 floating platforms\n" +
            "• 3 moving platforms (freeze to cross)\n" +
            "• 3 laser hazards (freeze to pass)\n" +
            "• 2 rotating hazards\n" +
            "• 2 Memory Fragments\n" +
            "• 2 Checkpoints\n" +
            "• Exit portal at Z=64", "OK");
    }

    static void MakeLaserHazard(string name, Vector3 pos, Vector3 scale,
                                 Vector3 moveA, Vector3 moveB, GameObject parent)
    {
        if (parent.transform.Find(name) != null) return;

        // Visual beam
        var beam = GameObject.CreatePrimitive(PrimitiveType.Cube);
        beam.name = name;
        beam.transform.SetParent(parent.transform);
        beam.transform.position   = pos;
        beam.transform.localScale = scale;

        var r = beam.GetComponent<Renderer>();
        if (r != null)
        {
            var mat = new Material(Shader.Find("Universal Render Pipeline/Lit") ?? r.sharedMaterial.shader);
            mat.color = RuleBreakMasterBuilder.ColRed;
            mat.EnableKeyword("_EMISSION");
            mat.SetColor("_EmissionColor", RuleBreakMasterBuilder.ColRed * 1.5f);
            r.sharedMaterial = mat;
        }

        // Kill trigger
        var col = beam.GetComponent<BoxCollider>();
        if (col != null) col.isTrigger = true;

        beam.AddComponent<RuleBreak.Gameplay.KillZone>();

        // Make it a moving freezable object
        var mp = beam.AddComponent<RuleBreak.Gameplay.MovingPlatform>();
        var so = new SerializedObject(mp);
        so.FindProperty("pointA").vector3Value = moveA;
        so.FindProperty("pointB").vector3Value = moveB;
        so.ApplyModifiedProperties();

        beam.AddComponent<RuleBreak.Time.FreezableObject>();
        Undo.RegisterCreatedObjectUndo(beam, "Create " + name);
    }

    static void MakeRotator(string name, Vector3 pos, GameObject parent)
    {
        if (parent.transform.Find(name) != null) return;

        var rot = GameObject.CreatePrimitive(PrimitiveType.Cube);
        rot.name = name;
        rot.transform.SetParent(parent.transform);
        rot.transform.position   = pos;
        rot.transform.localScale = new Vector3(6f, 0.2f, 0.5f);

        var r = rot.GetComponent<Renderer>();
        if (r != null)
        {
            var mat = new Material(Shader.Find("Universal Render Pipeline/Lit") ?? r.sharedMaterial.shader);
            mat.color = RuleBreakMasterBuilder.ColAmber;
            mat.EnableKeyword("_EMISSION");
            mat.SetColor("_EmissionColor", RuleBreakMasterBuilder.ColAmber * 0.8f);
            r.sharedMaterial = mat;
        }

        var col = rot.GetComponent<BoxCollider>();
        if (col != null) col.isTrigger = true;

        rot.AddComponent<RuleBreak.Gameplay.KillZone>();
        rot.AddComponent<RuleBreak.Time.RotatingHazard>();
        Undo.RegisterCreatedObjectUndo(rot, "Create " + name);
    }

    static void SetupLevel02Player()
    {
        var player = GameObject.FindGameObjectWithTag("Player") ?? GameObject.Find("Player");
        if (player == null)
        {
            player = GameObject.CreatePrimitive(PrimitiveType.Capsule);
            player.name = "Player";
            player.tag  = "Player";
            Undo.RegisterCreatedObjectUndo(player, "Create Player");
        }
        player.transform.position = new Vector3(0f, 1.2f, 0f);

        // TimeFreezeManager
        if (GameObject.Find("TimeFreezeManager") == null)
        {
            var tfm = new GameObject("TimeFreezeManager");
            tfm.AddComponent<RuleBreak.Time.TimeFreezeManager>();
            Undo.RegisterCreatedObjectUndo(tfm, "Create TimeFreezeManager");
        }
    }
}

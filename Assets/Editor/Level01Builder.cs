// =============================================================================
// Level01Builder.cs — RULE//BREAK (Complete rebuild)
// Menu: RULE//BREAK > Build Level 01 Environment
// Builds a complete sci-fi gravity-puzzle level.
// =============================================================================
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;
using RuleBreak.Player;

public static class Level01Builder
{
    [MenuItem("RULE//BREAK/Build Level 01 Environment")]
    public static void BuildLevel01()
    {
        if (!RuleBreakMasterBuilder.EnsureScene("Level01")) return;

        // ── Player setup ────────────────────────────────────────────────────
        SetupPlayer();

        // ── Environment groups ───────────────────────────────────────────────
        var grpStart      = RuleBreakMasterBuilder.GetOrCreateGroup("L01_Start");
        var grpFloat      = RuleBreakMasterBuilder.GetOrCreateGroup("L01_FloatPath");
        var grpCeiling    = RuleBreakMasterBuilder.GetOrCreateGroup("L01_CeilingPath");
        var grpHazards    = RuleBreakMasterBuilder.GetOrCreateGroup("L01_Hazards");
        var grpMoving     = RuleBreakMasterBuilder.GetOrCreateGroup("L01_Moving");
        var grpCollect    = RuleBreakMasterBuilder.GetOrCreateGroup("L01_Collectibles");
        var grpCheckpoint = RuleBreakMasterBuilder.GetOrCreateGroup("L01_Checkpoints");
        var grpExit       = RuleBreakMasterBuilder.GetOrCreateGroup("L01_Exit");
        var grpLights     = RuleBreakMasterBuilder.GetOrCreateGroup("L01_Lights");

        // ── Starting area ────────────────────────────────────────────────────
        // Main floor
        RuleBreakMasterBuilder.MakePlatform("Floor_Start", new Vector3(0f, -0.15f, 0f),
            new Vector3(12f, 0.3f, 10f), grpStart, RuleBreakMasterBuilder.ColDeepNavy);
        // Tutorial sign platform
        RuleBreakMasterBuilder.MakePlatform("Floor_TutorialBridge", new Vector3(0f, -0.15f, 10f),
            new Vector3(6f, 0.3f, 6f), grpStart, RuleBreakMasterBuilder.ColSurface);

        // ── Floating platform route ──────────────────────────────────────────
        RuleBreakMasterBuilder.MakePlatform("Platform_01", new Vector3(0f, 0f, 16f),   new Vector3(4f, 0.3f, 4f), grpFloat, RuleBreakMasterBuilder.ColSurface, true);
        RuleBreakMasterBuilder.MakePlatform("Platform_02", new Vector3(5f, 1f, 22f),   new Vector3(3f, 0.3f, 3f), grpFloat, RuleBreakMasterBuilder.ColSurface, true);
        RuleBreakMasterBuilder.MakePlatform("Platform_03", new Vector3(-4f, 2f, 28f),  new Vector3(3f, 0.3f, 3f), grpFloat, RuleBreakMasterBuilder.ColSurface, true);
        RuleBreakMasterBuilder.MakePlatform("Platform_04", new Vector3(0f, 3f, 35f),   new Vector3(5f, 0.3f, 5f), grpFloat, RuleBreakMasterBuilder.ColSurface, true);
        RuleBreakMasterBuilder.MakePlatform("Platform_05", new Vector3(7f, 4f, 40f),   new Vector3(3f, 0.3f, 3f), grpFloat, RuleBreakMasterBuilder.ColSurface, true);
        RuleBreakMasterBuilder.MakePlatform("Platform_06", new Vector3(0f, 5f, 47f),   new Vector3(6f, 0.3f, 6f), grpFloat, RuleBreakMasterBuilder.ColNeonBlue, true);

        // ── Ceiling platforms (G puzzle) ─────────────────────────────────────
        // Sit at Y=10-12, the player reaches by flipping gravity
        RuleBreakMasterBuilder.MakePlatform("Ceiling_01", new Vector3(0f, 10f, 20f),  new Vector3(5f, 0.3f, 5f), grpCeiling, RuleBreakMasterBuilder.ColViolet, true);
        RuleBreakMasterBuilder.MakePlatform("Ceiling_02", new Vector3(6f, 11f, 28f),  new Vector3(4f, 0.3f, 4f), grpCeiling, RuleBreakMasterBuilder.ColViolet, true);
        RuleBreakMasterBuilder.MakePlatform("Ceiling_03", new Vector3(-6f, 12f, 35f), new Vector3(4f, 0.3f, 4f), grpCeiling, RuleBreakMasterBuilder.ColViolet, true);
        // Ceiling solid "roof" so player can walk on it
        RuleBreakMasterBuilder.MakePlatform("CeilingRoof", new Vector3(0f, 13f, 28f), new Vector3(20f, 0.5f, 30f), grpCeiling, RuleBreakMasterBuilder.ColDeepNavy);

        // ── Hidden path ──────────────────────────────────────────────────────
        RuleBreakMasterBuilder.MakePlatform("Hidden_01", new Vector3(-9f, 0.5f, 18f), new Vector3(2f, 0.3f, 6f), grpFloat, RuleBreakMasterBuilder.ColSurface);
        RuleBreakMasterBuilder.MakePlatform("Hidden_02", new Vector3(-9f, 1f, 26f),   new Vector3(2f, 0.3f, 6f), grpFloat, RuleBreakMasterBuilder.ColSurface);
        RuleBreakMasterBuilder.MakePlatform("Hidden_03", new Vector3(-6f, 2f, 33f),   new Vector3(2f, 0.3f, 3f), grpFloat, RuleBreakMasterBuilder.ColSurface);

        // ── Hazards ──────────────────────────────────────────────────────────
        RuleBreakMasterBuilder.MakeHazard("Spikes_01", new Vector3(3f, 0.2f, 18f),  grpHazards);
        RuleBreakMasterBuilder.MakeHazard("Spikes_02", new Vector3(-3f, 1.2f, 24f), grpHazards);
        RuleBreakMasterBuilder.MakeHazard("Spikes_03", new Vector3(4f, 3.2f, 32f),  grpHazards);
        RuleBreakMasterBuilder.MakeVoidKillZone("VoidKill", new Vector3(0f, -15f, 25f), grpHazards);

        // ── Moving platforms ─────────────────────────────────────────────────
        RuleBreakMasterBuilder.MakeMovingPlatform("Moving_01",
            new Vector3(-4f, 2f, 22f), new Vector3(4f, 2f, 22f), grpMoving);
        RuleBreakMasterBuilder.MakeMovingPlatform("Moving_02",
            new Vector3(0f, 4f, 40f), new Vector3(0f, 7f, 40f), grpMoving);

        // ── Memory Fragments ─────────────────────────────────────────────────
        RuleBreakMasterBuilder.MakeFragment("Fragment_L01_01",
            new Vector3(-9f, 2f, 22f), grpCollect,
            "SYSTEM LOG 001\n\"Reality integrity: 87%.\"\nSomething is off.", 0);
        RuleBreakMasterBuilder.MakeFragment("Fragment_L01_02",
            new Vector3(6f, 12f, 30f), grpCollect,     // on ceiling path
            "SYSTEM LOG 002\n\"Anomaly detected in sector G.\"\nProceed with caution.", 1);

        // ── Checkpoint ───────────────────────────────────────────────────────
        RuleBreakMasterBuilder.MakeCheckpoint("Checkpoint_L01", new Vector3(0f, 3.5f, 35f), grpCheckpoint);

        // ── Exit portal ──────────────────────────────────────────────────────
        RuleBreakMasterBuilder.MakeExitPortal("Exit_L01", new Vector3(0f, 5.5f, 52f), "Level02", grpExit);

        // ── Atmospheric lights ───────────────────────────────────────────────
        RuleBreakMasterBuilder.MakePointLight("Light_01", new Vector3(0f, 4f, 16f),   RuleBreakMasterBuilder.ColCyan,  3f, 15f, grpLights);
        RuleBreakMasterBuilder.MakePointLight("Light_02", new Vector3(5f, 6f, 28f),   RuleBreakMasterBuilder.ColNeonBlue, 2f, 12f, grpLights);
        RuleBreakMasterBuilder.MakePointLight("Light_03", new Vector3(-5f, 8f, 36f),  RuleBreakMasterBuilder.ColViolet, 2f, 12f, grpLights);
        RuleBreakMasterBuilder.MakePointLight("Light_Exit", new Vector3(0f, 7f, 52f), RuleBreakMasterBuilder.ColCyan,  4f, 18f, grpLights);

        RuleBreakMasterBuilder.SaveScene();

        EditorUtility.DisplayDialog("RULE//BREAK",
            "✓ Level 01 (GRAVITY) built!\n\n" +
            "Layout:\n" +
            "• Starting area with tutorial zone\n" +
            "• 6 floating platforms rising in Z\n" +
            "• 3 ceiling platforms + solid roof (for G-flip)\n" +
            "• Hidden path on left flank\n" +
            "• 3 spike hazards + void kill zone\n" +
            "• 2 moving platforms\n" +
            "• 2 Memory Fragments\n" +
            "• 1 Checkpoint at Z=35\n" +
            "• Exit portal at Z=52\n\n" +
            "Next: RULE//BREAK > Build Level 02 Environment", "OK");
    }

    static void SetupPlayer()
    {
        var player = GameObject.FindGameObjectWithTag("Player");
        if (player == null) player = GameObject.Find("Player");

        if (player == null)
        {
            player = GameObject.CreatePrimitive(PrimitiveType.Capsule);
            player.name = "Player";
            Undo.RegisterCreatedObjectUndo(player, "Create Player");
        }

        player.tag = "Player";
        player.transform.position = new Vector3(0f, 1.2f, 0f);

        if (player.GetComponent<Rigidbody>() == null)
        {
            var rb = player.AddComponent<Rigidbody>();
            rb.mass = 1f; rb.constraints = RigidbodyConstraints.FreezeRotation;
            rb.interpolation = RigidbodyInterpolation.Interpolate;
        }
        if (player.GetComponent<PlayerController>() == null)
            player.AddComponent<PlayerController>();
        if (player.GetComponent<RuleBreak.Gravity.GravityShifter>() == null)
            player.AddComponent<RuleBreak.Gravity.GravityShifter>();
        if (player.GetComponent<RuleBreak.Player.PlayerRespawn>() == null)
            player.AddComponent<RuleBreak.Player.PlayerRespawn>();

        // Ground check child
        Transform gc = player.transform.Find("GroundCheck");
        if (gc == null)
        {
            gc = new GameObject("GroundCheck").transform;
            gc.SetParent(player.transform);
            gc.localPosition = new Vector3(0f, -1f, 0f);
            Undo.RegisterCreatedObjectUndo(gc.gameObject, "Create GroundCheck");
        }

        // Camera
        Camera cam = Camera.main;
        if (cam == null)
        {
            var camGO = new GameObject("Main Camera");
            cam = camGO.AddComponent<Camera>();
            camGO.tag = "MainCamera";
            camGO.AddComponent<AudioListener>();
            Undo.RegisterCreatedObjectUndo(camGO, "Create Camera");
        }
        cam.transform.position = new Vector3(0f, 4f, -6f);

        var pc = cam.gameObject.GetComponent<PlayerCamera>();
        if (pc == null) pc = cam.gameObject.AddComponent<PlayerCamera>();
        var so = new SerializedObject(pc);
        so.FindProperty("target").objectReferenceValue = player.transform;
        so.ApplyModifiedProperties();

        // Directional light
        if (GameObject.Find("Directional Light") == null)
        {
            var lg = new GameObject("Directional Light");
            var l  = lg.AddComponent<Light>();
            l.type = LightType.Directional;
            l.intensity = 1f;
            l.color = new Color(0.7f, 0.85f, 1f);
            lg.transform.rotation = Quaternion.Euler(50f, -30f, 0f);
            Undo.RegisterCreatedObjectUndo(lg, "Create Directional Light");
        }
    }
}

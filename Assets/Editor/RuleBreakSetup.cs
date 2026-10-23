// =============================================================================
// RuleBreakSetup.cs — RULE//BREAK
// Overarching setup commands for the entire project.
// Menu: RULE//BREAK > Setup Level 01
// Menu: RULE//BREAK > Setup Level 02
// Menu: RULE//BREAK > Setup Level 03
// Menu: RULE//BREAK > Setup Level 04
// Menu: RULE//BREAK > Generate All Levels
// =============================================================================
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;
using RuleBreak.Player;

public static class RuleBreakSetup
{
    // ── Setup Level 01 ────────────────────────────────────────────────────────
    [MenuItem("RULE//BREAK/Setup Level 01")]
    public static void SetupLevel01() => RunSetupForLevel(1, "Gravity Break", "Master gravity inversion [G] to navigate the facility and reach the exit.");

    // ── Setup Level 02 ────────────────────────────────────────────────────────
    [MenuItem("RULE//BREAK/Setup Level 02")]
    public static void SetupLevel02() => RunSetupForLevel(2, "Time Fracture", "Freeze temporal fields [T] to bypass rotating hazards and laser grids.");

    // ── Setup Level 03 ────────────────────────────────────────────────────────
    [MenuItem("RULE//BREAK/Setup Level 03")]
    public static void SetupLevel03() => RunSetupForLevel(3, "Dimension Shift", "Shift realities [E] to reveal alternate platforms and cross the void.");

    // ── Setup Level 04 ────────────────────────────────────────────────────────
    [MenuItem("RULE//BREAK/Setup Level 04")]
    public static void SetupLevel04() => RunSetupForLevel(4, "Rule Corruption", "Unify all abilities and corrupt core system barriers [R] to break free.");

    private static void RunSetupForLevel(int index, string levelName, string objective)
    {
        if (EditorApplication.isPlaying)
        {
            EditorUtility.DisplayDialog("RULE//BREAK",
                "Cannot set up or modify scenes while Unity is in Play Mode.\n\nPlease exit Play Mode first (press the Play button in Unity to stop).", "OK");
            return;
        }

        if (!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo()) return;
        string expectedName = $"Level0{index}";
        Scene scene = SceneManager.GetActiveScene();

        if (scene.name != expectedName)
        {
            string scenePath = $"Assets/Scenes/{expectedName}.unity";
            if (System.IO.File.Exists(scenePath))
            {
                bool load = EditorUtility.DisplayDialog("RULE//BREAK",
                    $"Active scene is '{scene.name}', but Setup Level 0{index} expects '{expectedName}'. Open it now?", "Open Scene", "Cancel");
                if (load)
                {
                    scene = EditorSceneManager.OpenScene(scenePath);
                }
                else return;
            }
            else
            {
                EditorUtility.DisplayDialog("RULE//BREAK", $"Could not find scene at '{scenePath}'.", "OK");
                return;
            }
        }

        SetupBaseScene(scene, index, levelName, objective);

        EditorSceneManager.MarkSceneDirty(scene);
        EditorSceneManager.SaveScene(scene);

        EditorUtility.DisplayDialog("RULE//BREAK",
            $"✓ Level 0{index} base setup complete!\n\n" +
            "Components created & wired:\n" +
            "• Player (Rigidbody, PlayerController, GravityShifter, PlayerRespawn, CapsuleProceduralAnimator)\n" +
            "• Main Camera (+ PlayerCamera targeting player)\n" +
            "• Directional Light\n" +
            "• Managers: GlitchEnergy, RealityIntegrity, TimeFreeze, RuleCorruption, GameProgress, LevelManager\n" +
            "• In-Game HUD Canvas + Pause Menu + Level Complete Screen", "OK");
    }

    // ── Generate All Levels command ───────────────────────────────────────────
    [MenuItem("RULE//BREAK/Generate All Levels")]
    public static void GenerateAllLevels()
    {
        if (EditorApplication.isPlaying)
        {
            EditorUtility.DisplayDialog("RULE//BREAK",
                "Cannot generate levels while Unity is in Play Mode.\n\nPlease exit Play Mode first (press the Play button in Unity to stop).", "OK");
            return;
        }

        bool confirm = EditorUtility.DisplayDialog("RULE//BREAK",
            "This will open each scene (MainMenu + Levels 01-04), configure base components,\n" +
            "generate full environments, and register all scenes in Build Settings.\n\nProceed?", "Build All", "Cancel");

        if (!confirm) return;

        // 1. Build MainMenu
        if (System.IO.File.Exists("Assets/Scenes/MainMenu.unity"))
        {
            var mmScene = EditorSceneManager.OpenScene("Assets/Scenes/MainMenu.unity");
            if (mmScene.IsValid())
            {
                SceneManager.SetActiveScene(mmScene);
                MainMenuBuilder.BuildMainMenu();
            }
        }

        // 2. Build Levels
        string[] scenes = {
            "Assets/Scenes/Level01.unity",
            "Assets/Scenes/Level02.unity",
            "Assets/Scenes/Level03.unity",
            "Assets/Scenes/Level04.unity"
        };

        string[] names = {
            "Gravity Break",
            "Time Fracture",
            "Dimension Shift",
            "Rule Corruption"
        };

        string[] objectives = {
            "Master gravity inversion [G] to navigate the facility and reach the exit.",
            "Freeze temporal fields [T] to bypass rotating hazards and laser grids.",
            "Shift realities [E] to reveal alternate platforms and cross the void.",
            "Unify all abilities and corrupt core system barriers [R] to break free."
        };

        System.Action[] builders = {
            Level01Builder.BuildLevel01,
            Level02Builder.BuildLevel02,
            Level03Builder.BuildLevel03,
            Level04Builder.BuildLevel04,
        };

        for (int i = 0; i < scenes.Length; i++)
        {
            var scene = EditorSceneManager.OpenScene(scenes[i]);
            if (!scene.IsValid())
            {
                Debug.LogWarning($"Could not open {scenes[i]}");
                continue;
            }

            SceneManager.SetActiveScene(scene);
            SetupBaseScene(scene, i + 1, names[i], objectives[i]);
            builders[i]();

            EditorSceneManager.MarkSceneDirty(scene);
            EditorSceneManager.SaveScene(scene);
        }

        // 3. Register scenes in build settings
        RuleBreakMasterBuilder.RegisterScenes();

        EditorUtility.DisplayDialog("RULE//BREAK",
            "✓ COMPLETE RULE//BREAK GENERATION SUCCESSFUL!\n\n" +
            "• Main Menu scene built with full UI & backdrop\n" +
            "• Levels 01, 02, 03, and 04 environments built\n" +
            "• Player controls, procedural animation, and cameras configured\n" +
            "• HUD, Pause Menus, and Level Complete screens wired\n" +
            "• All 5 scenes registered in Build Settings in order (0 to 4)\n\n" +
            "Ready to test! Open MainMenu or Level01 and press Play.", "OK");
    }

    // ── Shared base setup (Player + Camera + Light + Managers + UI) ───────────
    public static void SetupBaseScene(Scene scene, int levelIndex = 1, string levelName = "Gravity Break", string objective = "Reach the exit portal.")
    {
        // Player
        var player = GameObject.FindGameObjectWithTag("Player") ?? GameObject.Find("Player");
        if (player == null)
        {
            player = GameObject.CreatePrimitive(PrimitiveType.Capsule);
            player.name = "Player";
            Undo.RegisterCreatedObjectUndo(player, "Create Player");
        }
        player.tag = "Player";
        if (player.transform.position.y < 0.5f)
            player.transform.position = new Vector3(0f, 1.2f, 0f);

        // Rigidbody
        var rb = player.GetComponent<Rigidbody>();
        if (rb == null) rb = player.AddComponent<Rigidbody>();
        rb.mass = 1f;
        rb.freezeRotation = true;
        rb.interpolation = RigidbodyInterpolation.Interpolate;

        // Components
        AddIfMissing<PlayerController>(player);
        AddIfMissing<RuleBreak.Gravity.GravityShifter>(player);
        AddIfMissing<RuleBreak.Player.PlayerRespawn>(player);
        AddIfMissing<RuleBreak.Player.CapsuleProceduralAnimator>(player);

        // Ground check
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
        cam.backgroundColor = RuleBreakMasterBuilder.ColVoid;
        cam.clearFlags = CameraClearFlags.SolidColor;
        cam.transform.position = new Vector3(0f, 4f, -6f);

        var pc = AddIfMissing<PlayerCamera>(cam.gameObject);
        var pcso = new SerializedObject(pc);
        pcso.FindProperty("target").objectReferenceValue = player.transform;
        pcso.ApplyModifiedProperties();

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

        // Managers
        CreateManagerIfMissing<RuleBreak.Level.GlitchEnergyManager>("GlitchEnergyManager");
        CreateManagerIfMissing<RuleBreak.Level.RealityIntegrityManager>("RealityIntegrityManager");
        CreateManagerIfMissing<RuleBreak.Level.GameProgressManager>("GameProgressManager");
        CreateManagerIfMissing<RuleBreak.Time.TimeFreezeManager>("TimeFreezeManager");
        CreateManagerIfMissing<RuleBreak.Corruption.RuleCorruptionManager>("RuleCorruptionManager");
        CreateManagerIfMissing<RuleBreak.Dimension.DimensionManager>("DimensionManager");
        CreateManagerIfMissing<RuleBreak.Audio.AudioManager>("AudioManager");
        CreateManagerIfMissing<RuleBreak.Effects.SciFiEffectSystem>("SciFiEffectSystem");

        // Build in-game UI Canvas (HUD + Pause + LevelComplete)
        GameplayUIBuilder.BuildGameplayCanvas(levelIndex, levelName, objective);
    }

    static T AddIfMissing<T>(GameObject go) where T : Component
    {
        var c = go.GetComponent<T>();
        if (c == null) c = go.AddComponent<T>();
        return c;
    }

    static void CreateManagerIfMissing<T>(string name) where T : Component
    {
        if (GameObject.Find(name) != null) return;
        var go = new GameObject(name);
        go.AddComponent<T>();
        Undo.RegisterCreatedObjectUndo(go, "Create " + name);
    }
}

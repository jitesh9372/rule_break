// =============================================================================
// MainMenuBuilder.cs — RULE//BREAK
// Menu: RULE//BREAK > Build Main Menu Scene
// Builds the advanced sci-fi Main Menu scene with full UI canvas,
// animated atmospheric background, Level Select modal, Controls modal,
// Credits modal, and New Game confirmation dialog.
// =============================================================================
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using RuleBreak.UI;

public static class MainMenuBuilder
{
    private static Font DefaultFont => Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");

    [MenuItem("RULE//BREAK/Build Main Menu Scene")]
    public static void BuildMainMenu()
    {
        if (!RuleBreakMasterBuilder.EnsureScene("MainMenu")) return;

        // ── Background camera ─────────────────────────────────────────────────
        Camera cam = Camera.main;
        if (cam == null)
        {
            var camGO = new GameObject("Main Camera");
            cam = camGO.AddComponent<Camera>();
            camGO.tag = "MainCamera";
            camGO.AddComponent<AudioListener>();
            Undo.RegisterCreatedObjectUndo(camGO, "Create Camera");
        }
        cam.backgroundColor = new Color(0.02f, 0.043f, 0.078f); // #050B14
        cam.clearFlags = CameraClearFlags.SolidColor;
        cam.transform.position = new Vector3(0f, 2f, -10f);
        cam.transform.LookAt(Vector3.zero);

        // ── Directional light ─────────────────────────────────────────────────
        if (GameObject.Find("Directional Light") == null)
        {
            var lg = new GameObject("Directional Light");
            var l  = lg.AddComponent<Light>();
            l.type = LightType.Directional;
            l.intensity = 0.5f;
            l.color = new Color(0.26f, 0.53f, 1f);
            lg.transform.rotation = Quaternion.Euler(50f, -30f, 0f);
            Undo.RegisterCreatedObjectUndo(lg, "Create Directional Light");
        }

        // ── Floating atmospheric geometry ─────────────────────────────────────
        var grpAtmo = RuleBreakMasterBuilder.GetOrCreateGroup("MM_Atmosphere");
        MakeFloatingCube("Atmo_01", new Vector3(-4f, 0f, 4f),   new Vector3(1.6f, 1.6f, 1.6f), RuleBreakMasterBuilder.ColNeonBlue,  grpAtmo);
        MakeFloatingCube("Atmo_02", new Vector3(4f,  1f, 5f),   new Vector3(1.2f, 1.2f, 1.2f), RuleBreakMasterBuilder.ColViolet,    grpAtmo);
        MakeFloatingCube("Atmo_03", new Vector3(-6f, 2.5f, 6f), new Vector3(0.9f, 0.9f, 0.9f), RuleBreakMasterBuilder.ColCyan,      grpAtmo);
        MakeFloatingCube("Atmo_04", new Vector3(6f, -1.5f, 7f), new Vector3(2.5f, 0.4f, 2.5f), RuleBreakMasterBuilder.ColSurface,   grpAtmo);
        MakeFloatingCube("Atmo_05", new Vector3(0f, 3.5f, 8f),  new Vector3(1.4f, 1.4f, 1.4f), RuleBreakMasterBuilder.ColMagenta,  grpAtmo);

        RuleBreakMasterBuilder.MakePointLight("MM_Light_01", new Vector3(-4f, 2f, 2f), RuleBreakMasterBuilder.ColNeonBlue, 3f, 14f, grpAtmo);
        RuleBreakMasterBuilder.MakePointLight("MM_Light_02", new Vector3(4f,  2f, 3f), RuleBreakMasterBuilder.ColViolet,   3f, 14f, grpAtmo);

        // ── UI Canvas ─────────────────────────────────────────────────────────
        GameObject existingCanvas = GameObject.Find("MainMenuCanvas");
        if (existingCanvas != null)
        {
            Undo.DestroyObjectImmediate(existingCanvas);
        }

        var canvasGO = new GameObject("MainMenuCanvas");
        var canvas   = canvasGO.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;

        var scaler = canvasGO.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);
        scaler.matchWidthOrHeight = 0.5f;

        canvasGO.AddComponent<GraphicRaycaster>();

        // Background subtle vignette/overlay
        MakePanel("VignetteOverlay", canvasGO, new Color(0.02f, 0.043f, 0.078f, 0.75f),
                  Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero, new Vector2(0.5f, 0.5f));

        // ── Main Brand / Title ────────────────────────────────────────────────
        var titlePanel = MakePanel("TitleContainer", canvasGO, Color.clear,
            new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0f, -140f), new Vector2(900f, 140f), new Vector2(0.5f, 1f));

        MakeText("TitleText", titlePanel, "RULE//BREAK",
                 new Vector2(0f, -20f), new Vector2(850f, 80f),
                 RuleBreakMasterBuilder.ColCyan, 76, TextAnchor.MiddleCenter, FontStyle.Bold, new Vector2(0.5f, 1f));

        MakeText("TaglineText", titlePanel, "REALITY IS ONLY A RULE WAITING TO BE BROKEN.",
                 new Vector2(0f, -85f), new Vector2(850f, 32f),
                 new Color(0.65f, 0.72f, 0.88f), 20, TextAnchor.MiddleCenter, FontStyle.Normal, new Vector2(0.5f, 1f));

        // ── Main Menu Buttons ─────────────────────────────────────────────────
        var navPanel = MakePanel("NavPanel", canvasGO, Color.clear,
            new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0f, -30f), new Vector2(340f, 440f), new Vector2(0.5f, 0.5f));

        float btnY = 170f;
        float btnSpacing = 58f;
        var btnPlay     = MakeButton("PlayButton",     "NEW GAME",     navPanel, new Vector2(0f, btnY),                 new Vector2(300f, 48f), RuleBreakMasterBuilder.ColCyan);
        var btnContinue = MakeButton("ContinueButton", "CONTINUE",     navPanel, new Vector2(0f, btnY - btnSpacing),     new Vector2(300f, 48f), RuleBreakMasterBuilder.ColNeonBlue);
        var btnLevel    = MakeButton("LevelSelectBtn", "LEVEL SELECT", navPanel, new Vector2(0f, btnY - btnSpacing*2), new Vector2(300f, 48f), RuleBreakMasterBuilder.ColViolet);
        var btnSettings = MakeButton("SettingsButton", "SETTINGS",     navPanel, new Vector2(0f, btnY - btnSpacing*3), new Vector2(300f, 48f), RuleBreakMasterBuilder.ColNeonBlue);
        var btnControls = MakeButton("ControlsButton", "CONTROLS",     navPanel, new Vector2(0f, btnY - btnSpacing*4), new Vector2(300f, 48f), RuleBreakMasterBuilder.ColCyan);
        var btnCredits  = MakeButton("CreditsButton",  "CREDITS",      navPanel, new Vector2(0f, btnY - btnSpacing*5), new Vector2(300f, 48f), new Color(0.65f, 0.75f, 0.9f));
        var btnQuit     = MakeButton("QuitButton",     "EXIT SYSTEM",  navPanel, new Vector2(0f, btnY - btnSpacing*6), new Vector2(300f, 48f), new Color(0.7f, 0.4f, 0.45f));

        // Version Label
        MakeText("VersionLabel", canvasGO, "RULE//BREAK  v1.0.4  //  QUANTUM ENGINE ACTIVE",
            new Vector2(20f, 20f), new Vector2(400f, 24f), new Color(0.4f, 0.5f, 0.65f), 13, TextAnchor.LowerLeft, FontStyle.Normal, new Vector2(0f, 0f));

        // ── 1. Level Select Modal ─────────────────────────────────────────────
        var lvlSelectPanel = MakePanel("LevelSelectPanel", canvasGO, new Color(0.04f, 0.07f, 0.14f, 0.96f),
            Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero, new Vector2(0.5f, 0.5f));

        MakeText("LSTitle", lvlSelectPanel, "ARCHIVE PROTOCOLS // LEVEL SELECTION",
            new Vector2(0f, 220f), new Vector2(800f, 50f), RuleBreakMasterBuilder.ColCyan, 36, TextAnchor.MiddleCenter, FontStyle.Bold, new Vector2(0.5f, 0.5f));

        var btnL1 = MakeButton("BtnL1", "LEVEL 01 // GRAVITY BREAK", lvlSelectPanel, new Vector2(0f, 130f), new Vector2(440f, 52f), RuleBreakMasterBuilder.ColCyan);
        var btnL2 = MakeButton("BtnL2", "LEVEL 02 // TIME FRACTURE", lvlSelectPanel, new Vector2(0f, 60f),  new Vector2(440f, 52f), RuleBreakMasterBuilder.ColNeonBlue);
        var btnL3 = MakeButton("BtnL3", "LEVEL 03 // DIMENSION SHIFT", lvlSelectPanel, new Vector2(0f, -10f), new Vector2(440f, 52f), RuleBreakMasterBuilder.ColViolet);
        var btnL4 = MakeButton("BtnL4", "LEVEL 04 // RULE CORRUPTION", lvlSelectPanel, new Vector2(0f, -80f), new Vector2(440f, 52f), RuleBreakMasterBuilder.ColMagenta);
        var btnCloseLS = MakeButton("BtnCloseLS", "RETURN", lvlSelectPanel, new Vector2(0f, -170f), new Vector2(260f, 46f), new Color(0.6f, 0.65f, 0.75f));
        lvlSelectPanel.SetActive(false);

        // ── 2. Controls Guide Modal ───────────────────────────────────────────
        var controlsPanel = MakePanel("ControlsPanel", canvasGO, new Color(0.04f, 0.07f, 0.14f, 0.96f),
            Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero, new Vector2(0.5f, 0.5f));

        MakeText("ControlsTitle", controlsPanel, "NEURAL INTERFACE // CONTROLS",
            new Vector2(0f, 230f), new Vector2(700f, 50f), RuleBreakMasterBuilder.ColCyan, 36, TextAnchor.MiddleCenter, FontStyle.Bold, new Vector2(0.5f, 0.5f));

        string controlsGuide =
            "W / A / S / D  —  Omni-Directional Movement\n" +
            "SPACEBAR       —  Impulse Jump\n" +
            "LEFT SHIFT     —  Sub-System Sprint\n" +
            "MOUSE LOOK     —  Orbital Camera Navigation\n\n" +
            "[G] KEY        —  GRAVITY INVERSION (Shift Up / Down)\n" +
            "[T] KEY        —  TEMPORAL FREEZE (Pause Moving Hazards)\n" +
            "[E] KEY        —  DIMENSIONAL SHIFT (Reality Alpha / Beta)\n" +
            "[R] KEY        —  RULE CORRUPTION (Distort Core Rules)\n" +
            "[F] KEY        —  Interact with Terminals & Logs\n" +
            "[ESC] KEY      —  Suspend Simulation (Pause Menu)";

        MakeText("ControlsBody", controlsPanel, controlsGuide,
            new Vector2(0f, 20f), new Vector2(740f, 320f), new Color(0.85f, 0.9f, 0.98f), 18, TextAnchor.MiddleCenter, FontStyle.Normal, new Vector2(0.5f, 0.5f));

        var btnCloseControls = MakeButton("BtnCloseControls", "ACKNOWLEDGE", controlsPanel, new Vector2(0f, -200f), new Vector2(260f, 46f), RuleBreakMasterBuilder.ColCyan);
        controlsPanel.SetActive(false);

        // ── 3. Credits Modal ──────────────────────────────────────────────────
        var creditsPanel = MakePanel("CreditsPanel", canvasGO, new Color(0.04f, 0.07f, 0.14f, 0.96f),
            Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero, new Vector2(0.5f, 0.5f));

        MakeText("CreditsTitle", creditsPanel, "RULE//BREAK // CREDITS",
            new Vector2(0f, 220f), new Vector2(700f, 50f), RuleBreakMasterBuilder.ColCyan, 36, TextAnchor.MiddleCenter, FontStyle.Bold, new Vector2(0.5f, 0.5f));

        string creditsGuide =
            "PROJECT: RULE//BREAK\n" +
            "GENRE: Sci-Fi Puzzle Platformer\n\n" +
            "SYSTEM ARCHITECTURE & GAMEPLAY PROGRAMMING\n" +
            "TECHNICAL ART, URP RENDERING & SHADERS\n" +
            "LEVEL DESIGN & CORRUPTION PUZZLE MECHANICS\n" +
            "HOLOGRAPHIC UI / UX DESIGN\n\n" +
            "\"Reality is only a rule waiting to be broken.\"\n" +
            "Developed with Unity 6.6 Universal Render Pipeline.";

        MakeText("CreditsBody", creditsPanel, creditsGuide,
            new Vector2(0f, 20f), new Vector2(700f, 280f), new Color(0.8f, 0.86f, 0.96f), 18, TextAnchor.MiddleCenter, FontStyle.Normal, new Vector2(0.5f, 0.5f));

        var btnCloseCredits = MakeButton("BtnCloseCredits", "RETURN", creditsPanel, new Vector2(0f, -190f), new Vector2(260f, 46f), RuleBreakMasterBuilder.ColCyan);
        creditsPanel.SetActive(false);

        // ── 4. Confirm New Game Modal ─────────────────────────────────────────
        var confirmPanel = MakePanel("ConfirmNewGamePanel", canvasGO, new Color(0.04f, 0.07f, 0.14f, 0.96f),
            Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero, new Vector2(0.5f, 0.5f));

        MakeText("ConfirmTitle", confirmPanel, "WARNING // OVERWRITE DETECTED",
            new Vector2(0f, 80f), new Vector2(700f, 45f), RuleBreakMasterBuilder.ColRed, 32, TextAnchor.MiddleCenter, FontStyle.Bold, new Vector2(0.5f, 0.5f));

        MakeText("ConfirmBody", confirmPanel, "Starting a New Game will reset all unlocked sectors, memory fragments, and records.\n\nProceed with timeline purge?",
            new Vector2(0f, 10f), new Vector2(650f, 80f), new Color(0.85f, 0.9f, 0.98f), 18, TextAnchor.MiddleCenter, FontStyle.Normal, new Vector2(0.5f, 0.5f));

        var btnConfirmYes = MakeButton("BtnConfirmYes", "PURGE & RESTART", confirmPanel, new Vector2(-130f, -80f), new Vector2(220f, 48f), RuleBreakMasterBuilder.ColRed);
        var btnConfirmNo  = MakeButton("BtnConfirmNo",  "CANCEL",          confirmPanel, new Vector2(130f, -80f),  new Vector2(220f, 48f), RuleBreakMasterBuilder.ColCyan);
        confirmPanel.SetActive(false);

        // ── Wire MainMenuUI ───────────────────────────────────────────────────
        var menuUI = canvasGO.AddComponent<MainMenuUI>();
        var so = new SerializedObject(menuUI);
        so.FindProperty("playButton").objectReferenceValue         = btnPlay;
        so.FindProperty("continueButton").objectReferenceValue     = btnContinue;
        so.FindProperty("levelSelectButton").objectReferenceValue = btnLevel;
        so.FindProperty("settingsButton").objectReferenceValue     = btnSettings;
        so.FindProperty("controlsButton").objectReferenceValue     = btnControls;
        so.FindProperty("creditsButton").objectReferenceValue      = btnCredits;
        so.FindProperty("quitButton").objectReferenceValue         = btnQuit;

        so.FindProperty("levelSelectPanel").objectReferenceValue   = lvlSelectPanel;
        so.FindProperty("controlsPanel").objectReferenceValue      = controlsPanel;
        so.FindProperty("creditsPanel").objectReferenceValue       = creditsPanel;
        so.FindProperty("confirmNewGamePanel").objectReferenceValue = confirmPanel;

        so.FindProperty("level1Button").objectReferenceValue       = btnL1;
        so.FindProperty("level2Button").objectReferenceValue       = btnL2;
        so.FindProperty("level3Button").objectReferenceValue       = btnL3;
        so.FindProperty("level4Button").objectReferenceValue       = btnL4;
        so.FindProperty("backFromLevelSelectButton").objectReferenceValue = btnCloseLS;

        so.FindProperty("closeControlsButton").objectReferenceValue = btnCloseControls;
        so.FindProperty("closeCreditsButton").objectReferenceValue  = btnCloseCredits;

        so.FindProperty("confirmYesButton").objectReferenceValue   = btnConfirmYes;
        so.FindProperty("confirmNoButton").objectReferenceValue    = btnConfirmNo;

        so.FindProperty("cameraTransform").objectReferenceValue    = cam.transform;
        so.ApplyModifiedProperties();

        // ── AudioManager in MainMenu ──────────────────────────────────────────
        if (GameObject.Find("AudioManager") == null)
        {
            var am = new GameObject("AudioManager");
            var src1 = am.AddComponent<AudioSource>();
            var src2 = am.AddComponent<AudioSource>();
            am.AddComponent<RuleBreak.Audio.AudioManager>();
            Undo.RegisterCreatedObjectUndo(am, "Create AudioManager");
        }

        Undo.RegisterCreatedObjectUndo(canvasGO, "Create MainMenuCanvas");
        RuleBreakMasterBuilder.SaveScene();

        EditorUtility.DisplayDialog("RULE//BREAK",
            "✓ Advanced Sci-Fi Main Menu successfully built!\n\n" +
            "• Void atmospheric backdrop & floating neon cubes\n" +
            "• Animated camera drift\n" +
            "• Play / Continue / Level Select / Settings / Controls / Credits / Quit\n" +
            "• Full interactive modals for Level Select, Controls, Credits & Purge Confirm", "OK");
    }

    static void MakeFloatingCube(string name, Vector3 pos, Vector3 scale, Color col, GameObject parent)
    {
        if (parent.transform.Find(name) != null) return;
        var go = GameObject.CreatePrimitive(PrimitiveType.Cube);
        go.name = name;
        go.transform.SetParent(parent.transform);
        go.transform.position   = pos;
        go.transform.localScale = scale;
        go.transform.rotation   = Quaternion.Euler(15f, 30f, 5f);

        var col2 = go.GetComponent<Collider>();
        if (col2) Object.DestroyImmediate(col2);

        var r = go.GetComponent<Renderer>();
        if (r != null)
        {
            var mat = new Material(Shader.Find("Universal Render Pipeline/Lit") ?? r.sharedMaterial.shader);
            mat.color = col;
            mat.EnableKeyword("_EMISSION");
            mat.SetColor("_EmissionColor", col * 0.5f);
            r.sharedMaterial = mat;
        }
        Undo.RegisterCreatedObjectUndo(go, "Create " + name);
    }

    static GameObject MakePanel(string name, GameObject parent, Color col,
                                Vector2 anchorMin, Vector2 anchorMax, Vector2 anchoredPos, Vector2 size, Vector2 pivot)
    {
        var go = new GameObject(name);
        go.transform.SetParent(parent.transform, false);
        var rt = go.AddComponent<RectTransform>();
        rt.anchorMin = anchorMin;
        rt.anchorMax = anchorMax;
        rt.pivot = pivot;
        rt.anchoredPosition = anchoredPos;
        rt.sizeDelta = size;
        var img = go.AddComponent<Image>();
        img.color = col;
        Undo.RegisterCreatedObjectUndo(go, "Create " + name);
        return go;
    }

    static Button MakeButton(string name, string label, GameObject parent,
                             Vector2 pos, Vector2 size, Color accentCol)
    {
        var go = new GameObject(name);
        go.transform.SetParent(parent.transform, false);
        var rt = go.AddComponent<RectTransform>();
        rt.anchorMin = new Vector2(0.5f, 0.5f);
        rt.anchorMax = new Vector2(0.5f, 0.5f);
        rt.pivot = new Vector2(0.5f, 0.5f);
        rt.anchoredPosition = pos;
        rt.sizeDelta = size;

        var img = go.AddComponent<Image>();
        img.color = new Color(accentCol.r * 0.15f, accentCol.g * 0.15f, accentCol.b * 0.15f, 0.9f);

        var btn = go.AddComponent<Button>();
        var colors = btn.colors;
        colors.normalColor = img.color;
        colors.highlightedColor = new Color(accentCol.r * 0.35f, accentCol.g * 0.35f, accentCol.b * 0.35f, 0.95f);
        colors.pressedColor = accentCol;
        colors.selectedColor = colors.highlightedColor;
        btn.colors = colors;

        // Label
        var lblGO = new GameObject("Label");
        lblGO.transform.SetParent(go.transform, false);
        var lrt = lblGO.AddComponent<RectTransform>();
        lrt.anchorMin = Vector2.zero;
        lrt.anchorMax = Vector2.one;
        lrt.sizeDelta = Vector2.zero;

        var txt = lblGO.AddComponent<Text>();
        txt.text = label;
        txt.color = accentCol;
        txt.fontSize = 18;
        txt.alignment = TextAnchor.MiddleCenter;
        txt.fontStyle = FontStyle.Bold;
        txt.font = DefaultFont;

        Undo.RegisterCreatedObjectUndo(go, "Create " + name);
        return btn;
    }

    static Text MakeText(string name, GameObject parent, string content,
                         Vector2 pos, Vector2 size, Color col, int fontSize,
                         TextAnchor anchor, FontStyle style, Vector2 pivot)
    {
        var go = new GameObject(name);
        go.transform.SetParent(parent.transform, false);
        var rt = go.AddComponent<RectTransform>();
        rt.anchorMin = pivot;
        rt.anchorMax = pivot;
        rt.pivot = pivot;
        rt.anchoredPosition = pos;
        rt.sizeDelta = size;

        var txt = go.AddComponent<Text>();
        txt.text      = content;
        txt.color     = col;
        txt.fontSize  = fontSize;
        txt.alignment = anchor;
        txt.fontStyle = style;
        txt.font      = DefaultFont;
        Undo.RegisterCreatedObjectUndo(go, "Create " + name);
        return txt;
    }
}

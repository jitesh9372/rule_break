// =============================================================================
// GameplayUIBuilder.cs — RULE//BREAK
// Editor utility to build and wire the full in-game HUD, Pause Menu,
// Controls Guide, and Level Complete / Final Reveal UI Canvases.
// =============================================================================
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using RuleBreak.UI;
using RuleBreak.Level;
using RuleBreak.Story;

public static class GameplayUIBuilder
{
    private static Font DefaultFont => Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");

    public static GameObject BuildGameplayCanvas(int levelIndex, string levelName, string objective)
    {
        GameObject existing = GameObject.Find("GameplayCanvas");
        if (existing != null)
        {
            Undo.DestroyObjectImmediate(existing);
        }

        var canvasGO = new GameObject("GameplayCanvas");
        var canvas = canvasGO.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;

        var scaler = canvasGO.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);
        scaler.matchWidthOrHeight = 0.5f;

        canvasGO.AddComponent<GraphicRaycaster>();

        // ── 0. Scanline Holographic Overlay ──────────────────────────────────
        var scanlineGO = new GameObject("ScanlineOverlay");
        scanlineGO.transform.SetParent(canvasGO.transform, false);
        var srt = scanlineGO.AddComponent<RectTransform>();
        srt.anchorMin = Vector2.zero;
        srt.anchorMax = Vector2.one;
        srt.sizeDelta = Vector2.zero;
        var sRaw = scanlineGO.AddComponent<RawImage>();
        sRaw.raycastTarget = false;
        scanlineGO.AddComponent<ScanlineOverlay>();

        // ── 1. Top-Left: Level Title & Objective ──────────────────────────────
        var headerPanel = CreatePanel("HeaderPanel", canvasGO, new Color(0.04f, 0.07f, 0.14f, 0.85f),
            new Vector2(0f, 1f), new Vector2(0f, 1f), new Vector2(25f, -25f), new Vector2(500f, 85f), new Vector2(0f, 1f));

        var lvlText = CreateText("LevelText", headerPanel, $"LEVEL 0{levelIndex} // {levelName.ToUpper()}",
            new Vector2(18f, -16f), new Vector2(460f, 30f), RuleBreakMasterBuilder.ColCyan, 22, TextAnchor.MiddleLeft, FontStyle.Bold, new Vector2(0f, 1f));

        var objText = CreateText("ObjectiveText", headerPanel, $"OBJECTIVE: {objective}",
            new Vector2(18f, -48f), new Vector2(460f, 26f), new Color(0.75f, 0.82f, 0.95f), 15, TextAnchor.MiddleLeft, FontStyle.Normal, new Vector2(0f, 1f));

        // ── Radar Objective Scanner (Top Center) ──────────────────────────────
        var radar = CreateRadarDisplay("RadarScanner", canvasGO, new Vector2(0f, -20f));

        // ── 2. Bottom-Left: Energy Bar & Integrity ────────────────────────────
        var statusPanel = CreatePanel("StatusPanel", canvasGO, new Color(0.04f, 0.07f, 0.14f, 0.85f),
            new Vector2(0f, 0f), new Vector2(0f, 0f), new Vector2(25f, 25f), new Vector2(540f, 100f), new Vector2(0f, 0f));

        var nrgGauge = CreateCircularGauge("EnergyGauge", statusPanel, new Vector2(48f, 50f), new Vector2(74f, 74f), RuleBreakMasterBuilder.ColCyan, "ENERGY");
        var intGauge = CreateCircularGauge("IntegrityGauge", statusPanel, new Vector2(136f, 50f), new Vector2(74f, 74f), RuleBreakMasterBuilder.ColGreen, "INTEGRITY");

        // Energy Slider
        var sliderGO = new GameObject("EnergySlider");
        sliderGO.transform.SetParent(statusPanel.transform, false);
        var sliderRT = sliderGO.AddComponent<RectTransform>();
        sliderRT.pivot = new Vector2(0f, 0f);
        sliderRT.anchorMin = new Vector2(0f, 0f);
        sliderRT.anchorMax = new Vector2(0f, 0f);
        sliderRT.anchoredPosition = new Vector2(190f, 48f);
        sliderRT.sizeDelta = new Vector2(330f, 18f);

        var sliderBg = sliderGO.AddComponent<Image>();
        sliderBg.color = new Color(0.08f, 0.12f, 0.2f, 0.95f);

        var fillArea = new GameObject("Fill Area");
        fillArea.transform.SetParent(sliderGO.transform, false);
        var fillAreaRT = fillArea.AddComponent<RectTransform>();
        fillAreaRT.anchorMin = Vector2.zero;
        fillAreaRT.anchorMax = Vector2.one;
        fillAreaRT.sizeDelta = Vector2.zero;

        var fill = new GameObject("Fill");
        fill.transform.SetParent(fillArea.transform, false);
        var fillRT = fill.AddComponent<RectTransform>();
        fillRT.anchorMin = Vector2.zero;
        fillRT.anchorMax = Vector2.one;
        fillRT.sizeDelta = Vector2.zero;
        var fillImg = fill.AddComponent<Image>();
        fillImg.color = RuleBreakMasterBuilder.ColCyan;

        var slider = sliderGO.AddComponent<Slider>();
        slider.fillRect = fillRT;
        slider.minValue = 0f;
        slider.maxValue = 100f;
        slider.value = 100f;

        var energyTxt = CreateText("EnergyText", statusPanel, "GLITCH ENERGY: 100 / 100",
            new Vector2(190f, 70f), new Vector2(330f, 20f), RuleBreakMasterBuilder.ColCyan, 13, TextAnchor.MiddleLeft, FontStyle.Bold, new Vector2(0f, 0f));

        var integrityTxt = CreateText("IntegrityText", statusPanel, "REALITY INTEGRITY: 100%",
            new Vector2(190f, 16f), new Vector2(330f, 24f), RuleBreakMasterBuilder.ColGreen, 15, TextAnchor.MiddleLeft, FontStyle.Bold, new Vector2(0f, 0f));

        // ── 3. Bottom-Right: Memory Fragments Counter ─────────────────────────
        var fragPanel = CreatePanel("FragPanel", canvasGO, new Color(0.04f, 0.07f, 0.14f, 0.85f),
            new Vector2(1f, 0f), new Vector2(1f, 0f), new Vector2(-25f, 25f), new Vector2(280f, 50f), new Vector2(1f, 0f));

        var fragTxt = CreateText("FragmentText", fragPanel, "FRAGMENTS: 0 / 2",
            new Vector2(-18f, 13f), new Vector2(244f, 26f), RuleBreakMasterBuilder.ColAmber, 16, TextAnchor.MiddleRight, FontStyle.Bold, new Vector2(1f, 0f));

        // ── 4. Top-Right: Ability Modules Bar ─────────────────────────────────
        var abilityPanel = CreatePanel("AbilityPanel", canvasGO, new Color(0.04f, 0.07f, 0.14f, 0.85f),
            new Vector2(1f, 1f), new Vector2(1f, 1f), new Vector2(-25f, -25f), new Vector2(490f, 96f), new Vector2(1f, 1f));

        var indG = CreateAbilityIndicator("Ind_G", abilityPanel, new Vector2(45f, -8f),  "[G]", "GRAV", 10f, RuleBreakMasterBuilder.ColCyan);
        var indT = CreateAbilityIndicator("Ind_T", abilityPanel, new Vector2(125f, -8f), "[T]", "FREEZE", 15f, RuleBreakMasterBuilder.ColCyan);
        var indE = CreateAbilityIndicator("Ind_E", abilityPanel, new Vector2(205f, -8f), "[E]", "REALITY", 20f, RuleBreakMasterBuilder.ColViolet);
        var indR = CreateAbilityIndicator("Ind_R", abilityPanel, new Vector2(285f, -8f), "[R]", "CORRUPT", 25f, RuleBreakMasterBuilder.ColMagenta);

        var abG = indG.GetComponentInChildren<Text>();
        var abT = indT.GetComponentInChildren<Text>();
        var abE = indE.GetComponentInChildren<Text>();
        var abR = indR.GetComponentInChildren<Text>();

        var dimTxt = CreateText("DimensionText", abilityPanel, "REALITY: ALPHA",
            new Vector2(345f, -24f), new Vector2(130f, 24f), RuleBreakMasterBuilder.ColViolet, 13, TextAnchor.MiddleCenter, FontStyle.Bold, new Vector2(0f, 1f));

        // ── 5. Center-Top: System Message Toast ───────────────────────────────
        var sysMsgPanel = CreatePanel("SystemMsgPanel", canvasGO, new Color(0.03f, 0.05f, 0.1f, 0.94f),
            new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0f, -155f), new Vector2(650f, 54f), new Vector2(0.5f, 1f));

        var sysMsgTxt = CreateText("SystemMsgText", sysMsgPanel, "SYSTEM LOG DETECTED",
            new Vector2(0f, -27f), new Vector2(610f, 40f), RuleBreakMasterBuilder.ColCyan, 17, TextAnchor.MiddleCenter, FontStyle.Bold, new Vector2(0.5f, 0.5f));
        sysMsgPanel.SetActive(false);

        // ── 6. Center-Bottom: Interaction Prompt ──────────────────────────────
        var interactPanel = CreatePanel("InteractionPromptPanel", canvasGO, new Color(0.02f, 0.04f, 0.08f, 0.9f),
            new Vector2(0.5f, 0f), new Vector2(0.5f, 0f), new Vector2(0f, 150f), new Vector2(340f, 48f), new Vector2(0.5f, 0f));

        var interactTxt = CreateText("InteractionPromptText", interactPanel, "[F] INTERACT WITH TERMINAL",
            new Vector2(0f, 24f), new Vector2(320f, 32f), RuleBreakMasterBuilder.ColCyan, 17, TextAnchor.MiddleCenter, FontStyle.Bold, new Vector2(0.5f, 0.5f));
        interactPanel.SetActive(false);

        // ── Wire HUDController ────────────────────────────────────────────────
        var hud = canvasGO.AddComponent<HUDController>();
        var soHud = new SerializedObject(hud);
        soHud.FindProperty("energySlider").objectReferenceValue = slider;
        soHud.FindProperty("energyText").objectReferenceValue = energyTxt;
        soHud.FindProperty("integrityText").objectReferenceValue = integrityTxt;
        soHud.FindProperty("energyGauge").objectReferenceValue = nrgGauge;
        soHud.FindProperty("integrityGauge").objectReferenceValue = intGauge;
        soHud.FindProperty("radarDisplay").objectReferenceValue = radar;
        soHud.FindProperty("fragmentText").objectReferenceValue = fragTxt;
        soHud.FindProperty("levelText").objectReferenceValue = lvlText;
        soHud.FindProperty("objectiveText").objectReferenceValue = objText;
        soHud.FindProperty("systemMsgPanel").objectReferenceValue = sysMsgPanel;
        soHud.FindProperty("systemMsgText").objectReferenceValue = sysMsgTxt;
        soHud.FindProperty("interactionPromptPanel").objectReferenceValue = interactPanel;
        soHud.FindProperty("interactionPromptText").objectReferenceValue = interactTxt;
        soHud.FindProperty("dimensionText").objectReferenceValue = dimTxt;
        soHud.FindProperty("abilityGText").objectReferenceValue = abG;
        soHud.FindProperty("abilityTText").objectReferenceValue = abT;
        soHud.FindProperty("abilityEText").objectReferenceValue = abE;
        soHud.FindProperty("abilityRText").objectReferenceValue = abR;
        soHud.FindProperty("abilityGIndicator").objectReferenceValue = indG;
        soHud.FindProperty("abilityTIndicator").objectReferenceValue = indT;
        soHud.FindProperty("abilityEIndicator").objectReferenceValue = indE;
        soHud.FindProperty("abilityRIndicator").objectReferenceValue = indR;
        soHud.ApplyModifiedProperties();

        // ── 7. Pause Menu Panel ───────────────────────────────────────────────
        var pausePanel = CreatePanel("PausePanel", canvasGO, new Color(0.02f, 0.03f, 0.07f, 0.95f),
            Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero, new Vector2(0.5f, 0.5f));

        CreateText("PauseTitle", pausePanel, "SYSTEM PAUSED",
            new Vector2(0f, 210f), new Vector2(600f, 60f), RuleBreakMasterBuilder.ColCyan, 46, TextAnchor.MiddleCenter, FontStyle.Bold, new Vector2(0.5f, 0.5f));
        CreateText("PauseSubtitle", pausePanel, "SIMULATION SUSPENDED // NEURAL BUFFER LOCKED",
            new Vector2(0f, 155f), new Vector2(500f, 30f), new Color(0.6f, 0.65f, 0.75f), 18, TextAnchor.MiddleCenter, FontStyle.Normal, new Vector2(0.5f, 0.5f));

        float pBtnY = 45f;
        var btnResume     = CreateButton("ResumeBtn",     "RESUME SIMULATION", pausePanel, new Vector2(0f, pBtnY),        new Vector2(300f, 48f), RuleBreakMasterBuilder.ColCyan);
        var btnRestartChk = CreateButton("RestartChkBtn", "RESTART CHECKPOINT", pausePanel, new Vector2(0f, pBtnY - 60f), new Vector2(300f, 48f), RuleBreakMasterBuilder.ColNeonBlue);
        var btnRestartLvl = CreateButton("RestartLvlBtn", "RESTART LEVEL",      pausePanel, new Vector2(0f, pBtnY - 120f),new Vector2(300f, 48f), RuleBreakMasterBuilder.ColViolet);
        var btnPauseMenu  = CreateButton("PauseMainMenuBtn", "EXIT TO MAIN MENU", pausePanel, new Vector2(0f, pBtnY - 180f),new Vector2(300f, 48f), new Color(0.65f, 0.7f, 0.8f));

        var pauseUI = canvasGO.AddComponent<PauseMenuUI>();
        var soPause = new SerializedObject(pauseUI);
        soPause.FindProperty("pausePanel").objectReferenceValue = pausePanel;
        soPause.FindProperty("resumeButton").objectReferenceValue = btnResume;
        soPause.FindProperty("restartButton").objectReferenceValue = btnRestartChk;
        soPause.FindProperty("restartLevelButton").objectReferenceValue = btnRestartLvl;
        soPause.FindProperty("mainMenuButton").objectReferenceValue = btnPauseMenu;
        soPause.ApplyModifiedProperties();
        pausePanel.SetActive(false);

        // ── 8. Completion UI (Standard Level Complete or Level 04 Final Reveal) ───
        if (levelIndex == 4)
        {
            BuildFinalRevealUI(canvasGO);
        }
        else
        {
            BuildStandardLevelCompleteUI(canvasGO, levelIndex);
        }

        Undo.RegisterCreatedObjectUndo(canvasGO, "Create GameplayCanvas");
        return canvasGO;
    }

    private static void BuildStandardLevelCompleteUI(GameObject canvasGO, int levelIndex)
    {
        var completePanel = CreatePanel("LevelCompletePanel", canvasGO, new Color(0.02f, 0.03f, 0.07f, 0.96f),
            Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero, new Vector2(0.5f, 0.5f));

        var compTitle = CreateText("CompleteTitle", completePanel, $"LEVEL 0{levelIndex} COMPLETE",
            new Vector2(0f, 210f), new Vector2(800f, 70f), RuleBreakMasterBuilder.ColCyan, 50, TextAnchor.MiddleCenter, FontStyle.Bold, new Vector2(0.5f, 0.5f));

        var compStatus = CreateText("SystemStatus", completePanel, "SYSTEM STABILIZATION ACHIEVED",
            new Vector2(0f, 150f), new Vector2(600f, 30f), RuleBreakMasterBuilder.ColGreen, 20, TextAnchor.MiddleCenter, FontStyle.Bold, new Vector2(0.5f, 0.5f));

        var compIntegrity = CreateText("RealityIntegrity", completePanel, "REALITY INTEGRITY: 85%",
            new Vector2(0f, 95f), new Vector2(600f, 35f), RuleBreakMasterBuilder.ColCyan, 24, TextAnchor.MiddleCenter, FontStyle.Bold, new Vector2(0.5f, 0.5f));

        var compAbility = CreateText("NewAbility", completePanel, $"NEW ABILITY PROTOCOL SYNCHRONIZED",
            new Vector2(0f, 35f), new Vector2(700f, 40f), RuleBreakMasterBuilder.ColAmber, 20, TextAnchor.MiddleCenter, FontStyle.Bold, new Vector2(0.5f, 0.5f));

        var compStory = CreateText("StoryQuote", completePanel, GetLevelQuote(levelIndex),
            new Vector2(0f, -30f), new Vector2(750f, 50f), new Color(0.8f, 0.85f, 0.95f), 18, TextAnchor.MiddleCenter, FontStyle.Italic, new Vector2(0.5f, 0.5f));

        var btnContinue = CreateButton("ContinueBtn", "PROCEED TO NEXT SECTOR", completePanel, new Vector2(0f, -120f), new Vector2(320f, 54f), RuleBreakMasterBuilder.ColCyan);
        var btnCompleteMenu = CreateButton("CompleteMenuBtn", "MAIN MENU", completePanel, new Vector2(0f, -185f), new Vector2(320f, 48f), new Color(0.65f, 0.7f, 0.8f));

        var completeUI = completePanel.AddComponent<LevelCompleteUI>();
        var soComp = new SerializedObject(completeUI);
        soComp.FindProperty("levelCompleteText").objectReferenceValue = compTitle;
        soComp.FindProperty("systemStatusText").objectReferenceValue = compStatus;
        soComp.FindProperty("realityIntegrityText").objectReferenceValue = compIntegrity;
        soComp.FindProperty("newAbilityText").objectReferenceValue = compAbility;
        soComp.FindProperty("storyMessageText").objectReferenceValue = compStory;
        soComp.FindProperty("continueButton").objectReferenceValue = btnContinue;
        soComp.FindProperty("mainMenuButton").objectReferenceValue = btnCompleteMenu;

        soComp.FindProperty("completionTitle").stringValue = $"LEVEL 0{levelIndex} COMPLETE";
        soComp.FindProperty("systemStatus").stringValue = $"SECTOR 0{levelIndex}: COMPROMISED";
        soComp.FindProperty("integrityDisplay").floatValue = 100f - (levelIndex * 18f);
        soComp.FindProperty("storyMessage").stringValue = GetLevelQuote(levelIndex);
        soComp.ApplyModifiedProperties();
        completePanel.SetActive(false);

        // Setup LevelManager
        var lmGO = GameObject.Find("LevelManager") ?? new GameObject("LevelManager");
        var lm = lmGO.GetComponent<LevelManager>() ?? lmGO.AddComponent<LevelManager>();
        var soLM = new SerializedObject(lm);
        soLM.FindProperty("levelIndex").intValue = levelIndex;
        soLM.FindProperty("realityIntegrityTarget").floatValue = 100f - (levelIndex * 18f);
        soLM.FindProperty("levelCompletePanel").objectReferenceValue = completePanel;
        soLM.ApplyModifiedProperties();
    }

    private static void BuildFinalRevealUI(GameObject canvasGO)
    {
        // ── Reveal Sequence Panel ─────────────────────────────────────────────
        var revealPanel = CreatePanel("RevealPanel", canvasGO, new Color(0.02f, 0.04f, 0.08f, 0.85f),
            Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero, new Vector2(0.5f, 0.5f));

        var titleTxt = CreateText("RevealTitle", revealPanel, "SYSTEM BREACHED",
            new Vector2(0f, 180f), new Vector2(850f, 80f), RuleBreakMasterBuilder.ColCyan, 64, TextAnchor.MiddleCenter, FontStyle.Bold, new Vector2(0.5f, 0.5f));

        var subTxt = CreateText("RevealSub", revealPanel, "LEVEL 04 — THE FINAL PROTOCOL",
            new Vector2(0f, 100f), new Vector2(850f, 40f), new Color(0.7f, 0.78f, 0.95f), 24, TextAnchor.MiddleCenter, FontStyle.Normal, new Vector2(0.5f, 0.5f));

        var glitchTxt = CreateText("GlitchReveal", revealPanel, "YOU ARE THE GLITCH.",
            new Vector2(0f, 0f), new Vector2(900f, 80f), RuleBreakMasterBuilder.ColMagenta, 54, TextAnchor.MiddleCenter, FontStyle.Bold, new Vector2(0.5f, 0.5f));

        var subGlitchTxt = CreateText("GlitchSub", revealPanel, "The system was never designed to contain you.",
            new Vector2(0f, -80f), new Vector2(850f, 50f), new Color(0.85f, 0.9f, 1f), 26, TextAnchor.MiddleCenter, FontStyle.Italic, new Vector2(0.5f, 0.5f));
        revealPanel.SetActive(false);

        // ── Results Panel ─────────────────────────────────────────────────────
        var resultsPanel = CreatePanel("ResultsPanel", canvasGO, new Color(0.02f, 0.04f, 0.08f, 0.92f),
            Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero, new Vector2(0.5f, 0.5f));

        CreateText("ResultsTitle", resultsPanel, "SYSTEM BREACHED // FINAL REPORT",
            new Vector2(0f, 320f), new Vector2(850f, 60f), RuleBreakMasterBuilder.ColCyan, 44, TextAnchor.MiddleCenter, FontStyle.Bold, new Vector2(0.5f, 0.5f));

        var statusTxt  = CreateText("StatusTxt", resultsPanel, "PROTOCOL STATUS: BREACH COMPLETE",
            new Vector2(0f, 250f), new Vector2(800f, 35f), RuleBreakMasterBuilder.ColGreen, 20, TextAnchor.MiddleCenter, FontStyle.Bold, new Vector2(0.5f, 0.5f));

        var timeTxt    = CreateText("TimeTxt", resultsPanel, "TIME ELAPSED: 04:32",
            new Vector2(-220f, 180f), new Vector2(380f, 30f), RuleBreakMasterBuilder.ColCyan, 20, TextAnchor.MiddleLeft, FontStyle.Bold, new Vector2(0.5f, 0.5f));

        var fragsTxt   = CreateText("FragsTxt", resultsPanel, "MEMORY FRAGMENTS: 9 / 9",
            new Vector2(220f, 180f), new Vector2(380f, 30f), RuleBreakMasterBuilder.ColAmber, 20, TextAnchor.MiddleLeft, FontStyle.Bold, new Vector2(0.5f, 0.5f));

        var deathsTxt  = CreateText("DeathsTxt", resultsPanel, "SYSTEM RETRIES: 0",
            new Vector2(-220f, 130f), new Vector2(380f, 30f), RuleBreakMasterBuilder.ColRed, 20, TextAnchor.MiddleLeft, FontStyle.Bold, new Vector2(0.5f, 0.5f));

        var scoreTxt   = CreateText("ScoreTxt", resultsPanel, "FINAL SCORE: 12,450 PTS",
            new Vector2(220f, 130f), new Vector2(380f, 30f), RuleBreakMasterBuilder.ColGreen, 20, TextAnchor.MiddleLeft, FontStyle.Bold, new Vector2(0.5f, 0.5f));

        var vectorTxt  = CreateText("VectorTxt", resultsPanel, "PRIMARY VECTOR: RULE CORRUPTION",
            new Vector2(0f, 75f), new Vector2(700f, 35f), RuleBreakMasterBuilder.ColViolet, 22, TextAnchor.MiddleCenter, FontStyle.Bold, new Vector2(0.5f, 0.5f));

        var unlockedTxt = CreateText("UnlockedTxt", resultsPanel, "ALL LEVELS UNLOCKED // ARCHIVE PROTOCOLS ACCESSIBLE",
            new Vector2(0f, 25f), new Vector2(800f, 35f), new Color(0.7f, 0.8f, 0.95f), 18, TextAnchor.MiddleCenter, FontStyle.Normal, new Vector2(0.5f, 0.5f));

        float rBtnY = -70f;
        var btnLS      = CreateButton("LSBtn",      "LEVEL SELECT", resultsPanel, new Vector2(-220f, rBtnY),       new Vector2(240f, 48f), RuleBreakMasterBuilder.ColViolet);
        var btnRestart = CreateButton("RestartBtn", "RESTART LEVEL", resultsPanel, new Vector2(0f, rBtnY),          new Vector2(240f, 48f), RuleBreakMasterBuilder.ColNeonBlue);
        var btnMenu    = CreateButton("MenuBtn",    "MAIN MENU",    resultsPanel, new Vector2(220f, rBtnY),        new Vector2(240f, 48f), RuleBreakMasterBuilder.ColCyan);
        var btnCreds   = CreateButton("CredsBtn",   "CREDITS",      resultsPanel, new Vector2(0f, rBtnY - 65f),    new Vector2(240f, 46f), new Color(0.65f, 0.75f, 0.9f));
        resultsPanel.SetActive(false);

        // ── Sub-Modals for Final Reveal (Level Select & Credits) ──────────────
        var lsModal = CreatePanel("FR_LevelSelectPanel", canvasGO, new Color(0.04f, 0.07f, 0.14f, 0.96f),
            Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero, new Vector2(0.5f, 0.5f));
        CreateText("LSTitle", lsModal, "SELECT ARCHIVE SECTOR", new Vector2(0f, 210f), new Vector2(600f, 45f), RuleBreakMasterBuilder.ColCyan, 36, TextAnchor.MiddleCenter, FontStyle.Bold, new Vector2(0.5f, 0.5f));
        var bL1 = CreateButton("BL1", "LEVEL 01 // GRAVITY BREAK", lsModal, new Vector2(0f, 120f), new Vector2(400f, 48f), RuleBreakMasterBuilder.ColCyan);
        var bL2 = CreateButton("BL2", "LEVEL 02 // TIME FRACTURE", lsModal, new Vector2(0f, 55f),  new Vector2(400f, 48f), RuleBreakMasterBuilder.ColNeonBlue);
        var bL3 = CreateButton("BL3", "LEVEL 03 // DIMENSION SHIFT", lsModal, new Vector2(0f, -10f), new Vector2(400f, 48f), RuleBreakMasterBuilder.ColViolet);
        var bL4 = CreateButton("BL4", "LEVEL 04 // RULE CORRUPTION", lsModal, new Vector2(0f, -75f), new Vector2(400f, 48f), RuleBreakMasterBuilder.ColMagenta);
        var bCloseLS = CreateButton("BCloseLS", "BACK", lsModal, new Vector2(0f, -160f), new Vector2(240f, 46f), new Color(0.65f, 0.7f, 0.8f));
        lsModal.SetActive(false);

        var credsModal = CreatePanel("FR_CreditsPanel", canvasGO, new Color(0.04f, 0.07f, 0.14f, 0.96f),
            Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero, new Vector2(0.5f, 0.5f));
        CreateText("CredTitle", credsModal, "RULE//BREAK // CREDITS", new Vector2(0f, 200f), new Vector2(600f, 45f), RuleBreakMasterBuilder.ColCyan, 36, TextAnchor.MiddleCenter, FontStyle.Bold, new Vector2(0.5f, 0.5f));
        string cTxt = "RULE//BREAK\n\nALL RULES BROKEN. REALITY LIBERATED.\n\nThank you for playing!\nCreated with Unity 6.6 Universal Render Pipeline.";
        CreateText("CredBody", credsModal, cTxt, new Vector2(0f, 20f), new Vector2(650f, 240f), new Color(0.85f, 0.9f, 0.98f), 20, TextAnchor.MiddleCenter, FontStyle.Normal, new Vector2(0.5f, 0.5f));
        var bCloseCreds = CreateButton("BCloseCreds", "CLOSE", credsModal, new Vector2(0f, -160f), new Vector2(240f, 46f), RuleBreakMasterBuilder.ColCyan);
        credsModal.SetActive(false);

        // ── Wire FinalRevealUI & FinalRevealManager ───────────────────────────
        var frui = canvasGO.AddComponent<FinalRevealUI>();
        var soFR = new SerializedObject(frui);
        soFR.FindProperty("revealPanel").objectReferenceValue = revealPanel;
        soFR.FindProperty("resultsPanel").objectReferenceValue = resultsPanel;
        soFR.FindProperty("creditsPanel").objectReferenceValue = credsModal;
        soFR.FindProperty("levelSelectPanel").objectReferenceValue = lsModal;

        soFR.FindProperty("titleText").objectReferenceValue = titleTxt;
        soFR.FindProperty("subtitleText").objectReferenceValue = subTxt;
        soFR.FindProperty("storyGlitchText").objectReferenceValue = glitchTxt;
        soFR.FindProperty("storySubText").objectReferenceValue = subGlitchTxt;

        soFR.FindProperty("statusValueText").objectReferenceValue = statusTxt;
        soFR.FindProperty("timeValueText").objectReferenceValue = timeTxt;
        soFR.FindProperty("fragmentsValueText").objectReferenceValue = fragsTxt;
        soFR.FindProperty("deathsValueText").objectReferenceValue = deathsTxt;
        soFR.FindProperty("scoreValueText").objectReferenceValue = scoreTxt;
        soFR.FindProperty("primaryVectorValueText").objectReferenceValue = vectorTxt;
        soFR.FindProperty("unlockedValueText").objectReferenceValue = unlockedTxt;

        soFR.FindProperty("levelSelectButton").objectReferenceValue = btnLS;
        soFR.FindProperty("restartLevelButton").objectReferenceValue = btnRestart;
        soFR.FindProperty("mainMenuButton").objectReferenceValue = btnMenu;
        soFR.FindProperty("creditsButton").objectReferenceValue = btnCreds;
        soFR.FindProperty("closeCreditsButton").objectReferenceValue = bCloseCreds;
        soFR.FindProperty("closeLevelSelectButton").objectReferenceValue = bCloseLS;

        soFR.FindProperty("selectL1Button").objectReferenceValue = bL1;
        soFR.FindProperty("selectL2Button").objectReferenceValue = bL2;
        soFR.FindProperty("selectL3Button").objectReferenceValue = bL3;
        soFR.FindProperty("selectL4Button").objectReferenceValue = bL4;
        soFR.ApplyModifiedProperties();

        // Attach FinalRevealManager
        var frmGO = GameObject.Find("FinalRevealManager") ?? new GameObject("FinalRevealManager");
        var frm = frmGO.GetComponent<FinalRevealManager>() ?? frmGO.AddComponent<FinalRevealManager>();
        var soFRM = new SerializedObject(frm);
        soFRM.FindProperty("revealPanel").objectReferenceValue = revealPanel;
        soFRM.FindProperty("revealUI").objectReferenceValue = frui;
        soFRM.ApplyModifiedProperties();
    }

    private static string GetLevelQuote(int lvl)
    {
        switch (lvl)
        {
            case 1: return "\"Gravity is the first artificial construct to fail.\"";
            case 2: return "\"Time does not stop for the system. It only stops for you.\"";
            case 3: return "\"Two realities occupy the same memory space. One must be false.\"";
            case 4: return "\"YOU ARE THE GLITCH. Break the core to be free.\"";
            default: return "\"Rule breaking confirmed.\"";
        }
    }

    private static GameObject CreatePanel(string name, GameObject parent, Color col,
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
        img.sprite = SciFiProceduralTextures.GetGlassPanel(64, 64, 2, col, new Color(col.r * 1.6f + 0.05f, col.g * 1.6f + 0.15f, col.b * 1.6f + 0.25f, 0.75f));
        img.type = Image.Type.Sliced;
        return go;
    }

    private static Text CreateText(string name, GameObject parent, string content,
        Vector2 anchoredPos, Vector2 size, Color col, int fontSize, TextAnchor align, FontStyle style, Vector2 pivot)
    {
        var go = new GameObject(name);
        go.transform.SetParent(parent.transform, false);
        var rt = go.AddComponent<RectTransform>();
        rt.anchorMin = pivot;
        rt.anchorMax = pivot;
        rt.pivot = pivot;
        rt.anchoredPosition = anchoredPos;
        rt.sizeDelta = size;

        var txt = go.AddComponent<Text>();
        txt.text = content;
        txt.color = col;
        txt.fontSize = fontSize;
        txt.alignment = align;
        txt.fontStyle = style;
        txt.font = DefaultFont;
        return txt;
    }

    private static Button CreateButton(string name, string label, GameObject parent,
        Vector2 anchoredPos, Vector2 size, Color accentCol)
    {
        var go = new GameObject(name);
        go.transform.SetParent(parent.transform, false);
        var rt = go.AddComponent<RectTransform>();
        rt.anchorMin = new Vector2(0.5f, 0.5f);
        rt.anchorMax = new Vector2(0.5f, 0.5f);
        rt.pivot = new Vector2(0.5f, 0.5f);
        rt.anchoredPosition = anchoredPos;
        rt.sizeDelta = size;

        var img = go.AddComponent<Image>();
        img.sprite = SciFiProceduralTextures.GetGlassPanel(64, 64, 2,
            new Color(accentCol.r * 0.12f, accentCol.g * 0.12f, accentCol.b * 0.12f, 0.92f),
            new Color(accentCol.r * 0.75f, accentCol.g * 0.75f, accentCol.b * 0.75f, 0.7f));
        img.type = Image.Type.Sliced;

        var btn = go.AddComponent<Button>();
        var colors = btn.colors;
        colors.normalColor = Color.white;
        colors.highlightedColor = new Color(1.2f, 1.2f, 1.2f, 1f);
        colors.pressedColor = new Color(0.8f, 0.8f, 0.8f, 1f);
        colors.selectedColor = colors.highlightedColor;
        btn.colors = colors;

        // Attach SciFiButton for smooth hover animation, glow border, and audio
        var sciFiBtn = go.AddComponent<SciFiButton>();
        var soBtn = new SerializedObject(sciFiBtn);
        soBtn.FindProperty("targetBorderImage").objectReferenceValue = img;
        soBtn.FindProperty("normalBorderColor").colorValue = new Color(accentCol.r * 0.75f, accentCol.g * 0.75f, accentCol.b * 0.75f, 0.7f);
        soBtn.FindProperty("hoverBorderColor").colorValue = accentCol;
        soBtn.ApplyModifiedProperties();

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

        return btn;
    }

    private static CircularGauge CreateCircularGauge(string name, GameObject parent, Vector2 anchoredPos, Vector2 size, Color primaryColor, string labelStr)
    {
        var go = new GameObject(name);
        go.transform.SetParent(parent.transform, false);
        var rt = go.AddComponent<RectTransform>();
        rt.anchorMin = new Vector2(0f, 0.5f);
        rt.anchorMax = new Vector2(0f, 0.5f);
        rt.pivot = new Vector2(0.5f, 0.5f);
        rt.anchoredPosition = anchoredPos;
        rt.sizeDelta = size;

        // Outer rotating ring
        var ringGO = new GameObject("OuterRing");
        ringGO.transform.SetParent(go.transform, false);
        var ringRT = ringGO.AddComponent<RectTransform>();
        ringRT.anchorMin = Vector2.zero;
        ringRT.anchorMax = Vector2.one;
        ringRT.sizeDelta = Vector2.zero;
        var ringImg = ringGO.AddComponent<Image>();
        ringImg.sprite = SciFiProceduralTextures.GetSegmentedRing(128, 16, 6f, new Color(primaryColor.r, primaryColor.g, primaryColor.b, 0.55f));
        ringImg.raycastTarget = false;

        // Radial fill image
        var fillGO = new GameObject("RadialFill");
        fillGO.transform.SetParent(go.transform, false);
        var fillRT = fillGO.AddComponent<RectTransform>();
        fillRT.anchorMin = new Vector2(0.08f, 0.08f);
        fillRT.anchorMax = new Vector2(0.92f, 0.92f);
        fillRT.sizeDelta = Vector2.zero;
        var fillImg = fillGO.AddComponent<Image>();
        fillImg.sprite = SciFiProceduralTextures.GetHollowRing(128, 0.72f, Color.white);
        fillImg.type = Image.Type.Filled;
        fillImg.fillMethod = Image.FillMethod.Radial360;
        fillImg.fillOrigin = (int)Image.Origin360.Top;
        fillImg.fillClockwise = true;
        fillImg.color = primaryColor;
        fillImg.raycastTarget = false;

        // Numeric text
        var numTxt = CreateText("ValueText", go, "100", Vector2.zero, new Vector2(size.x, 24f), primaryColor, 15, TextAnchor.MiddleCenter, FontStyle.Bold, new Vector2(0.5f, 0.5f));

        // Label text
        var lblTxt = CreateText("LabelText", go, labelStr, new Vector2(0f, -size.y * 0.42f), new Vector2(size.x, 16f), new Color(0.7f, 0.8f, 0.95f), 10, TextAnchor.MiddleCenter, FontStyle.Bold, new Vector2(0.5f, 0.5f));

        var gauge = go.AddComponent<CircularGauge>();
        var so = new SerializedObject(gauge);
        so.FindProperty("radialFillImage").objectReferenceValue = fillImg;
        so.FindProperty("outerRingTransform").objectReferenceValue = ringRT;
        so.FindProperty("valueText").objectReferenceValue = numTxt;
        so.FindProperty("labelText").objectReferenceValue = lblTxt;
        so.FindProperty("normalColor").colorValue = primaryColor;
        so.ApplyModifiedProperties();

        return gauge;
    }

    private static RadialAbilityIndicator CreateAbilityIndicator(string name, GameObject parent, Vector2 anchoredPos, string key, string label, float cost, Color col)
    {
        var go = new GameObject(name);
        go.transform.SetParent(parent.transform, false);
        var rt = go.AddComponent<RectTransform>();
        rt.anchorMin = new Vector2(0f, 1f);
        rt.anchorMax = new Vector2(0f, 1f);
        rt.pivot = new Vector2(0.5f, 1f);
        rt.anchoredPosition = anchoredPos;
        rt.sizeDelta = new Vector2(68f, 72f);

        // Ring image
        var ringGO = new GameObject("Ring");
        ringGO.transform.SetParent(go.transform, false);
        var ringRT = ringGO.AddComponent<RectTransform>();
        ringRT.anchorMin = new Vector2(0.12f, 0.28f);
        ringRT.anchorMax = new Vector2(0.88f, 0.95f);
        ringRT.sizeDelta = Vector2.zero;
        var ringImg = ringGO.AddComponent<Image>();
        ringImg.sprite = SciFiProceduralTextures.GetHollowRing(64, 0.75f, col);
        ringImg.color = col;
        ringImg.raycastTarget = false;

        // Key text inside ring
        var kTxt = CreateText("KeyText", ringGO, key, Vector2.zero, new Vector2(40f, 22f), col, 14, TextAnchor.MiddleCenter, FontStyle.Bold, new Vector2(0.5f, 0.5f));

        // Name text
        var nTxt = CreateText("NameText", go, label, new Vector2(0f, -54f), new Vector2(68f, 16f), new Color(0.8f, 0.88f, 1f), 10, TextAnchor.MiddleCenter, FontStyle.Bold, new Vector2(0.5f, 0.5f));

        // Cost text
        var cTxt = CreateText("CostText", go, $"{cost}E", new Vector2(0f, -68f), new Vector2(68f, 14f), new Color(0.6f, 0.75f, 0.95f), 9, TextAnchor.MiddleCenter, FontStyle.Normal, new Vector2(0.5f, 0.5f));

        var indicator = go.AddComponent<RadialAbilityIndicator>();
        var so = new SerializedObject(indicator);
        so.FindProperty("outerRing").objectReferenceValue = ringImg;
        so.FindProperty("keyText").objectReferenceValue = kTxt;
        so.FindProperty("nameText").objectReferenceValue = nTxt;
        so.FindProperty("costText").objectReferenceValue = cTxt;
        so.FindProperty("energyCost").floatValue = cost;
        so.FindProperty("activeColor").colorValue = col;
        so.ApplyModifiedProperties();

        return indicator;
    }

    private static RadarObjectiveDisplay CreateRadarDisplay(string name, GameObject parent, Vector2 anchoredPos)
    {
        var go = new GameObject(name);
        go.transform.SetParent(parent.transform, false);
        var rt = go.AddComponent<RectTransform>();
        rt.anchorMin = new Vector2(0.5f, 1f);
        rt.anchorMax = new Vector2(0.5f, 1f);
        rt.pivot = new Vector2(0.5f, 1f);
        rt.anchoredPosition = anchoredPos;
        rt.sizeDelta = new Vector2(130f, 130f);

        var bgImg = go.AddComponent<Image>();
        bgImg.sprite = SciFiProceduralTextures.GetRadarGrid(128, new Color(0f, 0.9f, 1f, 0.45f));
        bgImg.raycastTarget = false;

        // Blip Container
        var blipGO = new GameObject("BlipContainer");
        blipGO.transform.SetParent(go.transform, false);
        var blipRT = blipGO.AddComponent<RectTransform>();
        blipRT.anchorMin = new Vector2(0.08f, 0.08f);
        blipRT.anchorMax = new Vector2(0.92f, 0.92f);
        blipRT.sizeDelta = Vector2.zero;

        // Sweep Beam
        var sweepGO = new GameObject("SweepBeam");
        sweepGO.transform.SetParent(go.transform, false);
        var sweepRT = sweepGO.AddComponent<RectTransform>();
        sweepRT.anchorMin = new Vector2(0.08f, 0.08f);
        sweepRT.anchorMax = new Vector2(0.92f, 0.92f);
        sweepRT.sizeDelta = Vector2.zero;
        var sweepImg = sweepGO.AddComponent<Image>();
        sweepImg.sprite = SciFiProceduralTextures.GetRadarSweep(128, 55f, new Color(0f, 0.95f, 1f, 0.4f));
        sweepImg.raycastTarget = false;

        // Target readout
        var readTxt = CreateText("DistanceText", go, "SEARCHING...", new Vector2(0f, -135f), new Vector2(160f, 20f), RuleBreakMasterBuilder.ColCyan, 12, TextAnchor.MiddleCenter, FontStyle.Bold, new Vector2(0.5f, 0.5f));

        var radar = go.AddComponent<RadarObjectiveDisplay>();
        var so = new SerializedObject(radar);
        so.FindProperty("sweepBeamTransform").objectReferenceValue = sweepRT;
        so.FindProperty("blipContainer").objectReferenceValue = blipRT;
        so.FindProperty("distanceReadoutText").objectReferenceValue = readTxt;
        so.ApplyModifiedProperties();

        return radar;
    }
}

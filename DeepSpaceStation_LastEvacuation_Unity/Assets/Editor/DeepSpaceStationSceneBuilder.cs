using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public static class DeepSpaceStationSceneBuilder
{
    private static readonly Vector3 PlayerStart = new Vector3(0f, 1.1f, 10f);

    [MenuItem("Deep Space Station/Build Playable Scene")]
    public static void BuildPlayableScene()
    {
        EnsureFolder("Assets", "Scenes");
        EnsureFolder("Assets", "Materials");
        EnsureFolder("Assets", "Prefabs");

        Scene scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);

        Material deck = Mat("M_Deck_Ribbed", new Color(0.09f, 0.13f, 0.15f), new Color(0.02f, 0.05f, 0.06f));
        Material wall = Mat("M_Wall_ColdSteel", new Color(0.16f, 0.23f, 0.27f), new Color(0.01f, 0.03f, 0.04f));
        Material dark = Mat("M_Dark_Machinery", new Color(0.025f, 0.03f, 0.035f), Color.black);
        Material yellow = Mat("M_Warning_Yellow", new Color(1f, 0.78f, 0.12f), new Color(0.22f, 0.14f, 0.01f));
        Material cyan = Mat("M_Cyan_Emissive", new Color(0.04f, 0.8f, 1f), new Color(0.02f, 1f, 1.35f));
        Material green = Mat("M_Green_Emissive", new Color(0.06f, 1f, 0.45f), new Color(0.04f, 1.2f, 0.45f));
        Material red = Mat("M_Red_Emissive", new Color(1f, 0.1f, 0.04f), new Color(1.4f, 0.06f, 0.02f));
        Material orange = Mat("M_Orange_Emissive", new Color(1f, 0.38f, 0.05f), new Color(1.2f, 0.22f, 0.01f));
        Material glass = Mat("M_Glass_Transparent", new Color(0.25f, 0.75f, 1f, 0.28f), new Color(0.05f, 0.45f, 0.7f), true);
        Material hazard = Mat("M_Radiation_Transparent", new Color(1f, 0.05f, 0f, 0.38f), new Color(1.2f, 0f, 0f), true);
        Material pod = Mat("M_EscapePod_Ceramic", new Color(0.75f, 0.88f, 0.9f), new Color(0.05f, 0.09f, 0.1f));

        CreateLighting(cyan, red, green);
        CreateStation(deck, wall, dark, yellow, cyan, red, orange, glass, hazard);

        new GameObject("GameManager").AddComponent<GameManager>();
        GameObject player = CreatePlayer();
        CreateHUD();

        CreateMissionBoard(wall, cyan, yellow);
        CreateTerminal(new Vector3(-7.4f, 0f, 7.8f), "Life Support", cyan, dark, green, yellow);
        CreateTerminal(new Vector3(7.4f, 0f, 3.3f), "Navigation", cyan, dark, green, yellow);
        CreateTerminal(new Vector3(-7.4f, 0f, -6.5f), "Reactor", cyan, dark, green, yellow);

        CreateDoor("Access Door", new Vector3(0f, 1.6f, 5.2f), false, wall, yellow, cyan);
        CreateDoor("Escape Lock Door", new Vector3(0f, 1.6f, -12.3f), true, wall, yellow, red);
        CreateEscapePod(new Vector3(0f, 1f, -17.2f), pod, green, glass, yellow);

        CreateOxygenCanister(new Vector3(6.2f, 0.75f, 8.2f), cyan, yellow);
        CreateOxygenCanister(new Vector3(-6.3f, 0.75f, -1.2f), cyan, yellow);

        Selection.activeGameObject = player;
        EditorSceneManager.SaveScene(scene, "Assets/Scenes/Station_A.unity");
        AssetDatabase.SaveAssets();
        EditorUtility.DisplayDialog("Deep Space Station", "Upgraded playable scene created at Assets/Scenes/Station_A.unity", "OK");
    }

    private static void EnsureFolder(string parent, string folder)
    {
        if (!AssetDatabase.IsValidFolder(parent + "/" + folder))
        {
            AssetDatabase.CreateFolder(parent, folder);
        }
    }

    private static Material Mat(string name, Color color, Color emission, bool transparent = false)
    {
        string path = "Assets/Materials/" + name + ".mat";
        Material material = AssetDatabase.LoadAssetAtPath<Material>(path);
        if (material == null)
        {
            material = new Material(Shader.Find("Standard"));
            AssetDatabase.CreateAsset(material, path);
        }

        material.color = color;
        material.EnableKeyword("_EMISSION");
        material.SetColor("_EmissionColor", emission);

        if (transparent)
        {
            material.SetFloat("_Mode", 3f);
            material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
            material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
            material.SetInt("_ZWrite", 0);
            material.DisableKeyword("_ALPHATEST_ON");
            material.EnableKeyword("_ALPHABLEND_ON");
            material.renderQueue = 3000;
        }

        return material;
    }

    private static void CreateLighting(Material cyan, Material red, Material green)
    {
        RenderSettings.ambientLight = new Color(0.035f, 0.055f, 0.07f);

        GameObject sun = new GameObject("Low Emergency Directional Light");
        Light sunLight = sun.AddComponent<Light>();
        sunLight.type = LightType.Directional;
        sunLight.intensity = 0.22f;
        sun.transform.rotation = Quaternion.Euler(42f, -30f, 0f);

        PointLight("Cold Atrium Light", new Vector3(0f, 3.2f, 5f), new Color(0.2f, 0.75f, 1f), 5.2f, 17f);
        PointLight("Reactor Warning Light", new Vector3(-5.5f, 3f, -6.6f), new Color(1f, 0.08f, 0.03f), 4.5f, 11f);
        PointLight("Escape Pod Green Beacon", new Vector3(0f, 3.3f, -17f), new Color(0.1f, 1f, 0.45f), 4.8f, 12f);

        CeilingLight(new Vector3(-6f, 2.95f, 7.4f), cyan);
        CeilingLight(new Vector3(6f, 2.95f, 3.2f), cyan);
        CeilingLight(new Vector3(-6f, 2.95f, -6.5f), red);
        CeilingLight(new Vector3(0f, 2.95f, -16f), green);
    }

    private static void CreateStation(Material deck, Material wall, Material dark, Material yellow, Material cyan, Material red, Material orange, Material glass, Material hazard)
    {
        Cube("Main Deck", new Vector3(0f, -0.12f, 0f), new Vector3(20f, 0.24f, 27f), deck);
        Cube("Escape Bay Deck", new Vector3(0f, -0.1f, -16.5f), new Vector3(9f, 0.24f, 8f), deck);

        Wall("North Wall", new Vector3(0f, 1.55f, 13.5f), new Vector3(20f, 3.1f, 0.45f), wall, yellow);
        Wall("West Wall", new Vector3(-10f, 1.55f, 0f), new Vector3(0.45f, 3.1f, 27f), wall, yellow);
        Wall("East Wall", new Vector3(10f, 1.55f, 0f), new Vector3(0.45f, 3.1f, 27f), wall, yellow);
        Wall("South Wall Left", new Vector3(-6f, 1.55f, -13.5f), new Vector3(6.5f, 3.1f, 0.45f), wall, yellow);
        Wall("South Wall Right", new Vector3(6f, 1.55f, -13.5f), new Vector3(6.5f, 3.1f, 0.45f), wall, yellow);
        Wall("Pod Bay Back Wall", new Vector3(0f, 1.55f, -20.5f), new Vector3(9f, 3.1f, 0.45f), wall, yellow);
        Wall("Pod Bay West Wall", new Vector3(-4.5f, 1.55f, -16.8f), new Vector3(0.45f, 3.1f, 7.8f), wall, yellow);
        Wall("Pod Bay East Wall", new Vector3(4.5f, 1.55f, -16.8f), new Vector3(0.45f, 3.1f, 7.8f), wall, yellow);

        for (int z = -10; z <= 10; z += 4)
        {
            Cube("Floor Rib Z " + z, new Vector3(0f, 0.025f, z), new Vector3(19f, 0.035f, 0.09f), dark);
        }

        for (int x = -8; x <= 8; x += 4)
        {
            Cube("Floor Rib X " + x, new Vector3(x, 0.035f, 0f), new Vector3(0.09f, 0.04f, 25f), dark);
        }

        CreateFloorPath(cyan, yellow, red);
        CreatePipes(dark, cyan, orange);
        CreateProps(dark, wall, yellow, glass);

        GameObject leak = Cube("Radiation Leak Trigger", new Vector3(4.5f, 0.07f, -7.5f), new Vector3(4.2f, 0.12f, 4.2f), hazard);
        leak.GetComponent<Collider>().isTrigger = true;
        leak.AddComponent<HazardZone>();
        WorldLabel("RADIATION LEAK\nO2 DRAIN x2.6", new Vector3(4.5f, 1.15f, -7.5f), PlayerStart, Color.red, 0.018f);
    }

    private static void CreateFloorPath(Material cyan, Material yellow, Material red)
    {
        Cube("Main Objective Path A", new Vector3(0f, 0.055f, 7.8f), new Vector3(13.5f, 0.035f, 0.18f), cyan);
        Cube("Main Objective Path B", new Vector3(7.4f, 0.055f, 5.3f), new Vector3(0.18f, 0.035f, 4.8f), cyan);
        Cube("Main Objective Path C", new Vector3(-7.4f, 0.055f, 0.8f), new Vector3(0.18f, 0.035f, 14f), cyan);
        Cube("Escape Route", new Vector3(0f, 0.06f, -10.5f), new Vector3(0.25f, 0.04f, 5.5f), yellow);
        Cube("Danger Stripe A", new Vector3(2.4f, 0.07f, -7.5f), new Vector3(0.16f, 0.04f, 4.2f), red);
        Cube("Danger Stripe B", new Vector3(6.6f, 0.07f, -7.5f), new Vector3(0.16f, 0.04f, 4.2f), red);
    }

    private static void CreateProps(Material dark, Material wall, Material yellow, Material glass)
    {
        Cube("Central Reactor Core", new Vector3(0f, 0.9f, -2f), new Vector3(2.2f, 1.8f, 2.2f), dark);
        Cube("Reactor Glass Band", new Vector3(0f, 1.25f, -2f), new Vector3(2.35f, 0.22f, 2.35f), glass);
        Cube("Broken Cargo A", new Vector3(5.7f, 0.55f, -4.5f), new Vector3(2f, 1.1f, 2.1f), wall);
        Cube("Broken Cargo B", new Vector3(-5.2f, 0.45f, 4.5f), new Vector3(2.3f, 0.9f, 1.8f), wall);
        Cube("Tool Chest", new Vector3(6.7f, 0.35f, 9.5f), new Vector3(1.2f, 0.7f, 0.7f), yellow);
        Cube("Observation Window", new Vector3(0f, 1.8f, 13.24f), new Vector3(6f, 1.4f, 0.08f), glass);
        WorldLabel("MISSION:\nRepair 3 terminals\nFollow glowing floor lines", new Vector3(0f, 1.9f, 11.7f), PlayerStart, Color.white, 0.016f);
    }

    private static void CreatePipes(Material dark, Material cyan, Material orange)
    {
        Cylinder("Ceiling Pipe Left", new Vector3(-8.6f, 2.8f, 0f), new Vector3(0.12f, 13f, 0.12f), Quaternion.Euler(90f, 0f, 0f), dark);
        Cylinder("Ceiling Pipe Right", new Vector3(8.6f, 2.8f, 0f), new Vector3(0.12f, 13f, 0.12f), Quaternion.Euler(90f, 0f, 0f), dark);
        Cylinder("Cyan Cable", new Vector3(-7.4f, 0.35f, 2f), new Vector3(0.055f, 6f, 0.055f), Quaternion.Euler(90f, 0f, 0f), cyan);
        Cylinder("Orange Cable", new Vector3(7.4f, 0.35f, -2f), new Vector3(0.055f, 5f, 0.055f), Quaternion.Euler(90f, 0f, 0f), orange);
    }

    private static GameObject CreatePlayer()
    {
        GameObject player = GameObject.CreatePrimitive(PrimitiveType.Capsule);
        player.name = "Player";
        player.transform.position = PlayerStart;
        Object.DestroyImmediate(player.GetComponent<CapsuleCollider>());

        CharacterController controller = player.AddComponent<CharacterController>();
        controller.height = 1.8f;
        controller.radius = 0.35f;
        controller.center = new Vector3(0f, 0.8f, 0f);

        GameObject cameraObject = new GameObject("Main Camera");
        cameraObject.tag = "MainCamera";
        cameraObject.transform.SetParent(player.transform);
        cameraObject.transform.localPosition = new Vector3(0f, 1.45f, 0f);
        Camera playerCamera = cameraObject.AddComponent<Camera>();
        playerCamera.fieldOfView = 75f;
        cameraObject.AddComponent<AudioListener>();

        FirstPersonController movement = player.AddComponent<FirstPersonController>();
        GameObjectInteractor interactor = player.AddComponent<GameObjectInteractor>();
        player.AddComponent<PlayerOxygen>();

        SetObject(movement, "playerCamera", playerCamera);
        SetObject(interactor, "viewCamera", playerCamera);
        SetFloat(interactor, "interactDistance", 5.5f);
        return player;
    }

    private static void CreateTerminal(Vector3 basePosition, string systemName, Material cyan, Material dark, Material green, Material yellow)
    {
        GameObject root = new GameObject(systemName + " Terminal");
        root.transform.position = basePosition;
        BoxCollider hitbox = root.AddComponent<BoxCollider>();
        hitbox.size = new Vector3(2.4f, 2.4f, 1.5f);
        hitbox.center = new Vector3(0f, 1.1f, 0f);

        TerminalTask task = root.AddComponent<TerminalTask>();
        SetString(task, "systemName", systemName);

        Cube(systemName + " Terminal Base", basePosition + new Vector3(0f, 0.35f, 0f), new Vector3(1.8f, 0.7f, 1f), dark).transform.SetParent(root.transform);
        Cube(systemName + " Terminal Column", basePosition + new Vector3(0f, 1.1f, 0.22f), new Vector3(1.5f, 1.2f, 0.35f), dark).transform.SetParent(root.transform);
        GameObject screen = Cube(systemName + " Glowing Screen", basePosition + new Vector3(0f, 1.25f, -0.02f), new Vector3(1.25f, 0.65f, 0.08f), cyan);
        screen.transform.SetParent(root.transform);
        Cube(systemName + " Keyboard", basePosition + new Vector3(0f, 0.78f, -0.35f), new Vector3(1.25f, 0.08f, 0.45f), yellow).transform.SetParent(root.transform);

        Light status = PointLight(systemName + " Terminal Beacon", basePosition + new Vector3(0f, 2.1f, -0.35f), Color.red, 3f, 5f);
        status.transform.SetParent(root.transform);
        SetObject(task, "screenRenderer", screen.GetComponent<Renderer>());
        SetObject(task, "statusLight", status);

        WorldLabel("E  REPAIR\n" + systemName.ToUpper(), basePosition + new Vector3(0f, 2.45f, -0.25f), PlayerStart, Color.cyan, 0.014f).transform.SetParent(root.transform);
        PointLight(systemName + " Area Light", basePosition + new Vector3(0f, 2.7f, 0f), new Color(0.1f, 0.9f, 1f), 2.8f, 6f).transform.SetParent(root.transform);
    }

    private static void CreateDoor(string name, Vector3 position, bool requiresRepairs, Material wall, Material yellow, Material beaconMaterial)
    {
        GameObject frame = Cube(name + " Frame", position, new Vector3(3.6f, 3.2f, 0.35f), yellow);
        GameObject panel = Cube(name + " Sliding Panel", position, new Vector3(2.35f, 2.65f, 0.5f), wall);
        panel.transform.SetParent(frame.transform);
        Cube(name + " Status Strip", position + new Vector3(0f, 1.45f, -0.31f), new Vector3(2.2f, 0.18f, 0.05f), beaconMaterial).transform.SetParent(frame.transform);

        DoorController door = frame.AddComponent<DoorController>();
        SetObject(door, "doorPanel", panel.transform);
        SetBool(door, "requiresRepairs", requiresRepairs);
        WorldLabel(requiresRepairs ? "LOCKED UNTIL\n3 SYSTEMS ONLINE" : "E  OPEN DOOR", position + new Vector3(0f, 2.35f, -0.55f), PlayerStart, requiresRepairs ? Color.red : Color.cyan, 0.014f).transform.SetParent(frame.transform);
    }

    private static void CreateEscapePod(Vector3 position, Material pod, Material green, Material glass, Material yellow)
    {
        GameObject root = GameObject.CreatePrimitive(PrimitiveType.Capsule);
        root.name = "EscapePod";
        root.transform.position = position;
        root.transform.rotation = Quaternion.Euler(90f, 0f, 0f);
        root.transform.localScale = new Vector3(1.25f, 1.9f, 1.25f);
        root.GetComponent<Renderer>().material = pod;
        EscapePod escapePod = root.AddComponent<EscapePod>();

        Cube("EscapePod Window", position + new Vector3(0f, 0.35f, 1.25f), new Vector3(1.2f, 0.6f, 0.08f), glass).transform.SetParent(root.transform);
        Cube("EscapePod Launch Rail", position + new Vector3(0f, -0.8f, 0f), new Vector3(3.8f, 0.18f, 4f), yellow);
        Light beacon = PointLight("EscapePod Beacon", position + new Vector3(0f, 2f, 0f), Color.red, 4.6f, 8f);
        beacon.transform.SetParent(root.transform);
        SetObject(escapePod, "beacon", beacon);
        WorldLabel("ESCAPE POD\nRepair all systems, then press E", position + new Vector3(0f, 2.65f, 1.2f), PlayerStart, Color.green, 0.014f);
    }

    private static void CreateOxygenCanister(Vector3 position, Material cyan, Material yellow)
    {
        GameObject canister = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        canister.name = "Oxygen Canister";
        canister.transform.position = position;
        canister.transform.localScale = new Vector3(0.42f, 0.7f, 0.42f);
        canister.GetComponent<Renderer>().material = cyan;
        canister.AddComponent<OxygenCanister>();
        Cube("Oxygen Canister Cap", position + new Vector3(0f, 0.75f, 0f), new Vector3(0.75f, 0.12f, 0.75f), yellow).transform.SetParent(canister.transform);
        WorldLabel("E  O2 REFILL", position + new Vector3(0f, 1.65f, 0f), PlayerStart, Color.cyan, 0.012f).transform.SetParent(canister.transform);
        PointLight("Oxygen Canister Light", position + new Vector3(0f, 1.2f, 0f), new Color(0.05f, 1f, 1f), 2.2f, 4f).transform.SetParent(canister.transform);
    }

    private static void CreateHUD()
    {
        GameObject canvasObject = new GameObject("HUD Canvas");
        Canvas canvas = canvasObject.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvasObject.AddComponent<CanvasScaler>().uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        canvasObject.AddComponent<GraphicRaycaster>();

        Font font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        Text oxygen = TextUI(canvasObject.transform, "OxygenText", "O2 100%", new Vector2(24f, -24f), TextAnchor.UpperLeft, 24, font);
        Image oxygenFill = Bar(canvasObject.transform, "OxygenFill", new Vector2(24f, -60f), new Vector2(210f, 16f), new Color(0.05f, 0.95f, 0.8f));
        Text timer = TextUI(canvasObject.transform, "TimerText", "TIME 07:00", new Vector2(24f, -90f), TextAnchor.UpperLeft, 24, font);
        Text repairs = TextUI(canvasObject.transform, "RepairText", "SYSTEMS 0/3", new Vector2(24f, -122f), TextAnchor.UpperLeft, 24, font);
        Text objective = TextUI(canvasObject.transform, "ObjectiveText", "", new Vector2(24f, -162f), TextAnchor.UpperLeft, 20, font);
        Text crosshair = TextUI(canvasObject.transform, "Crosshair", "+", new Vector2(0f, 0f), TextAnchor.MiddleCenter, 30, font);
        crosshair.color = new Color(0.2f, 1f, 0.95f, 0.85f);
        Text prompt = TextUI(canvasObject.transform, "PromptText", "", new Vector2(0f, 84f), TextAnchor.LowerCenter, 28, font);
        Text message = TextUI(canvasObject.transform, "MessageText", "", new Vector2(0f, 150f), TextAnchor.LowerCenter, 26, font);

        GameObject panel = new GameObject("EndPanel");
        panel.transform.SetParent(canvasObject.transform, false);
        Image panelImage = panel.AddComponent<Image>();
        panelImage.color = new Color(0f, 0f, 0f, 0.82f);
        RectTransform panelRect = panel.GetComponent<RectTransform>();
        panelRect.anchorMin = Vector2.zero;
        panelRect.anchorMax = Vector2.one;
        panelRect.offsetMin = Vector2.zero;
        panelRect.offsetMax = Vector2.zero;

        Text endTitle = TextUI(panel.transform, "EndTitle", "", new Vector2(0f, 70f), TextAnchor.MiddleCenter, 40, font);
        Text endBody = TextUI(panel.transform, "EndBody", "", new Vector2(0f, -20f), TextAnchor.MiddleCenter, 23, font);
        panel.SetActive(false);

        HUDController hud = canvasObject.AddComponent<HUDController>();
        SetObject(hud, "oxygenText", oxygen);
        SetObject(hud, "oxygenFill", oxygenFill);
        SetObject(hud, "timerText", timer);
        SetObject(hud, "repairText", repairs);
        SetObject(hud, "objectiveText", objective);
        SetObject(hud, "promptText", prompt);
        SetObject(hud, "messageText", message);
        SetObject(hud, "endPanel", panel);
        SetObject(hud, "endTitleText", endTitle);
        SetObject(hud, "endBodyText", endBody);
    }

    private static GameObject WorldLabel(string text, Vector3 position, Vector3 lookAt, Color color, float scale)
    {
        GameObject canvasObject = new GameObject("World Label - " + text.Replace("\n", " "));
        canvasObject.transform.position = position;
        canvasObject.transform.LookAt(new Vector3(lookAt.x, position.y, lookAt.z));
        canvasObject.transform.Rotate(0f, 180f, 0f);
        canvasObject.transform.localScale = Vector3.one * scale;
        Canvas canvas = canvasObject.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.WorldSpace;

        Text label = TextUI(canvasObject.transform, "Text", text, Vector2.zero, TextAnchor.MiddleCenter, 42, Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf"));
        label.color = color;
        label.fontStyle = FontStyle.Bold;
        RectTransform rect = label.GetComponent<RectTransform>();
        rect.sizeDelta = new Vector2(520f, 160f);
        return canvasObject;
    }

    private static Text TextUI(Transform parent, string name, string value, Vector2 position, TextAnchor anchor, int size, Font font)
    {
        GameObject textObject = new GameObject(name);
        textObject.transform.SetParent(parent, false);
        Text text = textObject.AddComponent<Text>();
        text.text = value;
        text.font = font;
        text.fontSize = size;
        text.color = Color.white;
        text.alignment = anchor;

        RectTransform rect = text.GetComponent<RectTransform>();
        rect.sizeDelta = new Vector2(960f, 90f);
        rect.anchoredPosition = position;

        if (anchor == TextAnchor.UpperLeft)
        {
            rect.anchorMin = new Vector2(0f, 1f);
            rect.anchorMax = new Vector2(0f, 1f);
            rect.pivot = new Vector2(0f, 1f);
        }
        else if (anchor == TextAnchor.LowerCenter)
        {
            rect.anchorMin = new Vector2(0.5f, 0f);
            rect.anchorMax = new Vector2(0.5f, 0f);
            rect.pivot = new Vector2(0.5f, 0f);
        }
        else
        {
            rect.anchorMin = new Vector2(0.5f, 0.5f);
            rect.anchorMax = new Vector2(0.5f, 0.5f);
            rect.pivot = new Vector2(0.5f, 0.5f);
        }

        return text;
    }

    private static Image Bar(Transform parent, string name, Vector2 position, Vector2 size, Color color)
    {
        GameObject barObject = new GameObject(name);
        barObject.transform.SetParent(parent, false);
        Image image = barObject.AddComponent<Image>();
        image.color = color;
        image.type = Image.Type.Filled;
        image.fillMethod = Image.FillMethod.Horizontal;
        RectTransform rect = image.GetComponent<RectTransform>();
        rect.anchorMin = new Vector2(0f, 1f);
        rect.anchorMax = new Vector2(0f, 1f);
        rect.pivot = new Vector2(0f, 1f);
        rect.anchoredPosition = position;
        rect.sizeDelta = size;
        return image;
    }

    private static void CreateMissionBoard(Material wall, Material cyan, Material yellow)
    {
        Cube("Mission Board Backplate", new Vector3(0f, 1.55f, 11.8f), new Vector3(5.5f, 2f, 0.18f), wall);
        Cube("Mission Board Header", new Vector3(0f, 2.45f, 11.68f), new Vector3(5.2f, 0.18f, 0.08f), yellow);
        Cube("Mission Board Glow", new Vector3(0f, 1.45f, 11.62f), new Vector3(5f, 1.35f, 0.06f), cyan);
    }

    private static void Wall(string name, Vector3 position, Vector3 scale, Material material, Material trim)
    {
        Cube(name, position, scale, material);
        Cube(name + " Upper Trim", position + new Vector3(0f, 1.35f, -Mathf.Sign(position.z) * 0.03f), new Vector3(Mathf.Max(0.15f, scale.x), 0.12f, Mathf.Max(0.08f, scale.z)), trim);
        Cube(name + " Lower Trim", position + new Vector3(0f, -1.25f, -Mathf.Sign(position.z) * 0.03f), new Vector3(Mathf.Max(0.15f, scale.x), 0.12f, Mathf.Max(0.08f, scale.z)), trim);
    }

    private static void CeilingLight(Vector3 position, Material material)
    {
        Cube("Ceiling Light Bar", position, new Vector3(2f, 0.08f, 0.22f), material);
    }

    private static Light PointLight(string name, Vector3 position, Color color, float intensity, float range)
    {
        GameObject lightObject = new GameObject(name);
        lightObject.transform.position = position;
        Light light = lightObject.AddComponent<Light>();
        light.type = LightType.Point;
        light.color = color;
        light.intensity = intensity;
        light.range = range;
        return light;
    }

    private static GameObject Cylinder(string name, Vector3 position, Vector3 scale, Quaternion rotation, Material material)
    {
        GameObject cylinder = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        cylinder.name = name;
        cylinder.transform.position = position;
        cylinder.transform.rotation = rotation;
        cylinder.transform.localScale = scale;
        cylinder.GetComponent<Renderer>().material = material;
        return cylinder;
    }

    private static GameObject Cube(string name, Vector3 position, Vector3 scale, Material material)
    {
        GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
        cube.name = name;
        cube.transform.position = position;
        cube.transform.localScale = scale;
        cube.GetComponent<Renderer>().material = material;
        return cube;
    }

    private static void SetObject(Object target, string propertyName, Object value)
    {
        SerializedObject serialized = new SerializedObject(target);
        serialized.FindProperty(propertyName).objectReferenceValue = value;
        serialized.ApplyModifiedPropertiesWithoutUndo();
    }

    private static void SetString(Object target, string propertyName, string value)
    {
        SerializedObject serialized = new SerializedObject(target);
        serialized.FindProperty(propertyName).stringValue = value;
        serialized.ApplyModifiedPropertiesWithoutUndo();
    }

    private static void SetBool(Object target, string propertyName, bool value)
    {
        SerializedObject serialized = new SerializedObject(target);
        serialized.FindProperty(propertyName).boolValue = value;
        serialized.ApplyModifiedPropertiesWithoutUndo();
    }

    private static void SetFloat(Object target, string propertyName, float value)
    {
        SerializedObject serialized = new SerializedObject(target);
        serialized.FindProperty(propertyName).floatValue = value;
        serialized.ApplyModifiedPropertiesWithoutUndo();
    }
}

using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public static class DeepSpaceStationSceneBuilder
{
    [MenuItem("Deep Space Station/Build Playable Scene")]
    public static void BuildPlayableScene()
    {
        EnsureFolders();

        Scene scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
        scene.name = "Station_A";

        Material floor = CreateMaterial("M_Floor_DarkSteel", new Color(0.12f, 0.14f, 0.16f), 0f);
        Material wall = CreateMaterial("M_Wall_BlueGrey", new Color(0.18f, 0.24f, 0.28f), 0f);
        Material trim = CreateMaterial("M_Trim_Yellow", new Color(0.95f, 0.74f, 0.18f), 0f);
        Material terminal = CreateMaterial("M_Terminal_Black", new Color(0.02f, 0.025f, 0.03f), 0f);
        Material danger = CreateMaterial("M_Danger_Red", new Color(1f, 0.12f, 0.06f, 0.35f), 0.35f);
        Material pod = CreateMaterial("M_EscapePod_White", new Color(0.78f, 0.9f, 0.92f), 0f);

        CreateLighting();
        CreateStationGeometry(floor, wall, trim, danger);

        GameObject gameManager = new GameObject("GameManager");
        gameManager.AddComponent<GameManager>();

        Camera playerCamera;
        GameObject player = CreatePlayer(out playerCamera);
        CreateHUD();

        CreateTerminal("Life Support Terminal", new Vector3(-7f, 1.1f, 8f), "Life Support", terminal);
        CreateTerminal("Navigation Terminal", new Vector3(8f, 1.1f, 1f), "Navigation", terminal);
        CreateTerminal("Reactor Terminal", new Vector3(-8f, 1.1f, -8f), "Reactor", terminal);

        CreateDoor("Forward Security Door", new Vector3(0f, 1.6f, 5.5f), false, wall, trim);
        CreateDoor("Escape Pod Blast Door", new Vector3(0f, 1.6f, -12.5f), true, wall, trim);
        CreateEscapePod(new Vector3(0f, 1f, -17f), pod);

        CreateOxygenCanister(new Vector3(6f, 0.7f, 8f), trim);
        CreateOxygenCanister(new Vector3(-6f, 0.7f, -2f), trim);

        Selection.activeGameObject = player;
        EditorSceneManager.SaveScene(scene, "Assets/Scenes/Station_A.unity");
        AssetDatabase.SaveAssets();
        EditorUtility.DisplayDialog("Deep Space Station", "Playable scene created at Assets/Scenes/Station_A.unity", "OK");
    }

    private static void EnsureFolders()
    {
        CreateFolder("Assets", "Scenes");
        CreateFolder("Assets", "Materials");
        CreateFolder("Assets", "Prefabs");
    }

    private static void CreateFolder(string parent, string folder)
    {
        if (!AssetDatabase.IsValidFolder(parent + "/" + folder))
        {
            AssetDatabase.CreateFolder(parent, folder);
        }
    }

    private static Material CreateMaterial(string name, Color color, float alpha)
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
        material.SetColor("_EmissionColor", color * 0.25f);

        if (alpha > 0f)
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

    private static void CreateLighting()
    {
        RenderSettings.ambientLight = new Color(0.08f, 0.12f, 0.16f);

        GameObject sun = new GameObject("Dim Emergency Sun");
        Light sunLight = sun.AddComponent<Light>();
        sunLight.type = LightType.Directional;
        sunLight.intensity = 0.45f;
        sun.transform.rotation = Quaternion.Euler(50f, -25f, 0f);

        CreatePointLight("Blue Corridor Light", new Vector3(0f, 3.2f, 0f), new Color(0.25f, 0.75f, 1f), 4f, 18f);
        CreatePointLight("Red Emergency Light", new Vector3(-6f, 2.8f, -7f), new Color(1f, 0.14f, 0.06f), 3f, 12f);
        CreatePointLight("Pod Bay Beacon", new Vector3(0f, 3.2f, -16f), new Color(0.1f, 1f, 0.55f), 3.5f, 12f);
    }

    private static Light CreatePointLight(string name, Vector3 position, Color color, float intensity, float range)
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

    private static void CreateStationGeometry(Material floor, Material wall, Material trim, Material danger)
    {
        CreateCube("Main Deck", new Vector3(0f, -0.1f, 0f), new Vector3(18f, 0.2f, 24f), floor);
        CreateCube("Pod Bay Floor", new Vector3(0f, -0.08f, -16f), new Vector3(8f, 0.2f, 8f), floor);

        CreateCube("North Wall", new Vector3(0f, 1.6f, 12f), new Vector3(18f, 3.2f, 0.4f), wall);
        CreateCube("South Wall Left", new Vector3(-6f, 1.6f, -12f), new Vector3(6f, 3.2f, 0.4f), wall);
        CreateCube("South Wall Right", new Vector3(6f, 1.6f, -12f), new Vector3(6f, 3.2f, 0.4f), wall);
        CreateCube("West Wall", new Vector3(-9f, 1.6f, 0f), new Vector3(0.4f, 3.2f, 24f), wall);
        CreateCube("East Wall", new Vector3(9f, 1.6f, 0f), new Vector3(0.4f, 3.2f, 24f), wall);

        CreateCube("Pod Bay Back Wall", new Vector3(0f, 1.6f, -20f), new Vector3(8f, 3.2f, 0.4f), wall);
        CreateCube("Pod Bay West Wall", new Vector3(-4f, 1.6f, -16f), new Vector3(0.4f, 3.2f, 8f), wall);
        CreateCube("Pod Bay East Wall", new Vector3(4f, 1.6f, -16f), new Vector3(0.4f, 3.2f, 8f), wall);

        CreateCube("Central Reactor Block", new Vector3(0f, 0.8f, -2f), new Vector3(3f, 1.6f, 3f), trim);
        CreateCube("Broken Cargo A", new Vector3(5.5f, 0.6f, -5f), new Vector3(2f, 1.2f, 2.2f), wall);
        CreateCube("Broken Cargo B", new Vector3(-5.2f, 0.5f, 4.5f), new Vector3(2.3f, 1f, 1.8f), wall);

        GameObject hazard = CreateCube("Radiation Leak Trigger", new Vector3(4f, 0.05f, -8f), new Vector3(4f, 0.1f, 4f), danger);
        Collider hazardCollider = hazard.GetComponent<Collider>();
        hazardCollider.isTrigger = true;
        hazard.AddComponent<HazardZone>();
    }

    private static GameObject CreatePlayer(out Camera playerCamera)
    {
        GameObject player = GameObject.CreatePrimitive(PrimitiveType.Capsule);
        player.name = "Player";
        player.transform.position = new Vector3(0f, 1.1f, 9f);
        Object.DestroyImmediate(player.GetComponent<CapsuleCollider>());

        CharacterController controller = player.AddComponent<CharacterController>();
        controller.height = 1.8f;
        controller.radius = 0.35f;
        controller.center = new Vector3(0f, 0.8f, 0f);

        GameObject cameraObject = new GameObject("Main Camera");
        cameraObject.tag = "MainCamera";
        cameraObject.transform.SetParent(player.transform);
        cameraObject.transform.localPosition = new Vector3(0f, 1.45f, 0f);
        playerCamera = cameraObject.AddComponent<Camera>();
        playerCamera.fieldOfView = 72f;
        cameraObject.AddComponent<AudioListener>();

        FirstPersonController controllerScript = player.AddComponent<FirstPersonController>();
        GameObjectInteractor interactor = player.AddComponent<GameObjectInteractor>();
        player.AddComponent<PlayerOxygen>();

        SetObject(controllerScript, "playerCamera", playerCamera);
        SetObject(interactor, "viewCamera", playerCamera);

        return player;
    }

    private static void CreateHUD()
    {
        GameObject canvasObject = new GameObject("HUD Canvas");
        Canvas canvas = canvasObject.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvasObject.AddComponent<CanvasScaler>().uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        canvasObject.AddComponent<GraphicRaycaster>();

        Font font = Resources.GetBuiltinResource<Font>("Arial.ttf");
        Text oxygen = CreateText(canvasObject.transform, "OxygenText", "O2 100%", new Vector2(24f, -24f), TextAnchor.UpperLeft, 24, font);
        Image oxygenFill = CreateBar(canvasObject.transform, "OxygenFill", new Vector2(24f, -60f), new Vector2(190f, 14f), new Color(0.05f, 0.95f, 0.8f));
        Text timer = CreateText(canvasObject.transform, "TimerText", "TIME 07:00", new Vector2(24f, -84f), TextAnchor.UpperLeft, 24, font);
        Text repairs = CreateText(canvasObject.transform, "RepairText", "SYSTEMS 0/3", new Vector2(24f, -116f), TextAnchor.UpperLeft, 24, font);
        Text objective = CreateText(canvasObject.transform, "ObjectiveText", "", new Vector2(24f, -152f), TextAnchor.UpperLeft, 20, font);
        Text prompt = CreateText(canvasObject.transform, "PromptText", "", new Vector2(0f, 84f), TextAnchor.LowerCenter, 24, font);
        Text message = CreateText(canvasObject.transform, "MessageText", "", new Vector2(0f, 150f), TextAnchor.LowerCenter, 26, font);

        GameObject panel = new GameObject("EndPanel");
        panel.transform.SetParent(canvasObject.transform, false);
        Image panelImage = panel.AddComponent<Image>();
        panelImage.color = new Color(0f, 0f, 0f, 0.82f);
        RectTransform panelRect = panel.GetComponent<RectTransform>();
        panelRect.anchorMin = Vector2.zero;
        panelRect.anchorMax = Vector2.one;
        panelRect.offsetMin = Vector2.zero;
        panelRect.offsetMax = Vector2.zero;

        Text endTitle = CreateText(panel.transform, "EndTitle", "", new Vector2(0f, 70f), TextAnchor.MiddleCenter, 38, font);
        Text endBody = CreateText(panel.transform, "EndBody", "", new Vector2(0f, -20f), TextAnchor.MiddleCenter, 22, font);
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

    private static Text CreateText(Transform parent, string name, string value, Vector2 anchoredPosition, TextAnchor anchor, int size, Font font)
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
        rect.sizeDelta = new Vector2(900f, 80f);
        rect.anchoredPosition = anchoredPosition;

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

    private static Image CreateBar(Transform parent, string name, Vector2 anchoredPosition, Vector2 size, Color color)
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
        rect.anchoredPosition = anchoredPosition;
        rect.sizeDelta = size;
        return image;
    }

    private static void CreateTerminal(string name, Vector3 position, string systemName, Material material)
    {
        GameObject terminal = CreateCube(name, position, new Vector3(1.4f, 1.8f, 0.7f), material);
        TerminalTask task = terminal.AddComponent<TerminalTask>();
        SetString(task, "systemName", systemName);

        GameObject screen = CreateCube(name + " Screen", position + new Vector3(0f, 0.25f, -0.38f), new Vector3(0.9f, 0.55f, 0.05f), material);
        screen.transform.SetParent(terminal.transform);
        Renderer screenRenderer = screen.GetComponent<Renderer>();
        Light light = CreatePointLight(name + " Status Light", position + new Vector3(0f, 1.25f, -0.3f), Color.red, 2f, 4f);
        light.transform.SetParent(terminal.transform);

        SetObject(task, "screenRenderer", screenRenderer);
        SetObject(task, "statusLight", light);
    }

    private static void CreateDoor(string name, Vector3 position, bool requiresRepairs, Material wall, Material trim)
    {
        GameObject frame = CreateCube(name + " Frame", position, new Vector3(3.2f, 3.2f, 0.35f), trim);
        GameObject panel = CreateCube(name + " Panel", position, new Vector3(2.2f, 2.8f, 0.45f), wall);
        panel.transform.SetParent(frame.transform);

        DoorController door = frame.AddComponent<DoorController>();
        SetObject(door, "doorPanel", panel.transform);
        SetBool(door, "requiresRepairs", requiresRepairs);
    }

    private static void CreateEscapePod(Vector3 position, Material material)
    {
        GameObject podBody = CreateCube("EscapePod", position, new Vector3(2.6f, 1.8f, 3.4f), material);
        EscapePod pod = podBody.AddComponent<EscapePod>();
        Light beacon = CreatePointLight("EscapePod Beacon", position + new Vector3(0f, 1.5f, 0f), Color.red, 3.5f, 6f);
        beacon.transform.SetParent(podBody.transform);
        SetObject(pod, "beacon", beacon);
    }

    private static void CreateOxygenCanister(Vector3 position, Material material)
    {
        GameObject canister = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        canister.name = "Oxygen Canister";
        canister.transform.position = position;
        canister.transform.localScale = new Vector3(0.45f, 0.7f, 0.45f);
        canister.GetComponent<Renderer>().material = material;
        canister.AddComponent<OxygenCanister>();
    }

    private static GameObject CreateCube(string name, Vector3 position, Vector3 scale, Material material)
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
}

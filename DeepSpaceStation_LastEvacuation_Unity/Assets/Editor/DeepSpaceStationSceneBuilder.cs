using System.IO;
using TMPro;
using UnityEditor;
using UnityEditor.Events;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UPMInfo = UnityEditor.PackageManager.PackageInfo;

public static class DeepSpaceStationSceneBuilder
{
    private static readonly Vector3 PlayerStart = new Vector3(0f, 1.1f, 18f);
    private static readonly Vector3 EndlessStart = new Vector3(70f, 1.1f, 12f);
    private static Vector3 labelLookAt = PlayerStart;
    private static TMP_FontAsset cachedSciFiFont;
    private const float StationCeilingY = 4.32f;
    private const float StationWallHeight = 4.2f;
    private const float SideDoorWidth = 3.8f;
    private static readonly bool UseFullImportedCorridorShells = false;
    private const float ImportedModuleY = 1f;
    private static readonly bool UseImportedEnvironmentVisuals = true;
    private const string ImportedSkyboxPath = "Assets/Free Skyboxes - Space/SBS Space 1/Large/Skybox_Space 1 Large.mat";
    private const string ModularCorridorPath = "Assets/Sci Fi Modular Pack/Prefabs/Starter/Profile1.prefab";
    private const string ModularDoorProfilePath = "Assets/Sci Fi Modular Pack/Prefabs/Starter/ProfileDoor.prefab";
    private const string ModularDoorPath = "Assets/Sci Fi Modular Pack/Prefabs/Door.prefab";
    private const string ModularDoorFramePath = "Assets/Sci Fi Modular Pack/Prefabs/DoorFrame.prefab";
    private const string ModularWindowPath = "Assets/Sci Fi Modular Pack/Prefabs/ProfileWindow.prefab";
    private const string ModularProfileEndPath = "Assets/Sci Fi Modular Pack/Prefabs/ProfileEnd.prefab";
    private const string ModularWallDoorPath = "Assets/Sci Fi Modular Pack/Prefabs/WallDoor.prefab";
    private const string ModularLight1Path = "Assets/Sci Fi Modular Pack/Prefabs/Light1.prefab";
    private const string ModularLight2Path = "Assets/Sci Fi Modular Pack/Prefabs/Light2.prefab";
    private const string ModularBoxSmallPath = "Assets/Sci Fi Modular Pack/Prefabs/Box1.prefab";
    private const string ModularBoxLargePath = "Assets/Sci Fi Modular Pack/Prefabs/Box3.prefab";
    private const string ModularFloorMaterialPath = "Assets/Sci Fi Modular Pack/Materials/Floor1.mat";
    private const string ModularWallMaterialPath = "Assets/Sci Fi Modular Pack/Materials/Wall1.mat";
    private const string ModularDarkMaterialPath = "Assets/Sci Fi Modular Pack/Materials/Bar.mat";
    private const string ModularDoorMaterialPath = "Assets/Sci Fi Modular Pack/Materials/Door.mat";
    private const string CosmicComputerPath = "Assets/Cosmic_Retro_Station_Props_FREE/Prefabs/CR_Computer_PanelOnly.prefab";
    private const string CosmicMonitorPath = "Assets/Cosmic_Retro_Station_Props_FREE/Prefabs/CR_Monitor_Small_1.prefab";
    private const string CosmicCratePath = "Assets/Cosmic_Retro_Station_Props_FREE/Prefabs/CR_StorageCrate_Large_1.prefab";
    private const string CosmicLockerPath = "Assets/Cosmic_Retro_Station_Props_FREE/Prefabs/CR_Locker_Emergency_Red.prefab";
    private const string CosmicTablePath = "Assets/Cosmic_Retro_Station_Props_FREE/Prefabs/CR_Table_Small_2.prefab";
    private const string CosmicChairPath = "Assets/Cosmic_Retro_Station_Props_FREE/Prefabs/CR_Chair_Medium.prefab";
    private const string CosmicControlPanelPath = "Assets/Cosmic_Retro_Station_Props_FREE/Prefabs/CR_WallPanel_Control_2.prefab";
    private const string MainMenuCoverPath = "Assets/UI/Menu/main-menu-cover.png";
    private const string SciFiFontPath = "Assets/Fonts/Teko-Bold.ttf";
    private const string SciFiFontAssetPath = "Assets/Fonts/Teko-Bold SDF.asset";
    private const string TmpSettingsPath = "Assets/TextMesh Pro/Resources/TMP Settings.asset";
    private const string SystemsFolderName = "00_SYSTEMS";
    private const string UiFolderName = "01_USER_INTERFACE";
    private const string LightingFolderName = "02_LIGHTING";
    private const string TimedFolderName = "03_TIMED_EVACUATION";
    private const string EndlessFolderName = "04_ENDLESS_SURVIVAL";
    private const string UnsortedFolderName = "99_UNSORTED";

    [MenuItem("Deep Space Station/Build Playable Scene")]
    public static void BuildPlayableScene()
    {
        if (EditorApplication.isPlayingOrWillChangePlaymode)
        {
            EditorUtility.DisplayDialog("Deep Space Station", "Please stop Play Mode before rebuilding the scene.", "OK");
            return;
        }

        EnsureFolder("Assets", "Scenes");
        EnsureFolder("Assets", "Materials");
        EnsureFolder("Assets", "Prefabs");
        TMP_FontAsset font = EnsureSciFiTypography();

        Scene scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);

        Material deck = Mat("M_Deck_Ribbed", new Color(0.72f, 0.79f, 0.84f), new Color(0.08f, 0.12f, 0.15f));
        Material wall = Mat("M_Wall_ColdSteel", new Color(0.84f, 0.9f, 0.94f), new Color(0.08f, 0.12f, 0.16f));
        Material dark = Mat("M_Dark_Machinery", new Color(0.055f, 0.07f, 0.085f), new Color(0.004f, 0.006f, 0.01f));
        Material yellow = Mat("M_Warning_Yellow", new Color(1f, 0.72f, 0.12f), new Color(0.7f, 0.42f, 0.03f));
        Material cyan = Mat("M_Cyan_Emissive", new Color(0.02f, 0.43f, 1f), new Color(0.02f, 1.15f, 2.25f));
        Material green = Mat("M_Green_Emissive", new Color(0.06f, 1f, 0.45f), new Color(0.04f, 1.2f, 0.45f));
        Material red = Mat("M_Red_Emissive", new Color(1f, 0.1f, 0.04f), new Color(1.4f, 0.06f, 0.02f));
        Material orange = Mat("M_Orange_Emissive", new Color(1f, 0.38f, 0.05f), new Color(1.2f, 0.22f, 0.01f));
        Material glass = Mat("M_Glass_Transparent", new Color(0.52f, 0.86f, 1f, 0.34f), new Color(0.09f, 0.55f, 0.95f), true);
        Material hazard = Mat("M_Radiation_Transparent", new Color(1f, 0.05f, 0f, 0.38f), new Color(1.2f, 0f, 0f), true);
        Material pod = Mat("M_EscapePod_Ceramic", new Color(0.92f, 0.97f, 0.98f), new Color(0.08f, 0.12f, 0.14f));
        Material suit = Mat("M_Astronaut_Suit", new Color(0.92f, 0.95f, 0.94f), new Color(0.08f, 0.09f, 0.1f));
        Material whiteGlow = Mat("M_White_Emission", new Color(0.96f, 0.99f, 1f), new Color(1.25f, 1.55f, 1.9f));

        if (UseImportedEnvironmentVisuals)
        {
            deck = ImportedMaterial(ModularFloorMaterialPath, deck);
            wall = ImportedMaterial(ModularWallMaterialPath, wall);
            dark = ImportedMaterial(ModularDarkMaterialPath, dark);
            pod = ImportedMaterial(ModularDoorMaterialPath, pod);
        }

        CreateLighting(cyan, red, green);
        ApplyImportedSkybox();
        CreateStation(deck, wall, dark, yellow, cyan, red, orange, glass, hazard, whiteGlow);
        CreateImportedAssetDressing();

        new GameObject("GameManager").AddComponent<GameManager>();
        GameObject player = CreatePlayer(suit, glass, cyan, dark, yellow);
        CreateSpawnMarker("Timed Evacuation Spawn", PlayerStart, Quaternion.identity);
        CreateHUD(font);

        CreateMissionBoard(wall, cyan, yellow);

        CreateTerminal(new Vector3(0f, 0f, 5.8f), "Corridor Relay", 3.2f, cyan, wall, green, yellow, KeyCode.Alpha1, KeyCode.Alpha3, KeyCode.Alpha2);
        CreateTerminal(new Vector3(-11.4f, 0f, 0f), "Life Support", 3.8f, cyan, wall, green, yellow, KeyCode.Alpha2, KeyCode.Alpha1, KeyCode.Alpha3);
        CreateTerminal(new Vector3(11.5f, 0f, -4.4f), "Navigation", 4.1f, cyan, wall, green, yellow, KeyCode.Alpha4, KeyCode.Alpha2, KeyCode.Alpha1);
        CreateTerminal(new Vector3(-11.8f, 0f, -13.8f), "Medical Air Mix", 4.2f, cyan, wall, green, yellow, KeyCode.Alpha2, KeyCode.Alpha4, KeyCode.Alpha3);
        CreateTerminal(new Vector3(11.8f, 0f, -17.5f), "Security Override", 4.5f, cyan, wall, green, yellow, KeyCode.Alpha3, KeyCode.Alpha2, KeyCode.Alpha4);
        CreateTerminal(new Vector3(-11.7f, 0f, -28.2f), "Reactor", 5.4f, cyan, wall, green, yellow, KeyCode.Alpha3, KeyCode.Alpha1, KeyCode.Alpha4, KeyCode.Alpha2);
        CreateTerminal(new Vector3(11.5f, 0f, -31.6f), "Comms Relay", 5f, cyan, wall, green, yellow, KeyCode.Alpha1, KeyCode.Alpha4, KeyCode.Alpha2, KeyCode.Alpha3);

        CreateDoor("Pod Bay Exit", new Vector3(0f, 1.55f, 12.4f), Quaternion.identity, false, 0, wall, cyan, cyan, "", "", false, 5.25f);
        CreateDoor("Life Support Bulkhead", new Vector3(-4.2f, 1.55f, 0f), Quaternion.Euler(0f, 90f, 0f), true, 1, wall, cyan, cyan, doorWidth: SideDoorWidth);
        CreateDoor("Navigation Bulkhead", new Vector3(4.2f, 1.55f, -4.4f), Quaternion.Euler(0f, 90f, 0f), true, 1, wall, cyan, cyan, doorWidth: SideDoorWidth);
        CreateDoor("Medical Bay Bulkhead", new Vector3(-4.2f, 1.55f, -13.8f), Quaternion.Euler(0f, 90f, 0f), true, 2, wall, cyan, green, doorWidth: SideDoorWidth);
        CreateDoor("Security Office Bulkhead", new Vector3(4.2f, 1.55f, -17.5f), Quaternion.Euler(0f, 90f, 0f), true, 3, wall, cyan, orange, "security_keycard", "Security Keycard", false, SideDoorWidth);
        CreateDoor("Reactor Fuse Lock", new Vector3(-4.2f, 1.55f, -28.2f), Quaternion.Euler(0f, 90f, 0f), true, 4, wall, cyan, red, "reactor_fuse", "Reactor Fuse", true, SideDoorWidth);
        CreateDoor("Comms Decoder Lock", new Vector3(4.2f, 1.55f, -31.6f), Quaternion.Euler(0f, 90f, 0f), true, 5, wall, cyan, red, "comms_decoder", "Comms Decoder", false, SideDoorWidth);
        CreateDoor("Escape Lock Door", new Vector3(0f, 1.55f, -39.5f), Quaternion.identity, true, 7, wall, cyan, red);
        CreateEscapePod(new Vector3(0f, 1f, -44.2f), pod, green, glass, yellow);

        CreateResourcePickup(new Vector3(-11.8f, 0.7f, 3.3f), "security_keycard", "Security Keycard", cyan, yellow);
        CreateResourcePickup(new Vector3(-11.6f, 0.7f, -10.6f), "reactor_fuse", "Reactor Fuse", orange, yellow);
        CreateResourcePickup(new Vector3(11.5f, 0.7f, -20.8f), "comms_decoder", "Comms Decoder", green, yellow);

        CreateOxygenCanister(new Vector3(1.8f, 0.75f, 16.8f), cyan, yellow);
        CreateOxygenCanister(new Vector3(-11.5f, 0.75f, -3.2f), cyan, yellow);
        CreateOxygenCanister(new Vector3(11.3f, 0.75f, -10.1f), cyan, yellow);
        CreateOxygenCanister(new Vector3(-2.1f, 0.75f, -24.5f), cyan, yellow);
        CreateOxygenCanister(new Vector3(2.3f, 0.75f, -37.2f), cyan, yellow);

        CreateConsumablePickup(new Vector3(-13.6f, 0.65f, -13.1f), "medkit", "Medkit", red, yellow);
        CreateConsumablePickup(new Vector3(13.8f, 0.65f, -3.1f), "battery", "Battery Pack", cyan, yellow);
        CreateConsumablePickup(new Vector3(13.6f, 0.65f, -31f), "battery", "Battery Pack", cyan, yellow);
        CreateConsumablePickup(new Vector3(-13.7f, 0.65f, -27.1f), "medkit", "Medkit", red, yellow);

        CreateOptionalSupplyCache("Security Risk Supply Cache", new Vector3(15.2f, 0f, -20.6f), dark, yellow, cyan, red, false, true, true);
        CreateOptionalSupplyCache("Reactor Risk Supply Cache", new Vector3(-15.2f, 0f, -31.4f), dark, yellow, cyan, red, true, true, false);

        CreateSecurityRobot(new Vector3(0f, 0.75f, -18f), dark, red, cyan, new Vector3(0f, 0.75f, -8f), new Vector3(0f, 0.75f, -27f));
        CreateSecurityRobot(new Vector3(10.6f, 0.75f, -17.5f), dark, red, orange, new Vector3(7.3f, 0.75f, -17.5f), new Vector3(14.2f, 0.75f, -17.5f));

        CreateEndlessSurvivalArena(deck, wall, dark, yellow, cyan, green, red, orange, glass, hazard, whiteGlow);
        OrganizeSceneHierarchy(scene);

        Selection.activeGameObject = player;
        EditorSceneManager.SaveScene(scene, "Assets/Scenes/Station_A.unity");
        AssetDatabase.SaveAssets();
        if (Application.isBatchMode)
        {
            Debug.Log("Upgraded playable scene created at Assets/Scenes/Station_A.unity");
        }
        else
        {
            EditorUtility.DisplayDialog("Deep Space Station", "Upgraded playable scene created at Assets/Scenes/Station_A.unity", "OK");
        }
    }

    [MenuItem("Deep Space Station/Apply TMP + HUD Visual Polish")]
    public static void UpgradeCurrentSceneTypography()
    {
        if (EditorApplication.isPlayingOrWillChangePlaymode)
        {
            return;
        }

        Scene scene = SceneManager.GetActiveScene();
        if (!scene.IsValid() || scene.path != "Assets/Scenes/Station_A.unity")
        {
            Debug.LogWarning("Typography upgrade is only available for Assets/Scenes/Station_A.unity.");
            return;
        }

        TMP_FontAsset font = EnsureSciFiTypography();
        Text[] legacyTexts = Object.FindObjectsOfType<Text>(true);
        foreach (Text legacyText in legacyTexts)
        {
            ConvertLegacyText(legacyText, font);
        }

        ApplySceneTypographyPolish(font);
        int organizedObjects = OrganizeSceneHierarchy(scene);

        EditorSceneManager.MarkSceneDirty(scene);
        EditorSceneManager.SaveScene(scene);
        AssetDatabase.SaveAssets();
        Debug.Log("Applied TMP sci-fi HUD polish in " + scene.path + "; converted " + legacyTexts.Length + " legacy text components; organized " + organizedObjects + " hierarchy objects.");
    }

    [MenuItem("Deep Space Station/Organize Current Scene Hierarchy")]
    public static void OrganizeCurrentSceneHierarchy()
    {
        if (EditorApplication.isPlayingOrWillChangePlaymode)
        {
            EditorUtility.DisplayDialog("Deep Space Station", "Please stop Play Mode before organizing the scene hierarchy.", "OK");
            return;
        }

        Scene scene = SceneManager.GetActiveScene();
        if (!scene.IsValid() || scene.path != "Assets/Scenes/Station_A.unity")
        {
            EditorUtility.DisplayDialog("Deep Space Station", "Open Assets/Scenes/Station_A.unity before organizing its hierarchy.", "OK");
            return;
        }

        int organizedObjects = OrganizeSceneHierarchy(scene);
        EditorSceneManager.MarkSceneDirty(scene);
        EditorSceneManager.SaveScene(scene);
        AssetDatabase.SaveAssets();
        Debug.Log("Organized " + organizedObjects + " root objects into a production-friendly hierarchy in " + scene.path + ".");
    }

    // Organizes generated scene roots without changing world-space layout or gameplay references.
    private static int OrganizeSceneHierarchy(Scene scene)
    {
        Transform systems = EnsureSceneHierarchyFolder(scene, SystemsFolderName, 0);
        Transform ui = EnsureSceneHierarchyFolder(scene, UiFolderName, 1);
        Transform lighting = EnsureSceneHierarchyFolder(scene, LightingFolderName, 2);
        Transform timed = EnsureSceneHierarchyFolder(scene, TimedFolderName, 3);
        Transform endless = EnsureSceneHierarchyFolder(scene, EndlessFolderName, 4);

        EnsureHierarchyFolder(systems, "00_MANAGERS");
        EnsureHierarchyFolder(systems, "01_PLAYER");
        EnsureHierarchyFolder(systems, "02_SPAWN_POINTS");
        EnsureHierarchyFolder(lighting, "00_GLOBAL");
        EnsureHierarchyFolder(lighting, "01_TIMED_EVACUATION");
        EnsureHierarchyFolder(lighting, "02_ENDLESS_SURVIVAL");
        PrepareModeHierarchy(timed);
        PrepareModeHierarchy(endless);

        GameObject legacyEndlessRoot = FindRootObject(scene, "Endless Survival Map");
        if (legacyEndlessRoot != null)
        {
            while (legacyEndlessRoot.transform.childCount > 0)
            {
                legacyEndlessRoot.transform.GetChild(0).SetParent(null, true);
            }

            Object.DestroyImmediate(legacyEndlessRoot);
        }

        int movedObjects = 0;
        foreach (GameObject rootObject in scene.GetRootGameObjects())
        {
            if (IsSceneHierarchyFolder(rootObject.name))
            {
                continue;
            }

            Transform destination = ResolveHierarchyDestination(rootObject, scene, systems, ui, lighting, timed, endless);
            if (destination != null && rootObject.transform.parent != destination)
            {
                rootObject.transform.SetParent(destination, true);
                movedObjects++;
            }
        }

        return movedObjects;
    }

    private static void PrepareModeHierarchy(Transform modeRoot)
    {
        EnsureHierarchyFolder(modeRoot, "00_ARCHITECTURE");
        EnsureHierarchyFolder(modeRoot, "01_INTERACTABLES/00_DOORS");
        EnsureHierarchyFolder(modeRoot, "01_INTERACTABLES/01_TERMINALS");
        EnsureHierarchyFolder(modeRoot, "01_INTERACTABLES/02_OBJECTIVES");
        EnsureHierarchyFolder(modeRoot, "02_PICKUPS");
        EnsureHierarchyFolder(modeRoot, "03_HAZARDS");
        EnsureHierarchyFolder(modeRoot, "04_ENEMIES");
        EnsureHierarchyFolder(modeRoot, "05_DRESSING");
        EnsureHierarchyFolder(modeRoot, "06_WORLD_LABELS");
    }

    private static Transform ResolveHierarchyDestination(GameObject target, Scene scene, Transform systems, Transform ui, Transform lighting, Transform timed, Transform endless)
    {
        string name = target.name;
        if (target.GetComponent<GameManager>() != null)
        {
            return EnsureHierarchyFolder(systems, "00_MANAGERS");
        }

        if (name == "Player" || target.CompareTag("Player"))
        {
            return EnsureHierarchyFolder(systems, "01_PLAYER");
        }

        if (target.GetComponent<HUDController>() != null || target.GetComponent<EventSystem>() != null)
        {
            return ui;
        }

        bool isEndless = IsEndlessObject(target);
        Transform modeRoot = isEndless ? endless : timed;

        if (name.IndexOf("Spawn", System.StringComparison.OrdinalIgnoreCase) >= 0)
        {
            return EnsureHierarchyFolder(systems, "02_SPAWN_POINTS");
        }

        if (target.GetComponent<Light>() != null || IsLightingVisual(name))
        {
            if (name == "Soft Orbital Directional Light")
            {
                return EnsureHierarchyFolder(lighting, "00_GLOBAL");
            }

            return EnsureHierarchyFolder(lighting, isEndless ? "02_ENDLESS_SURVIVAL" : "01_TIMED_EVACUATION");
        }

        if (name.StartsWith("World Label", System.StringComparison.Ordinal))
        {
            return EnsureHierarchyFolder(modeRoot, "06_WORLD_LABELS");
        }

        if (target.GetComponent<DoorController>() != null)
        {
            return EnsureHierarchyFolder(modeRoot, "01_INTERACTABLES/00_DOORS");
        }

        if (target.GetComponent<TerminalTask>() != null)
        {
            return EnsureHierarchyFolder(modeRoot, "01_INTERACTABLES/01_TERMINALS");
        }

        if (target.GetComponent<EscapePod>() != null || name.StartsWith("EscapePod", System.StringComparison.Ordinal))
        {
            return EnsureHierarchyFolder(modeRoot, "01_INTERACTABLES/02_OBJECTIVES");
        }

        if (target.GetComponent<ResourcePickup>() != null ||
            target.GetComponent<ConsumablePickup>() != null ||
            target.GetComponent<OxygenCanister>() != null)
        {
            return EnsureHierarchyFolder(modeRoot, "02_PICKUPS");
        }

        if (target.GetComponent<HazardZone>() != null)
        {
            return EnsureHierarchyFolder(modeRoot, "03_HAZARDS");
        }

        if (target.GetComponent<SecurityRobot>() != null || name.StartsWith("Patrol Point", System.StringComparison.Ordinal))
        {
            return EnsureHierarchyFolder(modeRoot, "04_ENEMIES");
        }

        if (IsArchitectureObject(name))
        {
            return EnsureHierarchyFolder(modeRoot, "00_ARCHITECTURE/" + ArchitectureZone(name, isEndless));
        }

        if (IsDressingObject(name) || isEndless)
        {
            return EnsureHierarchyFolder(modeRoot, "05_DRESSING/" + DressingZone(name, isEndless));
        }

        return EnsureSceneHierarchyFolder(scene, UnsortedFolderName, 99);
    }

    private static bool IsSceneHierarchyFolder(string name)
    {
        return name == SystemsFolderName ||
               name == UiFolderName ||
               name == LightingFolderName ||
               name == TimedFolderName ||
               name == EndlessFolderName ||
               name == UnsortedFolderName;
    }

    private static bool IsEndlessObject(GameObject target)
    {
        return target.name.StartsWith("Endless", System.StringComparison.Ordinal) || target.transform.position.x > 30f;
    }

    private static bool IsLightingVisual(string name)
    {
        return name.IndexOf("Ceiling Light Bar", System.StringComparison.Ordinal) >= 0 ||
               name.IndexOf("Soft Light Panel", System.StringComparison.Ordinal) >= 0 ||
               name.IndexOf("Interior Light Band", System.StringComparison.Ordinal) >= 0 ||
               name.IndexOf("Ring Light", System.StringComparison.Ordinal) >= 0;
    }

    private static bool IsArchitectureObject(string name)
    {
        return name.IndexOf("Deck", System.StringComparison.Ordinal) >= 0 ||
               name.IndexOf("Wall", System.StringComparison.Ordinal) >= 0 ||
               name.IndexOf("Hull", System.StringComparison.Ordinal) >= 0 ||
               name.IndexOf("Floor", System.StringComparison.Ordinal) >= 0 ||
               name.IndexOf("Ceiling", System.StringComparison.Ordinal) >= 0 ||
               name.IndexOf("Boundary", System.StringComparison.Ordinal) >= 0 ||
               name.IndexOf("Collision", System.StringComparison.Ordinal) >= 0;
    }

    private static bool IsDressingObject(string name)
    {
        return name.StartsWith("Imported", System.StringComparison.Ordinal) ||
               name.IndexOf("Mission Board", System.StringComparison.Ordinal) >= 0 ||
               name.IndexOf("Hologram", System.StringComparison.Ordinal) >= 0 ||
               name.IndexOf("Tank", System.StringComparison.Ordinal) >= 0 ||
               name.IndexOf("Star Map", System.StringComparison.Ordinal) >= 0 ||
               name.IndexOf("Cryo", System.StringComparison.Ordinal) >= 0 ||
               name.IndexOf("Locker", System.StringComparison.Ordinal) >= 0 ||
               name.IndexOf("Core", System.StringComparison.Ordinal) >= 0 ||
               name.IndexOf("Dish", System.StringComparison.Ordinal) >= 0 ||
               name.IndexOf("Tool Chest", System.StringComparison.Ordinal) >= 0 ||
               name.IndexOf("Window", System.StringComparison.Ordinal) >= 0 ||
               name.IndexOf("Distant Station", System.StringComparison.Ordinal) >= 0 ||
               name.IndexOf("Pipe", System.StringComparison.Ordinal) >= 0 ||
               name.IndexOf("Cable", System.StringComparison.Ordinal) >= 0 ||
               name.IndexOf("Guide Path", System.StringComparison.Ordinal) >= 0 ||
               name.IndexOf("Spine", System.StringComparison.Ordinal) >= 0 ||
               name.IndexOf("Branch Path", System.StringComparison.Ordinal) >= 0 ||
               name.IndexOf("Escape Route", System.StringComparison.Ordinal) >= 0 ||
               name.IndexOf("Supply Cache", System.StringComparison.Ordinal) >= 0 ||
               name.IndexOf("Danger Stripe", System.StringComparison.Ordinal) >= 0;
    }

    private static string ArchitectureZone(string name, bool isEndless)
    {
        if (isEndless)
        {
            if (name.IndexOf("Data Vault", System.StringComparison.Ordinal) >= 0)
            {
                return "01_DATA_VAULT";
            }
            if (name.IndexOf("Reactor Annex", System.StringComparison.Ordinal) >= 0)
            {
                return "02_REACTOR_ANNEX";
            }
            if (name.IndexOf("Supply Annex", System.StringComparison.Ordinal) >= 0)
            {
                return "03_SUPPLY_ANNEX";
            }
            if (name.IndexOf("Security Annex", System.StringComparison.Ordinal) >= 0)
            {
                return "04_SECURITY_ANNEX";
            }
            if (name.IndexOf("Core", System.StringComparison.Ordinal) >= 0)
            {
                return "05_CORE";
            }
            return "00_RING_AND_HULL";
        }

        if (name.IndexOf("Outer Hull", System.StringComparison.Ordinal) >= 0 ||
            name.IndexOf("Sealed Outer", System.StringComparison.Ordinal) >= 0 ||
            name.IndexOf("Floor Rib", System.StringComparison.Ordinal) >= 0)
        {
            return "00_OUTER_HULL";
        }
        if (name.IndexOf("Pod Bay", System.StringComparison.Ordinal) >= 0)
        {
            return "01_POD_BAY";
        }
        if (name.IndexOf("Life Support", System.StringComparison.Ordinal) >= 0)
        {
            return "03_LIFE_SUPPORT";
        }
        if (name.IndexOf("Navigation", System.StringComparison.Ordinal) >= 0)
        {
            return "04_NAVIGATION";
        }
        if (name.IndexOf("Medical", System.StringComparison.Ordinal) >= 0)
        {
            return "05_MEDICAL_BAY";
        }
        if (name.IndexOf("Security", System.StringComparison.Ordinal) >= 0)
        {
            return "06_SECURITY_OFFICE";
        }
        if (name.IndexOf("Reactor", System.StringComparison.Ordinal) >= 0)
        {
            return "07_REACTOR";
        }
        if (name.IndexOf("Comms", System.StringComparison.Ordinal) >= 0)
        {
            return "08_COMMS_LAB";
        }
        if (name.IndexOf("Escape", System.StringComparison.Ordinal) >= 0)
        {
            return "09_ESCAPE_BAY";
        }
        return "02_MAIN_CORRIDOR";
    }

    private static string DressingZone(string name, bool isEndless)
    {
        if (isEndless)
        {
            if (name.StartsWith("Imported", System.StringComparison.Ordinal))
            {
                return "90_IMPORTED_VISUALS";
            }
            if (name.IndexOf("Core", System.StringComparison.Ordinal) >= 0)
            {
                return "01_CORE";
            }
            return "00_ARENA_DETAILS";
        }

        if (name.StartsWith("Imported", System.StringComparison.Ordinal))
        {
            return "90_IMPORTED_VISUALS";
        }
        if (name.IndexOf("Pod Bay", System.StringComparison.Ordinal) >= 0 ||
            name.IndexOf("Mission Board", System.StringComparison.Ordinal) >= 0 ||
            name.IndexOf("Observation", System.StringComparison.Ordinal) >= 0 ||
            name.IndexOf("Distant Station", System.StringComparison.Ordinal) >= 0)
        {
            return "01_POD_BAY";
        }
        if (name.IndexOf("Life Support", System.StringComparison.Ordinal) >= 0)
        {
            return "03_LIFE_SUPPORT";
        }
        if (name.IndexOf("Navigation", System.StringComparison.Ordinal) >= 0)
        {
            return "04_NAVIGATION";
        }
        if (name.IndexOf("Medical", System.StringComparison.Ordinal) >= 0)
        {
            return "05_MEDICAL_BAY";
        }
        if (name.IndexOf("Security", System.StringComparison.Ordinal) >= 0)
        {
            return "06_SECURITY_OFFICE";
        }
        if (name.IndexOf("Reactor", System.StringComparison.Ordinal) >= 0)
        {
            return "07_REACTOR";
        }
        if (name.IndexOf("Comms", System.StringComparison.Ordinal) >= 0)
        {
            return "08_COMMS_LAB";
        }
        if (name.IndexOf("Escape", System.StringComparison.Ordinal) >= 0)
        {
            return "09_ESCAPE_BAY";
        }
        return "02_MAIN_CORRIDOR";
    }

    private static GameObject FindRootObject(Scene scene, string name)
    {
        foreach (GameObject rootObject in scene.GetRootGameObjects())
        {
            if (rootObject.name == name)
            {
                return rootObject;
            }
        }

        return null;
    }

    private static Transform EnsureSceneHierarchyFolder(Scene scene, string name, int siblingIndex)
    {
        GameObject existing = FindRootObject(scene, name);
        if (existing != null)
        {
            existing.transform.SetSiblingIndex(siblingIndex);
            return existing.transform;
        }

        GameObject folder = new GameObject(name);
        SceneManager.MoveGameObjectToScene(folder, scene);
        folder.transform.SetSiblingIndex(siblingIndex);
        return folder.transform;
    }

    private static Transform EnsureHierarchyFolder(Transform parent, string path)
    {
        Transform current = parent;
        string[] folderNames = path.Split('/');
        foreach (string folderName in folderNames)
        {
            Transform child = current.Find(folderName);
            if (child == null)
            {
                GameObject folder = new GameObject(folderName);
                child = folder.transform;
                child.SetParent(current, false);
            }

            current = child;
        }

        return current;
    }

    private static TMP_Text ConvertLegacyText(Text legacyText, TMP_FontAsset font)
    {
        GameObject textObject = legacyText.gameObject;
        string value = legacyText.text;
        Color color = legacyText.color;
        int size = legacyText.fontSize;
        TextAnchor alignment = legacyText.alignment;
        FontStyle style = legacyText.fontStyle;
        bool raycastTarget = legacyText.raycastTarget;
        bool resizeForBestFit = legacyText.resizeTextForBestFit;
        int resizeMinSize = legacyText.resizeTextMinSize;
        int resizeMaxSize = legacyText.resizeTextMaxSize;
        bool isWorldLabel = textObject.transform.parent != null && textObject.transform.parent.name.StartsWith("World Label");

        Object.DestroyImmediate(legacyText);

        TextMeshProUGUI replacement = textObject.AddComponent<TextMeshProUGUI>();
        replacement.text = value;
        replacement.font = font;
        replacement.color = color;
        replacement.fontSize = Mathf.RoundToInt(size * (isWorldLabel ? 1.12f : 1.18f));
        replacement.alignment = ConvertAlignment(alignment);
        replacement.fontStyle = style == FontStyle.Bold ? FontStyles.Bold : FontStyles.Normal;
        replacement.characterSpacing = 1.5f;
        replacement.enableWordWrapping = true;
        replacement.enableAutoSizing = resizeForBestFit;
        replacement.fontSizeMin = resizeMinSize;
        replacement.fontSizeMax = resizeMaxSize;
        replacement.raycastTarget = raycastTarget;
        return replacement;
    }

    private static TextAlignmentOptions ConvertAlignment(TextAnchor alignment)
    {
        switch (alignment)
        {
            case TextAnchor.UpperLeft:
                return TextAlignmentOptions.TopLeft;
            case TextAnchor.UpperCenter:
                return TextAlignmentOptions.Top;
            case TextAnchor.UpperRight:
                return TextAlignmentOptions.TopRight;
            case TextAnchor.MiddleLeft:
                return TextAlignmentOptions.Left;
            case TextAnchor.MiddleRight:
                return TextAlignmentOptions.Right;
            case TextAnchor.LowerLeft:
                return TextAlignmentOptions.BottomLeft;
            case TextAnchor.LowerCenter:
                return TextAlignmentOptions.Bottom;
            case TextAnchor.LowerRight:
                return TextAlignmentOptions.BottomRight;
            default:
                return TextAlignmentOptions.Center;
        }
    }

    private static void RebindHUDTypography(HUDController hud)
    {
        SetObject(hud, "oxygenText", FindTMPText(hud.transform, "OxygenText"));
        SetObject(hud, "healthText", FindTMPText(hud.transform, "HealthText"));
        SetObject(hud, "batteryText", FindTMPText(hud.transform, "BatteryText"));
        SetObject(hud, "timerText", FindTMPText(hud.transform, "TimerText"));
        SetObject(hud, "repairText", FindTMPText(hud.transform, "RepairText"));
        SetObject(hud, "pressureText", FindTMPText(hud.transform, "PressureText"));
        SetObject(hud, "inventoryText", FindTMPText(hud.transform, "InventoryText"));
        SetObject(hud, "backpackItemsText", FindTMPText(hud.transform, "BackpackItems"));
        SetObject(hud, "objectiveText", FindTMPText(hud.transform, "ObjectiveText"));
        SetObject(hud, "promptText", FindTMPText(hud.transform, "PromptText"));
        SetObject(hud, "messageText", FindTMPText(hud.transform, "MessageText"));
        SetObject(hud, "scanText", FindTMPText(hud.transform, "ScanText"));
        SetObject(hud, "recordsText", FindTMPText(hud.transform, "RecordsText"));
        SetObject(hud, "mainMenuInfoText", FindTMPText(hud.transform, "MainMenuInfoText"));
        SetObject(hud, "mainMenuPopupTitleText", FindTMPText(hud.transform, "MainMenuPopupTitle"));
        SetObject(hud, "mainMenuPopupBodyText", FindTMPText(hud.transform, "MainMenuPopupBody"));
        SetObject(hud, "mainMenuVolumeValueText", FindTMPText(hud.transform, "MainMenuVolumeValueText"));
        SetObject(hud, "mainMenuMouseSensitivityValueText", FindTMPText(hud.transform, "MainMenuMouseSensitivityValueText"));
        Transform mainMenuPanel = FindChild(hud.transform, "MainMenuPanel");
        Transform mainMenuPopupPanel = FindChild(hud.transform, "MainMenuPopupPanel");
        Transform mainMenuOptionsControls = FindChild(hud.transform, "MainMenuOptionsControls");
        Transform modeSelectPanel = FindChild(hud.transform, "ModeSelectPanel");
        Transform briefingPanel = FindChild(hud.transform, "BriefingPanel");
        Transform pausePanel = FindChild(hud.transform, "PausePanel");
        Transform endPanel = FindChild(hud.transform, "EndPanel");
        SetObject(hud, "mainMenuPanel", mainMenuPanel == null ? null : mainMenuPanel.gameObject);
        SetObject(hud, "mainMenuPopupPanel", mainMenuPopupPanel == null ? null : mainMenuPopupPanel.gameObject);
        SetObject(hud, "mainMenuOptionsControls", mainMenuOptionsControls == null ? null : mainMenuOptionsControls.gameObject);
        SetObject(hud, "modeSelectPanel", modeSelectPanel == null ? null : modeSelectPanel.gameObject);
        SetObject(hud, "briefingPanel", briefingPanel == null ? null : briefingPanel.gameObject);
        SetObject(hud, "pausePanel", pausePanel == null ? null : pausePanel.gameObject);
        SetObject(hud, "endPanel", endPanel == null ? null : endPanel.gameObject);
        SetObject(hud, "briefingTitleText", FindTMPText(hud.transform, "BriefingTitle"));
        SetObject(hud, "briefingBodyText", FindTMPText(hud.transform, "BriefingBody"));
        SetObject(hud, "volumeValueText", FindTMPText(hud.transform, "VolumeValueText"));
        SetObject(hud, "mouseSensitivityValueText", FindTMPText(hud.transform, "MouseSensitivityValueText"));
        SetObject(hud, "endTitleText", FindTMPText(hud.transform, "EndTitle"));
        SetObject(hud, "endBodyText", FindTMPText(hud.transform, "EndBody"));
        SetObject(hud, "endRecordText", FindTMPText(hud.transform, "EndRecordText"));
        SetObject(hud, "endHintText", FindTMPText(hud.transform, "EndHintText"));
    }

    private static TMP_Text FindTMPText(Transform root, string objectName)
    {
        TMP_Text[] texts = root.GetComponentsInChildren<TMP_Text>(true);
        foreach (TMP_Text text in texts)
        {
            if (text.name == objectName)
            {
                return text;
            }
        }

        return null;
    }

    private static void ApplySceneTypographyPolish(TMP_FontAsset font)
    {
        TMP_Text[] textComponents = Object.FindObjectsOfType<TMP_Text>(true);
        foreach (TMP_Text text in textComponents)
        {
            text.font = font;
            text.raycastTarget = false;

            if (text.transform.parent != null && text.transform.parent.name.StartsWith("World Label"))
            {
                text.fontStyle = FontStyles.Bold;
                text.characterSpacing = 2f;
            }

            EditorUtility.SetDirty(text);
        }

        HUDController hud = Object.FindObjectOfType<HUDController>();
        if (hud != null)
        {
            EnsurePresentationWidgets(hud, font);
            RebindHUDTypography(hud);
            ApplyHUDPolish(hud.transform, font);
            EditorUtility.SetDirty(hud);
        }
    }

    private static void ApplyHUDPolish(Transform root, TMP_FontAsset font)
    {
        Color panel = new Color(0.015f, 0.035f, 0.065f, 0.88f);
        Color modal = new Color(0.018f, 0.045f, 0.082f, 0.96f);
        Color overlay = new Color(0.006f, 0.018f, 0.04f, 0.94f);
        Color accent = new Color(0.08f, 0.82f, 1f);
        Color primary = new Color(0.78f, 0.92f, 1f);
        Color secondary = new Color(0.42f, 0.68f, 0.8f);
        Color warning = new Color(1f, 0.69f, 0.2f);

        TintImage(root, "MissionPanel", panel);
        TintImage(root, "StatusPanel", panel);
        TintImage(root, "BackpackPanel", modal);
        TintImage(root, "PausePanel", modal);
        TintImage(root, "EndPanel", overlay);
        TintImage(root, "MainMenuCommandCard", new Color(0.008f, 0.027f, 0.055f, 0.72f));
        TintImage(root, "MainMenuInfoPanel", new Color(0.008f, 0.027f, 0.055f, 0.7f));
        TintImage(root, "MainMenuPopupPanel", new Color(0.008f, 0.027f, 0.055f, 0.92f));
        TintImage(root, "MainMenuOptionsControls", new Color(0.012f, 0.045f, 0.082f, 0.72f));
        TintImage(root, "ModeSelectPanel", overlay);
        TintImage(root, "ModeSelectCard", modal);
        TintImage(root, "BriefingPanel", modal);

        AddPanelAccent(root, "MissionPanel", accent);
        AddPanelAccent(root, "StatusPanel", accent);
        AddPanelAccent(root, "BackpackPanel", accent);
        AddPanelAccent(root, "PausePanel", accent);
        AddPanelAccent(root, "MainMenuCommandCard", accent);
        AddPanelAccent(root, "MainMenuInfoPanel", accent);
        AddPanelAccent(root, "MainMenuPopupPanel", accent);
        AddPanelAccent(root, "ModeSelectCard", accent);
        AddPanelAccent(root, "BriefingPanel", accent);

        EnsureBarTrack(root, "OxygenFill");
        EnsureBarTrack(root, "HealthFill");
        EnsureBarTrack(root, "BatteryFill");

        StyleHUDText(root, "MissionTitle", font, accent, 20f, FontStyles.Bold, 2.8f);
        StyleHUDText(root, "ObjectiveText", font, primary, 18f, FontStyles.Normal, 1.5f);
        StyleHUDText(root, "TimerText", font, warning, 21f, FontStyles.Bold, 2f);
        StyleHUDText(root, "RepairText", font, accent, 19f, FontStyles.Normal, 1.8f);
        StyleHUDText(root, "PressureText", font, warning, 18f, FontStyles.Normal, 1.8f);

        StyleHUDText(root, "OxygenText", font, primary, 19f, FontStyles.Bold, 1.8f);
        StyleHUDText(root, "HealthText", font, primary, 19f, FontStyles.Bold, 1.8f);
        StyleHUDText(root, "BatteryText", font, primary, 19f, FontStyles.Bold, 1.8f);
        StyleHUDText(root, "InventoryText", font, secondary, 16f, FontStyles.Normal, 1.6f);
        StyleHUDText(root, "Crosshair", font, accent, 33f, FontStyles.Bold, 0f);
        StyleHUDText(root, "PromptText", font, accent, 25f, FontStyles.Bold, 2.2f);
        StyleHUDText(root, "MessageText", font, warning, 26f, FontStyles.Bold, 2.2f);

        StyleHUDText(root, "BackpackTitle", font, accent, 29f, FontStyles.Bold, 3f);
        StyleHUDText(root, "BackpackHint", font, secondary, 19f, FontStyles.Normal, 2f);
        StyleHUDText(root, "BackpackItems", font, primary, 24f, FontStyles.Normal, 1.8f);
        StyleHUDText(root, "ScanTitle", font, accent, 42f, FontStyles.Bold, 3f);
        StyleHUDText(root, "ScanText", font, primary, 29f, FontStyles.Normal, 1.8f);
        StyleHUDText(root, "ScanHint", font, warning, 27f, FontStyles.Bold, 2.4f);
        StyleHUDText(root, "PauseTitle", font, accent, 42f, FontStyles.Bold, 3f);
        StyleHUDText(root, "PauseBody", font, primary, 26f, FontStyles.Normal, 2f);
        StyleHUDText(root, "SettingsTitle", font, accent, 22f, FontStyles.Bold, 2.5f);
        StyleHUDText(root, "MasterVolumeLabel", font, primary, 23f, FontStyles.Normal, 2f);
        StyleHUDText(root, "VolumeValueText", font, warning, 24f, FontStyles.Bold, 2f);
        StyleHUDText(root, "MouseSensitivityLabel", font, primary, 23f, FontStyles.Normal, 2f);
        StyleHUDText(root, "MouseSensitivityValueText", font, warning, 24f, FontStyles.Bold, 2f);
        StyleHUDText(root, "PauseSettingsHint", font, secondary, 17f, FontStyles.Normal, 1.5f);
        StyleHUDText(root, "EndTitle", font, accent, 52f, FontStyles.Bold, 3.5f);
        StyleHUDText(root, "EndBody", font, primary, 27f, FontStyles.Normal, 2f);
        StyleHUDText(root, "MainMenuHeader", font, accent, 34f, FontStyles.Bold, 2.8f);
        StyleHUDText(root, "MainMenuPrototype", font, secondary, 17f, FontStyles.Bold, 2.4f);
        StyleHUDText(root, "MainMenuTitle", font, accent, 27f, FontStyles.Bold, 2.6f);
        StyleHUDText(root, "MainMenuSubtitle", font, primary, 39f, FontStyles.Bold, 3f);
        StyleHUDText(root, "MainMenuInfoText", font, primary, 18f, FontStyles.Normal, 1.7f);
        StyleHUDText(root, "MainMenuHint", font, secondary, 14f, FontStyles.Bold, 1.8f);
        StyleHUDText(root, "MainMenuPopupTitle", font, accent, 31f, FontStyles.Bold, 3f);
        StyleHUDText(root, "MainMenuPopupBody", font, primary, 20f, FontStyles.Normal, 1.8f);
        StyleHUDText(root, "MainMenuVolumeLabel", font, primary, 18f, FontStyles.Bold, 1.6f);
        StyleHUDText(root, "MainMenuVolumeValueText", font, warning, 20f, FontStyles.Bold, 1.8f);
        StyleHUDText(root, "MainMenuMouseSensitivityLabel", font, primary, 18f, FontStyles.Bold, 1.6f);
        StyleHUDText(root, "MainMenuMouseSensitivityValueText", font, warning, 20f, FontStyles.Bold, 1.8f);
        StyleHUDText(root, "MainMenuBackButtonLabel", font, Color.white, 18f, FontStyles.Bold, 1.8f);
        StyleHUDText(root, "MainMenuVolumeDownButtonLabel", font, Color.white, 22f, FontStyles.Bold, 1.5f);
        StyleHUDText(root, "MainMenuVolumeUpButtonLabel", font, Color.white, 22f, FontStyles.Bold, 1.5f);
        StyleHUDText(root, "MainMenuSensitivityDownButtonLabel", font, Color.white, 22f, FontStyles.Bold, 1.5f);
        StyleHUDText(root, "MainMenuSensitivityUpButtonLabel", font, Color.white, 22f, FontStyles.Bold, 1.5f);
        StyleHUDText(root, "ModeTitle", font, accent, 50f, FontStyles.Bold, 3.5f);
        StyleHUDText(root, "ModeSubtitle", font, primary, 30f, FontStyles.Normal, 2.8f);
        StyleHUDText(root, "ModeBody", font, secondary, 21f, FontStyles.Normal, 2f);
        StyleHUDText(root, "ModeHint", font, warning, 22f, FontStyles.Bold, 2.3f);
        StyleHUDText(root, "RecordsTitle", font, accent, 19f, FontStyles.Bold, 2.4f);
        StyleHUDText(root, "RecordsText", font, primary, 18f, FontStyles.Normal, 1.8f);
        StyleHUDText(root, "BriefingTitle", font, accent, 37f, FontStyles.Bold, 3f);
        StyleHUDText(root, "BriefingBody", font, primary, 23f, FontStyles.Normal, 2f);
        StyleHUDText(root, "EndRecordText", font, warning, 30f, FontStyles.Bold, 2.8f);
        StyleHUDText(root, "EndHintText", font, secondary, 22f, FontStyles.Bold, 2.2f);
        StyleHUDText(root, "NewGameButtonLabel", font, Color.white, 24f, FontStyles.Bold, 2f);
        StyleHUDText(root, "OptionsButtonLabel", font, primary, 22f, FontStyles.Bold, 1.8f);
        StyleHUDText(root, "HowToPlayButtonLabel", font, primary, 22f, FontStyles.Bold, 1.8f);
        StyleHUDText(root, "CreditsButtonLabel", font, primary, 22f, FontStyles.Bold, 1.8f);
        StyleHUDText(root, "TimedEvacuationButtonTitle", font, Color.white, 27f, FontStyles.Bold, 2f);
        StyleHUDText(root, "TimedEvacuationButtonSubtitle", font, primary, 17f, FontStyles.Normal, 1.5f);
        StyleHUDText(root, "EndlessSurvivalButtonTitle", font, Color.white, 27f, FontStyles.Bold, 2f);
        StyleHUDText(root, "EndlessSurvivalButtonSubtitle", font, primary, 17f, FontStyles.Normal, 1.5f);

        StyleButton(root, "TimedEvacuationButton", new Color(0.035f, 0.42f, 0.62f, 0.96f));
        StyleButton(root, "EndlessSurvivalButton", new Color(0.025f, 0.29f, 0.43f, 0.96f));
        StyleButton(root, "MainMenuBackButton", new Color(0.035f, 0.42f, 0.72f, 0.96f));
        StyleButton(root, "MainMenuVolumeDownButton", new Color(0.025f, 0.29f, 0.43f, 0.96f));
        StyleButton(root, "MainMenuVolumeUpButton", new Color(0.035f, 0.42f, 0.62f, 0.96f));
        StyleButton(root, "MainMenuSensitivityDownButton", new Color(0.025f, 0.29f, 0.43f, 0.96f));
        StyleButton(root, "MainMenuSensitivityUpButton", new Color(0.035f, 0.42f, 0.62f, 0.96f));
        StyleButton(root, "VolumeDownButton", new Color(0.025f, 0.29f, 0.43f, 0.96f));
        StyleButton(root, "VolumeUpButton", new Color(0.035f, 0.42f, 0.62f, 0.96f));
        StyleButton(root, "SensitivityDownButton", new Color(0.025f, 0.29f, 0.43f, 0.96f));
        StyleButton(root, "SensitivityUpButton", new Color(0.035f, 0.42f, 0.62f, 0.96f));
    }

    private static void EnsureMainMenuWidgets(HUDController hud, TMP_FontAsset font)
    {
        Transform root = hud.transform;
        Transform mainMenuPanel = FindChild(root, "MainMenuPanel");

        if (mainMenuPanel == null)
        {
            GameObject created = CreateMainMenuPanel(
                root,
                font,
                new Color(0.02f, 0.42f, 0.88f),
                new Color(0.08f, 0.14f, 0.22f));
            mainMenuPanel = created.transform;
        }
        else
        {
            Image mainMenuImage = mainMenuPanel.GetComponent<Image>();
            Sprite coverSprite = LoadMainMenuCoverSprite();
            if (mainMenuImage != null && coverSprite != null)
            {
                mainMenuImage.sprite = coverSprite;
                mainMenuImage.color = Color.white;
                mainMenuImage.type = Image.Type.Simple;
                mainMenuImage.preserveAspect = false;
            }

            for (int i = mainMenuPanel.childCount - 1; i >= 0; i--)
            {
                Object.DestroyImmediate(mainMenuPanel.GetChild(i).gameObject);
            }

            BuildMainMenuContent(mainMenuPanel.transform, font, new Color(0.02f, 0.42f, 0.88f), new Color(0.08f, 0.14f, 0.22f));
        }

        GameManager manager = Object.FindObjectOfType<GameManager>();
        if (manager != null)
        {
            EnsureButtonListener(mainMenuPanel, "NewGameButton", new UnityEngine.Events.UnityAction(manager.OpenModeSelection));
        }

        EnsureMainMenuPopupWidgets(hud, mainMenuPanel, font);
        EnsureButtonListener(mainMenuPanel, "OptionsButton", new UnityEngine.Events.UnityAction(hud.ShowMainMenuOptions));
        EnsureButtonListener(mainMenuPanel, "HowToPlayButton", new UnityEngine.Events.UnityAction(hud.ShowMainMenuHowToPlay));
        EnsureButtonListener(mainMenuPanel, "CreditsButton", new UnityEngine.Events.UnityAction(hud.ShowMainMenuCredits));
        EnsureButtonListener(mainMenuPanel, "MainMenuBackButton", new UnityEngine.Events.UnityAction(hud.CloseMainMenuPopup));
        EnsureButtonListener(mainMenuPanel, "MainMenuVolumeDownButton", new UnityEngine.Events.UnityAction(hud.DecreaseMasterVolume));
        EnsureButtonListener(mainMenuPanel, "MainMenuVolumeUpButton", new UnityEngine.Events.UnityAction(hud.IncreaseMasterVolume));
        EnsureButtonListener(mainMenuPanel, "MainMenuSensitivityDownButton", new UnityEngine.Events.UnityAction(hud.DecreaseMouseSensitivity));
        EnsureButtonListener(mainMenuPanel, "MainMenuSensitivityUpButton", new UnityEngine.Events.UnityAction(hud.IncreaseMouseSensitivity));

        Transform modeSelectPanel = FindChild(root, "ModeSelectPanel");
        if (modeSelectPanel != null)
        {
            modeSelectPanel.gameObject.SetActive(false);
            SetObject(hud, "modeSelectPanel", modeSelectPanel.gameObject);
        }

        mainMenuPanel.gameObject.SetActive(true);
        SetObject(hud, "mainMenuPanel", mainMenuPanel.gameObject);
        SetObject(hud, "mainMenuInfoText", null);
    }

    private static void EnsureMainMenuPopupWidgets(HUDController hud, Transform mainMenuPanel, TMP_FontAsset font)
    {
        Transform popup = FindChild(mainMenuPanel, "MainMenuPopupPanel");
        if (popup != null && FindChild(popup, "MainMenuBackButton") == null)
        {
            Object.DestroyImmediate(popup.gameObject);
            popup = null;
        }

        if (popup == null)
        {
            popup = Panel(mainMenuPanel, "MainMenuPopupPanel", new Vector2(-54f, -116f), new Vector2(382f, 342f), new Color(0.008f, 0.027f, 0.055f, 0.92f)).transform;
            TMP_Text title = TextUI(popup, "MainMenuPopupTitle", "OPTIONS", new Vector2(22f, -20f), TextAlignmentOptions.TopLeft, 31, font);
            title.color = new Color(0.08f, 0.82f, 1f);
            title.fontStyle = FontStyles.Bold;
            title.enableWordWrapping = false;
            title.GetComponent<RectTransform>().sizeDelta = new Vector2(330f, 42f);

            TMP_Text body = TextUI(popup, "MainMenuPopupBody", "", new Vector2(22f, -70f), TextAlignmentOptions.TopLeft, 20, font);
            body.color = new Color(0.78f, 0.92f, 1f);
            body.GetComponent<RectTransform>().sizeDelta = new Vector2(336f, 148f);

            GameObject controls = TopLeftPanel(popup, "MainMenuOptionsControls", new Vector2(20f, -204f), new Vector2(342f, 84f), new Color(0.012f, 0.045f, 0.082f, 0.72f));
            TMP_Text volumeLabel = TextUI(controls.transform, "MainMenuVolumeLabel", "MASTER VOLUME", new Vector2(12f, -9f), TextAlignmentOptions.TopLeft, 18, font);
            volumeLabel.color = new Color(0.78f, 0.92f, 1f);
            volumeLabel.fontStyle = FontStyles.Bold;
            volumeLabel.enableWordWrapping = false;
            volumeLabel.GetComponent<RectTransform>().sizeDelta = new Vector2(150f, 28f);
            TMP_Text volumeValue = TextUI(controls.transform, "MainMenuVolumeValueText", "80%", new Vector2(178f, -9f), TextAlignmentOptions.TopLeft, 20, font);
            volumeValue.color = new Color(1f, 0.69f, 0.2f);
            volumeValue.fontStyle = FontStyles.Bold;
            volumeValue.enableWordWrapping = false;
            volumeValue.GetComponent<RectTransform>().sizeDelta = new Vector2(58f, 28f);
            TMP_Text sensitivityLabel = TextUI(controls.transform, "MainMenuMouseSensitivityLabel", "LOOK SENSITIVITY", new Vector2(12f, -48f), TextAlignmentOptions.TopLeft, 18, font);
            sensitivityLabel.color = new Color(0.78f, 0.92f, 1f);
            sensitivityLabel.fontStyle = FontStyles.Bold;
            sensitivityLabel.enableWordWrapping = false;
            sensitivityLabel.GetComponent<RectTransform>().sizeDelta = new Vector2(170f, 28f);
            TMP_Text sensitivityValue = TextUI(controls.transform, "MainMenuMouseSensitivityValueText", "2.2", new Vector2(178f, -48f), TextAlignmentOptions.TopLeft, 20, font);
            sensitivityValue.color = new Color(1f, 0.69f, 0.2f);
            sensitivityValue.fontStyle = FontStyles.Bold;
            sensitivityValue.enableWordWrapping = false;
            sensitivityValue.GetComponent<RectTransform>().sizeDelta = new Vector2(58f, 28f);

            MainMenuPopupButton(controls.transform, "MainMenuVolumeDownButton", "-", new Vector2(246f, -6f), new Vector2(34f, 28f), font);
            MainMenuPopupButton(controls.transform, "MainMenuVolumeUpButton", "+", new Vector2(292f, -6f), new Vector2(34f, 28f), font);
            MainMenuPopupButton(controls.transform, "MainMenuSensitivityDownButton", "-", new Vector2(246f, -45f), new Vector2(34f, 28f), font);
            MainMenuPopupButton(controls.transform, "MainMenuSensitivityUpButton", "+", new Vector2(292f, -45f), new Vector2(34f, 28f), font);
            MainMenuPopupButton(popup, "MainMenuBackButton", "BACK", new Vector2(246f, -294f), new Vector2(98f, 34f), font);
            controls.SetActive(false);
            popup.gameObject.SetActive(false);
        }

        SetObject(hud, "mainMenuPopupPanel", popup.gameObject);
        SetObject(hud, "mainMenuPopupTitleText", FindTMPText(popup, "MainMenuPopupTitle"));
        SetObject(hud, "mainMenuPopupBodyText", FindTMPText(popup, "MainMenuPopupBody"));
        Transform optionsControls = FindChild(popup, "MainMenuOptionsControls");
        SetObject(hud, "mainMenuOptionsControls", optionsControls == null ? null : optionsControls.gameObject);
        SetObject(hud, "mainMenuVolumeValueText", FindTMPText(popup, "MainMenuVolumeValueText"));
        SetObject(hud, "mainMenuMouseSensitivityValueText", FindTMPText(popup, "MainMenuMouseSensitivityValueText"));
    }

    private static void EnsureButtonListener(Transform root, string objectName, UnityEngine.Events.UnityAction callback)
    {
        Transform child = FindChild(root, objectName);
        if (child == null)
        {
            return;
        }

        Button button = child.GetComponent<Button>();
        if (button != null && button.onClick.GetPersistentEventCount() == 0)
        {
            UnityEventTools.AddPersistentListener(button.onClick, callback);
        }
    }

    private static void EnsurePresentationWidgets(HUDController hud, TMP_FontAsset font)
    {
        EnsureMainMenuWidgets(hud, font);

        Transform root = hud.transform;
        Transform modeCard = FindChild(root, "ModeSelectCard");
        if (modeCard != null)
        {
            modeCard.GetComponent<RectTransform>().sizeDelta = new Vector2(760f, 610f);
            PositionText(root, "ModeTitle", new Vector2(0f, 237f));
            PositionText(root, "ModeSubtitle", new Vector2(0f, 194f));
            PositionText(root, "ModeBody", new Vector2(0f, 150f));
            PositionRect(root, "TimedEvacuationButton", new Vector2(0f, -28f));
            PositionRect(root, "EndlessSurvivalButton", new Vector2(0f, -134f));
            PositionText(root, "ModeHint", new Vector2(0f, -258f));

            EnsureTextElement(modeCard, "RecordsTitle", "PERSONAL ARCHIVE", new Vector2(0f, 98f), TextAlignmentOptions.Center, 19, font, new Vector2(660f, 28f));
            TMP_Text records = EnsureTextElement(
                modeCard,
                "RecordsText",
                "TIMED   FASTEST --:--   BEST RANK --\nENDLESS HIGH SCORE 0000   LONGEST --:--",
                new Vector2(0f, 58f),
                TextAlignmentOptions.Center,
                18,
                font,
                new Vector2(700f, 60f));
            SetObject(hud, "recordsText", records);
        }

        Transform briefingPanel = FindChild(root, "BriefingPanel");
        if (briefingPanel == null)
        {
            briefingPanel = CenterPanel(root, "BriefingPanel", new Vector2(720f, 220f), new Color(0.018f, 0.045f, 0.082f, 0.96f)).transform;
        }

        TMP_Text briefingTitle = EnsureTextElement(briefingPanel, "BriefingTitle", "EVACUATION PROTOCOL ACTIVE", new Vector2(0f, 52f), TextAlignmentOptions.Center, 37, font, new Vector2(660f, 48f));
        TMP_Text briefingBody = EnsureTextElement(briefingPanel, "BriefingBody", "", new Vector2(0f, -22f), TextAlignmentOptions.Center, 23, font, new Vector2(650f, 94f));
        briefingPanel.gameObject.SetActive(false);
        SetObject(hud, "briefingPanel", briefingPanel.gameObject);
        SetObject(hud, "briefingTitleText", briefingTitle);
        SetObject(hud, "briefingBodyText", briefingBody);

        Transform pausePanel = FindChild(root, "PausePanel");
        if (pausePanel != null)
        {
            pausePanel.GetComponent<RectTransform>().sizeDelta = new Vector2(560f, 610f);
            PositionText(root, "PauseTitle", new Vector2(0f, 228f));
            TMP_Text pauseBody = FindTMPText(root, "PauseBody");
            if (pauseBody != null)
            {
                pauseBody.text = "ESC  RESUME\nR    RESTART CHECKPOINT\nTAB  BACKPACK   Q  SCANNER\nH    MEDKIT     B  BATTERY";
                RectTransform bodyRect = pauseBody.GetComponent<RectTransform>();
                bodyRect.anchoredPosition = new Vector2(0f, 116f);
                bodyRect.sizeDelta = new Vector2(480f, 150f);
            }

            EnsureTextElement(pausePanel, "SettingsTitle", "SYSTEM SETTINGS", new Vector2(0f, 32f), TextAlignmentOptions.Center, 22, font, new Vector2(470f, 30f));
            EnsureTextElement(pausePanel, "MasterVolumeLabel", "MASTER VOLUME", new Vector2(-112f, -22f), TextAlignmentOptions.Left, 23, font, new Vector2(198f, 36f));
            TMP_Text volumeValue = EnsureTextElement(pausePanel, "VolumeValueText", "80%", new Vector2(76f, -22f), TextAlignmentOptions.Center, 24, font, new Vector2(64f, 36f));
            EnsureTextElement(pausePanel, "MouseSensitivityLabel", "LOOK SENSITIVITY", new Vector2(-112f, -82f), TextAlignmentOptions.Left, 23, font, new Vector2(198f, 36f));
            TMP_Text sensitivityValue = EnsureTextElement(pausePanel, "MouseSensitivityValueText", "2.2", new Vector2(76f, -82f), TextAlignmentOptions.Center, 24, font, new Vector2(64f, 36f));
            EnsureSettingButton(pausePanel, "VolumeDownButton", "-", new Vector2(134f, -22f), font, new UnityEngine.Events.UnityAction(hud.DecreaseMasterVolume));
            EnsureSettingButton(pausePanel, "VolumeUpButton", "+", new Vector2(194f, -22f), font, new UnityEngine.Events.UnityAction(hud.IncreaseMasterVolume));
            EnsureSettingButton(pausePanel, "SensitivityDownButton", "-", new Vector2(134f, -82f), font, new UnityEngine.Events.UnityAction(hud.DecreaseMouseSensitivity));
            EnsureSettingButton(pausePanel, "SensitivityUpButton", "+", new Vector2(194f, -82f), font, new UnityEngine.Events.UnityAction(hud.IncreaseMouseSensitivity));
            EnsureTextElement(pausePanel, "PauseSettingsHint", "SETTINGS SAVE AUTOMATICALLY", new Vector2(0f, -158f), TextAlignmentOptions.Center, 17, font, new Vector2(480f, 28f));
            SetObject(hud, "volumeValueText", volumeValue);
            SetObject(hud, "mouseSensitivityValueText", sensitivityValue);
        }

        Transform endPanel = FindChild(root, "EndPanel");
        if (endPanel != null)
        {
            PositionText(root, "EndTitle", new Vector2(0f, 228f));
            TMP_Text endBody = FindTMPText(root, "EndBody");
            if (endBody != null)
            {
                RectTransform bodyRect = endBody.GetComponent<RectTransform>();
                bodyRect.anchoredPosition = Vector2.zero;
                bodyRect.sizeDelta = new Vector2(680f, 390f);
            }

            TMP_Text endRecord = EnsureTextElement(endPanel, "EndRecordText", "", new Vector2(0f, -246f), TextAlignmentOptions.Center, 30, font, new Vector2(760f, 44f));
            TMP_Text endHint = EnsureTextElement(endPanel, "EndHintText", "R  REDEPLOY", new Vector2(0f, -302f), TextAlignmentOptions.Center, 22, font, new Vector2(620f, 34f));
            SetObject(hud, "endRecordText", endRecord);
            SetObject(hud, "endHintText", endHint);
        }
    }

    private static TMP_Text EnsureTextElement(Transform parent, string name, string value, Vector2 position, TextAlignmentOptions alignment, int size, TMP_FontAsset font, Vector2 sizeDelta)
    {
        Transform child = parent.Find(name);
        TMP_Text text = child == null ? null : child.GetComponent<TMP_Text>();
        if (text == null)
        {
            text = TextUI(parent, name, value, position, alignment, size, font);
        }

        text.text = value;
        text.font = font;
        text.alignment = alignment;
        text.fontSize = size;
        RectTransform rect = text.GetComponent<RectTransform>();
        rect.anchoredPosition = position;
        rect.sizeDelta = sizeDelta;
        return text;
    }

    private static Button EnsureSettingButton(Transform parent, string name, string label, Vector2 position, TMP_FontAsset font, UnityEngine.Events.UnityAction callback)
    {
        Transform existing = parent.Find(name);
        bool created = existing == null;
        GameObject buttonObject;
        if (created)
        {
            buttonObject = new GameObject(name, typeof(RectTransform), typeof(Image), typeof(Button));
            buttonObject.transform.SetParent(parent, false);
        }
        else
        {
            buttonObject = existing.gameObject;
        }

        Image image = buttonObject.GetComponent<Image>();
        Button button = buttonObject.GetComponent<Button>();
        image.color = new Color(0.035f, 0.42f, 0.62f, 0.96f);
        RectTransform rect = buttonObject.GetComponent<RectTransform>();
        rect.anchorMin = new Vector2(0.5f, 0.5f);
        rect.anchorMax = new Vector2(0.5f, 0.5f);
        rect.pivot = new Vector2(0.5f, 0.5f);
        rect.anchoredPosition = position;
        rect.sizeDelta = new Vector2(46f, 38f);

        TMP_Text buttonLabel = EnsureTextElement(buttonObject.transform, name + "Label", label, Vector2.zero, TextAlignmentOptions.Center, 26, font, new Vector2(42f, 34f));
        buttonLabel.color = Color.white;
        buttonLabel.fontStyle = FontStyles.Bold;
        if (created)
        {
            UnityEventTools.AddPersistentListener(button.onClick, callback);
        }

        return button;
    }

    private static void PositionText(Transform root, string objectName, Vector2 position)
    {
        TMP_Text text = FindTMPText(root, objectName);
        if (text != null)
        {
            text.GetComponent<RectTransform>().anchoredPosition = position;
        }
    }

    private static void PositionRect(Transform root, string objectName, Vector2 position)
    {
        Transform child = FindChild(root, objectName);
        if (child != null)
        {
            child.GetComponent<RectTransform>().anchoredPosition = position;
        }
    }

    private static void StyleHUDText(Transform root, string objectName, TMP_FontAsset font, Color color, float size, FontStyles style, float spacing)
    {
        TMP_Text text = FindTMPText(root, objectName);
        if (text == null)
        {
            return;
        }

        text.font = font;
        text.color = color;
        text.fontSize = size;
        text.fontStyle = style;
        text.characterSpacing = spacing;
        EditorUtility.SetDirty(text);
    }

    private static void TintImage(Transform root, string objectName, Color color)
    {
        Transform child = FindChild(root, objectName);
        Image image = child != null ? child.GetComponent<Image>() : null;
        if (image != null)
        {
            image.color = color;
            EditorUtility.SetDirty(image);
        }
    }

    private static void StyleButton(Transform root, string objectName, Color color)
    {
        Transform child = FindChild(root, objectName);
        Button button = child != null ? child.GetComponent<Button>() : null;
        Image image = child != null ? child.GetComponent<Image>() : null;
        if (button == null || image == null)
        {
            return;
        }

        image.color = color;
        ColorBlock colors = button.colors;
        colors.normalColor = color;
        colors.highlightedColor = Color.Lerp(color, Color.white, 0.24f);
        colors.pressedColor = Color.Lerp(color, Color.black, 0.24f);
        colors.selectedColor = colors.highlightedColor;
        button.colors = colors;
        EditorUtility.SetDirty(image);
        EditorUtility.SetDirty(button);
    }

    private static void AddPanelAccent(Transform root, string panelName, Color color)
    {
        Transform panel = FindChild(root, panelName);
        if (panel == null)
        {
            return;
        }

        Transform accent = panel.Find("AccentLine");
        if (accent == null)
        {
            GameObject accentObject = new GameObject("AccentLine", typeof(RectTransform), typeof(Image));
            accentObject.transform.SetParent(panel, false);
            accent = accentObject.transform;
        }

        Image image = accent.GetComponent<Image>();
        image.color = color;
        image.raycastTarget = false;
        RectTransform rect = accent.GetComponent<RectTransform>();
        rect.anchorMin = new Vector2(0f, 1f);
        rect.anchorMax = new Vector2(1f, 1f);
        rect.pivot = new Vector2(0.5f, 1f);
        rect.anchoredPosition = Vector2.zero;
        rect.sizeDelta = new Vector2(0f, 3f);
        EditorUtility.SetDirty(image);
    }

    private static void EnsureBarTrack(Transform root, string fillName)
    {
        Transform fillTransform = FindChild(root, fillName);
        Image fill = fillTransform != null ? fillTransform.GetComponent<Image>() : null;
        if (fill == null || fillTransform.parent == null)
        {
            return;
        }

        string trackName = fillName + "Track";
        Transform trackTransform = fillTransform.parent.Find(trackName);
        if (trackTransform == null)
        {
            GameObject trackObject = new GameObject(trackName, typeof(RectTransform), typeof(Image));
            trackObject.transform.SetParent(fillTransform.parent, false);
            trackTransform = trackObject.transform;
        }

        Image track = trackTransform.GetComponent<Image>();
        track.color = new Color(0.04f, 0.12f, 0.18f, 0.95f);
        track.raycastTarget = false;
        RectTransform fillRect = fillTransform.GetComponent<RectTransform>();
        RectTransform trackRect = trackTransform.GetComponent<RectTransform>();
        trackRect.anchorMin = fillRect.anchorMin;
        trackRect.anchorMax = fillRect.anchorMax;
        trackRect.pivot = fillRect.pivot;
        trackRect.anchoredPosition = fillRect.anchoredPosition;
        trackRect.sizeDelta = fillRect.sizeDelta;
        trackTransform.SetSiblingIndex(fillTransform.GetSiblingIndex());
        EditorUtility.SetDirty(track);
    }

    private static Transform FindChild(Transform root, string objectName)
    {
        Transform[] children = root.GetComponentsInChildren<Transform>(true);
        foreach (Transform child in children)
        {
            if (child.name == objectName)
            {
                return child;
            }
        }

        return null;
    }

    private static void EnsureFolder(string parent, string folder)
    {
        if (!AssetDatabase.IsValidFolder(parent + "/" + folder))
        {
            AssetDatabase.CreateFolder(parent, folder);
        }
    }

    private static TMP_FontAsset EnsureSciFiTypography()
    {
        if (cachedSciFiFont != null)
        {
            return cachedSciFiFont;
        }

        EnsureTextMeshProResources();
        EnsureFolder("Assets", "Fonts");

        TMP_FontAsset fontAsset = AssetDatabase.LoadAssetAtPath<TMP_FontAsset>(SciFiFontAssetPath);
        if (fontAsset == null)
        {
            Font sourceFont = EnsureSciFiSourceFont();
            if (sourceFont != null)
            {
                fontAsset = TMP_FontAsset.CreateFontAsset(sourceFont);
                if (fontAsset != null)
                {
                    fontAsset.name = "Teko-Bold SDF";
                    fontAsset.atlasPopulationMode = AtlasPopulationMode.Dynamic;
                    AssetDatabase.CreateAsset(fontAsset, SciFiFontAssetPath);

                    if (fontAsset.atlasTextures != null && fontAsset.atlasTextures.Length > 0)
                    {
                        fontAsset.atlasTextures[0].name = "Teko-Bold Atlas";
                        AssetDatabase.AddObjectToAsset(fontAsset.atlasTextures[0], fontAsset);
                    }

                    if (fontAsset.material != null)
                    {
                        fontAsset.material.name = "Teko-Bold Atlas Material";
                        AssetDatabase.AddObjectToAsset(fontAsset.material, fontAsset);
                    }

                    EditorUtility.SetDirty(fontAsset);
                    AssetDatabase.SaveAssets();
                }
            }
        }

        if (fontAsset == null)
        {
            fontAsset = Resources.Load<TMP_FontAsset>("Fonts & Materials/LiberationSans SDF");
            Debug.LogWarning("Sci-fi display font could not be installed. Falling back to the TMP default font.");
        }

        SetDefaultTMPFont(fontAsset);
        cachedSciFiFont = fontAsset;
        return fontAsset;
    }

    private static void EnsureTextMeshProResources()
    {
        if (AssetDatabase.LoadAssetAtPath<TMP_Settings>(TmpSettingsPath) != null)
        {
            return;
        }

        UPMInfo package = UPMInfo.FindForAssembly(typeof(TMP_Text).Assembly);
        if (package == null)
        {
            Debug.LogWarning("TextMeshPro package resources could not be located.");
            return;
        }

        string resourcesPackage = Path.Combine(package.resolvedPath, "Package Resources", "TMP Essential Resources.unitypackage");
        if (!File.Exists(resourcesPackage))
        {
            Debug.LogWarning("TextMeshPro Essential Resources package could not be located.");
            return;
        }

        AssetDatabase.ImportPackage(resourcesPackage, false);
        AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport);
    }

    private static Font EnsureSciFiSourceFont()
    {
        Font sourceFont = AssetDatabase.LoadAssetAtPath<Font>(SciFiFontPath);
        if (sourceFont != null)
        {
            return sourceFont;
        }

        UPMInfo coveragePackage = null;
        foreach (UPMInfo package in UPMInfo.GetAllRegisteredPackages())
        {
            if (package.name == "com.unity.testtools.codecoverage")
            {
                coveragePackage = package;
                break;
            }
        }

        if (coveragePackage == null)
        {
            Debug.LogWarning("Teko source font package could not be located.");
            return null;
        }

        string packagedFont = Path.Combine(
            coveragePackage.resolvedPath,
            "Samples~",
            "Tutorial",
            "Asteroids",
            "Visuals",
            "UI",
            "Fonts",
            "Teko-Bold.ttf");
        if (!File.Exists(packagedFont))
        {
            Debug.LogWarning("Teko source font file could not be located.");
            return null;
        }

        FileUtil.CopyFileOrDirectory(packagedFont, SciFiFontPath);
        AssetDatabase.ImportAsset(SciFiFontPath, ImportAssetOptions.ForceSynchronousImport);
        return AssetDatabase.LoadAssetAtPath<Font>(SciFiFontPath);
    }

    private static void SetDefaultTMPFont(TMP_FontAsset fontAsset)
    {
        if (fontAsset == null)
        {
            return;
        }

        TMP_Settings settings = AssetDatabase.LoadAssetAtPath<TMP_Settings>(TmpSettingsPath);
        if (settings == null)
        {
            return;
        }

        SerializedObject serializedSettings = new SerializedObject(settings);
        SerializedProperty defaultFont = serializedSettings.FindProperty("m_defaultFontAsset");
        if (defaultFont != null)
        {
            defaultFont.objectReferenceValue = fontAsset;
            serializedSettings.ApplyModifiedPropertiesWithoutUndo();
            EditorUtility.SetDirty(settings);
            AssetDatabase.SaveAssets();
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
        material.SetFloat("_Glossiness", transparent ? 0.88f : 0.52f);
        material.SetFloat("_Metallic", transparent ? 0f : 0.18f);

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
        else
        {
            material.SetFloat("_Mode", 0f);
            material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
            material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.Zero);
            material.SetInt("_ZWrite", 1);
            material.DisableKeyword("_ALPHATEST_ON");
            material.DisableKeyword("_ALPHABLEND_ON");
            material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
            material.renderQueue = -1;
        }

        return material;
    }

    private static Material ImportedMaterial(string assetPath, Material fallback)
    {
        Material imported = AssetDatabase.LoadAssetAtPath<Material>(assetPath);
        return imported == null ? fallback : imported;
    }

    private static void CreateLighting(Material cyan, Material red, Material green)
    {
        RenderSettings.ambientLight = new Color(0.27f, 0.31f, 0.36f);
        RenderSettings.reflectionIntensity = 0.65f;

        GameObject sun = new GameObject("Soft Orbital Directional Light");
        Light sunLight = sun.AddComponent<Light>();
        sunLight.type = LightType.Directional;
        sunLight.intensity = 0.58f;
        sunLight.shadows = LightShadows.Soft;
        sun.transform.rotation = Quaternion.Euler(42f, -30f, 0f);

        PointLight("Pod Bay Cold Light", new Vector3(0f, 3.2f, 17f), new Color(0.52f, 0.86f, 1f), 5.6f, 12f);
        PointLight("Main Corridor Light A", new Vector3(0f, 3.85f, 5f), new Color(0.52f, 0.86f, 1f), 4.4f, 11f);
        PointLight("Main Corridor Light B", new Vector3(0f, 3.85f, -12f), new Color(0.52f, 0.86f, 1f), 4.4f, 11f);
        PointLight("Main Corridor Light C", new Vector3(0f, 3.85f, -28f), new Color(1f, 0.72f, 0.18f), 3.8f, 10f);
        PointLight("Life Support Blue Light", new Vector3(-10.8f, 3f, 0f), new Color(0.5f, 0.88f, 1f), 4.2f, 11f);
        PointLight("Navigation Blue Light", new Vector3(10.8f, 3f, -4.4f), new Color(0.45f, 0.75f, 1f), 4f, 10f);
        PointLight("Medical Bay Green Light", new Vector3(-10.8f, 3f, -13.8f), new Color(0.42f, 1f, 0.66f), 3.6f, 10f);
        PointLight("Security Amber Light", new Vector3(10.8f, 3f, -17.5f), new Color(1f, 0.68f, 0.2f), 3.6f, 10f);
        PointLight("Reactor Warning Light", new Vector3(-10.8f, 3f, -28.2f), new Color(1f, 0.12f, 0.05f), 4.5f, 12f);
        PointLight("Comms Red Light", new Vector3(10.8f, 3f, -31.6f), new Color(1f, 0.2f, 0.1f), 3.8f, 10f);
        PointLight("Escape Pod Green Beacon", new Vector3(0f, 3.3f, -44f), new Color(0.25f, 1f, 0.55f), 5f, 12f);

        CeilingLight(new Vector3(0f, StationCeilingY - 0.18f, 17f), cyan);
        CeilingLight(new Vector3(0f, StationCeilingY - 0.18f, 5f), cyan);
        CeilingLight(new Vector3(0f, StationCeilingY - 0.18f, -12f), cyan);
        CeilingLight(new Vector3(0f, StationCeilingY - 0.18f, -28f), red);
        CeilingLight(new Vector3(0f, StationCeilingY - 0.18f, -44f), green);
    }

    private static void ApplyImportedSkybox()
    {
        Material skybox = AssetDatabase.LoadAssetAtPath<Material>(ImportedSkyboxPath);
        if (skybox == null)
        {
            return;
        }

        RenderSettings.skybox = skybox;
        RenderSettings.ambientMode = UnityEngine.Rendering.AmbientMode.Skybox;
        RenderSettings.ambientIntensity = 0.75f;
        DynamicGI.UpdateEnvironment();
    }

    private static void CreateStation(Material deck, Material wall, Material dark, Material yellow, Material cyan, Material red, Material orange, Material glass, Material hazard, Material whiteGlow)
    {
        Deck("Starting Pod Bay Deck", new Vector3(0f, -0.12f, 17f), new Vector3(10f, 0.24f, 8f), deck);
        Deck("Main Corridor Deck", new Vector3(0f, -0.12f, -12f), new Vector3(8f, 0.24f, 54f), deck);
        Deck("Life Support Deck", new Vector3(-10.8f, -0.1f, 0f), new Vector3(13f, 0.24f, 9f), deck);
        Deck("Navigation Deck", new Vector3(10.8f, -0.1f, -4.4f), new Vector3(13f, 0.24f, 9f), deck);
        Deck("Medical Bay Deck", new Vector3(-10.8f, -0.1f, -13.8f), new Vector3(13f, 0.24f, 8.8f), deck);
        Deck("Security Office Deck", new Vector3(10.8f, -0.1f, -17.5f), new Vector3(13f, 0.24f, 8.8f), deck);
        Deck("Reactor Deck", new Vector3(-10.8f, -0.1f, -28.2f), new Vector3(13f, 0.24f, 9f), deck);
        Deck("Comms Lab Deck", new Vector3(10.8f, -0.1f, -31.6f), new Vector3(13f, 0.24f, 9f), deck);
        Deck("Escape Bay Deck", new Vector3(0f, -0.1f, -44.2f), new Vector3(9f, 0.24f, 8.5f), deck);
        Deck("Sealed Outer Hull Floor", new Vector3(0f, -0.28f, -14f), new Vector3(38f, 0.16f, 74f), dark);

        Wall("Outer Hull North Boundary", new Vector3(0f, 1.55f, 23f), new Vector3(38f, 3.1f, 0.55f), wall, cyan);
        Wall("Outer Hull South Boundary", new Vector3(0f, 1.55f, -51f), new Vector3(38f, 3.1f, 0.55f), wall, cyan);
        Wall("Outer Hull West Boundary", new Vector3(-19f, 1.55f, -14f), new Vector3(0.55f, 3.1f, 74f), wall, cyan);
        Wall("Outer Hull East Boundary", new Vector3(19f, 1.55f, -14f), new Vector3(0.55f, 3.1f, 74f), wall, cyan);

        Wall("Pod Bay North Wall", new Vector3(0f, 1.55f, 21f), new Vector3(10f, 3.1f, 0.45f), wall, cyan);
        Wall("Pod Bay West Wall", new Vector3(-5f, 1.55f, 17f), new Vector3(0.45f, 3.1f, 8f), wall, cyan);
        Wall("Pod Bay East Wall", new Vector3(5f, 1.55f, 17f), new Vector3(0.45f, 3.1f, 8f), wall, cyan);
        Wall("Pod Bay South Wall Left", new Vector3(-3.8f, 1.55f, 13f), new Vector3(2.4f, 3.1f, 0.45f), wall, cyan);
        Wall("Pod Bay South Wall Right", new Vector3(3.8f, 1.55f, 13f), new Vector3(2.4f, 3.1f, 0.45f), wall, cyan);

        Wall("Pod Bay Connector West Guard", new Vector3(-4.2f, 1.55f, 12.55f), new Vector3(0.45f, 3.1f, 1.4f), wall, cyan);
        Wall("Pod Bay Connector East Guard", new Vector3(4.2f, 1.55f, 12.55f), new Vector3(0.45f, 3.1f, 1.4f), wall, cyan);
        Wall("Corridor West Wall A", new Vector3(-4.2f, 1.55f, 8.25f), new Vector3(0.45f, 3.1f, 8.5f), wall, cyan);
        Wall("Corridor East Wall A", new Vector3(4.2f, 1.55f, 8.25f), new Vector3(0.45f, 3.1f, 8.5f), wall, cyan);
        Wall("Corridor West Wall B", new Vector3(-4.2f, 1.55f, -7f), new Vector3(0.45f, 3.1f, 8f), wall, cyan);
        Wall("Corridor East Wall B", new Vector3(4.2f, 1.55f, -9.55f), new Vector3(0.45f, 3.1f, 5.7f), wall, cyan);
        Wall("Corridor West Wall C", new Vector3(-4.2f, 1.55f, -21.2f), new Vector3(0.45f, 3.1f, 10f), wall, cyan);
        Wall("Corridor East Wall C", new Vector3(4.2f, 1.55f, -24.6f), new Vector3(0.45f, 3.1f, 8.8f), wall, cyan);
        Wall("Corridor West Wall D", new Vector3(-4.2f, 1.55f, -35.2f), new Vector3(0.45f, 3.1f, 7f), wall, cyan);
        Wall("Corridor East Wall D", new Vector3(4.2f, 1.55f, -39.9f), new Vector3(0.45f, 3.1f, 5.2f), wall, cyan);

        CreateRoomWalls("Life Support", -10.8f, 0f, true, wall, cyan);
        CreateRoomWalls("Navigation", 10.8f, -4.4f, false, wall, cyan);
        CreateRoomWalls("Medical Bay", -10.8f, -13.8f, true, wall, cyan);
        CreateRoomWalls("Security Office", 10.8f, -17.5f, false, wall, cyan);
        CreateRoomWalls("Reactor", -10.8f, -28.2f, true, wall, cyan);
        CreateRoomWalls("Comms Lab", 10.8f, -31.6f, false, wall, cyan);

        Wall("Escape Bay Back Wall", new Vector3(0f, 1.55f, -48.5f), new Vector3(9f, 3.1f, 0.45f), wall, cyan);
        Wall("Escape Bay West Wall", new Vector3(-4.5f, 1.55f, -44.2f), new Vector3(0.45f, 3.1f, 8.5f), wall, cyan);
        Wall("Escape Bay East Wall", new Vector3(4.5f, 1.55f, -44.2f), new Vector3(0.45f, 3.1f, 8.5f), wall, cyan);
        CreateCeilings(wall, cyan, whiteGlow);

        if (!UseFullImportedCorridorShells)
        {
            for (int z = -46; z <= 20; z += 4)
            {
                Cube("Floor Rib Z " + z, new Vector3(0f, 0.025f, z), new Vector3(32f, 0.035f, 0.09f), dark);
            }

            for (int x = -16; x <= 16; x += 4)
            {
                Cube("Floor Rib X " + x, new Vector3(x, 0.035f, -13f), new Vector3(0.09f, 0.04f, 64f), dark);
            }
        }

        CreateFloorPath(cyan, yellow, red);
        CreatePipes(dark, cyan, orange);
        CreateWallPanelDetails(wall, dark, cyan, whiteGlow);
        CreateProps(dark, wall, yellow, glass, cyan, whiteGlow);

        CreateHazardLeak("Broken Corridor Vent", new Vector3(0f, 0.07f, -20.8f), new Vector3(3.2f, 0.12f, 2.4f), hazard);
        CreateHazardLeak("Reactor Radiation Leak", new Vector3(-9.2f, 0.07f, -29.8f), new Vector3(3.8f, 0.12f, 3.4f), hazard);
        CreateHazardLeak("Airlock Vent Leak", new Vector3(0f, 0.07f, -40.8f), new Vector3(2.8f, 0.12f, 2.2f), hazard);
    }

    private static void CreateImportedAssetDressing()
    {
        GameObject parentObject = new GameObject("Imported Asset Dressing");
        Transform parent = parentObject.transform;

        if (UseFullImportedCorridorShells)
        {
            for (float z = 20f; z >= -46f; z -= 3f)
            {
                PlacePrefabVisual(ModularCorridorPath, new Vector3(0f, ImportedModuleY, z), Quaternion.identity, Vector3.one, parent, "Imported Main Corridor Module " + z);
            }

            Vector3[] sideRoomPositions =
            {
                new Vector3(-10.8f, 0f, 0f),
                new Vector3(10.8f, 0f, -4.4f),
                new Vector3(-10.8f, 0f, -13.8f),
                new Vector3(10.8f, 0f, -17.5f),
                new Vector3(-10.8f, 0f, -28.2f),
                new Vector3(10.8f, 0f, -31.6f)
            };

            foreach (Vector3 position in sideRoomPositions)
            {
                Vector3 liftedPosition = position + new Vector3(0f, ImportedModuleY, 0f);
                PlacePrefabVisual(ModularCorridorPath, liftedPosition, Quaternion.Euler(0f, 90f, 0f), new Vector3(1.15f, 1f, 1.15f), parent, "Imported Side Room Shell");
                PlacePrefabVisual(ModularProfileEndPath, liftedPosition + new Vector3(0f, 0f, 2.8f), Quaternion.Euler(0f, 90f, 0f), Vector3.one, parent, "Imported Side Room End Cap");
            }

            PlacePrefabVisual(ModularWindowPath, new Vector3(0f, ImportedModuleY, 20.6f), Quaternion.identity, new Vector3(1.45f, 1.1f, 1f), parent, "Imported Observation Window Profile");
            PlacePrefabVisual(ModularProfileEndPath, new Vector3(0f, ImportedModuleY, 22.1f), Quaternion.identity, Vector3.one, parent, "Imported Pod Bay End Cap");
            PlacePrefabVisual(ModularProfileEndPath, new Vector3(0f, ImportedModuleY, -47.2f), Quaternion.Euler(0f, 180f, 0f), Vector3.one, parent, "Imported Escape End Cap");
        }

        float[] lightZ = { 17f, 8f, 1f, -7f, -15f, -23f, -31f, -39f, -44f };
        foreach (float z in lightZ)
        {
            PlacePrefabVisual(ModularLight1Path, new Vector3(-1.55f, 2.65f, z), Quaternion.identity, Vector3.one, parent, "Imported Left Light " + z);
            PlacePrefabVisual(ModularLight2Path, new Vector3(1.55f, 2.65f, z), Quaternion.identity, Vector3.one, parent, "Imported Right Light " + z);
        }

        float[] detailZ = { 17f, 6f, -6f, -16f, -28f, -40f };
        foreach (float z in detailZ)
        {
            PlacePrefabVisual(ModularWallDoorPath, new Vector3(-4.03f, 1.65f, z), Quaternion.Euler(0f, 90f, 0f), new Vector3(0.9f, 0.9f, 0.9f), parent, "Imported West Wall Detail " + z);
            PlacePrefabVisual(ModularWallDoorPath, new Vector3(4.03f, 1.65f, z + 2f), Quaternion.Euler(0f, -90f, 0f), new Vector3(0.9f, 0.9f, 0.9f), parent, "Imported East Wall Detail " + z);
        }

        PlacePrefabVisual(CosmicCratePath, new Vector3(2.6f, 0f, 4.2f), Quaternion.Euler(0f, 25f, 0f), Vector3.one, parent, "Imported Corridor Storage Crate");
        PlacePrefabVisual(CosmicCratePath, new Vector3(-2.6f, 0f, -22f), Quaternion.Euler(0f, -18f, 0f), Vector3.one, parent, "Imported Broken Corridor Crate");
        PlacePrefabVisual(CosmicLockerPath, new Vector3(14.5f, 0f, -20.8f), Quaternion.Euler(0f, -90f, 0f), Vector3.one, parent, "Imported Security Locker");
        PlacePrefabVisual(CosmicLockerPath, new Vector3(-14.7f, 0f, -2.8f), Quaternion.Euler(0f, 90f, 0f), Vector3.one, parent, "Imported Life Support Locker");
        PlacePrefabVisual(CosmicTablePath, new Vector3(0f, 0f, 17f), Quaternion.identity, Vector3.one, parent, "Imported Pod Bay Table");
        PlacePrefabVisual(CosmicChairPath, new Vector3(-1.7f, 0f, 17.4f), Quaternion.Euler(0f, 25f, 0f), Vector3.one, parent, "Imported Pod Bay Chair");
        PlacePrefabVisual(CosmicChairPath, new Vector3(1.8f, 0f, 16.5f), Quaternion.Euler(0f, -35f, 0f), Vector3.one, parent, "Imported Pod Bay Chair B");
        PlacePrefabVisual(CosmicMonitorPath, new Vector3(15f, 1.15f, -1.4f), Quaternion.Euler(0f, -90f, 0f), Vector3.one, parent, "Imported Navigation Monitor");
        PlacePrefabVisual(CosmicMonitorPath, new Vector3(-15f, 1.15f, -12.6f), Quaternion.Euler(0f, 90f, 0f), Vector3.one, parent, "Imported Medical Monitor");
        PlacePrefabVisual(ModularBoxSmallPath, new Vector3(13.5f, 0f, -3.4f), Quaternion.Euler(0f, 35f, 0f), Vector3.one, parent, "Imported Small Cargo Box");
        PlacePrefabVisual(ModularBoxLargePath, new Vector3(-13.3f, 0f, -27.4f), Quaternion.Euler(0f, -20f, 0f), Vector3.one, parent, "Imported Large Cargo Box");
        PlacePrefabVisual(ModularBoxSmallPath, new Vector3(-2.8f, 0f, -34.2f), Quaternion.Euler(0f, -30f, 0f), Vector3.one, parent, "Imported Corridor Equipment Box");
        PlacePrefabVisual(ModularBoxLargePath, new Vector3(2.9f, 0f, -42.3f), Quaternion.Euler(0f, 22f, 0f), Vector3.one, parent, "Imported Escape Bay Cargo Box");
    }

    private static void CreateEndlessSurvivalArena(Material deck, Material wall, Material dark, Material yellow, Material cyan, Material green, Material red, Material orange, Material glass, Material hazard, Material whiteGlow)
    {
        Vector3 previousLookAt = labelLookAt;
        labelLookAt = EndlessStart;
        CreateSpawnMarker("Endless Survival Spawn", EndlessStart, Quaternion.Euler(0f, 180f, 0f));

        GameObject root = new GameObject("Endless Survival Map");

        Deck("Endless Outer Safety Floor", new Vector3(70f, -0.3f, 0f), new Vector3(54f, 0.16f, 72f), dark).transform.SetParent(root.transform);
        Deck("Endless North Ring Deck", new Vector3(70f, -0.1f, 14f), new Vector3(28f, 0.24f, 5f), deck).transform.SetParent(root.transform);
        Deck("Endless South Ring Deck", new Vector3(70f, -0.1f, -14f), new Vector3(28f, 0.24f, 5f), deck).transform.SetParent(root.transform);
        Deck("Endless West Ring Deck", new Vector3(56f, -0.1f, 0f), new Vector3(5f, 0.24f, 28f), deck).transform.SetParent(root.transform);
        Deck("Endless East Ring Deck", new Vector3(84f, -0.1f, 0f), new Vector3(5f, 0.24f, 28f), deck).transform.SetParent(root.transform);
        Deck("Endless Central Relay Deck", new Vector3(70f, -0.1f, 0f), new Vector3(12f, 0.24f, 12f), deck).transform.SetParent(root.transform);
        Deck("Endless Data Vault Deck", new Vector3(70f, -0.1f, 27f), new Vector3(16f, 0.24f, 12f), deck).transform.SetParent(root.transform);
        Deck("Endless Reactor Annex Deck", new Vector3(70f, -0.1f, -27f), new Vector3(16f, 0.24f, 12f), deck).transform.SetParent(root.transform);
        Deck("Endless Supply Annex Deck", new Vector3(43f, -0.1f, 0f), new Vector3(12f, 0.24f, 16f), deck).transform.SetParent(root.transform);
        Deck("Endless Security Annex Deck", new Vector3(97f, -0.1f, 0f), new Vector3(12f, 0.24f, 16f), deck).transform.SetParent(root.transform);

        Wall("Endless North Hull Left", new Vector3(57.2f, 1.55f, 19.5f), new Vector3(11.4f, 3.1f, 0.55f), wall, cyan);
        Wall("Endless North Hull Right", new Vector3(82.8f, 1.55f, 19.5f), new Vector3(11.4f, 3.1f, 0.55f), wall, cyan);
        Wall("Endless South Hull Left", new Vector3(57.2f, 1.55f, -19.5f), new Vector3(11.4f, 3.1f, 0.55f), wall, cyan);
        Wall("Endless South Hull Right", new Vector3(82.8f, 1.55f, -19.5f), new Vector3(11.4f, 3.1f, 0.55f), wall, cyan);
        Wall("Endless West Hull North", new Vector3(51.5f, 1.55f, 11.8f), new Vector3(0.55f, 3.1f, 15.4f), wall, cyan);
        Wall("Endless West Hull South", new Vector3(51.5f, 1.55f, -11.8f), new Vector3(0.55f, 3.1f, 15.4f), wall, cyan);
        Wall("Endless East Hull North", new Vector3(88.5f, 1.55f, 11.8f), new Vector3(0.55f, 3.1f, 15.4f), wall, cyan);
        Wall("Endless East Hull South", new Vector3(88.5f, 1.55f, -11.8f), new Vector3(0.55f, 3.1f, 15.4f), wall, cyan);

        Wall("Endless Core North Wall", new Vector3(70f, 1.55f, 6.2f), new Vector3(10.8f, 3.1f, 0.45f), dark, red);
        Wall("Endless Core South Wall", new Vector3(70f, 1.55f, -6.2f), new Vector3(10.8f, 3.1f, 0.45f), dark, red);
        Wall("Endless Core West Wall", new Vector3(64.6f, 1.55f, 0f), new Vector3(0.45f, 3.1f, 12.4f), dark, red);
        Wall("Endless Core East Wall", new Vector3(75.4f, 1.55f, 0f), new Vector3(0.45f, 3.1f, 12.4f), dark, red);
        Cube("Endless Core Radiation Glow", new Vector3(70f, 0.12f, 0f), new Vector3(8.4f, 0.08f, 8.4f), hazard).transform.SetParent(root.transform);
        Cube("Endless Core Reactor Block", new Vector3(70f, 1.2f, 0f), new Vector3(4.4f, 2.4f, 4.4f), dark).transform.SetParent(root.transform);
        Cube("Endless Core Red Band", new Vector3(70f, 1.75f, 0f), new Vector3(4.8f, 0.18f, 4.8f), red).transform.SetParent(root.transform);
        WorldLabel("ENDLESS CORE\nLoop repairs for score", new Vector3(70f, 3.25f, 7.25f), EndlessStart, Color.cyan, 0.011f).transform.SetParent(root.transform);

        CreateEndlessRoomWalls("Endless Data Vault", 70f, 27f, 16f, 12f, "south", wall, cyan);
        CreateEndlessRoomWalls("Endless Reactor Annex", 70f, -27f, 16f, 12f, "north", wall, cyan);
        CreateEndlessRoomWalls("Endless Supply Annex", 43f, 0f, 12f, 16f, "east", wall, cyan);
        CreateEndlessRoomWalls("Endless Security Annex", 97f, 0f, 12f, 16f, "west", wall, cyan);

        CreateDoor("Endless Data Vault Gate", new Vector3(70f, 1.55f, 20.75f), Quaternion.identity, true, 2, wall, cyan, green, doorWidth: 5.2f);
        CreateDoor("Endless Supply Gate", new Vector3(50.8f, 1.55f, 0f), Quaternion.Euler(0f, 90f, 0f), true, 3, wall, cyan, yellow, doorWidth: 5.2f);
        CreateDoor("Endless Security Gate", new Vector3(89.2f, 1.55f, 0f), Quaternion.Euler(0f, 90f, 0f), true, 4, wall, cyan, orange, doorWidth: 5.2f);
        CreateDoor("Endless Reactor Gate", new Vector3(70f, 1.55f, -20.75f), Quaternion.identity, true, 5, wall, cyan, red, doorWidth: 5.2f);

        for (int z = -16; z <= 16; z += 4)
        {
            Cube("Endless West Ring Light " + z, new Vector3(55.8f, 3.65f, z), new Vector3(0.2f, 0.08f, 2.2f), whiteGlow).transform.SetParent(root.transform);
            Cube("Endless East Ring Light " + z, new Vector3(84.2f, 3.65f, z), new Vector3(0.2f, 0.08f, 2.2f), whiteGlow).transform.SetParent(root.transform);
        }

        for (int x = 56; x <= 84; x += 4)
        {
            Cube("Endless North Ring Light " + x, new Vector3(x, 3.65f, 14f), new Vector3(2.2f, 0.08f, 0.2f), whiteGlow).transform.SetParent(root.transform);
            Cube("Endless South Ring Light " + x, new Vector3(x, 3.65f, -14f), new Vector3(2.2f, 0.08f, 0.2f), whiteGlow).transform.SetParent(root.transform);
        }

        CreateEndlessCeiling(new Vector3(70f, 4.75f, 14f), new Vector3(28f, 0.12f, 5.2f), wall, cyan);
        CreateEndlessCeiling(new Vector3(70f, 4.75f, -14f), new Vector3(28f, 0.12f, 5.2f), wall, cyan);
        CreateEndlessCeiling(new Vector3(56f, 4.75f, 0f), new Vector3(5.2f, 0.12f, 28f), wall, cyan);
        CreateEndlessCeiling(new Vector3(84f, 4.75f, 0f), new Vector3(5.2f, 0.12f, 28f), wall, cyan);
        CreateEndlessCeiling(new Vector3(70f, 4.75f, 27f), new Vector3(16f, 0.12f, 12f), wall, cyan);
        CreateEndlessCeiling(new Vector3(70f, 4.75f, -27f), new Vector3(16f, 0.12f, 12f), wall, cyan);
        CreateEndlessCeiling(new Vector3(43f, 4.75f, 0f), new Vector3(12f, 0.12f, 16f), wall, cyan);
        CreateEndlessCeiling(new Vector3(97f, 4.75f, 0f), new Vector3(12f, 0.12f, 16f), wall, cyan);

        Cube("Endless North Guide Path", new Vector3(70f, 0.06f, 14f), new Vector3(24f, 0.04f, 0.22f), cyan).transform.SetParent(root.transform);
        Cube("Endless South Guide Path", new Vector3(70f, 0.06f, -14f), new Vector3(24f, 0.04f, 0.22f), cyan).transform.SetParent(root.transform);
        Cube("Endless West Guide Path", new Vector3(56f, 0.06f, 0f), new Vector3(0.22f, 0.04f, 24f), cyan).transform.SetParent(root.transform);
        Cube("Endless East Guide Path", new Vector3(84f, 0.06f, 0f), new Vector3(0.22f, 0.04f, 24f), cyan).transform.SetParent(root.transform);

        CreateTerminal(new Vector3(70f, 0f, 13.25f), "Endless Relay Loop", 3.6f, cyan, wall, green, yellow, KeyCode.Alpha1, KeyCode.Alpha3, KeyCode.Alpha2);
        CreateTerminal(new Vector3(56f, 0f, 12.1f), "Endless Life Support Valve", 4.2f, cyan, wall, green, yellow, KeyCode.Alpha2, KeyCode.Alpha1, KeyCode.Alpha4);
        CreateTerminal(new Vector3(84f, 0f, -12.1f), "Endless Navigation Buffer", 4.4f, cyan, wall, green, yellow, KeyCode.Alpha4, KeyCode.Alpha2, KeyCode.Alpha1);
        CreateTerminal(new Vector3(70f, 0f, -13.25f), "Endless Comms Burst", 4.1f, cyan, wall, green, yellow, KeyCode.Alpha1, KeyCode.Alpha4, KeyCode.Alpha3);
        CreateTerminal(new Vector3(70f, 0f, 27f), "Endless Data Vault Relay", 4.8f, cyan, wall, green, yellow, KeyCode.Alpha3, KeyCode.Alpha1, KeyCode.Alpha2);
        CreateTerminal(new Vector3(43f, 0f, 0f), "Endless Medical Mixer", 4.6f, cyan, wall, green, yellow, KeyCode.Alpha2, KeyCode.Alpha4, KeyCode.Alpha3);
        CreateTerminal(new Vector3(97f, 0f, 0f), "Endless Security Scrambler", 5f, cyan, wall, green, yellow, KeyCode.Alpha3, KeyCode.Alpha2, KeyCode.Alpha4);
        CreateTerminal(new Vector3(70f, 0f, -27f), "Endless Reactor Drain", 5.2f, cyan, wall, green, yellow, KeyCode.Alpha4, KeyCode.Alpha1, KeyCode.Alpha3, KeyCode.Alpha2);

        CreateHazardLeak("Endless Vent Leak North West", new Vector3(58.5f, 0.22f, 14f), new Vector3(2.8f, 0.18f, 2.8f), hazard);
        CreateHazardLeak("Endless Vent Leak South East", new Vector3(81.5f, 0.22f, -14f), new Vector3(2.8f, 0.18f, 2.8f), hazard);
        CreateHazardLeak("Endless Core Side Leak", new Vector3(76.8f, 0.22f, 0f), new Vector3(2.2f, 0.18f, 3.2f), hazard);

        CreateOxygenCanister(new Vector3(60f, 0.75f, 16.4f), cyan, yellow);
        CreateOxygenCanister(new Vector3(80f, 0.75f, -16.4f), cyan, yellow);
        CreateOxygenCanister(new Vector3(43f, 0.75f, 5.8f), cyan, yellow);
        CreateOxygenCanister(new Vector3(97f, 0.75f, -5.8f), cyan, yellow);
        CreateConsumablePickup(new Vector3(60.5f, 0.65f, -16.1f), "battery", "Battery Pack", cyan, yellow);
        CreateConsumablePickup(new Vector3(79.5f, 0.65f, 16.1f), "battery", "Battery Pack", cyan, yellow);
        CreateConsumablePickup(new Vector3(70f, 0.65f, 31.2f), "medkit", "Medkit", red, yellow);
        CreateConsumablePickup(new Vector3(70f, 0.65f, -31.2f), "medkit", "Medkit", red, yellow);

        CreateEndlessProps(dark, wall, yellow, cyan, orange, glass, whiteGlow);
        CreateEndlessImportedVisuals(root.transform);

        CreateSecurityRobot(new Vector3(56f, 0.75f, 14f), dark, red, cyan,
            new Vector3(56f, 0.75f, 14f), new Vector3(84f, 0.75f, 14f), new Vector3(84f, 0.75f, -14f), new Vector3(56f, 0.75f, -14f));
        CreateSecurityRobot(new Vector3(84f, 0.75f, 6f), dark, red, orange,
            new Vector3(84f, 0.75f, 12f), new Vector3(70f, 0.75f, 14f), new Vector3(56f, 0.75f, 12f), new Vector3(56f, 0.75f, -12f), new Vector3(84f, 0.75f, -12f));
        CreateSecurityRobot(new Vector3(97f, 0.75f, 4f), dark, red, red,
            new Vector3(97f, 0.75f, 5.5f), new Vector3(97f, 0.75f, -5.5f), new Vector3(93f, 0.75f, -5.5f), new Vector3(93f, 0.75f, 5.5f));

        PointLight("Endless Hub Cold Light", new Vector3(70f, 3.8f, 12f), new Color(0.45f, 0.88f, 1f), 4.8f, 12f);
        PointLight("Endless Core Red Light", new Vector3(70f, 3.8f, 0f), new Color(1f, 0.12f, 0.05f), 5.6f, 14f);
        PointLight("Endless Vault Blue Light", new Vector3(70f, 3.5f, 27f), new Color(0.38f, 0.8f, 1f), 4f, 11f);
        PointLight("Endless Reactor Amber Light", new Vector3(70f, 3.5f, -27f), new Color(1f, 0.48f, 0.08f), 4.6f, 12f);

        labelLookAt = previousLookAt;
    }

    private static void CreateEndlessRoomWalls(string name, float centerX, float centerZ, float width, float depth, string openSide, Material wall, Material cyan)
    {
        float halfWidth = width * 0.5f;
        float halfDepth = depth * 0.5f;
        float gap = 5.8f;
        float horizontalSegment = Mathf.Max(0.5f, (width - gap) * 0.5f);
        float verticalSegment = Mathf.Max(0.5f, (depth - gap) * 0.5f);

        if (openSide == "north")
        {
            Wall(name + " North Wall Left", new Vector3(centerX - (gap * 0.5f + horizontalSegment * 0.5f), 1.55f, centerZ + halfDepth), new Vector3(horizontalSegment, 3.1f, 0.45f), wall, cyan);
            Wall(name + " North Wall Right", new Vector3(centerX + (gap * 0.5f + horizontalSegment * 0.5f), 1.55f, centerZ + halfDepth), new Vector3(horizontalSegment, 3.1f, 0.45f), wall, cyan);
        }
        else
        {
            Wall(name + " North Wall", new Vector3(centerX, 1.55f, centerZ + halfDepth), new Vector3(width, 3.1f, 0.45f), wall, cyan);
        }

        if (openSide == "south")
        {
            Wall(name + " South Wall Left", new Vector3(centerX - (gap * 0.5f + horizontalSegment * 0.5f), 1.55f, centerZ - halfDepth), new Vector3(horizontalSegment, 3.1f, 0.45f), wall, cyan);
            Wall(name + " South Wall Right", new Vector3(centerX + (gap * 0.5f + horizontalSegment * 0.5f), 1.55f, centerZ - halfDepth), new Vector3(horizontalSegment, 3.1f, 0.45f), wall, cyan);
        }
        else
        {
            Wall(name + " South Wall", new Vector3(centerX, 1.55f, centerZ - halfDepth), new Vector3(width, 3.1f, 0.45f), wall, cyan);
        }

        if (openSide == "west")
        {
            Wall(name + " West Wall North", new Vector3(centerX - halfWidth, 1.55f, centerZ + (gap * 0.5f + verticalSegment * 0.5f)), new Vector3(0.45f, 3.1f, verticalSegment), wall, cyan);
            Wall(name + " West Wall South", new Vector3(centerX - halfWidth, 1.55f, centerZ - (gap * 0.5f + verticalSegment * 0.5f)), new Vector3(0.45f, 3.1f, verticalSegment), wall, cyan);
        }
        else
        {
            Wall(name + " West Wall", new Vector3(centerX - halfWidth, 1.55f, centerZ), new Vector3(0.45f, 3.1f, depth), wall, cyan);
        }

        if (openSide == "east")
        {
            Wall(name + " East Wall North", new Vector3(centerX + halfWidth, 1.55f, centerZ + (gap * 0.5f + verticalSegment * 0.5f)), new Vector3(0.45f, 3.1f, verticalSegment), wall, cyan);
            Wall(name + " East Wall South", new Vector3(centerX + halfWidth, 1.55f, centerZ - (gap * 0.5f + verticalSegment * 0.5f)), new Vector3(0.45f, 3.1f, verticalSegment), wall, cyan);
        }
        else
        {
            Wall(name + " East Wall", new Vector3(centerX + halfWidth, 1.55f, centerZ), new Vector3(0.45f, 3.1f, depth), wall, cyan);
        }
    }

    private static void CreateEndlessCeiling(Vector3 position, Vector3 scale, Material wall, Material cyan)
    {
        Cube("Endless Ceiling Panel", position, scale, wall);
        Cube("Endless Ceiling Blue Track", position + new Vector3(0f, -0.08f, 0f), new Vector3(Mathf.Max(0.18f, scale.x * 0.62f), 0.05f, Mathf.Max(0.18f, scale.z * 0.08f)), cyan);
    }

    private static void CreateEndlessProps(Material dark, Material wall, Material yellow, Material cyan, Material orange, Material glass, Material whiteGlow)
    {
        Cube("Endless Hub Relay Console", new Vector3(70f, 0.7f, 8.4f), new Vector3(2.4f, 1.4f, 1.2f), dark);
        Cube("Endless Hub Relay Screen", new Vector3(70f, 1.35f, 7.85f), new Vector3(1.8f, 0.65f, 0.08f), cyan);
        Cube("Endless North Cargo Cover", new Vector3(60.5f, 0.55f, 12f), new Vector3(2.2f, 1.1f, 1.1f), wall);
        Cube("Endless East Cargo Cover", new Vector3(82.5f, 0.55f, 8f), new Vector3(1.1f, 1.1f, 2.2f), wall);
        Cube("Endless South Cargo Cover", new Vector3(79.5f, 0.55f, -12f), new Vector3(2.2f, 1.1f, 1.1f), wall);
        Cube("Endless West Cargo Cover", new Vector3(57.5f, 0.55f, -8f), new Vector3(1.1f, 1.1f, 2.2f), wall);
        Cube("Endless Data Rack A", new Vector3(65.2f, 1f, 30f), new Vector3(1.2f, 2f, 2.4f), dark);
        Cube("Endless Data Rack B", new Vector3(74.8f, 1f, 30f), new Vector3(1.2f, 2f, 2.4f), dark);
        Cube("Endless Medical Pod", new Vector3(39.8f, 0.55f, 3.2f), new Vector3(2.1f, 0.55f, 1.05f), glass);
        Cube("Endless Security Wall Monitor", new Vector3(101f, 1.55f, -3f), new Vector3(0.12f, 1.5f, 2.2f), orange);
        Cube("Endless Reactor Capacitor A", new Vector3(65.4f, 1f, -30.3f), new Vector3(1.2f, 2f, 1.2f), dark);
        Cube("Endless Reactor Capacitor B", new Vector3(74.6f, 1f, -30.3f), new Vector3(1.2f, 2f, 1.2f), dark);
        Cube("Endless Supply Yellow Crate", new Vector3(43f, 0.5f, -5.4f), new Vector3(1.6f, 1f, 1.1f), yellow);
        Cube("Endless Supply Blue Crate", new Vector3(46f, 0.45f, 5.1f), new Vector3(1.3f, 0.9f, 1.2f), cyan);
        PointLight("Endless Console Glow", new Vector3(70f, 2.2f, 8f), new Color(0.05f, 0.9f, 1f), 2.8f, 6f);
        PointLight("Endless Medical Green Glow", new Vector3(41f, 2.2f, 3f), new Color(0.25f, 1f, 0.55f), 2.6f, 6f);
        PointLight("Endless Security Orange Glow", new Vector3(97f, 2.5f, 0f), new Color(1f, 0.52f, 0.08f), 3.3f, 7f);
        PointLight("Endless Reactor Warning Glow", new Vector3(70f, 2.6f, -30f), new Color(1f, 0.15f, 0.05f), 4.2f, 9f);
    }

    private static void CreateEndlessImportedVisuals(Transform parent)
    {
        if (!UseImportedEnvironmentVisuals)
        {
            return;
        }

        for (float x = 58f; x <= 82f; x += 6f)
        {
            PlacePrefabVisual(ModularCorridorPath, new Vector3(x, ImportedModuleY, 14f), Quaternion.Euler(0f, 90f, 0f), new Vector3(1.05f, 1f, 1.05f), parent, "Imported Endless North Module " + x);
            PlacePrefabVisual(ModularCorridorPath, new Vector3(x, ImportedModuleY, -14f), Quaternion.Euler(0f, 90f, 0f), new Vector3(1.05f, 1f, 1.05f), parent, "Imported Endless South Module " + x);
        }

        for (float z = -10f; z <= 10f; z += 5f)
        {
            PlacePrefabVisual(ModularWallDoorPath, new Vector3(51.85f, 1.65f, z), Quaternion.Euler(0f, 90f, 0f), Vector3.one, parent, "Imported Endless West Detail " + z);
            PlacePrefabVisual(ModularWallDoorPath, new Vector3(88.15f, 1.65f, z), Quaternion.Euler(0f, -90f, 0f), Vector3.one, parent, "Imported Endless East Detail " + z);
            PlacePrefabVisual(ModularLight1Path, new Vector3(55.7f, 2.65f, z), Quaternion.identity, Vector3.one, parent, "Imported Endless West Light " + z);
            PlacePrefabVisual(ModularLight2Path, new Vector3(84.3f, 2.65f, z), Quaternion.identity, Vector3.one, parent, "Imported Endless East Light " + z);
        }

        PlacePrefabVisual(CosmicCratePath, new Vector3(60.5f, 0f, 12f), Quaternion.Euler(0f, 35f, 0f), Vector3.one, parent, "Imported Endless North Crate");
        PlacePrefabVisual(CosmicCratePath, new Vector3(79.5f, 0f, -12f), Quaternion.Euler(0f, -20f, 0f), Vector3.one, parent, "Imported Endless South Crate");
        PlacePrefabVisual(CosmicLockerPath, new Vector3(101.2f, 0f, 3.4f), Quaternion.Euler(0f, -90f, 0f), Vector3.one, parent, "Imported Endless Security Locker");
        PlacePrefabVisual(CosmicMonitorPath, new Vector3(70f, 1.15f, 31.5f), Quaternion.Euler(0f, 180f, 0f), Vector3.one, parent, "Imported Endless Data Monitor");
        PlacePrefabVisual(CosmicTablePath, new Vector3(43.5f, 0f, -2.8f), Quaternion.Euler(0f, 20f, 0f), Vector3.one, parent, "Imported Endless Supply Table");
    }

    private static bool IsNearSideDoorway(float z)
    {
        float[] sideDoorwayZ = { 0f, -4.4f, -13.8f, -17.5f, -28.2f, -31.6f };
        foreach (float doorZ in sideDoorwayZ)
        {
            if (Mathf.Abs(z - doorZ) < 4f)
            {
                return true;
            }
        }

        return false;
    }

    private static void CreateCeilings(Material wall, Material cyan, Material whiteGlow)
    {
        if (UseFullImportedCorridorShells)
        {
            for (int z = -44; z <= 20; z += 6)
            {
                Cube("Corridor Soft Light Panel " + z, new Vector3(0f, StationCeilingY - 0.24f, z), new Vector3(0.85f, 0.045f, 0.18f), whiteGlow);
            }

            return;
        }

        Cube("Pod Bay Clean Ceiling", new Vector3(0f, StationCeilingY, 17f), new Vector3(10f, 0.12f, 8f), wall);
        Cube("Main Corridor Clean Ceiling", new Vector3(0f, StationCeilingY, -12f), new Vector3(8f, 0.12f, 54f), wall);
        Cube("Life Support Clean Ceiling", new Vector3(-10.8f, StationCeilingY, 0f), new Vector3(13f, 0.12f, 9f), wall);
        Cube("Navigation Clean Ceiling", new Vector3(10.8f, StationCeilingY, -4.4f), new Vector3(13f, 0.12f, 9f), wall);
        Cube("Medical Bay Clean Ceiling", new Vector3(-10.8f, StationCeilingY, -13.8f), new Vector3(13f, 0.12f, 8.8f), wall);
        Cube("Security Office Clean Ceiling", new Vector3(10.8f, StationCeilingY, -17.5f), new Vector3(13f, 0.12f, 8.8f), wall);
        Cube("Reactor Clean Ceiling", new Vector3(-10.8f, StationCeilingY, -28.2f), new Vector3(13f, 0.12f, 9f), wall);
        Cube("Comms Clean Ceiling", new Vector3(10.8f, StationCeilingY, -31.6f), new Vector3(13f, 0.12f, 9f), wall);
        Cube("Escape Bay Clean Ceiling", new Vector3(0f, StationCeilingY, -44.2f), new Vector3(9f, 0.12f, 8.5f), wall);

        for (int z = -44; z <= 20; z += 6)
        {
            Cube("Corridor Ceiling Blue Rail L " + z, new Vector3(-1.45f, StationCeilingY - 0.14f, z), new Vector3(1.25f, 0.08f, 0.16f), cyan);
            Cube("Corridor Ceiling Blue Rail R " + z, new Vector3(1.45f, StationCeilingY - 0.14f, z), new Vector3(1.25f, 0.08f, 0.16f), cyan);
            Cube("Corridor Soft Light Panel " + z, new Vector3(0f, StationCeilingY - 0.16f, z), new Vector3(0.9f, 0.07f, 0.22f), whiteGlow);
        }
    }

    private static void CreateWallPanelDetails(Material wall, Material dark, Material cyan, Material whiteGlow)
    {
        if (UseFullImportedCorridorShells)
        {
            return;
        }

        for (int z = -42; z <= 18; z += 6)
        {
            Cube("West Corridor White Wall Module " + z, new Vector3(-3.955f, 1.55f, z), new Vector3(0.06f, 1.55f, 2.2f), wall);
            Cube("East Corridor White Wall Module " + z, new Vector3(3.955f, 1.55f, z), new Vector3(0.06f, 1.55f, 2.2f), wall);
            Cube("West Corridor Blue Wall Strip " + z, new Vector3(-3.92f, 1.92f, z), new Vector3(0.07f, 0.08f, 1.65f), cyan);
            Cube("East Corridor Blue Wall Strip " + z, new Vector3(3.92f, 1.92f, z), new Vector3(0.07f, 0.08f, 1.65f), cyan);
            Cube("West Corridor Vent Slot " + z, new Vector3(-3.91f, 0.72f, z), new Vector3(0.06f, 0.22f, 1.35f), dark);
            Cube("East Corridor Vent Slot " + z, new Vector3(3.91f, 0.72f, z), new Vector3(0.06f, 0.22f, 1.35f), dark);
        }

        float[] roomCenters = { 0f, -4.4f, -13.8f, -17.5f, -28.2f, -31.6f };
        foreach (float z in roomCenters)
        {
            Cube("Room Interior Light Band W " + z, new Vector3(-10.8f, 2.78f, z + 3.85f), new Vector3(4.2f, 0.08f, 0.12f), whiteGlow);
            Cube("Room Interior Light Band E " + z, new Vector3(10.8f, 2.78f, z + 3.85f), new Vector3(4.2f, 0.08f, 0.12f), whiteGlow);
        }
    }

    private static void CreateRoomWalls(string name, float centerX, float centerZ, bool westRoom, Material wall, Material cyan)
    {
        float outerX = westRoom ? centerX - 6.5f : centerX + 6.5f;
        float doorSideX = westRoom ? centerX + 6.5f : centerX - 6.5f;
        float northZ = centerZ + 4.5f;
        float southZ = centerZ - 4.5f;

        Wall(name + " Outer Wall", new Vector3(outerX, 1.55f, centerZ), new Vector3(0.45f, 3.1f, 9f), wall, cyan);
        Wall(name + " North Wall", new Vector3(centerX, 1.55f, northZ), new Vector3(13f, 3.1f, 0.45f), wall, cyan);
        Wall(name + " South Wall", new Vector3(centerX, 1.55f, southZ), new Vector3(13f, 3.1f, 0.45f), wall, cyan);
        const float doorwayHalfGap = 2.65f;
        const float doorSideLength = 1.65f;
        float doorSideOffset = doorwayHalfGap + doorSideLength * 0.5f;
        Wall(name + " Door Side A", new Vector3(doorSideX, 1.55f, centerZ + doorSideOffset), new Vector3(0.45f, 3.1f, doorSideLength), wall, cyan);
        Wall(name + " Door Side B", new Vector3(doorSideX, 1.55f, centerZ - doorSideOffset), new Vector3(0.45f, 3.1f, doorSideLength), wall, cyan);
    }

    private static void CreateFloorPath(Material cyan, Material yellow, Material red)
    {
        Cube("Pod Bay Guide Path", new Vector3(0f, 0.055f, 15.8f), new Vector3(0.28f, 0.035f, 5f), cyan);
        Cube("Main Corridor Spine", new Vector3(0f, 0.055f, -13.5f), new Vector3(0.24f, 0.035f, 52f), cyan);
        Cube("Life Support Branch Path", new Vector3(-7.4f, 0.055f, 0f), new Vector3(6.2f, 0.035f, 0.18f), cyan);
        Cube("Navigation Branch Path", new Vector3(7.4f, 0.055f, -4.4f), new Vector3(6.2f, 0.035f, 0.18f), cyan);
        Cube("Medical Branch Path", new Vector3(-7.4f, 0.055f, -13.8f), new Vector3(6.2f, 0.035f, 0.18f), cyan);
        Cube("Security Branch Path", new Vector3(7.4f, 0.055f, -17.5f), new Vector3(6.2f, 0.035f, 0.18f), cyan);
        Cube("Reactor Branch Path", new Vector3(-7.4f, 0.055f, -28.2f), new Vector3(6.2f, 0.035f, 0.18f), yellow);
        Cube("Comms Branch Path", new Vector3(7.4f, 0.055f, -31.6f), new Vector3(6.2f, 0.035f, 0.18f), yellow);
        Cube("Escape Route", new Vector3(0f, 0.06f, -41.8f), new Vector3(0.32f, 0.04f, 5.4f), yellow);
        Cube("Danger Stripe Corridor A", new Vector3(-1.6f, 0.07f, -20.8f), new Vector3(0.16f, 0.04f, 2.4f), red);
        Cube("Danger Stripe Corridor B", new Vector3(1.6f, 0.07f, -20.8f), new Vector3(0.16f, 0.04f, 2.4f), red);
        Cube("Danger Stripe Reactor A", new Vector3(-7.3f, 0.07f, -29.8f), new Vector3(0.16f, 0.04f, 3.5f), red);
        Cube("Danger Stripe Reactor B", new Vector3(-11.1f, 0.07f, -29.8f), new Vector3(0.16f, 0.04f, 3.5f), red);
    }

    private static void CreateProps(Material dark, Material wall, Material yellow, Material glass, Material cyan, Material whiteGlow)
    {
        Cube("Pod Bay Planning Table", new Vector3(0f, 0.58f, 17f), new Vector3(2.5f, 1.1f, 1.8f), dark);
        Cube("Pod Bay Hologram Glow", new Vector3(0f, 1.25f, 17f), new Vector3(2.1f, 0.08f, 1.35f), glass);
        Cube("Pod Bay Hologram Blue Frame", new Vector3(0f, 1.31f, 17f), new Vector3(2.35f, 0.05f, 1.55f), cyan);
        Cube("Life Support Tank A", new Vector3(-15f, 0.9f, 2.2f), new Vector3(1.2f, 1.8f, 1.2f), glass);
        Cube("Life Support Tank B", new Vector3(-15f, 0.9f, -2.2f), new Vector3(1.2f, 1.8f, 1.2f), glass);
        Cube("Life Support Tank A Top Ring", new Vector3(-15f, 1.85f, 2.2f), new Vector3(1.35f, 0.12f, 1.35f), cyan);
        Cube("Life Support Tank B Top Ring", new Vector3(-15f, 1.85f, -2.2f), new Vector3(1.35f, 0.12f, 1.35f), cyan);
        Cube("Navigation Star Map", new Vector3(15.1f, 1.15f, -1.4f), new Vector3(1.8f, 1.7f, 0.12f), glass);
        Cube("Navigation Star Map Frame", new Vector3(15.03f, 1.15f, -1.4f), new Vector3(0.08f, 1.92f, 2.08f), cyan);
        Cube("Medical Cryo Bed", new Vector3(-14.1f, 0.55f, -13.2f), new Vector3(2.1f, 0.55f, 1.05f), glass);
        Cube("Medical Cryo Bed Light", new Vector3(-14.1f, 0.88f, -13.2f), new Vector3(1.8f, 0.08f, 0.82f), cyan);
        Cube("Security Lockers", new Vector3(14.7f, 0.9f, -20.8f), new Vector3(1.2f, 1.8f, 2.6f), wall);
        Cube("Security Locker Blue Strip", new Vector3(14.08f, 1.15f, -20.8f), new Vector3(0.06f, 1.15f, 2.15f), cyan);
        Cube("Reactor Core", new Vector3(-14.2f, 1f, -28.2f), new Vector3(1.8f, 2f, 1.8f), dark);
        Cube("Reactor Glass Band", new Vector3(-14.2f, 1.35f, -28.2f), new Vector3(2f, 0.22f, 2f), glass);
        Cube("Comms Dish Rack", new Vector3(14.6f, 1.05f, -31.6f), new Vector3(1.1f, 1.9f, 2.4f), dark);
        Cube("Corridor Tool Chest", new Vector3(2.5f, 0.35f, 4.3f), new Vector3(1.2f, 0.7f, 0.7f), yellow);
        CreateObservationWindow(dark, glass, cyan, whiteGlow);
        WorldLabel("MISSION BRIEFING\nFollow HUD objectives", new Vector3(0f, 2.08f, 20.25f), labelLookAt, Color.cyan, 0.0055f);
    }

    private static void CreateObservationWindow(Material dark, Material glass, Material cyan, Material whiteGlow)
    {
        Cube("Observation Window Deep Space Backdrop", new Vector3(0f, 1.8f, 20.69f), new Vector3(6.55f, 1.72f, 0.06f), dark);
        Cube("Observation Window Glass", new Vector3(0f, 1.8f, 20.63f), new Vector3(6.35f, 1.5f, 0.045f), glass);
        Cube("Observation Window Top Frame", new Vector3(0f, 2.62f, 20.56f), new Vector3(6.75f, 0.12f, 0.12f), cyan);
        Cube("Observation Window Bottom Frame", new Vector3(0f, 0.98f, 20.56f), new Vector3(6.75f, 0.12f, 0.12f), cyan);
        Cube("Observation Window Left Frame", new Vector3(-3.42f, 1.8f, 20.56f), new Vector3(0.12f, 1.75f, 0.12f), cyan);
        Cube("Observation Window Right Frame", new Vector3(3.42f, 1.8f, 20.56f), new Vector3(0.12f, 1.75f, 0.12f), cyan);

        for (int i = 0; i < 30; i++)
        {
            float x = -2.9f + Mathf.Repeat(i * 1.37f, 5.8f);
            float y = 1.1f + Mathf.Repeat(i * 0.61f, 1.25f);
            float size = 0.035f + Mathf.Repeat(i * 0.017f, 0.045f);
            Cube("Observation Star " + i, new Vector3(x, y, 20.52f), new Vector3(size, size, 0.018f), whiteGlow);
        }

        Cube("Distant Station Silhouette", new Vector3(1.9f, 1.72f, 20.51f), new Vector3(1.25f, 0.18f, 0.025f), whiteGlow);
        Cube("Distant Station Tower", new Vector3(2.18f, 1.95f, 20.505f), new Vector3(0.18f, 0.58f, 0.025f), whiteGlow);
    }

    private static void CreatePipes(Material dark, Material cyan, Material orange)
    {
        if (UseImportedEnvironmentVisuals)
        {
            Cylinder("Ceiling Service Pipe Left", new Vector3(-2.55f, 3.35f, -13f), new Vector3(0.055f, 28f, 0.055f), Quaternion.Euler(90f, 0f, 0f), dark);
            Cylinder("Ceiling Service Pipe Right", new Vector3(2.55f, 3.35f, -13f), new Vector3(0.055f, 28f, 0.055f), Quaternion.Euler(90f, 0f, 0f), dark);
            return;
        }

        Cylinder("Long Corridor Pipe Left", new Vector3(-3.2f, 2.8f, -13f), new Vector3(0.12f, 28f, 0.12f), Quaternion.Euler(90f, 0f, 0f), dark);
        Cylinder("Long Corridor Pipe Right", new Vector3(3.2f, 2.8f, -13f), new Vector3(0.12f, 28f, 0.12f), Quaternion.Euler(90f, 0f, 0f), dark);
        Cylinder("Life Support Cyan Cable", new Vector3(-8.2f, 0.35f, 0f), new Vector3(0.055f, 4.3f, 0.055f), Quaternion.Euler(0f, 0f, 90f), cyan);
        Cylinder("Navigation Cyan Cable", new Vector3(8.2f, 0.35f, -4.4f), new Vector3(0.055f, 4.3f, 0.055f), Quaternion.Euler(0f, 0f, 90f), cyan);
        Cylinder("Medical Cyan Cable", new Vector3(-8.2f, 0.35f, -13.8f), new Vector3(0.055f, 4.3f, 0.055f), Quaternion.Euler(0f, 0f, 90f), cyan);
        Cylinder("Security Orange Cable", new Vector3(8.2f, 0.35f, -17.5f), new Vector3(0.055f, 4.3f, 0.055f), Quaternion.Euler(0f, 0f, 90f), orange);
        Cylinder("Reactor Orange Cable", new Vector3(-8.2f, 0.35f, -28.2f), new Vector3(0.055f, 4.3f, 0.055f), Quaternion.Euler(0f, 0f, 90f), orange);
        Cylinder("Comms Orange Cable", new Vector3(8.2f, 0.35f, -31.6f), new Vector3(0.055f, 4.3f, 0.055f), Quaternion.Euler(0f, 0f, 90f), orange);
    }

    private static GameObject CreatePlayer(Material suit, Material glass, Material cyan, Material dark, Material yellow)
    {
        GameObject player = new GameObject("Player Astronaut Controller");
        player.name = "Player";
        player.tag = "Player";
        player.transform.position = PlayerStart;

        CharacterController controller = player.AddComponent<CharacterController>();
        controller.height = 1.8f;
        controller.radius = 0.35f;
        controller.center = new Vector3(0f, 0.8f, 0f);

        GameObject cameraObject = new GameObject("Main Camera");
        cameraObject.tag = "MainCamera";
        cameraObject.transform.SetParent(player.transform);
        cameraObject.transform.localPosition = new Vector3(0f, 1.5f, 0.42f);
        Camera playerCamera = cameraObject.AddComponent<Camera>();
        playerCamera.fieldOfView = 75f;
        playerCamera.allowHDR = true;
        playerCamera.backgroundColor = new Color(0.01f, 0.018f, 0.035f);
        cameraObject.AddComponent<AudioListener>();

        FirstPersonController movement = player.AddComponent<FirstPersonController>();
        GameObjectInteractor interactor = player.AddComponent<GameObjectInteractor>();
        player.AddComponent<PlayerOxygen>();
        player.AddComponent<PlayerInventory>();
        player.AddComponent<PlayerHealth>();
        player.AddComponent<PlayerScanner>();
        player.AddComponent<PlayerItemUser>();
        player.AddComponent<PlayerFallGuard>();

        SetObject(movement, "playerCamera", playerCamera);
        SetObject(interactor, "viewCamera", playerCamera);
        SetFloat(interactor, "interactDistance", 5.5f);
        CreateAstronautModel(player.transform, suit, glass, cyan, dark, yellow);
        return player;
    }

    private static void CreateSpawnMarker(string name, Vector3 position, Quaternion rotation)
    {
        GameObject marker = new GameObject(name);
        marker.transform.SetPositionAndRotation(position, rotation);
    }

    private static void CreateAstronautModel(Transform parent, Material suit, Material glass, Material cyan, Material dark, Material yellow)
    {
        GameObject rig = new GameObject("Visible Astronaut Suit");
        rig.transform.SetParent(parent, false);

        VisualCapsule("Suit Torso", rig.transform, new Vector3(0f, 0.78f, -0.05f), new Vector3(0.45f, 0.62f, 0.36f), Quaternion.identity, suit);
        VisualSphere("Helmet", rig.transform, new Vector3(0f, 1.45f, -0.05f), new Vector3(0.62f, 0.62f, 0.62f), suit);
        VisualCube("Glass Visor", rig.transform, new Vector3(0f, 1.45f, 0.25f), new Vector3(0.42f, 0.22f, 0.04f), glass);
        VisualCube("Life Support Backpack", rig.transform, new Vector3(0f, 0.9f, -0.38f), new Vector3(0.56f, 0.72f, 0.22f), dark);
        VisualCube("Chest O2 Panel", rig.transform, new Vector3(0f, 0.95f, 0.25f), new Vector3(0.34f, 0.2f, 0.05f), cyan);

        VisualCapsule("Left Arm", rig.transform, new Vector3(-0.42f, 0.82f, 0.02f), new Vector3(0.16f, 0.42f, 0.16f), Quaternion.Euler(0f, 0f, 18f), suit);
        VisualCapsule("Right Arm", rig.transform, new Vector3(0.42f, 0.82f, 0.02f), new Vector3(0.16f, 0.42f, 0.16f), Quaternion.Euler(0f, 0f, -18f), suit);
        VisualSphere("Left Glove", rig.transform, new Vector3(-0.55f, 0.42f, 0.1f), new Vector3(0.22f, 0.22f, 0.22f), yellow);
        VisualSphere("Right Glove", rig.transform, new Vector3(0.55f, 0.42f, 0.1f), new Vector3(0.22f, 0.22f, 0.22f), yellow);

        VisualCapsule("Left Leg", rig.transform, new Vector3(-0.18f, 0.12f, -0.03f), new Vector3(0.18f, 0.48f, 0.18f), Quaternion.identity, suit);
        VisualCapsule("Right Leg", rig.transform, new Vector3(0.18f, 0.12f, -0.03f), new Vector3(0.18f, 0.48f, 0.18f), Quaternion.identity, suit);
        VisualCube("Left Boot", rig.transform, new Vector3(-0.18f, -0.3f, 0.08f), new Vector3(0.22f, 0.14f, 0.38f), dark);
        VisualCube("Right Boot", rig.transform, new Vector3(0.18f, -0.3f, 0.08f), new Vector3(0.22f, 0.14f, 0.38f), dark);

        PointLight("Suit Helmet Light", parent.position + new Vector3(0f, 1.7f, 0.45f), new Color(0.1f, 0.9f, 1f), 1.4f, 4f).transform.SetParent(rig.transform);
    }

    private static void CreateTerminal(Vector3 basePosition, string systemName, float repairSeconds, Material cyan, Material dark, Material green, Material yellow, params KeyCode[] sequence)
    {
        GameObject root = new GameObject(systemName + " Terminal");
        root.transform.position = basePosition;
        BoxCollider hitbox = root.AddComponent<BoxCollider>();
        hitbox.size = new Vector3(2.4f, 2.4f, 1.5f);
        hitbox.center = new Vector3(0f, 1.1f, 0f);

        TerminalTask task = root.AddComponent<TerminalTask>();
        SetString(task, "systemName", systemName);
        task.ConfigureTask(repairSeconds, sequence);

        Cube(systemName + " Terminal Base", basePosition + new Vector3(0f, 0.35f, 0f), new Vector3(1.8f, 0.7f, 1f), dark).transform.SetParent(root.transform);
        Cube(systemName + " Terminal Column", basePosition + new Vector3(0f, 1.1f, 0.22f), new Vector3(1.5f, 1.2f, 0.35f), dark).transform.SetParent(root.transform);
        GameObject screen = Cube(systemName + " Glowing Screen", basePosition + new Vector3(0f, 1.25f, -0.02f), new Vector3(1.25f, 0.65f, 0.08f), cyan);
        screen.transform.SetParent(root.transform);
        Cube(systemName + " Screen Top Rail", basePosition + new Vector3(0f, 1.64f, -0.07f), new Vector3(1.45f, 0.08f, 0.08f), cyan).transform.SetParent(root.transform);
        Cube(systemName + " Screen Bottom Rail", basePosition + new Vector3(0f, 0.86f, -0.07f), new Vector3(1.45f, 0.08f, 0.08f), cyan).transform.SetParent(root.transform);
        Cube(systemName + " Screen Left Rail", basePosition + new Vector3(-0.72f, 1.25f, -0.07f), new Vector3(0.08f, 0.78f, 0.08f), cyan).transform.SetParent(root.transform);
        Cube(systemName + " Screen Right Rail", basePosition + new Vector3(0.72f, 1.25f, -0.07f), new Vector3(0.08f, 0.78f, 0.08f), cyan).transform.SetParent(root.transform);
        Cube(systemName + " Keyboard", basePosition + new Vector3(0f, 0.78f, -0.35f), new Vector3(1.25f, 0.08f, 0.45f), yellow).transform.SetParent(root.transform);
        GameObject importedComputer = PlacePrefabVisual(CosmicComputerPath, basePosition + new Vector3(0f, 0f, -0.42f), Quaternion.Euler(0f, 180f, 0f), new Vector3(0.9f, 0.9f, 0.9f), root.transform, systemName + " Imported Computer");
        if (importedComputer == null)
        {
            PlacePrefabVisual(CosmicControlPanelPath, basePosition + new Vector3(0f, 0.8f, -0.4f), Quaternion.Euler(0f, 180f, 0f), Vector3.one, root.transform, systemName + " Imported Control Panel");
        }

        Light status = PointLight(systemName + " Terminal Beacon", basePosition + new Vector3(0f, 2.1f, -0.35f), Color.red, 3f, 5f);
        status.transform.SetParent(root.transform);
        SetObject(task, "screenRenderer", screen.GetComponent<Renderer>());
        SetObject(task, "statusLight", status);

        WorldLabel("E START\n" + systemName.ToUpper() + "\nCODE " + SequenceText(sequence), basePosition + new Vector3(0f, 2.05f, -0.25f), labelLookAt, Color.cyan, 0.012f).transform.SetParent(root.transform);
        PointLight(systemName + " Area Light", basePosition + new Vector3(0f, 2.7f, 0f), new Color(0.1f, 0.9f, 1f), 2.8f, 6f).transform.SetParent(root.transform);
    }

    private static void CreateDoor(string name, Vector3 position, Quaternion rotation, bool requiresRepairs, int requiredRepairCount, Material wall, Material yellow, Material beaconMaterial, string requiredItemName = "", string requiredItemDisplayName = "", bool consumeRequiredItem = false, float doorWidth = 2.25f)
    {
        GameObject root = new GameObject(name);
        root.transform.position = position;
        root.transform.rotation = rotation;

        float sealedWidth = Mathf.Max(doorWidth + 2.1f, 5.9f);
        float clearWidth = doorWidth > 4.5f ? 4.25f : Mathf.Min(doorWidth, 3.45f);
        float sideSealWidth = Mathf.Max(0.45f, (sealedWidth - clearWidth) * 0.5f);
        float sideSealX = (clearWidth * 0.5f) + (sideSealWidth * 0.5f);
        float blockerHeight = 3.7f;
        float blockerDepth = 1.12f;

        BoxCollider hitbox = root.AddComponent<BoxCollider>();
        hitbox.isTrigger = true;
        hitbox.size = new Vector3(sealedWidth + 0.6f, blockerHeight + 0.35f, blockerDepth + 0.55f);

        float sideX = (clearWidth * 0.5f) + 0.25f;

        VisualCube(name + " Permanent Frame Left Seal", root.transform, new Vector3(-sideSealX, 0f, 0f), new Vector3(sideSealWidth, 3.35f, 0.62f), wall);
        VisualCube(name + " Permanent Frame Right Seal", root.transform, new Vector3(sideSealX, 0f, 0f), new Vector3(sideSealWidth, 3.35f, 0.62f), wall);
        VisualCube(name + " Permanent Frame Top Seal", root.transform, new Vector3(0f, 1.48f, 0f), new Vector3(sealedWidth, 0.38f, 0.64f), yellow);
        VisualCube(name + " Permanent Frame Threshold", root.transform, new Vector3(0f, -1.5f, 0f), new Vector3(sealedWidth, 0.16f, 0.64f), yellow);

        GameObject panel;
        if (UseImportedEnvironmentVisuals)
        {
            float visualScale = Mathf.Max(1f, clearWidth / 2.4f);
            PlacePrefabVisualLocal(ModularDoorFramePath, root.transform, Vector3.zero, Quaternion.identity, new Vector3(visualScale, 1.18f, 1.18f), name + " Imported Door Frame");

            panel = new GameObject(name + " Lock Barrier");
            panel.transform.SetParent(root.transform, false);
            BoxCollider panelCollider = panel.AddComponent<BoxCollider>();
            panelCollider.size = new Vector3(sealedWidth, blockerHeight, blockerDepth);
            VisualCube(name + " Lock Field", panel.transform, Vector3.zero, new Vector3(sealedWidth, blockerHeight - 0.35f, 0.08f), beaconMaterial);
            PlacePrefabVisualLocal(ModularDoorPath, panel.transform, new Vector3(0f, 0.08f, -0.02f), Quaternion.identity, new Vector3(visualScale, 1.18f, 1.18f), name + " Imported Door Panel");
            VisualCube(name + " Status Strip", panel.transform, new Vector3(0f, 0.92f, -0.28f), new Vector3(Mathf.Max(0.5f, clearWidth - 0.15f), 0.09f, 0.04f), beaconMaterial);
            VisualCube(name + " Center Access Line", panel.transform, new Vector3(0f, 0f, -0.29f), new Vector3(0.08f, 1.8f, 0.04f), beaconMaterial);
        }
        else
        {
            SolidCube(name + " Frame Left", root.transform, new Vector3(-sideX, 0f, 0f), new Vector3(0.35f, 3.2f, 0.45f), yellow);
            SolidCube(name + " Frame Right", root.transform, new Vector3(sideX, 0f, 0f), new Vector3(0.35f, 3.2f, 0.45f), yellow);
            SolidCube(name + " Frame Top", root.transform, new Vector3(0f, 1.45f, 0f), new Vector3(clearWidth + 0.7f, 0.32f, 0.45f), yellow);
            SolidCube(name + " Frame Bottom", root.transform, new Vector3(0f, -1.45f, 0f), new Vector3(clearWidth + 0.7f, 0.18f, 0.45f), yellow);
            panel = SolidCube(name + " Lock Barrier", root.transform, Vector3.zero, new Vector3(sealedWidth, blockerHeight, 0.38f), wall);
            SolidCube(name + " Status Strip", panel.transform, new Vector3(0f, 0.46f, -0.8f), new Vector3(Mathf.Max(0.5f, clearWidth - 0.15f) / sealedWidth, 0.055f, 0.13f), beaconMaterial);
            SolidCube(name + " Center Access Line", panel.transform, new Vector3(0f, 0f, -0.82f), new Vector3(0.055f, 0.78f, 0.13f), beaconMaterial);
        }

        VisualCube(name + " Access Screen", root.transform, new Vector3(sideX + 0.42f, 0.15f, -0.32f), new Vector3(0.36f, 0.62f, 0.06f), beaconMaterial);

        DoorController door = root.AddComponent<DoorController>();
        SetObject(door, "doorPanel", panel.transform);
        SetBool(door, "requiresRepairs", requiresRepairs);
        SetInt(door, "requiredRepairCount", requiredRepairCount);
        SetString(door, "requiredItemName", requiredItemName);
        SetString(door, "requiredItemDisplayName", requiredItemDisplayName);
        SetBool(door, "consumeRequiredItem", consumeRequiredItem);

        string label = requiresRepairs ? "LOCKED\n" + requiredRepairCount + " SYSTEMS" : "E OPEN\nBULKHEAD";
        if (!string.IsNullOrEmpty(requiredItemDisplayName))
        {
            label += "\n" + requiredItemDisplayName.ToUpper();
        }

        float corridorSideOffset = position.x < -0.1f ? 1.25f : -1.25f;
        if (Mathf.Abs(position.x) < 0.1f)
        {
            corridorSideOffset = 1.25f;
        }

        WorldLabel(label, position + rotation * new Vector3(0f, 1.08f, corridorSideOffset), labelLookAt, requiresRepairs ? Color.red : Color.cyan, 0.0095f).transform.SetParent(root.transform);
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
        WorldLabel("ESCAPE POD\nRepair all systems, then press E", position + new Vector3(0f, 2.65f, 1.2f), labelLookAt, Color.green, 0.014f);
    }

    private static void CreateResourcePickup(Vector3 position, string itemName, string displayName, Material body, Material accent)
    {
        GameObject pickup = GameObject.CreatePrimitive(PrimitiveType.Cube);
        pickup.name = displayName;
        pickup.transform.position = position;
        pickup.transform.localScale = new Vector3(0.72f, 0.28f, 0.48f);
        pickup.GetComponent<Renderer>().material = body;

        ResourcePickup resource = pickup.AddComponent<ResourcePickup>();
        SetString(resource, "itemName", itemName);
        SetString(resource, "displayName", displayName);

        Cube(displayName + " Glow Strip", position + new Vector3(0f, 0.22f, 0f), new Vector3(0.82f, 0.08f, 0.56f), accent).transform.SetParent(pickup.transform);
        WorldLabel("E TAKE\n" + displayName.ToUpper(), position + new Vector3(0f, 1.1f, 0f), labelLookAt, Color.yellow, 0.012f).transform.SetParent(pickup.transform);
        PointLight(displayName + " Marker Light", position + new Vector3(0f, 0.85f, 0f), Color.yellow, 1.8f, 4f).transform.SetParent(pickup.transform);
    }

    private static void CreateConsumablePickup(Vector3 position, string itemName, string displayName, Material body, Material accent)
    {
        GameObject pickup = GameObject.CreatePrimitive(PrimitiveType.Cube);
        pickup.name = displayName;
        pickup.transform.position = position;
        pickup.transform.localScale = new Vector3(0.68f, 0.35f, 0.52f);
        pickup.GetComponent<Renderer>().material = body;

        ConsumablePickup consumable = pickup.AddComponent<ConsumablePickup>();
        SetString(consumable, "itemName", itemName);
        SetString(consumable, "displayName", displayName);
        SetInt(consumable, "quantity", 1);

        Cube(displayName + " Bright Label", position + new Vector3(0f, 0.25f, 0f), new Vector3(0.78f, 0.08f, 0.62f), accent).transform.SetParent(pickup.transform);
        WorldLabel("E PICK UP\n" + displayName.ToUpper(), position + new Vector3(0f, 1.15f, 0f), labelLookAt, Color.yellow, 0.012f).transform.SetParent(pickup.transform);
    }

    private static void CreateOptionalSupplyCache(string name, Vector3 center, Material dark, Material yellow, Material cyan, Material red, bool includeOxygen, bool includeMedkit, bool includeBattery)
    {
        GameObject root = new GameObject(name);
        root.transform.position = center;

        Cube(name + " Yellow Crate", center + new Vector3(0f, 0.42f, 0f), new Vector3(1.45f, 0.84f, 1.1f), yellow).transform.SetParent(root.transform);
        Cube(name + " Dark Lid", center + new Vector3(0f, 0.91f, 0f), new Vector3(1.6f, 0.16f, 1.25f), dark).transform.SetParent(root.transform);
        Cube(name + " Red Risk Stripe", center + new Vector3(0f, 1.03f, 0f), new Vector3(1.65f, 0.07f, 0.18f), red).transform.SetParent(root.transform);
        Cube(name + " Cyan Locator Strip", center + new Vector3(0f, 1.11f, 0.48f), new Vector3(1.3f, 0.06f, 0.12f), cyan).transform.SetParent(root.transform);
        WorldLabel("OPTIONAL SUPPLY CACHE\nRisk route", center + new Vector3(0f, 1.85f, 0f), labelLookAt, Color.yellow, 0.012f).transform.SetParent(root.transform);
        PointLight(name + " Warning Light", center + new Vector3(0f, 1.65f, 0f), new Color(1f, 0.72f, 0.12f), 2.1f, 5.5f).transform.SetParent(root.transform);

        if (includeOxygen)
        {
            CreateOxygenCanister(center + new Vector3(-0.95f, 0.75f, -0.85f), cyan, yellow);
        }

        if (includeMedkit)
        {
            CreateConsumablePickup(center + new Vector3(0.95f, 0.65f, -0.85f), "medkit", "Medkit", red, yellow);
        }

        if (includeBattery)
        {
            CreateConsumablePickup(center + new Vector3(0f, 0.65f, 1f), "battery", "Battery Pack", cyan, yellow);
        }
    }

    private static void CreateSecurityRobot(Vector3 position, Material body, Material red, Material accent, params Vector3[] points)
    {
        GameObject root = new GameObject("Security Robot");
        root.transform.position = position;
        SphereCollider sensorCollider = root.AddComponent<SphereCollider>();
        sensorCollider.radius = 0.9f;
        sensorCollider.center = new Vector3(0f, 0.85f, 0f);
        sensorCollider.isTrigger = true;

        VisualCapsule("Robot Body", root.transform, new Vector3(0f, 0.45f, 0f), new Vector3(0.55f, 0.75f, 0.55f), Quaternion.identity, body);
        VisualSphere("Robot Sensor Head", root.transform, new Vector3(0f, 1.28f, 0f), new Vector3(0.72f, 0.42f, 0.72f), body);
        VisualCube("Robot Red Sensor", root.transform, new Vector3(0f, 1.28f, 0.35f), new Vector3(0.45f, 0.12f, 0.06f), red);
        VisualCube("Robot Chest Light", root.transform, new Vector3(0f, 0.62f, 0.34f), new Vector3(0.36f, 0.18f, 0.06f), accent);
        Light eye = PointLight("Robot Sensor Light", position + new Vector3(0f, 1.25f, 0.45f), Color.red, 2.2f, 5f);
        eye.transform.SetParent(root.transform);

        Transform[] patrolPoints = new Transform[points.Length];
        for (int i = 0; i < points.Length; i++)
        {
            GameObject point = new GameObject("Patrol Point " + i);
            point.transform.position = points[i];
            point.transform.SetParent(root.transform.parent);
            patrolPoints[i] = point.transform;
        }

        SecurityRobot robot = root.AddComponent<SecurityRobot>();
        robot.ConfigurePatrol(patrolPoints);
        SetFloat(robot, "patrolSpeed", 1.9f);
        SetFloat(robot, "chaseSpeed", 3.35f);
        SetFloat(robot, "detectionRange", 7.25f);
        SetFloat(robot, "attackRange", 1.25f);
        SetFloat(robot, "attackDamage", 10f);
        SetFloat(robot, "attackCooldown", 2.1f);
        SetFloat(robot, "crouchDetectionMultiplier", 0.45f);
        SetFloat(robot, "sprintDetectionMultiplier", 1.15f);
        SetFloat(robot, "loseSightDelay", 1.15f);
        SetFloat(robot, "searchDuration", 3.4f);
        SetFloat(robot, "giveUpDistance", 12f);
        WorldLabel("SECURITY ROBOT\nAvoid contact", position + new Vector3(0f, 2.15f, 0f), labelLookAt, Color.red, 0.012f);
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
        WorldLabel("E  O2 REFILL", position + new Vector3(0f, 1.65f, 0f), labelLookAt, Color.cyan, 0.012f).transform.SetParent(canister.transform);
        PointLight("Oxygen Canister Light", position + new Vector3(0f, 1.2f, 0f), new Color(0.05f, 1f, 1f), 2.2f, 4f).transform.SetParent(canister.transform);
    }

    private static void CreateHUD(TMP_FontAsset font)
    {
        GameObject canvasObject = new GameObject("HUD Canvas");
        Canvas canvas = canvasObject.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvasObject.AddComponent<CanvasScaler>().uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        canvasObject.AddComponent<GraphicRaycaster>();
        if (Object.FindObjectOfType<EventSystem>() == null)
        {
            GameObject eventSystemObject = new GameObject("EventSystem");
            eventSystemObject.AddComponent<EventSystem>();
            eventSystemObject.AddComponent<StandaloneInputModule>();
        }

        Color uiBlue = new Color(0.02f, 0.42f, 0.88f);
        Color uiInk = new Color(0.08f, 0.14f, 0.22f);
        Color uiPanel = new Color(0.93f, 0.97f, 1f, 0.82f);
        GameObject missionPanel = TopLeftPanel(canvasObject.transform, "MissionPanel", new Vector2(16f, -16f), new Vector2(322f, 178f), uiPanel);
        TMP_Text missionTitle = TextUI(missionPanel.transform, "MissionTitle", "MISSIONS", new Vector2(14f, -10f), TextAlignmentOptions.TopLeft, 20, font);
        missionTitle.color = uiBlue;
        missionTitle.fontStyle = FontStyles.Bold;
        TMP_Text objective = TextUI(missionPanel.transform, "ObjectiveText", "", new Vector2(14f, -36f), TextAlignmentOptions.TopLeft, 18, font);
        objective.color = uiInk;
        objective.GetComponent<RectTransform>().sizeDelta = new Vector2(292f, 74f);
        TMP_Text timer = TextUI(missionPanel.transform, "TimerText", "TIME 10:20", new Vector2(14f, -118f), TextAlignmentOptions.TopLeft, 21, font);
        timer.color = uiBlue;
        TMP_Text repairs = TextUI(missionPanel.transform, "RepairText", "SYSTEMS 0/7", new Vector2(158f, -120f), TextAlignmentOptions.TopLeft, 19, font);
        repairs.color = uiInk;
        TMP_Text pressure = TextUI(missionPanel.transform, "PressureText", "O2 DRAIN x1.0", new Vector2(14f, -148f), TextAlignmentOptions.TopLeft, 18, font);
        pressure.color = uiInk;

        GameObject statusPanel = BottomLeftPanel(canvasObject.transform, "StatusPanel", new Vector2(16f, 16f), new Vector2(306f, 154f), uiPanel);
        TMP_Text oxygen = TextUI(statusPanel.transform, "OxygenText", "O2 100%", new Vector2(14f, -12f), TextAlignmentOptions.TopLeft, 19, font);
        oxygen.color = uiInk;
        Image oxygenFill = Bar(statusPanel.transform, "OxygenFill", new Vector2(14f, -36f), new Vector2(190f, 10f), new Color(0.05f, 0.95f, 0.8f));
        TMP_Text health = TextUI(statusPanel.transform, "HealthText", "HEALTH 100/100", new Vector2(14f, -56f), TextAlignmentOptions.TopLeft, 19, font);
        health.color = uiInk;
        Image healthFill = Bar(statusPanel.transform, "HealthFill", new Vector2(14f, -80f), new Vector2(190f, 10f), new Color(0.08f, 0.5f, 1f));
        TMP_Text battery = TextUI(statusPanel.transform, "BatteryText", "BATTERY 100%", new Vector2(14f, -100f), TextAlignmentOptions.TopLeft, 19, font);
        battery.color = uiInk;
        Image batteryFill = Bar(statusPanel.transform, "BatteryFill", new Vector2(150f, -104f), new Vector2(76f, 10f), new Color(0.18f, 0.65f, 1f));
        TMP_Text inventory = TextUI(statusPanel.transform, "InventoryText", "TAB/I BAG   Q SCAN   C CROUCH", new Vector2(14f, -126f), TextAlignmentOptions.TopLeft, 16, font);
        inventory.color = uiBlue;
        TMP_Text crosshair = TextUI(canvasObject.transform, "Crosshair", "+", new Vector2(0f, 0f), TextAlignmentOptions.Center, 33, font);
        crosshair.color = new Color(0.02f, 0.42f, 0.88f, 0.9f);
        TMP_Text prompt = TextUI(canvasObject.transform, "PromptText", "", new Vector2(0f, 54f), TextAlignmentOptions.Bottom, 25, font);
        prompt.color = uiBlue;
        TMP_Text message = TextUI(canvasObject.transform, "MessageText", "", new Vector2(0f, 112f), TextAlignmentOptions.Bottom, 26, font);
        message.color = uiBlue;

        GameObject backpackPanel = Panel(canvasObject.transform, "BackpackPanel", new Vector2(-24f, -24f), new Vector2(360f, 420f), new Color(0.93f, 0.97f, 1f, 0.92f));
        TMP_Text backpackTitle = TextUI(backpackPanel.transform, "BackpackTitle", "BACKPACK", new Vector2(22f, -18f), TextAlignmentOptions.TopLeft, 29, font);
        backpackTitle.color = uiBlue;
        backpackTitle.fontStyle = FontStyles.Bold;
        TMP_Text backpackHint = TextUI(backpackPanel.transform, "BackpackHint", "TAB / I", new Vector2(22f, -54f), TextAlignmentOptions.TopLeft, 19, font);
        backpackHint.color = uiBlue;
        TMP_Text backpackItems = TextUI(backpackPanel.transform, "BackpackItems", "NO ITEMS", new Vector2(22f, -94f), TextAlignmentOptions.TopLeft, 24, font);
        backpackItems.color = uiInk;
        backpackItems.GetComponent<RectTransform>().sizeDelta = new Vector2(316f, 300f);
        backpackPanel.SetActive(false);

        GameObject scanPanel = StretchPanel(canvasObject.transform, "ScanPanel", new Color(0.02f, 0.35f, 0.65f, 0.34f));
        TMP_Text scanTitle = TextUI(scanPanel.transform, "ScanTitle", "SCAN MODE", new Vector2(0f, 230f), TextAlignmentOptions.Center, 42, font);
        scanTitle.color = new Color(0.85f, 1f, 1f);
        scanTitle.fontStyle = FontStyles.Bold;
        TMP_Text scanText = TextUI(scanPanel.transform, "ScanText", "", new Vector2(-360f, 60f), TextAlignmentOptions.Center, 29, font);
        scanText.alignment = TextAlignmentOptions.TopLeft;
        scanText.GetComponent<RectTransform>().sizeDelta = new Vector2(520f, 260f);
        TMP_Text scanHint = TextUI(scanPanel.transform, "ScanHint", "HOLD Q TO SCAN", new Vector2(0f, -250f), TextAlignmentOptions.Center, 27, font);
        scanHint.color = new Color(1f, 0.86f, 0.25f);
        scanPanel.SetActive(false);

        GameObject pausePanel = CenterPanel(canvasObject.transform, "PausePanel", new Vector2(470f, 430f), new Color(0.93f, 0.97f, 1f, 0.94f));
        TMP_Text pauseTitle = TextUI(pausePanel.transform, "PauseTitle", "// PAUSED //", new Vector2(0f, 130f), TextAlignmentOptions.Center, 42, font);
        pauseTitle.color = uiBlue;
        pauseTitle.fontStyle = FontStyles.Bold;
        TMP_Text pauseBody = TextUI(pausePanel.transform, "PauseBody", "ESC  RESUME\nR    RESTART CHECKPOINT\nTAB  BACKPACK\nQ    SCANNER\nH    USE MEDKIT\nB    USE BATTERY", new Vector2(0f, 18f), TextAlignmentOptions.Center, 26, font);
        pauseBody.color = uiInk;
        pauseBody.GetComponent<RectTransform>().sizeDelta = new Vector2(390f, 220f);
        pausePanel.SetActive(false);

        GameObject panel = new GameObject("EndPanel");
        panel.transform.SetParent(canvasObject.transform, false);
        Image panelImage = panel.AddComponent<Image>();
        panelImage.color = new Color(0.93f, 0.97f, 1f, 0.9f);
        RectTransform panelRect = panel.GetComponent<RectTransform>();
        panelRect.anchorMin = Vector2.zero;
        panelRect.anchorMax = Vector2.one;
        panelRect.offsetMin = Vector2.zero;
        panelRect.offsetMax = Vector2.zero;

        TMP_Text endTitle = TextUI(panel.transform, "EndTitle", "", new Vector2(0f, 150f), TextAlignmentOptions.Center, 52, font);
        endTitle.color = uiBlue;
        endTitle.fontStyle = FontStyles.Bold;
        endTitle.GetComponent<RectTransform>().sizeDelta = new Vector2(720f, 80f);
        TMP_Text endBody = TextUI(panel.transform, "EndBody", "", new Vector2(0f, -35f), TextAlignmentOptions.Left, 27, font);
        endBody.color = uiInk;
        endBody.GetComponent<RectTransform>().sizeDelta = new Vector2(620f, 360f);
        panel.SetActive(false);

        GameObject mainMenuPanel = CreateMainMenuPanel(canvasObject.transform, font, uiBlue, uiInk);

        GameObject modePanel = StretchPanel(canvasObject.transform, "ModeSelectPanel", new Color(0.01f, 0.04f, 0.07f, 0.92f));
        GameObject modeCard = CenterPanel(modePanel.transform, "ModeSelectCard", new Vector2(720f, 470f), new Color(0.93f, 0.97f, 1f, 0.96f));
        TMP_Text modeTitle = TextUI(modeCard.transform, "ModeTitle", "DEEP SPACE STATION", new Vector2(0f, 170f), TextAlignmentOptions.Center, 50, font);
        modeTitle.color = uiBlue;
        modeTitle.fontStyle = FontStyles.Bold;
        TMP_Text modeSubtitle = TextUI(modeCard.transform, "ModeSubtitle", "FINAL EVACUATION", new Vector2(0f, 128f), TextAlignmentOptions.Center, 30, font);
        modeSubtitle.color = uiInk;
        TMP_Text modeBody = TextUI(modeCard.transform, "ModeBody", "CHOOSE THE MISSION STRUCTURE BEFORE ENTERING THE STATION.", new Vector2(0f, 82f), TextAlignmentOptions.Center, 21, font);
        modeBody.color = uiInk;
        Button timedButton = ModeButton(modeCard.transform, "TimedEvacuationButton", "1  TIMED EVACUATION", "Restore all systems and escape before the countdown ends.", new Vector2(0f, 8f), uiBlue, uiInk, font);
        Button endlessButton = ModeButton(modeCard.transform, "EndlessSurvivalButton", "2  ENDLESS SURVIVAL", "Keep repairing terminals for score. Robot threat rises as you score.", new Vector2(0f, -98f), new Color(0.03f, 0.58f, 0.72f), uiInk, font);
        TMP_Text modeHint = TextUI(modeCard.transform, "ModeHint", "CLICK A MODE OR PRESS 1 / 2", new Vector2(0f, -190f), TextAlignmentOptions.Center, 22, font);
        modeHint.color = uiBlue;
        mainMenuPanel.SetActive(true);
        modePanel.SetActive(false);

        GameManager manager = Object.FindObjectOfType<GameManager>();
        if (manager != null)
        {
            UnityEventTools.AddPersistentListener(timedButton.onClick, new UnityEngine.Events.UnityAction(manager.StartTimedEvacuationMode));
            UnityEventTools.AddPersistentListener(endlessButton.onClick, new UnityEngine.Events.UnityAction(manager.StartEndlessMode));
        }

        HUDController hud = canvasObject.AddComponent<HUDController>();
        SetObject(hud, "oxygenText", oxygen);
        SetObject(hud, "oxygenFill", oxygenFill);
        SetObject(hud, "healthText", health);
        SetObject(hud, "healthFill", healthFill);
        SetObject(hud, "batteryText", battery);
        SetObject(hud, "batteryFill", batteryFill);
        SetObject(hud, "timerText", timer);
        SetObject(hud, "repairText", repairs);
        SetObject(hud, "pressureText", pressure);
        SetObject(hud, "inventoryText", inventory);
        SetObject(hud, "backpackPanel", backpackPanel);
        SetObject(hud, "backpackItemsText", backpackItems);
        SetObject(hud, "objectiveText", objective);
        SetObject(hud, "promptText", prompt);
        SetObject(hud, "messageText", message);
        SetObject(hud, "scanPanel", scanPanel);
        SetObject(hud, "scanText", scanText);
        SetObject(hud, "mainMenuPanel", mainMenuPanel);
        SetObject(hud, "mainMenuInfoText", FindTMPText(mainMenuPanel.transform, "MainMenuInfoText"));
        SetObject(hud, "modeSelectPanel", modePanel);
        SetObject(hud, "pausePanel", pausePanel);
        SetObject(hud, "endPanel", panel);
        SetObject(hud, "endTitleText", endTitle);
        SetObject(hud, "endBodyText", endBody);
        EnsurePresentationWidgets(hud, font);
        ApplyHUDPolish(canvasObject.transform, font);
    }

    private static GameObject CreateMainMenuPanel(Transform parent, TMP_FontAsset font, Color uiBlue, Color uiInk)
    {
        GameObject mainMenuPanel = StretchPanel(parent, "MainMenuPanel", new Color(0.01f, 0.03f, 0.06f, 1f));
        mainMenuPanel.transform.SetAsLastSibling();
        Image mainMenuImage = mainMenuPanel.GetComponent<Image>();
        Sprite coverSprite = LoadMainMenuCoverSprite();
        if (coverSprite != null)
        {
            mainMenuImage.sprite = coverSprite;
            mainMenuImage.color = Color.white;
            mainMenuImage.type = Image.Type.Simple;
            mainMenuImage.preserveAspect = false;
        }

        BuildMainMenuContent(mainMenuPanel.transform, font, uiBlue, uiInk);
        return mainMenuPanel;
    }

    private static void BuildMainMenuContent(Transform mainMenuPanel, TMP_FontAsset font, Color uiBlue, Color uiInk)
    {
        MainMenuButton(mainMenuPanel, "NewGameButton", new Vector2(52f, -150f), new Vector2(278f, 32f));
        MainMenuButton(mainMenuPanel, "OptionsButton", new Vector2(52f, -186f), new Vector2(278f, 32f));
        MainMenuButton(mainMenuPanel, "HowToPlayButton", new Vector2(52f, -222f), new Vector2(278f, 32f));
        MainMenuButton(mainMenuPanel, "CreditsButton", new Vector2(52f, -257f), new Vector2(278f, 32f));
    }

    private static Sprite LoadMainMenuCoverSprite()
    {
        if (!File.Exists(MainMenuCoverPath))
        {
            return null;
        }

        AssetDatabase.ImportAsset(MainMenuCoverPath, ImportAssetOptions.ForceSynchronousImport);
        TextureImporter importer = AssetImporter.GetAtPath(MainMenuCoverPath) as TextureImporter;
        if (importer != null && importer.textureType != TextureImporterType.Sprite)
        {
            importer.textureType = TextureImporterType.Sprite;
            importer.spriteImportMode = SpriteImportMode.Single;
            importer.alphaSource = TextureImporterAlphaSource.None;
            importer.mipmapEnabled = false;
            importer.SaveAndReimport();
        }

        return AssetDatabase.LoadAssetAtPath<Sprite>(MainMenuCoverPath);
    }

    private static Button MainMenuButton(Transform parent, string name, Vector2 position, Vector2 size)
    {
        GameObject buttonObject = new GameObject(name, typeof(RectTransform));
        buttonObject.transform.SetParent(parent, false);
        Image image = buttonObject.AddComponent<Image>();
        image.color = new Color(0f, 0f, 0f, 0f);
        Button button = buttonObject.AddComponent<Button>();
        ColorBlock colors = button.colors;
        colors.normalColor = new Color(0f, 0f, 0f, 0f);
        colors.highlightedColor = new Color(0.08f, 0.55f, 1f, 0.12f);
        colors.pressedColor = new Color(0.08f, 0.55f, 1f, 0.2f);
        colors.selectedColor = colors.highlightedColor;
        colors.colorMultiplier = 1f;
        colors.fadeDuration = 0.08f;
        button.colors = colors;

        RectTransform rect = buttonObject.GetComponent<RectTransform>();
        rect.anchorMin = new Vector2(0f, 1f);
        rect.anchorMax = new Vector2(0f, 1f);
        rect.pivot = new Vector2(0f, 1f);
        rect.anchoredPosition = position;
        rect.sizeDelta = size;
        return button;
    }

    private static Button MainMenuPopupButton(Transform parent, string name, string label, Vector2 position, Vector2 size, TMP_FontAsset font)
    {
        GameObject buttonObject = new GameObject(name, typeof(RectTransform));
        buttonObject.transform.SetParent(parent, false);
        Image image = buttonObject.AddComponent<Image>();
        image.color = new Color(0.035f, 0.42f, 0.62f, 0.96f);
        Button button = buttonObject.AddComponent<Button>();
        ColorBlock colors = button.colors;
        colors.normalColor = image.color;
        colors.highlightedColor = Color.Lerp(image.color, Color.white, 0.18f);
        colors.pressedColor = Color.Lerp(image.color, Color.black, 0.2f);
        colors.selectedColor = colors.highlightedColor;
        colors.colorMultiplier = 1f;
        colors.fadeDuration = 0.08f;
        button.colors = colors;

        RectTransform rect = buttonObject.GetComponent<RectTransform>();
        rect.anchorMin = new Vector2(0f, 1f);
        rect.anchorMax = new Vector2(0f, 1f);
        rect.pivot = new Vector2(0f, 1f);
        rect.anchoredPosition = position;
        rect.sizeDelta = size;

        TMP_Text text = TextUI(buttonObject.transform, name + "Label", label, Vector2.zero, TextAlignmentOptions.Center, label.Length <= 1 ? 22 : 18, font);
        text.color = Color.white;
        text.fontStyle = FontStyles.Bold;
        text.enableWordWrapping = false;
        text.GetComponent<RectTransform>().sizeDelta = size;
        return button;
    }

    private static GameObject WorldLabel(string text, Vector3 position, Vector3 lookAt, Color color, float scale)
    {
        GameObject canvasObject = new GameObject("World Label - " + text.Replace("\n", " "));
        canvasObject.transform.position = position;
        canvasObject.transform.LookAt(new Vector3(lookAt.x, position.y, lookAt.z));
        canvasObject.transform.Rotate(0f, 180f, 0f);
        canvasObject.transform.localScale = Vector3.one * (scale * 0.68f);
        Canvas canvas = canvasObject.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.WorldSpace;

        TMP_Text label = TextUI(canvasObject.transform, "Text", text, Vector2.zero, TextAlignmentOptions.Center, 34, EnsureSciFiTypography());
        label.color = color;
        label.fontStyle = FontStyles.Bold;
        label.characterSpacing = 2f;
        RectTransform rect = label.GetComponent<RectTransform>();
        rect.sizeDelta = new Vector2(420f, 110f);
        return canvasObject;
    }

    private static Button ModeButton(Transform parent, string name, string titleValue, string subtitleValue, Vector2 position, Color baseColor, Color inkColor, TMP_FontAsset font)
    {
        GameObject buttonObject = new GameObject(name, typeof(RectTransform));
        buttonObject.transform.SetParent(parent, false);
        Image image = buttonObject.AddComponent<Image>();
        image.color = baseColor;
        Button button = buttonObject.AddComponent<Button>();
        ColorBlock colors = button.colors;
        colors.normalColor = baseColor;
        colors.highlightedColor = Color.Lerp(baseColor, Color.white, 0.18f);
        colors.pressedColor = Color.Lerp(baseColor, Color.black, 0.18f);
        colors.selectedColor = colors.highlightedColor;
        colors.colorMultiplier = 1f;
        colors.fadeDuration = 0.08f;
        button.colors = colors;

        RectTransform rect = buttonObject.GetComponent<RectTransform>();
        rect.anchorMin = new Vector2(0.5f, 0.5f);
        rect.anchorMax = new Vector2(0.5f, 0.5f);
        rect.pivot = new Vector2(0.5f, 0.5f);
        rect.anchoredPosition = position;
        rect.sizeDelta = new Vector2(560f, 82f);

        TMP_Text title = TextUI(buttonObject.transform, name + "Title", titleValue, new Vector2(0f, 14f), TextAlignmentOptions.Center, 27, font);
        title.color = Color.white;
        title.fontStyle = FontStyles.Bold;
        title.GetComponent<RectTransform>().sizeDelta = new Vector2(520f, 32f);

        TMP_Text subtitle = TextUI(buttonObject.transform, name + "Subtitle", subtitleValue.ToUpperInvariant(), new Vector2(0f, -20f), TextAlignmentOptions.Center, 17, font);
        subtitle.color = Color.Lerp(Color.white, inkColor, 0.18f);
        subtitle.GetComponent<RectTransform>().sizeDelta = new Vector2(520f, 32f);
        return button;
    }

    private static TMP_Text TextUI(Transform parent, string name, string value, Vector2 position, TextAlignmentOptions alignment, int size, TMP_FontAsset font)
    {
        GameObject textObject = new GameObject(name, typeof(RectTransform));
        textObject.transform.SetParent(parent, false);
        TextMeshProUGUI text = textObject.AddComponent<TextMeshProUGUI>();
        text.text = value;
        text.font = font;
        text.fontSize = size;
        text.color = Color.white;
        text.alignment = alignment;
        text.characterSpacing = 1.5f;
        text.enableWordWrapping = true;
        text.raycastTarget = false;

        RectTransform rect = text.GetComponent<RectTransform>();
        rect.sizeDelta = new Vector2(960f, 90f);
        rect.anchoredPosition = position;

        if (alignment == TextAlignmentOptions.TopLeft)
        {
            rect.anchorMin = new Vector2(0f, 1f);
            rect.anchorMax = new Vector2(0f, 1f);
            rect.pivot = new Vector2(0f, 1f);
        }
        else if (alignment == TextAlignmentOptions.Bottom)
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

    private static GameObject Panel(Transform parent, string name, Vector2 position, Vector2 size, Color color)
    {
        GameObject panel = new GameObject(name);
        panel.transform.SetParent(parent, false);
        Image image = panel.AddComponent<Image>();
        image.color = color;

        RectTransform rect = panel.GetComponent<RectTransform>();
        rect.anchorMin = new Vector2(1f, 1f);
        rect.anchorMax = new Vector2(1f, 1f);
        rect.pivot = new Vector2(1f, 1f);
        rect.anchoredPosition = position;
        rect.sizeDelta = size;
        return panel;
    }

    private static GameObject TopLeftPanel(Transform parent, string name, Vector2 position, Vector2 size, Color color)
    {
        GameObject panel = new GameObject(name);
        panel.transform.SetParent(parent, false);
        Image image = panel.AddComponent<Image>();
        image.color = color;

        RectTransform rect = panel.GetComponent<RectTransform>();
        rect.anchorMin = new Vector2(0f, 1f);
        rect.anchorMax = new Vector2(0f, 1f);
        rect.pivot = new Vector2(0f, 1f);
        rect.anchoredPosition = position;
        rect.sizeDelta = size;
        return panel;
    }

    private static GameObject BottomLeftPanel(Transform parent, string name, Vector2 position, Vector2 size, Color color)
    {
        GameObject panel = new GameObject(name);
        panel.transform.SetParent(parent, false);
        Image image = panel.AddComponent<Image>();
        image.color = color;

        RectTransform rect = panel.GetComponent<RectTransform>();
        rect.anchorMin = new Vector2(0f, 0f);
        rect.anchorMax = new Vector2(0f, 0f);
        rect.pivot = new Vector2(0f, 0f);
        rect.anchoredPosition = position;
        rect.sizeDelta = size;
        return panel;
    }

    private static GameObject CenterPanel(Transform parent, string name, Vector2 size, Color color)
    {
        GameObject panel = new GameObject(name);
        panel.transform.SetParent(parent, false);
        Image image = panel.AddComponent<Image>();
        image.color = color;

        RectTransform rect = panel.GetComponent<RectTransform>();
        rect.anchorMin = new Vector2(0.5f, 0.5f);
        rect.anchorMax = new Vector2(0.5f, 0.5f);
        rect.pivot = new Vector2(0.5f, 0.5f);
        rect.anchoredPosition = Vector2.zero;
        rect.sizeDelta = size;
        return panel;
    }

    private static GameObject StretchPanel(Transform parent, string name, Color color)
    {
        GameObject panel = new GameObject(name);
        panel.transform.SetParent(parent, false);
        Image image = panel.AddComponent<Image>();
        image.color = color;

        RectTransform rect = panel.GetComponent<RectTransform>();
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;
        return panel;
    }

    private static void CreateMissionBoard(Material wall, Material cyan, Material yellow)
    {
        Cube("Mission Board Backplate", new Vector3(0f, 1.55f, 20.7f), new Vector3(5.8f, 2f, 0.18f), wall);
        Cube("Mission Board Header", new Vector3(0f, 2.45f, 20.58f), new Vector3(5.5f, 0.18f, 0.08f), yellow);
        Cube("Mission Board Glow", new Vector3(0f, 1.45f, 20.52f), new Vector3(5.3f, 1.35f, 0.06f), cyan);
    }

    private static void CreateHazardLeak(string name, Vector3 position, Vector3 scale, Material hazard)
    {
        GameObject leak = Cube(name, position, scale, hazard);
        leak.GetComponent<Collider>().isTrigger = true;
        leak.AddComponent<HazardZone>();
        WorldLabel("LEAK\nLOCAL O2 x2.6", position + new Vector3(0f, 1.15f, 0f), labelLookAt, Color.red, 0.014f);
    }

    private static string SequenceText(KeyCode[] sequence)
    {
        if (sequence == null || sequence.Length == 0)
        {
            return "1-2-3";
        }

        string text = "";
        for (int i = 0; i < sequence.Length; i++)
        {
            if (i > 0)
            {
                text += "-";
            }

            text += KeyDigit(sequence[i]);
        }

        return text;
    }

    private static string KeyDigit(KeyCode key)
    {
        switch (key)
        {
            case KeyCode.Alpha1:
                return "1";
            case KeyCode.Alpha2:
                return "2";
            case KeyCode.Alpha3:
                return "3";
            case KeyCode.Alpha4:
                return "4";
            default:
                return "?";
        }
    }

    private static void Wall(string name, Vector3 position, Vector3 scale, Material material, Material trim)
    {
        Vector3 wallPosition = position;
        Vector3 wallScale = scale;
        if (scale.y >= 3f)
        {
            wallScale.y = StationWallHeight;
            wallPosition.y = StationWallHeight * 0.5f;
        }

        if (UseFullImportedCorridorShells)
        {
            CollisionBox(name + " Collision", wallPosition, wallScale);
            return;
        }

        Cube(name, wallPosition, wallScale, material);
        float trimOffset = -Mathf.Sign(position.z) * 0.03f;
        float upperY = wallScale.y * 0.5f - 0.22f;
        float lowerY = -wallScale.y * 0.5f + 0.28f;
        Cube(name + " Upper Trim", wallPosition + new Vector3(0f, upperY, trimOffset), new Vector3(Mathf.Max(0.15f, wallScale.x), 0.12f, Mathf.Max(0.08f, wallScale.z)), trim);
        Cube(name + " Lower Trim", wallPosition + new Vector3(0f, lowerY, trimOffset), new Vector3(Mathf.Max(0.15f, wallScale.x), 0.12f, Mathf.Max(0.08f, wallScale.z)), trim);
    }

    private static void CeilingLight(Vector3 position, Material material)
    {
        Cube("Ceiling Light Bar", position, new Vector3(2f, 0.08f, 0.22f), material);
    }

    private static GameObject PlacePrefabVisual(string assetPath, Vector3 position, Quaternion rotation, Vector3 scale, Transform parent, string instanceName)
    {
        GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(assetPath);
        if (prefab == null)
        {
            return null;
        }

        GameObject instance = PrefabUtility.InstantiatePrefab(prefab) as GameObject;
        if (instance == null)
        {
            instance = Object.Instantiate(prefab);
        }

        instance.name = instanceName;
        instance.transform.position = position;
        instance.transform.rotation = rotation;
        instance.transform.localScale = scale;
        if (parent != null)
        {
            instance.transform.SetParent(parent, true);
        }

        RemoveImportedColliders(instance);
        return instance;
    }

    private static GameObject PlacePrefabVisualLocal(string assetPath, Transform parent, Vector3 localPosition, Quaternion localRotation, Vector3 scale, string instanceName)
    {
        GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(assetPath);
        if (prefab == null)
        {
            return null;
        }

        GameObject instance = PrefabUtility.InstantiatePrefab(prefab) as GameObject;
        if (instance == null)
        {
            instance = Object.Instantiate(prefab);
        }

        instance.name = instanceName;
        instance.transform.SetParent(parent, false);
        instance.transform.localPosition = localPosition;
        instance.transform.localRotation = localRotation;
        instance.transform.localScale = scale;
        RemoveImportedColliders(instance);
        return instance;
    }

    private static void RemoveImportedColliders(GameObject root)
    {
        Collider[] colliders = root.GetComponentsInChildren<Collider>(true);
        foreach (Collider collider in colliders)
        {
            Object.DestroyImmediate(collider);
        }
    }

    private static GameObject SolidCube(string name, Transform parent, Vector3 localPosition, Vector3 scale, Material material)
    {
        GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
        cube.name = name;
        cube.transform.SetParent(parent, false);
        cube.transform.localPosition = localPosition;
        cube.transform.localScale = scale;
        cube.GetComponent<Renderer>().material = material;
        return cube;
    }

    private static GameObject VisualCube(string name, Transform parent, Vector3 localPosition, Vector3 scale, Material material)
    {
        GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
        cube.name = name;
        cube.transform.SetParent(parent, false);
        cube.transform.localPosition = localPosition;
        cube.transform.localScale = scale;
        cube.GetComponent<Renderer>().material = material;
        Object.DestroyImmediate(cube.GetComponent<Collider>());
        return cube;
    }

    private static GameObject VisualSphere(string name, Transform parent, Vector3 localPosition, Vector3 scale, Material material)
    {
        GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        sphere.name = name;
        sphere.transform.SetParent(parent, false);
        sphere.transform.localPosition = localPosition;
        sphere.transform.localScale = scale;
        sphere.GetComponent<Renderer>().material = material;
        Object.DestroyImmediate(sphere.GetComponent<Collider>());
        return sphere;
    }

    private static GameObject VisualCapsule(string name, Transform parent, Vector3 localPosition, Vector3 scale, Quaternion localRotation, Material material)
    {
        GameObject capsule = GameObject.CreatePrimitive(PrimitiveType.Capsule);
        capsule.name = name;
        capsule.transform.SetParent(parent, false);
        capsule.transform.localPosition = localPosition;
        capsule.transform.localRotation = localRotation;
        capsule.transform.localScale = scale;
        capsule.GetComponent<Renderer>().material = material;
        Object.DestroyImmediate(capsule.GetComponent<Collider>());
        return capsule;
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
        light.shadows = LightShadows.Soft;
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

    private static GameObject Deck(string name, Vector3 position, Vector3 scale, Material material)
    {
        if (UseFullImportedCorridorShells)
        {
            return CollisionBox(name + " Collision", position, scale);
        }

        return Cube(name, position, scale, material);
    }

    private static GameObject CollisionBox(string name, Vector3 position, Vector3 scale)
    {
        GameObject box = GameObject.CreatePrimitive(PrimitiveType.Cube);
        box.name = name;
        box.transform.position = position;
        box.transform.localScale = scale;
        Object.DestroyImmediate(box.GetComponent<Renderer>());
        return box;
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

    private static void SetInt(Object target, string propertyName, int value)
    {
        SerializedObject serialized = new SerializedObject(target);
        serialized.FindProperty(propertyName).intValue = value;
        serialized.ApplyModifiedPropertiesWithoutUndo();
    }

    private static void SetFloat(Object target, string propertyName, float value)
    {
        SerializedObject serialized = new SerializedObject(target);
        serialized.FindProperty(propertyName).floatValue = value;
        serialized.ApplyModifiedPropertiesWithoutUndo();
    }
}

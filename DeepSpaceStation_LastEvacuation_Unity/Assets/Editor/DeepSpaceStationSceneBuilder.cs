using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public static class DeepSpaceStationSceneBuilder
{
    private static readonly Vector3 PlayerStart = new Vector3(0f, 1.1f, 18f);
    private const float StationCeilingY = 4.32f;
    private const float StationWallHeight = 4.2f;

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

        CreateLighting(cyan, red, green);
        CreateStation(deck, wall, dark, yellow, cyan, red, orange, glass, hazard, whiteGlow);

        new GameObject("GameManager").AddComponent<GameManager>();
        GameObject player = CreatePlayer(suit, glass, cyan, dark, yellow);
        CreateHUD();

        CreateMissionBoard(wall, cyan, yellow);

        CreateTerminal(new Vector3(0f, 0f, 5.8f), "Corridor Relay", 3.2f, cyan, wall, green, yellow, KeyCode.Alpha1, KeyCode.Alpha3, KeyCode.Alpha2);
        CreateTerminal(new Vector3(-11.4f, 0f, 0f), "Life Support", 3.8f, cyan, wall, green, yellow, KeyCode.Alpha2, KeyCode.Alpha1, KeyCode.Alpha3);
        CreateTerminal(new Vector3(11.5f, 0f, -4.4f), "Navigation", 4.1f, cyan, wall, green, yellow, KeyCode.Alpha4, KeyCode.Alpha2, KeyCode.Alpha1);
        CreateTerminal(new Vector3(-11.8f, 0f, -13.8f), "Medical Air Mix", 4.2f, cyan, wall, green, yellow, KeyCode.Alpha2, KeyCode.Alpha4, KeyCode.Alpha3);
        CreateTerminal(new Vector3(11.8f, 0f, -17.5f), "Security Override", 4.5f, cyan, wall, green, yellow, KeyCode.Alpha3, KeyCode.Alpha2, KeyCode.Alpha4);
        CreateTerminal(new Vector3(-11.7f, 0f, -28.2f), "Reactor", 5.4f, cyan, wall, green, yellow, KeyCode.Alpha3, KeyCode.Alpha1, KeyCode.Alpha4, KeyCode.Alpha2);
        CreateTerminal(new Vector3(11.5f, 0f, -31.6f), "Comms Relay", 5f, cyan, wall, green, yellow, KeyCode.Alpha1, KeyCode.Alpha4, KeyCode.Alpha2, KeyCode.Alpha3);

        CreateDoor("Pod Bay Exit", new Vector3(0f, 1.55f, 12.4f), Quaternion.identity, false, 0, wall, cyan, cyan, "", "", false, 5.25f);
        CreateDoor("Life Support Bulkhead", new Vector3(-4.2f, 1.55f, 0f), Quaternion.Euler(0f, 90f, 0f), true, 1, wall, cyan, cyan);
        CreateDoor("Navigation Bulkhead", new Vector3(4.2f, 1.55f, -4.4f), Quaternion.Euler(0f, 90f, 0f), true, 1, wall, cyan, cyan);
        CreateDoor("Medical Bay Bulkhead", new Vector3(-4.2f, 1.55f, -13.8f), Quaternion.Euler(0f, 90f, 0f), true, 2, wall, cyan, green);
        CreateDoor("Security Office Bulkhead", new Vector3(4.2f, 1.55f, -17.5f), Quaternion.Euler(0f, 90f, 0f), true, 3, wall, cyan, orange, "security_keycard", "Security Keycard", false);
        CreateDoor("Reactor Fuse Lock", new Vector3(-4.2f, 1.55f, -28.2f), Quaternion.Euler(0f, 90f, 0f), true, 4, wall, cyan, red, "reactor_fuse", "Reactor Fuse", true);
        CreateDoor("Comms Decoder Lock", new Vector3(4.2f, 1.55f, -31.6f), Quaternion.Euler(0f, 90f, 0f), true, 5, wall, cyan, red, "comms_decoder", "Comms Decoder", false);
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

        CreateSecurityRobot(new Vector3(0f, 0.75f, -18f), dark, red, cyan, new Vector3(0f, 0.75f, -8f), new Vector3(0f, 0.75f, -27f));
        CreateSecurityRobot(new Vector3(10.6f, 0.75f, -17.5f), dark, red, orange, new Vector3(7.3f, 0.75f, -17.5f), new Vector3(14.2f, 0.75f, -17.5f));

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

    private static void CreateStation(Material deck, Material wall, Material dark, Material yellow, Material cyan, Material red, Material orange, Material glass, Material hazard, Material whiteGlow)
    {
        Cube("Starting Pod Bay Deck", new Vector3(0f, -0.12f, 17f), new Vector3(10f, 0.24f, 8f), deck);
        Cube("Main Corridor Deck", new Vector3(0f, -0.12f, -12f), new Vector3(8f, 0.24f, 54f), deck);
        Cube("Life Support Deck", new Vector3(-10.8f, -0.1f, 0f), new Vector3(13f, 0.24f, 9f), deck);
        Cube("Navigation Deck", new Vector3(10.8f, -0.1f, -4.4f), new Vector3(13f, 0.24f, 9f), deck);
        Cube("Medical Bay Deck", new Vector3(-10.8f, -0.1f, -13.8f), new Vector3(13f, 0.24f, 8.8f), deck);
        Cube("Security Office Deck", new Vector3(10.8f, -0.1f, -17.5f), new Vector3(13f, 0.24f, 8.8f), deck);
        Cube("Reactor Deck", new Vector3(-10.8f, -0.1f, -28.2f), new Vector3(13f, 0.24f, 9f), deck);
        Cube("Comms Lab Deck", new Vector3(10.8f, -0.1f, -31.6f), new Vector3(13f, 0.24f, 9f), deck);
        Cube("Escape Bay Deck", new Vector3(0f, -0.1f, -44.2f), new Vector3(9f, 0.24f, 8.5f), deck);
        Cube("Sealed Outer Hull Floor", new Vector3(0f, -0.28f, -14f), new Vector3(38f, 0.16f, 74f), dark);

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
        Wall("Corridor East Wall B", new Vector3(4.2f, 1.55f, -9f), new Vector3(0.45f, 3.1f, 7f), wall, cyan);
        Wall("Corridor West Wall C", new Vector3(-4.2f, 1.55f, -21.2f), new Vector3(0.45f, 3.1f, 10f), wall, cyan);
        Wall("Corridor East Wall C", new Vector3(4.2f, 1.55f, -25f), new Vector3(0.45f, 3.1f, 10f), wall, cyan);
        Wall("Corridor West Wall D", new Vector3(-4.2f, 1.55f, -35.2f), new Vector3(0.45f, 3.1f, 7f), wall, cyan);
        Wall("Corridor East Wall D", new Vector3(4.2f, 1.55f, -39.8f), new Vector3(0.45f, 3.1f, 5.5f), wall, cyan);

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

        for (int z = -46; z <= 20; z += 4)
        {
            Cube("Floor Rib Z " + z, new Vector3(0f, 0.025f, z), new Vector3(32f, 0.035f, 0.09f), dark);
        }

        for (int x = -16; x <= 16; x += 4)
        {
            Cube("Floor Rib X " + x, new Vector3(x, 0.035f, -13f), new Vector3(0.09f, 0.04f, 64f), dark);
        }

        CreateFloorPath(cyan, yellow, red);
        CreatePipes(dark, cyan, orange);
        CreateWallPanelDetails(wall, dark, cyan, whiteGlow);
        CreateProps(dark, wall, yellow, glass, cyan, whiteGlow);

        CreateHazardLeak("Broken Corridor Vent", new Vector3(0f, 0.07f, -20.8f), new Vector3(3.2f, 0.12f, 2.4f), hazard);
        CreateHazardLeak("Reactor Radiation Leak", new Vector3(-9.2f, 0.07f, -29.8f), new Vector3(3.8f, 0.12f, 3.4f), hazard);
        CreateHazardLeak("Airlock Vent Leak", new Vector3(0f, 0.07f, -40.8f), new Vector3(2.8f, 0.12f, 2.2f), hazard);
    }

    private static void CreateCeilings(Material wall, Material cyan, Material whiteGlow)
    {
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
        Wall(name + " Door Side A", new Vector3(doorSideX, 1.55f, centerZ + 2.8f), new Vector3(0.45f, 3.1f, 3.4f), wall, cyan);
        Wall(name + " Door Side B", new Vector3(doorSideX, 1.55f, centerZ - 2.8f), new Vector3(0.45f, 3.1f, 3.4f), wall, cyan);
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
        WorldLabel("MISSION:\nThis bay is only the start\nRepair corridor systems\nFind tools to open deeper rooms", new Vector3(0f, 1.9f, 19.2f), PlayerStart, Color.white, 0.015f);
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

        Light status = PointLight(systemName + " Terminal Beacon", basePosition + new Vector3(0f, 2.1f, -0.35f), Color.red, 3f, 5f);
        status.transform.SetParent(root.transform);
        SetObject(task, "screenRenderer", screen.GetComponent<Renderer>());
        SetObject(task, "statusLight", status);

        WorldLabel("E START\n" + systemName.ToUpper() + "\nCODE " + SequenceText(sequence), basePosition + new Vector3(0f, 2.55f, -0.25f), PlayerStart, Color.cyan, 0.013f).transform.SetParent(root.transform);
        PointLight(systemName + " Area Light", basePosition + new Vector3(0f, 2.7f, 0f), new Color(0.1f, 0.9f, 1f), 2.8f, 6f).transform.SetParent(root.transform);
    }

    private static void CreateDoor(string name, Vector3 position, Quaternion rotation, bool requiresRepairs, int requiredRepairCount, Material wall, Material yellow, Material beaconMaterial, string requiredItemName = "", string requiredItemDisplayName = "", bool consumeRequiredItem = false, float doorWidth = 2.25f)
    {
        GameObject root = new GameObject(name);
        root.transform.position = position;
        root.transform.rotation = rotation;

        BoxCollider hitbox = root.AddComponent<BoxCollider>();
        hitbox.isTrigger = true;
        hitbox.size = new Vector3(doorWidth + 1.3f, 3.1f, 1.3f);

        float sideX = (doorWidth * 0.5f) + 0.25f;
        SolidCube(name + " Frame Left", root.transform, new Vector3(-sideX, 0f, 0f), new Vector3(0.35f, 3.2f, 0.45f), yellow);
        SolidCube(name + " Frame Right", root.transform, new Vector3(sideX, 0f, 0f), new Vector3(0.35f, 3.2f, 0.45f), yellow);
        SolidCube(name + " Frame Top", root.transform, new Vector3(0f, 1.45f, 0f), new Vector3(doorWidth + 0.7f, 0.32f, 0.45f), yellow);
        SolidCube(name + " Frame Bottom", root.transform, new Vector3(0f, -1.45f, 0f), new Vector3(doorWidth + 0.7f, 0.18f, 0.45f), yellow);
        GameObject panel = SolidCube(name + " Sliding Panel", root.transform, Vector3.zero, new Vector3(doorWidth, 2.55f, 0.38f), wall);
        SolidCube(name + " Status Strip", root.transform, new Vector3(0f, 1.18f, -0.27f), new Vector3(Mathf.Max(0.5f, doorWidth - 0.15f), 0.14f, 0.05f), beaconMaterial);
        SolidCube(name + " Center Access Line", root.transform, new Vector3(0f, 0f, -0.28f), new Vector3(0.12f, 2f, 0.05f), beaconMaterial);
        SolidCube(name + " Access Screen", root.transform, new Vector3(sideX + 0.42f, 0.15f, -0.32f), new Vector3(0.36f, 0.62f, 0.06f), beaconMaterial);

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

        WorldLabel(label, position + rotation * new Vector3(0f, 2.15f, -0.65f), PlayerStart, requiresRepairs ? Color.red : Color.cyan, 0.013f).transform.SetParent(root.transform);
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
        WorldLabel("E TAKE\n" + displayName.ToUpper(), position + new Vector3(0f, 1.1f, 0f), PlayerStart, Color.yellow, 0.012f).transform.SetParent(pickup.transform);
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
        WorldLabel("E PICK UP\n" + displayName.ToUpper(), position + new Vector3(0f, 1.15f, 0f), PlayerStart, Color.yellow, 0.012f).transform.SetParent(pickup.transform);
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
        WorldLabel("SECURITY ROBOT\nAvoid contact", position + new Vector3(0f, 2.15f, 0f), PlayerStart, Color.red, 0.012f);
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
        Color uiBlue = new Color(0.02f, 0.42f, 0.88f);
        Color uiInk = new Color(0.08f, 0.14f, 0.22f);
        Color uiPanel = new Color(0.93f, 0.97f, 1f, 0.82f);
        GameObject missionPanel = TopLeftPanel(canvasObject.transform, "MissionPanel", new Vector2(16f, -16f), new Vector2(302f, 154f), uiPanel);
        Text missionTitle = TextUI(missionPanel.transform, "MissionTitle", "MISSIONS", new Vector2(14f, -10f), TextAnchor.UpperLeft, 18, font);
        missionTitle.color = uiBlue;
        Text objective = TextUI(missionPanel.transform, "ObjectiveText", "", new Vector2(14f, -36f), TextAnchor.UpperLeft, 15, font);
        objective.color = uiInk;
        objective.GetComponent<RectTransform>().sizeDelta = new Vector2(270f, 50f);
        Text timer = TextUI(missionPanel.transform, "TimerText", "TIME 10:20", new Vector2(14f, -92f), TextAnchor.UpperLeft, 18, font);
        timer.color = uiBlue;
        Text repairs = TextUI(missionPanel.transform, "RepairText", "SYSTEMS 0/7", new Vector2(148f, -94f), TextAnchor.UpperLeft, 16, font);
        repairs.color = uiInk;
        Text pressure = TextUI(missionPanel.transform, "PressureText", "O2 DRAIN x1.0", new Vector2(14f, -122f), TextAnchor.UpperLeft, 15, font);
        pressure.color = uiInk;

        GameObject statusPanel = BottomLeftPanel(canvasObject.transform, "StatusPanel", new Vector2(16f, 16f), new Vector2(306f, 154f), uiPanel);
        Text oxygen = TextUI(statusPanel.transform, "OxygenText", "O2 100%", new Vector2(14f, -12f), TextAnchor.UpperLeft, 16, font);
        oxygen.color = uiInk;
        Image oxygenFill = Bar(statusPanel.transform, "OxygenFill", new Vector2(14f, -36f), new Vector2(190f, 10f), new Color(0.05f, 0.95f, 0.8f));
        Text health = TextUI(statusPanel.transform, "HealthText", "HEALTH 100/100", new Vector2(14f, -56f), TextAnchor.UpperLeft, 16, font);
        health.color = uiInk;
        Image healthFill = Bar(statusPanel.transform, "HealthFill", new Vector2(14f, -80f), new Vector2(190f, 10f), new Color(0.08f, 0.5f, 1f));
        Text battery = TextUI(statusPanel.transform, "BatteryText", "BATTERY 100%", new Vector2(14f, -100f), TextAnchor.UpperLeft, 16, font);
        battery.color = uiInk;
        Image batteryFill = Bar(statusPanel.transform, "BatteryFill", new Vector2(150f, -104f), new Vector2(76f, 10f), new Color(0.18f, 0.65f, 1f));
        Text inventory = TextUI(statusPanel.transform, "InventoryText", "TAB/I BAG   Q SCAN   C CROUCH", new Vector2(14f, -126f), TextAnchor.UpperLeft, 14, font);
        inventory.color = uiBlue;
        Text crosshair = TextUI(canvasObject.transform, "Crosshair", "+", new Vector2(0f, 0f), TextAnchor.MiddleCenter, 30, font);
        crosshair.color = new Color(0.02f, 0.42f, 0.88f, 0.9f);
        Text prompt = TextUI(canvasObject.transform, "PromptText", "", new Vector2(0f, 84f), TextAnchor.LowerCenter, 28, font);
        prompt.color = uiBlue;
        Text message = TextUI(canvasObject.transform, "MessageText", "", new Vector2(0f, 150f), TextAnchor.LowerCenter, 26, font);
        message.color = uiBlue;

        GameObject backpackPanel = Panel(canvasObject.transform, "BackpackPanel", new Vector2(-24f, -24f), new Vector2(360f, 420f), new Color(0.93f, 0.97f, 1f, 0.92f));
        Text backpackTitle = TextUI(backpackPanel.transform, "BackpackTitle", "BACKPACK", new Vector2(22f, -18f), TextAnchor.UpperLeft, 24, font);
        backpackTitle.color = uiBlue;
        Text backpackHint = TextUI(backpackPanel.transform, "BackpackHint", "Tab / I", new Vector2(22f, -54f), TextAnchor.UpperLeft, 16, font);
        backpackHint.color = uiBlue;
        Text backpackItems = TextUI(backpackPanel.transform, "BackpackItems", "No items", new Vector2(22f, -94f), TextAnchor.UpperLeft, 20, font);
        backpackItems.color = uiInk;
        backpackItems.GetComponent<RectTransform>().sizeDelta = new Vector2(316f, 300f);
        backpackPanel.SetActive(false);

        GameObject scanPanel = StretchPanel(canvasObject.transform, "ScanPanel", new Color(0.02f, 0.35f, 0.65f, 0.34f));
        Text scanTitle = TextUI(scanPanel.transform, "ScanTitle", "SCAN MODE", new Vector2(0f, 230f), TextAnchor.MiddleCenter, 34, font);
        scanTitle.color = new Color(0.85f, 1f, 1f);
        Text scanText = TextUI(scanPanel.transform, "ScanText", "", new Vector2(-360f, 60f), TextAnchor.MiddleCenter, 24, font);
        scanText.alignment = TextAnchor.UpperLeft;
        scanText.GetComponent<RectTransform>().sizeDelta = new Vector2(520f, 260f);
        Text scanHint = TextUI(scanPanel.transform, "ScanHint", "HOLD Q TO SCAN", new Vector2(0f, -250f), TextAnchor.MiddleCenter, 22, font);
        scanHint.color = new Color(1f, 0.86f, 0.25f);
        scanPanel.SetActive(false);

        GameObject pausePanel = CenterPanel(canvasObject.transform, "PausePanel", new Vector2(470f, 430f), new Color(0.93f, 0.97f, 1f, 0.94f));
        Text pauseTitle = TextUI(pausePanel.transform, "PauseTitle", "// PAUSED //", new Vector2(0f, 130f), TextAnchor.MiddleCenter, 34, font);
        pauseTitle.color = uiBlue;
        Text pauseBody = TextUI(pausePanel.transform, "PauseBody", "ESC  Resume\nR    Restart Checkpoint\nTAB  Backpack\nQ    Scanner\nH    Use Medkit\nB    Use Battery", new Vector2(0f, 18f), TextAnchor.MiddleCenter, 22, font);
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

        Text endTitle = TextUI(panel.transform, "EndTitle", "", new Vector2(0f, 70f), TextAnchor.MiddleCenter, 40, font);
        endTitle.color = uiBlue;
        Text endBody = TextUI(panel.transform, "EndBody", "", new Vector2(0f, -20f), TextAnchor.MiddleCenter, 23, font);
        endBody.color = uiInk;
        panel.SetActive(false);

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
        SetObject(hud, "pausePanel", pausePanel);
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
        WorldLabel("LEAK\nLOCAL O2 x2.6", position + new Vector3(0f, 1.15f, 0f), PlayerStart, Color.red, 0.014f);
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

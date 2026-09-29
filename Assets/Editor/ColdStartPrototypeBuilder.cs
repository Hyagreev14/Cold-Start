using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class ColdStartPrototypeBuilder
{
    private const float FloorWidth = 18f;
    private const float FloorDepth = 14f;
    private const float FloorHeight = 4f;
    private const float FloorThickness = 0.25f;
    private const float WallThickness = 0.25f;

    [MenuItem("Cold Start/Build Player Test Scene")]
    public static void BuildPlayerTestScene()
    {
        Scene scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);

        CreateBuildingShell();
        CreatePlayer();
        CreateLighting();

        EditorSceneManager.SaveScene(scene, "Assets/Scenes/PlayerTest.unity");

        GameObject player = GameObject.Find("Player");
        Selection.activeGameObject = player;

        Debug.Log("Cold Start HQ blockout created. 2 above-ground floors with stairs. Controls: WASD + Mouse, Ctrl sprint, Shift crouch, Space jump.");
    }

    private static void CreateBuildingShell()
    {
        CreateCube(
            "Ground Floor",
            new Vector3(0f, 0f, 0f),
            new Vector3(FloorWidth, FloorThickness, FloorDepth)
        );

        CreateSecondFloorWithStairOpening();

        CreateCube(
            "Roof",
            new Vector3(0f, FloorHeight * 2f, 0f),
            new Vector3(FloorWidth, FloorThickness, FloorDepth)
        );

        float wallHeight = FloorHeight * 2f;

        CreateCube(
            "North Wall",
            new Vector3(0f, FloorHeight, FloorDepth * 0.5f),
            new Vector3(FloorWidth, wallHeight, WallThickness)
        );

        CreateCube(
            "South Wall",
            new Vector3(0f, FloorHeight, -FloorDepth * 0.5f),
            new Vector3(FloorWidth, wallHeight, WallThickness)
        );

        CreateCube(
            "East Wall",
            new Vector3(FloorWidth * 0.5f, FloorHeight, 0f),
            new Vector3(WallThickness, wallHeight, FloorDepth)
        );

        CreateCube(
            "West Wall",
            new Vector3(-FloorWidth * 0.5f, FloorHeight, 0f),
            new Vector3(WallThickness, wallHeight, FloorDepth)
        );

        CreateStairs();
        PaintAllWalls();
    }

    private static void PaintAllWalls()
    {
        Shader shader = Shader.Find("Universal Render Pipeline/Lit");
        if (shader == null)
        {
            Debug.LogWarning("Cold Start: URP/Lit shader was not found, so wall paint could not be applied.");
            return;
        }

        Material wallMaterial = new Material(shader);
        wallMaterial.name = "HQ Wall Paint";
        wallMaterial.color = new Color(0.72f, 0.72f, 0.68f);

        string[] wallNames =
        {
            "North Wall",
            "South Wall",
            "East Wall",
            "West Wall"
        };

        foreach (string wallName in wallNames)
        {
            GameObject wall = GameObject.Find(wallName);
            if (wall == null)
            {
                Debug.LogWarning($"Cold Start: Could not find {wallName} while applying wall paint.");
                continue;
            }

            Renderer renderer = wall.GetComponent<Renderer>();
            if (renderer != null)
                renderer.material = wallMaterial;
        }
    }

    private static void CreateSecondFloorWithStairOpening()
    {
        const float openingWidth = 3.2f;
        const float openingDepth = 7.0f;
        const float openingCenterX = -5.5f;
        const float openingCenterZ = -1.25f;

        float buildingMinX = -FloorWidth * 0.5f;
        float buildingMaxX = FloorWidth * 0.5f;
        float openingMinX = openingCenterX - openingWidth * 0.5f;
        float openingMaxX = openingCenterX + openingWidth * 0.5f;

        float leftWidth = openingMinX - buildingMinX;
        float rightWidth = buildingMaxX - openingMaxX;

        float buildingMinZ = -FloorDepth * 0.5f;
        float buildingMaxZ = FloorDepth * 0.5f;
        float openingMinZ = openingCenterZ - openingDepth * 0.5f;
        float openingMaxZ = openingCenterZ + openingDepth * 0.5f;

        float frontDepth = buildingMaxZ - openingMaxZ;
        float backDepth = openingMinZ - buildingMinZ;

        CreateCube(
            "Second Floor West Section",
            new Vector3(
                buildingMinX + leftWidth * 0.5f,
                FloorHeight,
                0f
            ),
            new Vector3(leftWidth, FloorThickness, FloorDepth)
        );

        CreateCube(
            "Second Floor East Section",
            new Vector3(
                openingMaxX + rightWidth * 0.5f,
                FloorHeight,
                0f
            ),
            new Vector3(rightWidth, FloorThickness, FloorDepth)
        );

        CreateCube(
            "Second Floor North Section",
            new Vector3(
                openingCenterX,
                FloorHeight,
                openingMaxZ + frontDepth * 0.5f
            ),
            new Vector3(openingWidth, FloorThickness, frontDepth)
        );

        CreateCube(
            "Second Floor South Section",
            new Vector3(
                openingCenterX,
                FloorHeight,
                buildingMinZ + backDepth * 0.5f
            ),
            new Vector3(openingWidth, FloorThickness, backDepth)
        );
    }

    private static void CreateStairs()
    {
        const int stepCount = 14;
        const float stepWidth = 2.4f;
        const float stepDepth = 0.5f;

        float stepHeight = FloorHeight / stepCount;
        float startZ = -4.75f;

        for (int i = 0; i < stepCount; i++)
        {
            float height = stepHeight * (i + 1);
            float z = startZ + stepDepth * i;

            CreateCube(
                $"Stair {i + 1:00}",
                new Vector3(-5.5f, height * 0.5f + FloorThickness * 0.5f, z),
                new Vector3(stepWidth, height, stepDepth)
            );
        }
    }

    private static void CreatePlayer()
    {
        GameObject player = new GameObject("Player");
        player.transform.position = new Vector3(0f, 0.15f, 0f);

        CharacterController controller = player.AddComponent<CharacterController>();
        controller.height = 1.8f;
        controller.radius = 0.35f;
        controller.center = new Vector3(0f, 0.9f, 0f);
        controller.stepOffset = 0.3f;
        controller.slopeLimit = 45f;

        player.AddComponent<PlayerController>();

        GameObject cameraObject = new GameObject("Player Camera");
        Camera camera = cameraObject.AddComponent<Camera>();
        camera.tag = "MainCamera";
        cameraObject.transform.SetParent(player.transform);
        cameraObject.transform.localPosition = new Vector3(0f, 1.65f, 0f);
        cameraObject.transform.localRotation = Quaternion.identity;
    }

    private static void CreateLighting()
    {
        Light lightObject = new GameObject("Sun").AddComponent<Light>();
        lightObject.type = LightType.Directional;
        lightObject.transform.rotation = Quaternion.Euler(50f, -30f, 0f);
    }

    private static GameObject CreateCube(string objectName, Vector3 position, Vector3 scale)
    {
        GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
        cube.name = objectName;
        cube.transform.position = position;
        cube.transform.localScale = scale;
        return cube;
    }
}

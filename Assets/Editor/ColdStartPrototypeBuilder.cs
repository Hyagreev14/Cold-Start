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
    }

    private static void CreateSecondFloorWithStairOpening()
    {
        float openingWidth = 3.2f;
        float openingDepth = 7.5f;
        float openingCenterX = -5.5f;
        float openingCenterZ = -1.0f;

        float sideWidth = (FloorWidth - openingWidth) * 0.5f;
        float frontDepth = (FloorDepth - openingDepth) * 0.5f;

        CreateCube(
            "Second Floor West Section",
            new Vector3(
                -FloorWidth * 0.5f + sideWidth * 0.5f,
                FloorHeight,
                0f
            ),
            new Vector3(sideWidth, FloorThickness, FloorDepth)
        );

        CreateCube(
            "Second Floor East Section",
            new Vector3(
                FloorWidth * 0.5f - sideWidth * 0.5f,
                FloorHeight,
                0f
            ),
            new Vector3(sideWidth, FloorThickness, FloorDepth)
        );

        CreateCube(
            "Second Floor North Section",
            new Vector3(
                openingCenterX,
                FloorHeight,
                FloorDepth * 0.5f - frontDepth * 0.5f
            ),
            new Vector3(openingWidth, FloorThickness, frontDepth)
        );

        CreateCube(
            "Second Floor South Section",
            new Vector3(
                openingCenterX,
                FloorHeight,
                -FloorDepth * 0.5f + frontDepth * 0.5f
            ),
            new Vector3(openingWidth, FloorThickness, frontDepth)
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

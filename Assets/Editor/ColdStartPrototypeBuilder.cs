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
        CreateAbandonedDetails();
        CreatePlayer();
        CreateLighting();

        EditorSceneManager.SaveScene(scene, "Assets/Scenes/PlayerTest.unity");

        GameObject player = GameObject.Find("Player");
        Selection.activeGameObject = player;

        Debug.Log("Cold Start abandoned HQ blockout created. 2 above-ground floors with stairs.");
    }

    private static void CreateBuildingShell()
    {
        Material floorMaterial = CreateMaterial(
            "Abandoned Concrete Floor",
            new Color(0.30f, 0.31f, 0.29f)
        );

        Material wallMaterial = CreateMaterial(
            "Faded Wall Paint",
            new Color(0.58f, 0.59f, 0.55f)
        );

        Material roofMaterial = CreateMaterial(
            "Weathered Roof",
            new Color(0.25f, 0.26f, 0.24f)
        );

        CreateCube(
            "Ground Floor",
            new Vector3(0f, 0f, 0f),
            new Vector3(FloorWidth, FloorThickness, FloorDepth),
            floorMaterial
        );

        CreateSecondFloorWithStairOpening(floorMaterial);

        CreateCube(
            "Roof",
            new Vector3(0f, FloorHeight * 2f, 0f),
            new Vector3(FloorWidth, FloorThickness, FloorDepth),
            roofMaterial
        );

        float wallHeight = FloorHeight * 2f;

        CreateCube(
            "North Wall",
            new Vector3(0f, FloorHeight, FloorDepth * 0.5f),
            new Vector3(FloorWidth, wallHeight, WallThickness),
            wallMaterial
        );

        CreateCube(
            "South Wall",
            new Vector3(0f, FloorHeight, -FloorDepth * 0.5f),
            new Vector3(FloorWidth, wallHeight, WallThickness),
            wallMaterial
        );

        CreateCube(
            "East Wall",
            new Vector3(FloorWidth * 0.5f, FloorHeight, 0f),
            new Vector3(WallThickness, wallHeight, FloorDepth),
            wallMaterial
        );

        CreateCube(
            "West Wall",
            new Vector3(-FloorWidth * 0.5f, FloorHeight, 0f),
            new Vector3(WallThickness, wallHeight, FloorDepth),
            wallMaterial
        );

        CreateStairs();
    }

    private static void CreateSecondFloorWithStairOpening(Material floorMaterial)
    {
        const float openingWidth = 3.2f;
        const float openingDepth = 7.0f;
        const float openingCenterX = -5.5f;

        // The stairs occupy z = -5.0 to 2.0, so the opening matches them.
        const float openingCenterZ = -1.5f;

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
            new Vector3(buildingMinX + leftWidth * 0.5f, FloorHeight, 0f),
            new Vector3(leftWidth, FloorThickness, FloorDepth),
            floorMaterial
        );

        CreateCube(
            "Second Floor East Section",
            new Vector3(openingMaxX + rightWidth * 0.5f, FloorHeight, 0f),
            new Vector3(rightWidth, FloorThickness, FloorDepth),
            floorMaterial
        );

        CreateCube(
            "Second Floor North Section",
            new Vector3(openingCenterX, FloorHeight, openingMaxZ + frontDepth * 0.5f),
            new Vector3(openingWidth, FloorThickness, frontDepth),
            floorMaterial
        );

        CreateCube(
            "Second Floor South Section",
            new Vector3(openingCenterX, FloorHeight, buildingMinZ + backDepth * 0.5f),
            new Vector3(openingWidth, FloorThickness, backDepth),
            floorMaterial
        );
    }

    private static void CreateStairs()
    {
        const int stepCount = 14;
        const float stepWidth = 2.4f;
        const float stepDepth = 0.5f;

        Material stairMaterial = CreateMaterial(
            "Worn Stair Concrete",
            new Color(0.34f, 0.34f, 0.31f)
        );

        float stepHeight = FloorHeight / stepCount;
        float startZ = -4.75f;

        for (int i = 0; i < stepCount; i++)
        {
            float height = stepHeight * (i + 1);
            float z = startZ + stepDepth * i;

            CreateCube(
                $"Stair {i + 1:00}",
                new Vector3(-5.5f, height * 0.5f + FloorThickness * 0.5f, z),
                new Vector3(stepWidth, height, stepDepth),
                stairMaterial
            );
        }
    }

    private static void CreateAbandonedDetails()
    {
        Material grimeMaterial = CreateMaterial(
            "Deep Grime",
            new Color(0.12f, 0.13f, 0.12f)
        );

        Material dirtMaterial = CreateMaterial(
            "Floor Dirt",
            new Color(0.20f, 0.19f, 0.16f)
        );

        // Large, subtle floor stains make the facility look neglected.
        CreateCube(
            "Ground Dirt Patch 01",
            new Vector3(3.5f, 0.135f, 2.8f),
            new Vector3(3.0f, 0.012f, 1.3f),
            dirtMaterial
        );

        CreateCube(
            "Ground Dirt Patch 02",
            new Vector3(-1.5f, 0.136f, -4.8f),
            new Vector3(2.0f, 0.012f, 0.8f),
            dirtMaterial
        );

        CreateCube(
            "Second Floor Dirt Patch 01",
            new Vector3(4.5f, FloorHeight + 0.135f, 4.0f),
            new Vector3(2.5f, 0.012f, 1.0f),
            dirtMaterial
        );

        // Vertical grime streaks on each wall, split between the two floors.
        CreateCube(
            "North Wall Grime Ground",
            new Vector3(-3.8f, 1.5f, FloorDepth * 0.5f - 0.13f),
            new Vector3(1.1f, 2.5f, 0.018f),
            grimeMaterial
        );

        CreateCube(
            "North Wall Grime Upper",
            new Vector3(4.2f, 5.7f, FloorDepth * 0.5f - 0.13f),
            new Vector3(1.4f, 2.8f, 0.018f),
            grimeMaterial
        );

        CreateCube(
            "South Wall Grime Ground",
            new Vector3(2.8f, 2.0f, -FloorDepth * 0.5f + 0.13f),
            new Vector3(1.0f, 2.2f, 0.018f),
            grimeMaterial
        );

        CreateCube(
            "South Wall Grime Upper",
            new Vector3(-3.5f, 6.0f, -FloorDepth * 0.5f + 0.13f),
            new Vector3(1.2f, 2.4f, 0.018f),
            grimeMaterial
        );

        CreateCube(
            "East Wall Grime Ground",
            new Vector3(FloorWidth * 0.5f - 0.13f, 1.7f, 2.7f),
            new Vector3(0.018f, 2.6f, 1.1f),
            grimeMaterial
        );

        CreateCube(
            "East Wall Grime Upper",
            new Vector3(FloorWidth * 0.5f - 0.13f, 5.8f, -3.4f),
            new Vector3(0.018f, 2.7f, 1.3f),
            grimeMaterial
        );

        CreateCube(
            "West Wall Grime Ground",
            new Vector3(-FloorWidth * 0.5f + 0.13f, 2.1f, -2.8f),
            new Vector3(0.018f, 2.8f, 1.2f),
            grimeMaterial
        );

        CreateCube(
            "West Wall Grime Upper",
            new Vector3(-FloorWidth * 0.5f + 0.13f, 5.5f, 3.3f),
            new Vector3(0.018f, 2.5f, 1.0f),
            grimeMaterial
        );

        // Small abandoned debris piles.
        CreateCube(
            "Debris 01",
            new Vector3(2.2f, 0.28f, 3.0f),
            new Vector3(0.45f, 0.28f, 0.35f),
            grimeMaterial
        );

        CreateCube(
            "Debris 02",
            new Vector3(2.7f, 0.18f, 3.25f),
            new Vector3(0.3f, 0.18f, 0.55f),
            grimeMaterial
        );

        CreateCube(
            "Debris 03",
            new Vector3(-2.2f, 0.22f, 4.1f),
            new Vector3(0.55f, 0.22f, 0.3f),
            grimeMaterial
        );

        CreateCube(
            "Debris 04",
            new Vector3(5.4f, FloorHeight + 0.2f, 3.1f),
            new Vector3(0.35f, 0.2f, 0.5f),
            grimeMaterial
        );
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
        lightObject.intensity = 0.65f;
        lightObject.transform.rotation = Quaternion.Euler(50f, -30f, 0f);
    }

    private static Material CreateMaterial(string materialName, Color color)
    {
        Shader shader = Shader.Find("Universal Render Pipeline/Lit");
        if (shader == null)
        {
            Debug.LogWarning($"Cold Start: URP/Lit shader was not found while creating {materialName}.");
            return null;
        }

        Material material = new Material(shader);
        material.name = materialName;
        material.color = color;
        return material;
    }

    private static GameObject CreateCube(
        string objectName,
        Vector3 position,
        Vector3 scale,
        Material material = null
    )
    {
        GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
        cube.name = objectName;
        cube.transform.position = position;
        cube.transform.localScale = scale;

        if (material != null)
        {
            Renderer renderer = cube.GetComponent<Renderer>();
            if (renderer != null)
                renderer.sharedMaterial = material;
        }

        return cube;
    }
}

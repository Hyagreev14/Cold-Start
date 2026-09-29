using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class ColdStartPrototypeBuilder
{
    [MenuItem("Cold Start/Build Player Test Scene")]
    public static void BuildPlayerTestScene()
    {
        Scene scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);

        GameObject ground = GameObject.CreatePrimitive(PrimitiveType.Plane);
        ground.name = "Ground";
        ground.transform.localScale = Vector3.one * 5f;

        GameObject player = new GameObject("Player");
        player.transform.position = new Vector3(0f, 0.05f, 0f);

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
        cameraObject.transform.localPosition = new Vector3(0f, 1.65f, -3.5f);
        cameraObject.transform.localRotation = Quaternion.Euler(12f, 0f, 0f);

        Light lightObject = new GameObject("Sun").AddComponent<Light>();
        lightObject.type = LightType.Directional;
        lightObject.transform.rotation = Quaternion.Euler(50f, -30f, 0f);

        EditorSceneManager.SaveScene(scene, "Assets/Scenes/PlayerTest.unity");
        Selection.activeGameObject = player;

        Debug.Log("Cold Start player test scene created. Press Play and use WASD, Shift, and Space.");
    }
}

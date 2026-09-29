using UnityEngine;

public class PlayerSetup : MonoBehaviour
{
    [SerializeField] private float capsuleHeight = 1.8f;
    [SerializeField] private float capsuleRadius = 0.35f;

    private void Reset()
    {
        CharacterController controller = GetComponent<CharacterController>();
        if (controller == null)
            controller = gameObject.AddComponent<CharacterController>();

        controller.height = capsuleHeight;
        controller.radius = capsuleRadius;
        controller.center = new Vector3(0f, capsuleHeight * 0.5f, 0f);
        controller.stepOffset = 0.3f;
        controller.slopeLimit = 45f;
    }
}

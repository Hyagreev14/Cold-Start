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

        CreateVisibleBody();
    }

    private void CreateVisibleBody()
    {
        Transform existingBody = transform.Find("Player Visual");
        if (existingBody != null)
            return;

        GameObject body = GameObject.CreatePrimitive(PrimitiveType.Capsule);
        body.name = "Player Visual";
        body.transform.SetParent(transform);
        body.transform.localPosition = new Vector3(0f, capsuleHeight * 0.5f, 0f);
        body.transform.localRotation = Quaternion.identity;
        body.transform.localScale = new Vector3(
            capsuleRadius * 2f,
            Mathf.Max(0.01f, (capsuleHeight - capsuleRadius * 2f) * 0.5f),
            capsuleRadius * 2f
        );

        Collider bodyCollider = body.GetComponent<Collider>();
        if (bodyCollider != null)
            DestroyImmediate(bodyCollider);
    }
}

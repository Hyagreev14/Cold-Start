using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float walkSpeed = 4.5f;
    [SerializeField] private float sprintSpeed = 7f;
    [SerializeField] private float acceleration = 12f;
    [SerializeField] private float gravity = -20f;
    [SerializeField] private float jumpHeight = 1.2f;

    [Header("References")]
    [SerializeField] private Transform cameraTransform;

    private CharacterController controller;
    private Vector3 horizontalVelocity;
    private float verticalVelocity;

    private void Awake()
    {
        controller = GetComponent<CharacterController>();

        if (cameraTransform == null && Camera.main != null)
            cameraTransform = Camera.main.transform;
    }

    private void Update()
    {
        Move();
    }

    private void Move()
    {
        float inputX = Input.GetAxisRaw("Horizontal");
        float inputZ = Input.GetAxisRaw("Vertical");

        Vector3 forward = cameraTransform != null ? cameraTransform.forward : transform.forward;
        Vector3 right = cameraTransform != null ? cameraTransform.right : transform.right;

        forward.y = 0f;
        right.y = 0f;
        forward.Normalize();
        right.Normalize();

        Vector3 input = (forward * inputZ + right * inputX).normalized;
        float targetSpeed = Input.GetKey(KeyCode.LeftShift) ? sprintSpeed : walkSpeed;
        Vector3 targetVelocity = input * targetSpeed;

        horizontalVelocity = Vector3.MoveTowards(
            horizontalVelocity,
            targetVelocity,
            acceleration * Time.deltaTime
        );

        if (controller.isGrounded)
        {
            if (verticalVelocity < 0f)
                verticalVelocity = -2f;

            if (Input.GetButtonDown("Jump"))
                verticalVelocity = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }

        verticalVelocity += gravity * Time.deltaTime;

        Vector3 movement = horizontalVelocity;
        movement.y = verticalVelocity;
        controller.Move(movement * Time.deltaTime);

        if (input.sqrMagnitude > 0.01f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(input, Vector3.up);
            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                targetRotation,
                12f * Time.deltaTime
            );
        }
    }
}

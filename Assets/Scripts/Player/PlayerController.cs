using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float walkSpeed = 4.5f;
    [SerializeField] private float sprintSpeed = 7f;
    [SerializeField] private float acceleration = 12f;
    [SerializeField] private float gravity = -20f;
    [SerializeField] private float jumpHeight = 1.2f;

    [Header("Crouch")]
    [SerializeField] private float standingHeight = 1.8f;
    [SerializeField] private float crouchingHeight = 1.1f;
    [SerializeField] private float crouchSpeed = 2.25f;
    [SerializeField] private float crouchTransitionSpeed = 10f;

    [Header("Mouse Look")]
    [SerializeField] private float mouseSensitivity = 0.12f;
    [SerializeField] private float maxLookAngle = 89f;

    [Header("First Person")]
    [SerializeField] private Transform cameraTransform;
    [SerializeField] private float standingCameraHeight = 1.65f;
    [SerializeField] private float crouchingCameraHeight = 1.0f;

    private CharacterController controller;
    private Vector3 horizontalVelocity;
    private float verticalVelocity;
    private float cameraPitch;
    private bool isCrouching;

    private void Awake()
    {
        controller = GetComponent<CharacterController>();

        if (cameraTransform == null && Camera.main != null)
            cameraTransform = Camera.main.transform;

        controller.height = standingHeight;
        controller.center = new Vector3(0f, standingHeight * 0.5f, 0f);

        if (cameraTransform != null)
        {
            cameraTransform.localPosition = new Vector3(0f, standingCameraHeight, 0f);
            cameraPitch = cameraTransform.localEulerAngles.x;
            if (cameraPitch > 180f)
                cameraPitch -= 360f;
        }
    }

    private void OnEnable()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void OnDisable()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    private void Update()
    {
        Look();
        Move();
        HandleCrouch();
    }

    private void Look()
    {
        if (cameraTransform == null || Mouse.current == null)
            return;

        Vector2 mouseDelta = Mouse.current.delta.ReadValue();

        transform.Rotate(Vector3.up * mouseDelta.x * mouseSensitivity);

        cameraPitch = Mathf.Clamp(
            cameraPitch - mouseDelta.y * mouseSensitivity,
            -maxLookAngle,
            maxLookAngle
        );

        cameraTransform.localRotation = Quaternion.Euler(cameraPitch, 0f, 0f);
    }

    private void Move()
    {
        float inputX = 0f;
        float inputZ = 0f;

        if (Keyboard.current != null)
        {
            if (Keyboard.current.aKey.isPressed) inputX -= 1f;
            if (Keyboard.current.dKey.isPressed) inputX += 1f;
            if (Keyboard.current.sKey.isPressed) inputZ -= 1f;
            if (Keyboard.current.wKey.isPressed) inputZ += 1f;
        }

        Vector3 input = new Vector3(inputX, 0f, inputZ);
        if (input.sqrMagnitude > 1f)
            input.Normalize();

        Vector3 inputDirection = transform.right * input.x + transform.forward * input.z;

        bool sprinting = Keyboard.current != null &&
                         Keyboard.current.leftCtrlKey.isPressed &&
                         !isCrouching;

        float targetSpeed = isCrouching
            ? crouchSpeed
            : sprinting ? sprintSpeed : walkSpeed;

        Vector3 targetVelocity = inputDirection * targetSpeed;

        horizontalVelocity = Vector3.MoveTowards(
            horizontalVelocity,
            targetVelocity,
            acceleration * Time.deltaTime
        );

        if (controller.isGrounded)
        {
            if (verticalVelocity < 0f)
                verticalVelocity = -2f;

            if (Keyboard.current != null &&
                Keyboard.current.spaceKey.wasPressedThisFrame &&
                !isCrouching)
            {
                verticalVelocity = Mathf.Sqrt(jumpHeight * -2f * gravity);
            }
        }

        verticalVelocity += gravity * Time.deltaTime;

        Vector3 movement = horizontalVelocity;
        movement.y = verticalVelocity;
        controller.Move(movement * Time.deltaTime);
    }

    private void HandleCrouch()
    {
        if (Keyboard.current == null)
            return;

        if (Keyboard.current.leftShiftKey.wasPressedThisFrame)
        {
            isCrouching = !isCrouching;
        }

        float targetHeight = isCrouching ? crouchingHeight : standingHeight;

        if (!isCrouching && !CanStand())
            return;

        controller.height = Mathf.MoveTowards(
            controller.height,
            targetHeight,
            crouchTransitionSpeed * Time.deltaTime
        );

        controller.center = new Vector3(0f, controller.height * 0.5f, 0f);

        if (cameraTransform != null)
        {
            float targetCameraHeight = isCrouching
                ? crouchingCameraHeight
                : standingCameraHeight;

            Vector3 cameraPosition = cameraTransform.localPosition;
            cameraPosition.y = Mathf.MoveTowards(
                cameraPosition.y,
                targetCameraHeight,
                crouchTransitionSpeed * Time.deltaTime
            );
            cameraPosition.z = 0f;
            cameraTransform.localPosition = cameraPosition;
        }
    }

    private bool CanStand()
    {
        float radius = controller.radius;
        float castDistance = standingHeight - crouchingHeight;

        Vector3 origin = transform.position + Vector3.up * (crouchingHeight - radius);
        return !Physics.SphereCast(
            origin,
            radius * 0.95f,
            Vector3.up,
            out _,
            castDistance
        );
    }
}

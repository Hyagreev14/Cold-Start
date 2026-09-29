using UnityEngine;

namespace ColdStart
{
    public class PlayerController : MonoBehaviour
    {
        [SerializeField] private float moveSpeed = 5f;
        [SerializeField] private float sprintMultiplier = 1.6f;
        private CharacterController controller;

        private void Awake() { controller = GetComponent<CharacterController>(); }
        private void Update()
        {
            Vector3 input = new Vector3(Input.GetAxisRaw("Horizontal"), 0f, Input.GetAxisRaw("Vertical"));
            input = Vector3.ClampMagnitude(input, 1f);
            float speed = Input.GetKey(KeyCode.LeftShift) ? moveSpeed * sprintMultiplier : moveSpeed;
            Vector3 motion = transform.TransformDirection(input) * speed;
            motion.y = -2f;
            controller.Move(motion * Time.deltaTime);
        }
    }
}

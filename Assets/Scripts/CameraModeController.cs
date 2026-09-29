using UnityEngine;

namespace ColdStart
{
    public class CameraModeController : MonoBehaviour
    {
        [SerializeField] private Camera firstPersonCamera;
        [SerializeField] private Camera thirdPersonCamera;
        [SerializeField] private KeyCode switchKey = KeyCode.V;

        private void Start() { SetFirstPerson(true); }
        private void Update() { if (Input.GetKeyDown(switchKey)) SetFirstPerson(!firstPersonCamera.enabled); }
        private void SetFirstPerson(bool firstPerson)
        {
            firstPersonCamera.enabled = firstPerson;
            thirdPersonCamera.enabled = !firstPerson;
        }
    }
}

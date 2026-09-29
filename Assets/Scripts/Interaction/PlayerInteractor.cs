using UnityEngine;

namespace ColdStart.Interaction
{
    public class PlayerInteractor : MonoBehaviour
    {
        [SerializeField] private Camera viewCamera;
        [SerializeField] private float interactionDistance = 3f;

        private void Update()
        {
            if (!Input.GetKeyDown(KeyCode.E)) return;
            Ray ray = new Ray(viewCamera.transform.position, viewCamera.transform.forward);
            if (Physics.Raycast(ray, out RaycastHit hit, interactionDistance))
            {
                Interactable interactable = hit.collider.GetComponentInParent<Interactable>();
                if (interactable != null) Debug.Log("Interacted with: " + interactable.InteractionPrompt);
            }
        }
    }
}

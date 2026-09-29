using UnityEngine;

namespace ColdStart.Interaction
{
    public class Interactable : MonoBehaviour
    {
        [SerializeField] private string interactionPrompt = "Interact";
        public string InteractionPrompt => interactionPrompt;
    }
}

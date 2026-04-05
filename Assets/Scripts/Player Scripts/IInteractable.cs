using UnityEngine;
using UnityEngine.InputSystem;

public interface IInteractable
{
    bool interactable { get; set; } 
    public void OnInteract(InputAction.CallbackContext context);
}

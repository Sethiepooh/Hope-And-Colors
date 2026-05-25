using UnityEngine;
using UnityEngine.InputSystem;

public interface IInteractable
{
    bool interactable { get; set; }
    bool activeInteraction { get; set; }
    Vector2 position { get; }
    public void OnInteract();
}

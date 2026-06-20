using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class InteractionManager : MonoBehaviour
{
    [SerializeField] private float interactionRadius = 2f;
    [SerializeField] GameObject interactionPrompt;
    public Sprite[] interactionSprites; //0 - keyboard, 1 - gamepad

    public PlayerInput PlayerInput;

    int controlScheme = 0; //0 - keyboard, 1 - gamepad
    [HideInInspector] public InputDirectionEnum.InputDirection lastInputDirection;

    [HideInInspector] public IInteractable nearbyInteractable;

    private void Awake()
    {
        PlayerInput = GetComponent<PlayerInput>();
        controlScheme = PlayerInput.currentControlScheme == "Keyboard&Mouse" ? 0 : 1;
    }

    private void Update()
    {
        //Debug.Log(nearbyInteractable);
        if (nearbyInteractable == null)
        {
            FindNearbyInteractables();
        }
        else
        {
            Collider2D[] nearby = Physics2D.OverlapCircleAll(transform.position, interactionRadius);
            if(!CheckForInteractables(nearby))
            {
                nearbyInteractable = null;
            }
        }

        if (nearbyInteractable != null && nearbyInteractable.interactable && !nearbyInteractable.activeInteraction)
        {
            if (interactionPrompt.activeSelf == false)
                SetInteractionPrompt(true);
        }
        else
        {
            if (interactionPrompt.activeSelf == true)
                SetInteractionPrompt(false);
        }
    }

    public void SetInteractionPrompt(bool b)
    {
        interactionPrompt.GetComponent<Image>().sprite = interactionSprites[controlScheme];
        interactionPrompt.SetActive(b);
    }

    public void OnInteract(InputAction.CallbackContext context)
    {
        if (!context.performed) return;

        //Priority: Save points first, then chests, then doors
        if (nearbyInteractable != null && nearbyInteractable.interactable)
        {
            nearbyInteractable.OnInteract();
            return;
        }
    }

    private void FindNearbyInteractables()
    {
        Collider2D[] nearby = Physics2D.OverlapCircleAll(transform.position, interactionRadius);

        IInteractable closestInteractable = null;
        float closestInteractableDistance = Mathf.Infinity;

        foreach (var col in nearby)
        {
            //Find closest CallAndResponse
            IInteractable interactableObj = col.GetComponent<IInteractable>();
            if (interactableObj != null && interactableObj.interactable)
            {
                float distance = Vector2.Distance(transform.position, interactableObj.position);
                if (distance < closestInteractableDistance)
                {
                    closestInteractableDistance = distance;
                    closestInteractable = interactableObj;
                }
            }         
        }

        nearbyInteractable = closestInteractable;
        //Debug.Log(nearbyInteractable);
    }

    public void OnRhythmInput(InputAction.CallbackContext context)
    {
        Vector2 dir = context.ReadValue<Vector2>();

        if (context.performed)
        {
            if (dir == Vector2.up)
            {
                lastInputDirection = InputDirectionEnum.InputDirection.Up;
            }
            else if (dir == Vector2.down)
            {
                lastInputDirection = InputDirectionEnum.InputDirection.Down;
            }
            else if (dir == Vector2.left)
            {
                lastInputDirection = InputDirectionEnum.InputDirection.Left;
            }
            else if (dir == Vector2.right)
            {
                lastInputDirection = InputDirectionEnum.InputDirection.Right;
            }
            else
            {
                lastInputDirection = InputDirectionEnum.InputDirection.None;
            }
        }
        else
        {
            lastInputDirection = InputDirectionEnum.InputDirection.None;
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, interactionRadius);
    }

    bool CheckForInteractables(Collider2D[] nearby)
    {
        foreach (var col in nearby)
        {
            if (col.GetComponent<IInteractable>() != null)
            {
                return true;
            }
        }
        return false;
    }
}

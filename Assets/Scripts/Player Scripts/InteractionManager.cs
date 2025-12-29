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
    bool interacting;
    InputDirection lastInputDirection;

    public CallAndResponse nearbyCallAndResponse;
    public Dialogue nearbyDialogue;



    private void Awake()
    {
        PlayerInput = GetComponent<PlayerInput>();
        controlScheme = PlayerInput.currentControlScheme == "Keyboard&Mouse" ? 0 : 1;
    }

    private void Update()
    {
        if (!interacting)
        {
            FindNearbyInteractables();

            if (nearbyDialogue != null && !nearbyDialogue.active && !nearbyDialogue.disabled && !nearbyDialogue.disableAfterUse || nearbyCallAndResponse != null && !nearbyCallAndResponse.active)
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
        if (nearbyCallAndResponse != null && nearbyCallAndResponse.canInteract)
        {
            nearbyCallAndResponse.InteractWith();
            return;
        }

        if (nearbyDialogue != null)
        {
            nearbyDialogue.InteractWith();
            return;
        }
    }

    private void FindNearbyInteractables()
    {
        Collider2D[] nearby = Physics2D.OverlapCircleAll(transform.position, interactionRadius);

        CallAndResponse closestCallAndResponse = null;
        Dialogue closestDialogue = null;
        float closestCallAndResponseDistance = float.MaxValue;
        float closestDialogueDistance = float.MaxValue;     

        foreach (var col in nearby)
        {
            //Find closest CallAndResponse
            CallAndResponse callAndResponse = col.GetComponent<CallAndResponse>();
            if (callAndResponse != null && callAndResponse.canInteract)
            {
                float distance = Vector2.Distance(transform.position, callAndResponse.transform.position);
                if (distance < closestCallAndResponseDistance)
                {
                    closestCallAndResponseDistance = distance;
                    closestCallAndResponse = callAndResponse;
                }
            }

            //Find closest Dialogue
            Dialogue dialogue = col.GetComponent<Dialogue>();
            if (dialogue != null)
            {
                float distance = Vector2.Distance(transform.position, dialogue.transform.position);
                if (distance < closestDialogueDistance)
                {
                    closestDialogueDistance = distance;
                    closestDialogue = dialogue;
                }
            }           
        }

        nearbyCallAndResponse = closestCallAndResponse;
        nearbyDialogue = closestDialogue;     
    }

    public void OnRhythmInput(InputAction.CallbackContext context)
    {
        Vector2 dir = context.ReadValue<Vector2>();

        if (context.performed)
        {
            if (dir == Vector2.up)
            {
                lastInputDirection = InputDirection.Up;
            }
            else if (dir == Vector2.down)
            {
                lastInputDirection = InputDirection.Down;
            }
            else if (dir == Vector2.left)
            {
                lastInputDirection = InputDirection.Left;
            }
            else if (dir == Vector2.right)
            {
                lastInputDirection = InputDirection.Right;
            }

            //Debug.Log(nearbyCallAndResponse);
            nearbyCallAndResponse.rhythmPatterns[nearbyCallAndResponse.currentPatternIndex].CheckHitBeat(lastInputDirection);
        }
        else
        {
            lastInputDirection = InputDirection.None;
        }

    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, interactionRadius);
    }
}

using UnityEngine;

public class DashEffectHandler : MonoBehaviour
{
    [SerializeField] private Sprite[] dashEffects; // Reference to the dash effect prefab

    private SpriteRenderer spriteRenderer; // Reference to the SpriteRenderer component

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        SetDashEffect(0, false); // Initialize with the first dash effect and set it to inactive
    }

    public void SetDashEffect(int i = 0, bool activeState = true)
    {
        if (activeState)
        {
            spriteRenderer.enabled = true;
        }
        else
        {
            spriteRenderer.enabled = false;
            return;
        }

        if (i >= 0 && i < dashEffects.Length)
        {
            spriteRenderer.sprite = dashEffects[i];
        }
        else
        {
            Debug.LogWarning("Invalid index for dash effect: " + i);
        }
    }

    public void FlipDashEffect(bool flip)
    {
        bool currentFlipState = spriteRenderer.flipX;
        spriteRenderer.flipX = flip;
        if (flip != currentFlipState)
        {
            transform.localPosition = new Vector2(-transform.localPosition.x , transform.localPosition.y);
        }
    }
}

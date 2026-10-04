using UnityEngine;

public class SpriteSwitch : MonoBehaviour
{
    SpriteRenderer sRend;
    Sprite defaultSprite;
    [SerializeField] Sprite switchSprite;


    private void Awake()
    {
        sRend = GetComponent<SpriteRenderer>();
        defaultSprite = sRend.sprite;
    }

    public void SwitchSprite(bool b)
    {
        if (b)
        {
            sRend.sprite = switchSprite;
        }
        else
        {
            sRend.sprite = defaultSprite;
        }
    }
}

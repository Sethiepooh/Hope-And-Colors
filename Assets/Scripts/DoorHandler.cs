using System.Collections;
using UnityEngine;

public class DoorHandler : MonoBehaviour
{
    Animator openAnim;
    [SerializeField] AnimationClip openAnimClip;
    //[SerializeField] Sprite closedSprite;
    //[SerializeField] Sprite openSprite;
    Collider2D col;
    SpriteRenderer sRend;
    bool opened;

    private void Awake()
    {
        sRend = GetComponent<SpriteRenderer>();
        col = GetComponent<Collider2D>();
        openAnim = GetComponent<Animator>();   
    }

    public void ToggleDoor(bool state)
    {
        if (!state)
        {
           // Debug.Log("Door Anim Length: " + openAnimClip.length);
            openAnim.SetBool("Open", true);
            if(openAnimClip != null )
                StartCoroutine(WaitForAnimation(openAnimClip.length));
            else
                StartCoroutine(WaitForAnimation(0.5f));
        }
        else
        {
            openAnim.SetBool("Open", false);
            col.enabled = true;
            //sRend.sprite = closedSprite;
        }
        
    }

    IEnumerator WaitForAnimation(float animTime)
    {
        if (openAnim != null)
            yield return new WaitForSeconds(animTime);
        col.enabled = false;
        //sRend.sprite = openSprite;

    }
}

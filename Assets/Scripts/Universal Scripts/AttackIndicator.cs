using UnityEngine;

public class AttackIndicator : MonoBehaviour
{
    SpriteRenderer sRend;
    Color activeColor;
    float alpha = 0;
    public float fadeStep;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        sRend = GetComponent<SpriteRenderer>();
        activeColor = sRend.color;
        sRend.color = new Color(activeColor.r, activeColor.g, activeColor.b, alpha);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if(alpha > 0)
        {
            alpha -= fadeStep;
            if(alpha < 0)
            {
                alpha = 0;
            }
        }

        sRend.color = new Color(activeColor.r,activeColor.g,activeColor.b, alpha);
    }

    public void AttackFlash()
    {
        alpha = 225;
    }
}

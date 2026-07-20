using UnityEngine;
using UnityEngine.UI;
public class ChangeOnBeat : MonoBehaviour
{
    public Color colorOne;
    //public Color colorTwo = Color.red;
    float alpha = 0;
    public float minAlpha = 0;
    public float fadeStep;
    SpriteRenderer sr;
    Image image;
    PulseManager pulseManager;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if(GetComponent<SpriteRenderer>() != null)
        {
            sr = GetComponent<SpriteRenderer>();
            sr.color = colorOne;    
        }
        else
            image = GetComponent<Image>();
        pulseManager = GameObject.FindWithTag("RhythmManager").GetComponent<PulseManager>();
        pulseManager.AddEntity(this.gameObject, pulseManager.entitiesToFlash);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (alpha > minAlpha)
        {
           
            alpha -= fadeStep;
            if (alpha < minAlpha)
            {
                alpha = minAlpha;
            }
        }

        if(sr != null)
            sr.color = new Color(colorOne.r, colorOne.g, colorOne.b, alpha);
        else
            image.color = new Color(image.color.r, image.color.g, image.color.b, alpha);
    }

    public void ChangeColor()
    {
        if(sr != null)
        {
            alpha = 225;
        }
        else
        {
            alpha = 1;
        }
            //Debug.Log("reset alpha");
    }
}

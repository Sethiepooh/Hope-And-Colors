using UnityEngine;

public class ChangeOnBeat : MonoBehaviour
{
    public Color colorOne = Color.blue;
    //public Color colorTwo = Color.red;
    float alpha = 0;
    public float minAlpha = 0;
    public float fadeStep;
    SpriteRenderer sr;
    PulseManager pulseManager;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        sr.color = colorOne;
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

        sr.color = new Color(colorOne.r, colorOne.g, colorOne.b, alpha);
    }

    public void ChangeColor()
    {
        alpha = 225;
    }
}

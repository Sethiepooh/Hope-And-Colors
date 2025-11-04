using UnityEngine;

public class ChangeOnBeat : MonoBehaviour
{
    public Color colorOne = Color.blue;
    public Color colorTwo = Color.red;
    bool toggle = false;
    SpriteRenderer sr;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        sr.color = colorOne;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if(sr.color != colorOne)
        {
            //sr.color = Color.Lerp(sr.color, colorOne, 0.1f);
        }
    }

    public void ChangeColor()
    {
        sr.material.color = colorTwo;
    }
}

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
        sr.material.color = colorOne;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ChangeColor()
    {
        if (toggle)
        {
            sr.material.color = colorOne;
        }
        else
        {
            sr.material.color = colorTwo;
        }
        toggle = !toggle;
    }
}

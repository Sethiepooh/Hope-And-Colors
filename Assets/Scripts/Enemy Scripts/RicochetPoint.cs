using UnityEngine;

public class RicochetPoint : MonoBehaviour
{

    bool active = false;
    Color defaultColor;
    SpriteRenderer sRend;
    Color activeColor = Color.yellow;   

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        sRend = GetComponent<SpriteRenderer>();
        defaultColor = sRend.color;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ActivatePoint()
    {
        active = true;
        sRend.color = activeColor;
    }

    public void DeactivatePoint()
    {
        active = false;
        sRend.color = defaultColor;
    }

    public Vector2 GetPosition()
    {
        return transform.position;
    }
}

using UnityEngine;

public class BobbingMovement : MonoBehaviour
{
    Vector3 defaultPos;
    [SerializeField] float bobbingSpeed = .5f;
    [SerializeField] float bobbingHeight = 2f;

    private void Awake()
    {
        defaultPos = transform.position;
    }
    // Update is called once per frame
    void Update()
    {
        if(transform.position.y < defaultPos.y + bobbingHeight)
        {
            transform.position = new Vector2(transform.position.x, defaultPos.y + Mathf.Sin(Time.time * bobbingSpeed) * bobbingHeight);
        }
        else
        {
            transform.position = new Vector2(transform.position.x, defaultPos.y - Mathf.Sin(Time.time * bobbingSpeed) * bobbingHeight);
        }
    }
}

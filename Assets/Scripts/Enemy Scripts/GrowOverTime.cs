using UnityEngine;

public class GrowOverTime : MonoBehaviour
{
    public float growRate = 2f;

    public bool growOnX = true;
    Vector3 defaultSize;

    private void Awake()
    {
        defaultSize = transform.localScale;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (!gameObject.activeSelf) return;

        if (growOnX)
        {
            transform.localScale += new Vector3(growRate * Time.fixedDeltaTime, 0, 0);
        }
        else
        {
            transform.localScale += new Vector3(0, growRate * Time.fixedDeltaTime, 0);
        }
    }

    private void OnDisable()
    {
        if (!gameObject.activeSelf)
        {
            transform.localScale = defaultSize;
        }
    }
}

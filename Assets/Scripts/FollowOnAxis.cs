using UnityEngine;

public class FollowOnAxis : MonoBehaviour
{
    public bool active;
    public bool followX = true;
    [SerializeField] private Transform target;
    [SerializeField] float followOffset = 10f;

    public void ToggleActivation(bool state)
    {
        active = state;
    }

    // Update is called once per frame
    void Update()
    {
        if (!active) return;

        if (followX)
        {
            transform.position = new Vector3(target.position.x + followOffset, transform.position.y, transform.position.z);
        }
        else
        {
            transform.position = new Vector3(transform.position.x, target.position.y + followOffset, transform.position.z);
        }
    }
}

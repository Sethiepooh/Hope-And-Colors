using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    [SerializeField] Transform startPoint;
    [SerializeField] Transform endPoint;
    [SerializeField] float speed = 2f;

    private void Update()
    {
        float step = speed * Time.deltaTime;
        transform.position = Vector3.MoveTowards(transform.position, endPoint.position, step);
        if (Vector3.Distance(transform.position, endPoint.position) < 0.001f)
        {
            Transform temp = startPoint;
            startPoint = endPoint;
            endPoint = temp;
        }
    }
}

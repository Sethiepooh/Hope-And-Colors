using System.Collections;
using UnityEngine;

public class RollingCrystals : MonoBehaviour
{
    Vector2 slideTarget;
    [SerializeField] float slideSpeed;
    [SerializeField] float slideDistance;


    public RollingCrystals Initialize()
    {
        GetSlideTarget();
        return this;    
    }

    void GetSlideTarget()
    {
        slideTarget = (Vector2)transform.position + (Vector2)(transform.forward * slideDistance);
    }

    public void StartSliding()
    {
        StartCoroutine(SlideToTarget());
    }

    IEnumerator SlideToTarget()
    {
        while(Vector2.Distance(transform.position, slideTarget) > .1f)
        {
            transform.position = Vector2.MoveTowards(transform.position, slideTarget, slideSpeed * Time.deltaTime);
            yield return null;
        }

        Destroy(this.gameObject);
    }
}

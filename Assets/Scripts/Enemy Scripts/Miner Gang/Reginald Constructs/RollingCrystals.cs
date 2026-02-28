using System.Collections;
using UnityEngine;

public class RollingCrystals : MonoBehaviour
{
    Vector2 slideTarget;
    Vector2 originalPos;
    [SerializeField] float slideSpeed;
    [SerializeField] float slideDistance;


    private void Start()
    {
        originalPos = transform.position;
        slideTarget = (Vector2)transform.position + (Vector2)(transform.forward * slideDistance);
        this.gameObject.SetActive(false);
    }

    public RollingCrystals Initialize()
    {
        transform.position = originalPos;
        return this; 
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

        gameObject.SetActive(false);
    }
}

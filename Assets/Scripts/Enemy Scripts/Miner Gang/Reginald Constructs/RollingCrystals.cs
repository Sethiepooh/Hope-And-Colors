using System.Collections;
using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;

public class RollingCrystals : MonoBehaviour
{
    Vector2 slideTarget;
    Vector2 originalPos;
    [SerializeField] float slideSpeed;
    [SerializeField] float slideDistance;
    List<GameObject> crystals = new List<GameObject>();
    public bool sliding = false;
    public bool initialized;


    private void Start()
    {
        originalPos = transform.position;
        slideTarget = (Vector2)transform.position + (Vector2)(transform.up * slideDistance);

        List<Transform> children = new List<Transform>();
        foreach (Transform child in transform)
        {
            children.Add(child);
        }

        foreach (Transform child in children)
        {
            crystals.Add(child.gameObject);
        }

        ToggleCrystals(false);
    }

    public RollingCrystals Initialize()
    {
        ToggleCrystals(true);
        transform.position = originalPos;
        initialized = true;
        return this; 
    }

    public void StartSliding()
    {
        StartCoroutine(SlideToTarget());
    }

    IEnumerator SlideToTarget()
    {
        sliding = true;

        while (Vector2.Distance(transform.position, slideTarget) > .1f)
        {
            transform.position = Vector2.MoveTowards(transform.position, slideTarget, slideSpeed * Time.deltaTime);
            yield return null;
        }

        ToggleCrystals(false);
        sliding = false;
        initialized = false;
    }

    void ToggleCrystals(bool b)
    {
        foreach (GameObject crystal in crystals)
        {
            crystal.SetActive(b);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawLine(transform.position, (Vector2)transform.position + (Vector2)(transform.up * slideDistance));
    }
}

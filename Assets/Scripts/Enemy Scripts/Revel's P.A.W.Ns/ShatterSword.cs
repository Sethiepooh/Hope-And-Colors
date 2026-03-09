using System.Collections;
using UnityEngine;

public class ShatterSword : ScatterShot
{
    [Header("Shatter Sword Settings")]
    [SerializeField] float detectionRadius;
    [SerializeField] LayerMask playerLayer;
    bool isShattering = false;

    [SerializeField] float timeToShatter;
    float shatterTimer = 0f;

    // Update is called once per frame
    void Update()
    {
        shatterTimer += Time.deltaTime;
        if(shatterTimer >= timeToShatter && !isShattering)
        {
            StartCoroutine(ShatterCoroutine());
        }

        Collider2D[] playerDetect = Physics2D.OverlapCircleAll(transform.position, detectionRadius, playerLayer);
        if(playerDetect.Length > 0 && !isShattering)
        {
            StartCoroutine(ShatterCoroutine());
        }
    }

    IEnumerator ShatterCoroutine()
    {
        isShattering = true;
        yield return new WaitForSeconds(0.5f);
        FireScatterShot();
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }
}

using UnityEngine;
using System.Collections;

public class Crate : BreakableObjectBase
{
    public override void OnDeath()
    {
        StartCoroutine(DestroyObstacle());
    }

    IEnumerator DestroyObstacle()
    {
        sRend.enabled = false;
        var col = gameObject.GetComponent<Collider2D>();
        col.enabled = false;
        var rb = gameObject.GetComponent<Rigidbody2D>();
        // rb.linearVelocity = Vector3.zero;
        if (deathParticles != null)
            deathParticles.Play();

        yield return new WaitForSeconds(.5f);
        this.gameObject.SetActive(false);
    }
}

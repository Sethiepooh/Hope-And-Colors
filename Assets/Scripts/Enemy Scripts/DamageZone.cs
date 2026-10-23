using System.Collections;
using UnityEngine;

public class DamageZone : MonoBehaviour
{
    [Header("Damage Settings")]
    [SerializeField] int damage;
    [SerializeField] LayerMask damageLayer;
    [SerializeField] bool canDamage = true;

    [Header("Crumble Settings")]
    [SerializeField] bool crumbleOnContact;
    [SerializeField] float crumbleDelay;
    [SerializeField] Color crumbleColor;

    [Header("Decay Settings")]
    [SerializeField] float decayTime;
    [SerializeField] bool decaying;
    float decayTimer;

    private void Update()
    {
        if (decaying)
        {
            decayTimer += Time.deltaTime;
            if (decayTimer >= decayTime)
            {
                Destroy(gameObject);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        canDamage = true;
        if (((1 << collision.gameObject.layer) & damageLayer) != 0)
        {
            if(crumbleOnContact)
            {
                StartCoroutine(CrumbleCoroutine(collision.gameObject));
            }
            else
            {
                StartCoroutine(DamageCoroutine(collision.gameObject));
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (((1 << collision.gameObject.layer) & damageLayer) != 0)
        {
            StopCoroutine(DamageCoroutine(collision.gameObject));
        }
        canDamage = false;
    }

    IEnumerator DamageCoroutine(GameObject target)
    {
        while (canDamage)
        {
            target.GetComponent<Health>().TakeDamage(damage);
            yield return new WaitForSeconds(1f);
        }
    }

    IEnumerator CrumbleCoroutine(GameObject target)
    {
        yield return new WaitForSeconds(crumbleDelay);
        crumbleOnContact = false;
        this.gameObject.GetComponent<SpriteRenderer>().color = crumbleColor;
        StartCoroutine(DamageCoroutine(target));
    }
}

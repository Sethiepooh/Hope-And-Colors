using System.Collections;
using UnityEngine;

public class HuntingDaggers : MonoBehaviour
{
    [SerializeField] float moveSpeed = 5.0f;
    [SerializeField] float dashTime;
    int expirationCounter = 0;
    Rigidbody2D rb;
    [SerializeField] Transform playerTransform;
    bool attacking = false;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        DeactivateDagger();
    }

    public void Initialize()
    {
        expirationCounter = 0;
    }

    private void Update()
    {
        if (playerTransform != null && !attacking)
        {
            Vector2 direction = ((Vector2)playerTransform.position - (Vector2)transform.position).normalized;
            transform.rotation = Quaternion.LookRotation(Vector3.forward, direction);
        }
    }

    public void DashTowardsPlayer()
    {
        if(expirationCounter >= 4 )
            DeactivateDagger();

        StopAllCoroutines();
        StartCoroutine(AttackCoroutine(playerTransform.position));
    }

    IEnumerator AttackCoroutine(Vector2 playerPos)
    {
        attacking = true;
        float elapsedTime = 0f;
        Vector2 direction = (playerPos - (Vector2)transform.position).normalized;
        transform.rotation = Quaternion.LookRotation(Vector3.forward, direction);
        while (elapsedTime < dashTime)
        {
            rb.linearVelocity = direction * moveSpeed;
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        rb.linearVelocity = Vector2.zero;
        attacking = false;
    }

    public void DeactivateDagger()
    {
        gameObject.SetActive(false);
    }
}

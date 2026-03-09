using System.Collections;
using UnityEngine;

public class FragileCrystal : ScatterShot
{
    Transform wanderCenterPoint;
    Vector2 currentTarget;
    Rigidbody2D rb;
    [SerializeField] float wanderRadius;
    [SerializeField] float speed;
    [SerializeField] float timeToLive;
    float timer;

    [SerializeField] float scatterDelay;
    float delayTimer;

    [SerializeField] float triggerRadius;
    [SerializeField] LayerMask playerLayer;

    public void Initialize(Transform point, ProjectilePool pool)
    {
        wanderCenterPoint = point;
        projectilePool = pool;
        rb = GetComponent<Rigidbody2D>();
        ChooseNewWanderPoint();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (rb == null) return;

        rb.linearVelocity = (currentTarget - (Vector2)transform.position).normalized * speed;


        if (Vector2.Distance(transform.position, currentTarget) < .5)
        {
            ChooseNewWanderPoint();
        }

        timer += Time.fixedDeltaTime;
        if (timer >= timeToLive)
        {
            FireScatterShot();
        }

        Collider2D[] hitObjects = Physics2D.OverlapCircleAll(transform.position, triggerRadius, playerLayer);
        foreach (Collider2D objects in hitObjects)
        {
            if (objects.gameObject.CompareTag("Player"))
            {
                FireScatterShot();
            }
        }
    }

    void ChooseNewWanderPoint()
    {
        currentTarget = (Vector2)wanderCenterPoint.position + UnityEngine.Random.insideUnitCircle * wanderRadius;
        Debug.Log("New Wander Point: " + currentTarget);
    }

    protected override void FireScatterShot()
    {
        StartCoroutine(ScatterShotCoroutine());
    }

    public IEnumerator ScatterShotCoroutine()
    {
        // Add a short delay before firing
        yield return new WaitForSeconds(0.2f);

        float angleStep = 360f / 8;
        float angle = 0f;
        for (int i = 0; i < 8; i++)
        {
            float projectileDirXPosition = transform.position.x + Mathf.Sin((angle * Mathf.PI) / 180);
            float projectileDirYPosition = transform.position.y + Mathf.Cos((angle * Mathf.PI) / 180);
            Vector3 projectileVector = new Vector3(projectileDirXPosition, projectileDirYPosition, 0);
            Vector3 projectileMoveDirection = (projectileVector - transform.position).normalized;
            Projectile projectileInstance = projectilePool.GetProjectile(
                transform.position,
                Quaternion.LookRotation(Vector3.forward, projectileVector)
            );
            projectileInstance.Initialize(projectilePool, false, projectileMoveDirection);
            angle += angleStep;
        }

        Destroy(this.gameObject);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, triggerRadius);
    }
}

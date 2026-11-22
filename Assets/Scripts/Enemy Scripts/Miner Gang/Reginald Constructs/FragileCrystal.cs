using System.Collections;
using UnityEngine;

public class FragileCrystal : BreakableObjectBase
{
    Transform wanderCenterPoint;
    Vector2 currentTarget;
    Rigidbody2D rb;
    [SerializeField] float wanderRadius;
    [SerializeField] float speed;
    [SerializeField] float timeToLive;
    float timer;

    [SerializeField] float scatterDelay;
    [SerializeField] int projectileNum;
    [SerializeField] GameObject projectile;
    float delayTimer;

    [SerializeField] float triggerRadius;
    [SerializeField] LayerMask playerLayer;

    public void Initialize(Transform point, ProjectilePool pool)
    {
        wanderCenterPoint = point;
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

    void FireScatterShot()
    {
        StartCoroutine(ScatterShotCoroutine());
    }

    public IEnumerator ScatterShotCoroutine()
    {
        // Add a short delay before firing
        yield return new WaitForSeconds(0.2f);

        Debug.Log("ScatterShot");
        float angleStep = 360f / (float)projectileNum;
        float angle = 0f;
        for (int i = 0; i < projectileNum; i++)
        {
            float projectileDirXPosition = transform.position.x + Mathf.Sin((angle * Mathf.PI) / 180);
            float projectileDirYPosition = transform.position.y + Mathf.Cos((angle * Mathf.PI) / 180);
            Vector3 projectileVector = new Vector3(projectileDirXPosition, projectileDirYPosition, 0);
            Vector3 projectileMoveDirection = (projectileVector - transform.position).normalized;
            GameObject tmpObj = Instantiate(projectile, transform.position, Quaternion.identity);
            tmpObj.GetComponent<Projectile>().direction = projectileMoveDirection;
            Debug.Log("ScatterShot");
            angle += angleStep;
        }

        OnDeath();
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, triggerRadius);
    }

    public override void OnDeath()
    {
        ManagerDeathEvent.Invoke();
        gameObject.SetActive(false);
    }
}

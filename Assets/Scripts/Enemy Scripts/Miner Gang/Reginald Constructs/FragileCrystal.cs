using UnityEngine;

public class FragileCrystal : MonoBehaviour
{
    Transform wanderCenterPoint;
    Vector2 currentTarget;
    Rigidbody2D rb;
    [SerializeField] float wanderRadius;
    [SerializeField] float speed;
    [SerializeField] ProjectilePool projectilePool;
    [SerializeField] float timeToLive;
    float timer;

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
            ScatterShot();
        }
    }

    void ChooseNewWanderPoint()
    {
        currentTarget = (Vector2)wanderCenterPoint.position + UnityEngine.Random.insideUnitCircle * wanderRadius;
        Debug.Log("New Wander Point: " + currentTarget);
    }

    public void ScatterShot()
    {
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
}

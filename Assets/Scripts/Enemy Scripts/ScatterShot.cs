using UnityEngine;

public abstract class ScatterShot : MonoBehaviour
{
    [Header("Scatter Shot Settings")]
    [SerializeField] protected int scatterCount = 8;
    [SerializeField] protected float scatterAngle = 360f;
    [SerializeField] protected ProjectilePool projectilePool;

    /// <summary>
    /// Fires projectiles in a scatter pattern from the current position.
    /// </summary>
    protected virtual void FireScatterShot()
    {
        float angleStep = scatterAngle / scatterCount;
        float angle = 0f;
        for (int i = 0; i < scatterCount; i++)
        {
            float projectileDirX = transform.position.x + Mathf.Sin((angle * Mathf.PI) / 180);
            float projectileDirY = transform.position.y + Mathf.Cos((angle * Mathf.PI) / 180);
            Vector3 projectileVector = new Vector3(projectileDirX, projectileDirY, 0);
            Vector3 projectileMoveDirection = (projectileVector - transform.position).normalized;

            Projectile projectileInstance = projectilePool.GetProjectile(
                transform.position,
                Quaternion.LookRotation(Vector3.forward, projectileVector)
            );
            projectileInstance.Initialize(projectilePool, false, projectileMoveDirection);

            angle += angleStep;
        }
    }
}

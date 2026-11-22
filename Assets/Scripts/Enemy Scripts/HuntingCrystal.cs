using UnityEngine;

public class HuntingCrystal : MonoBehaviour
{
    [SerializeField] float crystalSpread = 1f;
    [SerializeField] int projectileCount = 5;
    [SerializeField] GameObject player;
    [SerializeField] SeekerProjectile seekerProjectilePrefab;
    [SerializeField] float fireRange = 10f;
    [SerializeField] PlayerMovement playerMovement;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        playerMovement = player.GetComponent<PlayerMovement>();
        playerMovement.dashAction +=  FireScatterShot;
    }

    public void FireScatterShot()
    {
        float angleStep = crystalSpread / projectileCount;
        float angle = 0f;
        for (int i = 0; i < projectileCount; i++)
        {
            float projectileDirX = transform.position.x + Mathf.Sin((angle * Mathf.PI) / 180);
            float projectileDirY = transform.position.y + Mathf.Cos((angle * Mathf.PI) / 180);
            Vector3 projectileVector = new Vector3(projectileDirX, projectileDirY, 0);
            Vector3 projectileMoveDirection = (projectileVector - transform.position).normalized;

            FireProjectile(projectileMoveDirection);

            angle += angleStep;
        }
    }

    public void FireProjectile(Vector2 dir)
    {
        SeekerProjectile projectile = Instantiate(seekerProjectilePrefab, transform.position, Quaternion.identity);
        projectile.Initialize(player, dir.normalized);
    }
}

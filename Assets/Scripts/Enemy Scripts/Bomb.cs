using UnityEngine;

public class Bomb : MonoBehaviour
{
    [SerializeField] GameObject projectile;
    [SerializeField] int projectileNum;

    public void ScatterShot()
    {
        
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
    }
}

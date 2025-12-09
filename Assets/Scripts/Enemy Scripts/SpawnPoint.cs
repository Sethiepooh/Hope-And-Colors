using System.Collections;
using UnityEngine;

public class SpawnPoint : MonoBehaviour
{
    [SerializeField] ParticleSystem spawnEffect;
    EnemyManager enemyManager;
    GameObject currentEnemy;

    private void Start()
    {
        enemyManager = GameObject.FindWithTag("EnemyManager").GetComponent<EnemyManager>();
    }
    public IEnumerator SpawnEnemy(GameObject enemy)
    {
        if(currentEnemy != null)
        {
            yield break;
        }

        yield return new WaitForSeconds(4);
        GameObject spawnedEnemy =  Instantiate(enemy, transform.position, Quaternion.identity);
        currentEnemy = spawnedEnemy;
        if(spawnedEnemy.GetComponent<EnemyBase>() != null)
        {
            spawnedEnemy.GetComponent<EnemyBase>().active = true;
            enemyManager.spawnedEnemies.Add(spawnedEnemy);
        }
    }

    public void DestroyEnemy()
    {
        if (currentEnemy != null)
        {
            enemyManager.spawnedEnemies.Remove(currentEnemy);
            currentEnemy.GetComponent<Health>().TakeDamage(1000);
            Destroy(currentEnemy);
            currentEnemy = null;
        }    
    }

    public bool HasEnemy()
    {
        return currentEnemy != null;
    }

    public void PlayEffect()
    {
        spawnEffect.Play();
    }
}

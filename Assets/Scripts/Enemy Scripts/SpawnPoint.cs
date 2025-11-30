using System.Collections;
using UnityEngine;

public class SpawnPoint : MonoBehaviour
{
    [SerializeField] ParticleSystem spawnEffect;
    GameObject currentEnemy;

    public IEnumerator SpawnEnemy(GameObject enemy)
    {
        if(currentEnemy != null)
        {
            yield break;
        }

        yield return new WaitForSeconds(4);
        GameObject spawnedEnemy =  Instantiate(enemy, transform.position, Quaternion.identity);
        currentEnemy = spawnedEnemy;
        spawnedEnemy.GetComponent<EnemyBase>().active = true;
    }

    public void DestroyEnemy()
    {
        if (currentEnemy != null)
        {
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

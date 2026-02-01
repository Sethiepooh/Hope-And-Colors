using System.Collections;
using UnityEngine;
using static EnemyManager;

public class SpawnPoint : MonoBehaviour
{
    [SerializeField] ParticleSystem spawnEffect;
    EnemyManager enemyManager;
    public GameObject currentEnemy;
    public bool hasEnemy;

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

        hasEnemy = true;

        yield return new WaitForSeconds(4);
        GameObject spawnedEnemy =  Instantiate(enemy, transform.position, Quaternion.identity);
        currentEnemy = spawnedEnemy;
        if(spawnedEnemy.GetComponent<EnemyBase>() != null)
        {
            spawnedEnemy.GetComponent<EnemyBase>().active = true;
            enemyManager.spawnedEnemies.Add(spawnedEnemy);
        }
    }

    public IEnumerator SpawnAlixShaman(GameObject shaman)
    {
        if (currentEnemy != null)
        {
            yield break;
        }

        hasEnemy = true;

        yield return new WaitForSeconds(4);
        GameObject spawnedEnemy = Instantiate(shaman, transform.position, Quaternion.identity);
        currentEnemy = spawnedEnemy;
        if (spawnedEnemy.GetComponent<EnemyBase>() != null)
        {
            spawnedEnemy.GetComponent<EnemyBase>().active = true;
            enemyManager.spawnedEnemies.Add(spawnedEnemy);
        }

        GlitchShaman shamanScript =  spawnedEnemy.GetComponent<GlitchShaman>();
        shamanScript.SetProtectionTarget(GameObject.FindWithTag("Boss"));
    }

    public void DestroyEnemy()
    {
        if (currentEnemy != null)
        {
            hasEnemy = false;
            enemyManager.spawnedEnemies.Remove(currentEnemy);
            if(currentEnemy.activeSelf)
                currentEnemy.GetComponent<Health>().HandleEnemyDeath();
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

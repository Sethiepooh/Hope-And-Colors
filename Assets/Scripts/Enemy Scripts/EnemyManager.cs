
using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class EnemyManager : MonoBehaviour
{
    //public List<GameObject> enemies = new List<GameObject>();
    public EnemyGroup[] enemyGroups;
    public List<GameObject> spawnedEnemies = new List<GameObject>();
    RespawnManager respawnManager;
    BPMInteract bpmInteract;
    [HideInInspector]public bool angelBreak = false;
    bool doubleTimeActive = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        SetEnemies();
        bpmInteract = GameObject.FindWithTag("RhythmManager").GetComponent<BPMInteract>();
        respawnManager = GameObject.FindWithTag("RespawnManager").GetComponent<RespawnManager>();
    }

    // Update is called once per frame
    void Update()
    {
        if(CheckGroupDefeated(respawnManager.spawnIndex))
        {
            if (enemyGroups[respawnManager.spawnIndex].door != null)
            {
                enemyGroups[respawnManager.spawnIndex].door.SetActive(false);
            } 
        }
    }

    void SetEnemies()
    {
        foreach (EnemyGroup enemyGroup in enemyGroups)
        {
            for (int i = 0; i < enemyGroup.enemies.Length; i++)
            {
                enemyGroup.enemies[i] = NewEnenemy(enemyGroup.enemies[i]);
            }
        }
    }

    Enemy NewEnenemy(Enemy e)
    {
        return new Enemy(e.enemyObject, e.enemyObject.transform.position);
    }

    public void RespawnEnemies(int i)
    {
        for (int j = i; j < enemyGroups.Length; j++)
        {
            for(int x = 0; x < enemyGroups[j].enemies.Length; x++)
            {
                enemyGroups[j].enemies[x].enemyObject.GetComponent<Health>()?.Heal(1000);
                enemyGroups[j].enemies[x].enemyObject.GetComponent<SpriteRenderer>().enabled = true;
                enemyGroups[j].enemies[x].enemyObject.GetComponent<Collider2D>().enabled = true;

                if(j == i)
                {
                    if (enemyGroups[j].enemies[x].enemyObject.CompareTag("Enemy"))
                    {
                        enemyGroups[j].enemies[x].enemyObject.GetComponent<EnemyBase>().active = true;
                    }
                }
                    

                enemyGroups[j].enemies[x].enemyObject.SetActive(true);
                enemyGroups[j].enemies[x].enemyObject.transform.position = enemyGroups[j].enemies[x].startingPos;
            }
        }
    }

    public void ActivateGroup(int i)
    {
        if (enemyGroups.Length == 0 || i >= enemyGroups.Length)
        {
            return;
        }

        for (int x = 0; x < enemyGroups[i].enemies.Length; x++)
        {
            if (enemyGroups[i].enemies[x].enemyObject.CompareTag("Enemy"))
            {
                enemyGroups[i].enemies[x].enemyObject.GetComponent<EnemyBase>().active = true;
            }
        }
    }

    public bool CheckGroupDefeated(int i)
    {
        if(enemyGroups.Length == 0 || i >= enemyGroups.Length)
        {
            return false;
        }

        foreach (Enemy e in enemyGroups[i].enemies)
        {
            if (!e.enemyObject.CompareTag("Enemy"))
                continue;

            if (e.enemyObject.activeInHierarchy)
                return false;
        }
        return true;
    }

    public void AddBeatToNew()
    {
        if (angelBreak || doubleTimeActive)
            return;

        for(int i = 0; i < spawnedEnemies.Count; i++)
        {
            if (spawnedEnemies[i] != null && spawnedEnemies[i].activeInHierarchy)
                spawnedEnemies[i].GetComponent<EnemyBase>().AddToBeatCount();
        }
    }

    public void DoubleTimeAddBeatToNew()
    {
        if (angelBreak || !doubleTimeActive)
            return;

        for (int i = 0; i < spawnedEnemies.Count; i++)
        {
            if (spawnedEnemies[i] != null && spawnedEnemies[i].activeInHierarchy)
                spawnedEnemies[i].GetComponent<EnemyBase>().AddToBeatCount();
        }
    }


    public void AddBeatToAll()
    {
        if (angelBreak || doubleTimeActive)
            return;

        foreach (EnemyGroup enemy in enemyGroups)
        {
            foreach (Enemy e in enemy.enemies)
            {
                if (e.enemyObject != null)
                {
                    if(e.enemyObject.activeInHierarchy)
                        e.enemyObject.GetComponent<EnemyBase>()?.AddToBeatCount();

                    if(e.enemyObject.transform.GetChild(0).gameObject.activeInHierarchy)
                        e.enemyObject.transform.GetChild(0).gameObject.GetComponent<EnemyBase>()?.AddToBeatCount();
                }                  
            }              
        }
    }

    public void DoubleTime()
    {
        if(angelBreak || !doubleTimeActive)
            return;

        foreach (EnemyGroup enemy in enemyGroups)
        {
            foreach (Enemy e in enemy.enemies)
            {
                if (e.enemyObject != null)
                {
                    if (e.enemyObject.activeInHierarchy)
                        e.enemyObject.GetComponent<EnemyBase>().AddToBeatCount();
                }
            }
        }
    }

    public void TriggerDoubleTime(float sec)
    {
        StartCoroutine(DoubleTimeForSeconds(sec));
    }

    IEnumerator DoubleTimeForSeconds(float sec)
    {
        if (doubleTimeActive)
            yield return null;

        doubleTimeActive = true;
        yield return new WaitForSeconds(sec);
        doubleTimeActive = false;
    }


    [System.Serializable]
    public struct EnemyGroup
    {
        public Enemy[] enemies;
        public GameObject door;
    }

    [System.Serializable]
    public struct Enemy
    {
        public GameObject enemyObject;
        public Vector2 startingPos;

        public Enemy( GameObject obj, Vector2 pos)
        {
            enemyObject = obj;
            startingPos = pos;
        }
    }
}

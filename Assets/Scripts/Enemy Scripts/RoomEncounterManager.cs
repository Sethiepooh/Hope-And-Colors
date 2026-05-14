using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;
using static RoomEncounterManager;
using System.Collections;
using System;

public class RoomEncounterManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] ProjectilePool projectilePool;
    [SerializeField] BPMInteract bpmInteract;
    [SerializeField] EnemyFactory enemyFactory;
    [SerializeField] GameObject player;
    [SerializeField] PulseManager pulseManager;

    [Header("Spawnable Items")]
    [SerializeField] EnemyBase[] enemyPrefabs;
    [SerializeField] BreakableObjectBase[] breakableObjectPrefabs;

    [Header("Encounter Settings")]
    [SerializeField] List<SpawnableGroup> spawnableGroups = new List<SpawnableGroup>();

    bool angelBreakActive = false;
    bool doubleTimeActive = false;
    public enum EnemyType
    {
        GlitchChild, GlitchMother, GlitchFather, GlitchShaman, 
        Miner, Bruiser, Driller,
        Enforcer, Surgeon, Vanguard, Bishop
    }

    public enum BreakableObjectType
    {
        Crate, Bomb, Crystal
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        InitializeSpawnableGroups();
        for (int i = 0; i < spawnableGroups.Count; i++)
        {
            ToggleSpawnableGroupActivation(i, false);
        }
        bpmInteract = GameObject.FindWithTag("RhythmManager").GetComponent<BPMInteract>();
    }

    public void InitializeSpawnableGroups()
    {
        for (int i = 0; i < spawnableGroups.Count; i++)
        {
            spawnableGroups[i].InitializeGroup(this);
        }
    }

    public void ToggleSpawnableGroupActivation(int index, bool state)
    {
        if (index < 0 || index >= spawnableGroups.Count)
        {
            Debug.LogError("Invalid spawnable group index: " + index);
            return;
        }

        spawnableGroups[index].SetGroupActivationState(state);
    }


    public void ResetSpawnableGroup(int i)
    {
        spawnableGroups[i].ResetGroup();
    }

    public bool CheckGroupState(int i)
    {
        if (i < 0 || i >= spawnableGroups.Count)
        {
            Debug.LogError("Invalid spawnable group index: " + i);
            return false;
        }

        return spawnableGroups[i].IsActive();
    }

    public void AddBeatToSpawnableGroups()
    {
        if (angelBreakActive || doubleTimeActive)
            return;

        for (int i = 0; i < spawnableGroups.Count; i++)
        {
            spawnableGroups[i].AddBeatToGroup();
        }
    }

    public void DoubleTimeAddBeatToSpawnableGroups()
    {
        if (angelBreakActive || !doubleTimeActive)
            return;

        for (int i = 0; i < spawnableGroups.Count; i++)
        {
            spawnableGroups[i].AddBeatToGroup();
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

    public EnemyBase GetEnemyType(EnemyType enemyType)
    {
        foreach(EnemyBase enemy in enemyPrefabs)
        {
            string enemyName = enemy.GetType().Name;
            //Debug.Log("Checking enemy prefab: " + enemyName + " against type: " + enemyType.ToString());
            if (enemyName == enemyType.ToString())
            {
                return enemy;
            }
        }

        Debug.LogError("Enemy type not found: " + enemyType);
        return null;
    }

    public BreakableObjectBase GetBreakableObjectType(BreakableObjectType objectType)
    {
        foreach (BreakableObjectBase breakableObj in breakableObjectPrefabs)
        {
            string objName = nameof(breakableObj);
            if (objName == objectType.ToString())
            {
                return breakableObj;
            }
        }

        Debug.LogError("Breakable object type not found: " + objectType);
        return null;
    }

    [System.Serializable]
    public class SpawnableGroup
    {
        [SerializeField] List<Enemy> enemies;
        [SerializeField] List<BreakableObject> breakableObjects;
        bool isActive;
        [SerializeField] GameObject door;


        public void InitializeGroup(RoomEncounterManager eMan)
        {
            for (int i = 0; i < enemies.Count; i++)
            {
                if (enemies[i].GetEnemyType() == EnemyType.GlitchShaman || enemies[i].GetEnemyType() == EnemyType.Bishop)
                {
                    continue; 
                }
                enemies[i].Initialize(eMan);
                enemies[i].deathEvent += CheckLivingEnemies;
            }

            FindProtectedEnemies();

            for (int i = 0; i < enemies.Count; i++)
            {
                if (enemies[i].GetEnemyType() == EnemyType.GlitchShaman || enemies[i].GetEnemyType() == EnemyType.Bishop)
                {
                    enemies[i].Initialize(eMan);
                    enemies[i].deathEvent += CheckLivingEnemies;
                }
            }


            for (int i = 0; i < breakableObjects.Count; i++)
            {
                breakableObjects[i].Initialize(eMan);
            }
        }

        public void SetGroupActivationState(bool state)
        {
            for (int i = 0; i < enemies.Count; i++)
            {
                enemies[i].SetActivationState(state);
            }

            //breakable objects should be able to be active without the group being active, so they will never be forced to deactivate
            if (state)
            {
                for (int i = 0; i < breakableObjects.Count; i++)
                {
                    breakableObjects[i].SetActivationState(state);
                }
            }

            door.SetActive(state);
            isActive = state;
        }


        void FindProtectedEnemies()
        {
            List<EnemyBase> protectedEnemies = new List<EnemyBase>();
            for (int i = 0; i < enemies.Count; i++)
            {
                if (enemies[i].isProtected)
                {
                    protectedEnemies.Add(enemies[i].GetEnemyInstance());
                }
            }

            for (int i = 0; i < protectedEnemies.Count; i++)
            {
               for(int j = 0; j < enemies.Count; j++)
               {
                    if (enemies[j].GetEnemyType() == EnemyType.GlitchShaman || enemies[j].GetEnemyType() == EnemyType.Bishop)
                    {
                        if(enemies[j].protectedEnemy == null)
                            enemies[j].protectedEnemy = protectedEnemies[i];
                    }
               }
            }
        }

        public void AddBeatToGroup()
        {
            for (int i = 0; i < enemies.Count; i++)
            {
                if (!enemies[i].IsDead())
                    enemies[i].GetEnemyInstance().AddToBeatCount();
            }
        }

        public bool IsActive()
        {
            return isActive;
        }

        public void CheckLivingEnemies()
        {
            foreach (Enemy enemy in enemies)
            {
                if (!enemy.IsDead())
                {
                    return;
                }
            }

            SetGroupActivationState(false);
        }
         
        public void ResetGroup()
        {
            for (int i = 0; i < enemies.Count; i++)
            {
                enemies[i].HandleEnemyRespawn();
            }

            for(int i = 0;i < breakableObjects.Count; i++)
            {
                breakableObjects[i].HandleObjectRespawn();
            }
        }
    }

    [System.Serializable]
    class Enemy 
    {
        [SerializeField] Transform spawnPoint;
        [SerializeField] EnemyType enemyType;
        [SerializeField] EnemyBase enemyInstance;
        RoomEncounterManager encounterManager;
        public bool isProtected;
        public Action deathEvent;
        bool isDead;

        //Protector Settings
        [HideInInspector] public EnemyBase protectedEnemy;

        public void Initialize(RoomEncounterManager eMan)
        {
            encounterManager = eMan;
            enemyInstance = Instantiate(encounterManager.GetEnemyType(enemyType), spawnPoint.position, Quaternion.identity);
            enemyInstance.ManagerDeathEvent += HandleEnemyDeath;
            enemyInstance.Initialize(encounterManager.player, encounterManager.pulseManager, encounterManager.projectilePool, encounterManager, false);

            if(enemyType == EnemyType.GlitchShaman || enemyType == EnemyType.Bishop)
            {
                //Debug.Log("Protecting " + protectedEnemy);
                enemyInstance.GetComponent<IProtector>().InitializeProteciton(protectedEnemy);
            }

            isDead = false;
        }

        public void SetActivationState(bool state)
        {
            if(enemyInstance == null)
            {
                Debug.LogError("Enemy instance is null for enemy type: " + enemyType);
                return;
            }
            enemyInstance.gameObject.SetActive(state);
            enemyInstance.SetIsActive(state);
        }

        public void HandleEnemyDeath()
        {
            isDead = true;
            deathEvent?.Invoke();
        }

        public void HandleEnemyRespawn()
        {
            isDead = false;
            enemyInstance.ResetEnemy();
            enemyInstance.gameObject.transform.position = spawnPoint.position;
            SetActivationState(true);
        }

        public bool IsDead()
        {
            return isDead;
        }

        public EnemyBase GetEnemyInstance()
        {
            return enemyInstance;
        }

        public EnemyType GetEnemyType()
        {
            return enemyType;
        }
    }

    [System.Serializable]
    class BreakableObject
    {
        [SerializeField] Transform spawnPoint;
        [SerializeField] BreakableObjectType objectType;
        public BreakableObjectBase breakableObjInstance;
        RoomEncounterManager encounterManager;
        bool isDead;

        public void Initialize(RoomEncounterManager eMan)
        {
            encounterManager = eMan;
            breakableObjInstance = Instantiate(encounterManager.GetBreakableObjectType(objectType), spawnPoint.position, Quaternion.identity);
            breakableObjInstance.ManagerDeathEvent.AddListener(HandleBreakableObjectDeath);
            breakableObjInstance.Initialize();
            isDead = false;
        }

        public void SetActivationState(bool state)
        {
            breakableObjInstance.gameObject.SetActive(state);
        }

        public void HandleBreakableObjectDeath()
        {
            isDead = true;
        }

        public void HandleObjectRespawn()
        {
            isDead = false;
            breakableObjInstance.gameObject.transform.position = spawnPoint.position;
            SetActivationState(true);
        }

        public bool IsDead()
        {
            return isDead;
        }
    }
}

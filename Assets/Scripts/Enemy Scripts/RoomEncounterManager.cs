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
    [SerializeField] GameObject player;
    [SerializeField] PulseManager pulseManager;

    [Header("Spawnable Items")]
    [SerializeField] EnemyBase[] enemyPrefabs;
    [SerializeField] BreakableObjectBase[] breakableObjectPrefabs;

    [Header("Encounter Settings")]
    [SerializeField] List<SpawnableGroup> spawnableGroups = new List<SpawnableGroup>();
    SpawnableGroup dynamicallySpawnedGroup;

    bool angelBreakActive = false;
    bool doubleTimeActive = false;
  

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

    public EnemyBase SpawnEnemyGroup(List<EnemySpawnConfig> configs)
    {
        dynamicallySpawnedGroup.BuildFromConfigs(configs, this);        
        dynamicallySpawnedGroup.SetGroupActivationState(true);
        return dynamicallySpawnedGroup.GetFirstEnemyInGroup();
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

    public EnemyBase GetEnemyType(EnemyType.ChosenEnemyType enemyType)
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
            string objName = breakableObj.GetType().Name;
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
                if (enemies[i].GetEnemyType() == EnemyType.ChosenEnemyType.GlitchShaman || enemies[i].GetEnemyType() == EnemyType.ChosenEnemyType.Bishop || enemies[i].GetEnemyType() == EnemyType.ChosenEnemyType.TurretGenerator)
                {
                    continue; 
                }
                enemies[i].Initialize(eMan);
                enemies[i].deathEvent += CheckLivingEnemies;
            }

            FindProtectedEnemies();

            for (int i = 0; i < enemies.Count; i++)
            {
                if (enemies[i].GetEnemyType() == EnemyType.ChosenEnemyType.GlitchShaman || enemies[i].GetEnemyType() == EnemyType.ChosenEnemyType.Bishop || enemies[i].GetEnemyType() == EnemyType.ChosenEnemyType.TurretGenerator)
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

        public void BuildFromConfigs(List<EnemySpawnConfig> configs, RoomEncounterManager eMan)
        {
            enemies = new List<Enemy>();
            FindProtectedEnemies();
            foreach (var config in configs)
            {
                Enemy e = new Enemy();
                e.BuildFromConfig(config);
                e.Initialize(eMan);
                e.deathEvent += CheckLivingEnemies;
                enemies.Add(e);
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

            if (door != null)
                door.SetActive(state);
            isActive = state;
        }


        void FindProtectedEnemies()
        {
            List<Enemy> protectedEnemies = new List<Enemy>();
            for (int i = 0; i < enemies.Count; i++)
            {
                if (enemies[i].isProtected)
                {
                    if (enemies[i].CanAddProtector())
                        protectedEnemies.Add(enemies[i]);
                    //Debug.Log("Found protected enemy: " + enemies[i].GetEnemyType() + " at index: " + i);
                }
            }

            for (int i = 0; i < protectedEnemies.Count; i++)
            {
               for(int j = 0; j < enemies.Count; j++)
               {
                    if (enemies[j].GetEnemyType() == EnemyType.ChosenEnemyType.GlitchShaman || enemies[j].GetEnemyType() == EnemyType.ChosenEnemyType.Bishop || enemies[j].GetEnemyType() == EnemyType.ChosenEnemyType.TurretGenerator)
                    {
                        //Debug.Log("Assigning protected enemy: " + protectedEnemies[i].name);
                        if (enemies[j].protectedEnemy == null)
                        {
                            enemies[j].protectedEnemy = protectedEnemies[i].GetEnemyInstance();
                            protectedEnemies[i].AddProtector();
                            enemies[j].AddFuncToDeathEvent(protectedEnemies[i].RemoveProtector);
                            break;
                        }
                    }
               }
            }
        }

        public EnemyBase GetFirstEnemyInGroup()
        {
            for (int i = 0; i < enemies.Count; i++)
            {
                if (!enemies[i].IsDead())
                    return enemies[i].GetEnemyInstance();
            }
            return null;
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
                if (!enemy.IsDead()) return;

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
        [SerializeField] EnemyType.ChosenEnemyType enemyType;
        [SerializeField] EnemyBase enemyInstance;
        RoomEncounterManager encounterManager;
        public Action deathEvent;
        bool isDead;

        //Protector Settings
        public bool isProtected;
        [SerializeField] int maxProtectorsAllowed = 1;
        int protectorsActive;
        [HideInInspector] public EnemyBase protectedEnemy;

        public void Initialize(RoomEncounterManager eMan)
        {
            encounterManager = eMan;
            enemyInstance = Instantiate(encounterManager.GetEnemyType(enemyType), spawnPoint.position, Quaternion.identity);
            enemyInstance.ManagerDeathEvent += HandleEnemyDeath;
            enemyInstance.Initialize(encounterManager.player, encounterManager.pulseManager, encounterManager.projectilePool, encounterManager, false);

            if(enemyType == EnemyType.ChosenEnemyType.GlitchShaman || enemyType == EnemyType.ChosenEnemyType.Bishop || enemyType == EnemyType.ChosenEnemyType.TurretGenerator)
            {
                if(protectedEnemy != null)
                {
                    Debug.Log("Protecting " + protectedEnemy);
                    enemyInstance.GetComponent<IProtector>().InitializeProteciton(protectedEnemy);
                }
            }

            isDead = false;
        }

        public void BuildFromConfig(EnemySpawnConfig config)
        {
            enemyType = config.enemyType;
            spawnPoint = config.spawnPosition; 
            isProtected = config.isProtected;
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

        public EnemyType.ChosenEnemyType GetEnemyType()
        {
            return enemyType;
        }

        public void AddFuncToDeathEvent(Action func)
        {
            deathEvent += func;
        }

        public void AddProtector()
        {
            if (protectorsActive < maxProtectorsAllowed)
            {
                protectorsActive++;
            }
        }

        public void RemoveProtector()
        {
            if (protectorsActive > 0)
            {
                protectorsActive--;
            }
        }

        public bool CanAddProtector()
        {
            return protectorsActive < maxProtectorsAllowed;
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

    [System.Serializable]
    public class EnemySpawnConfig
    {
        public EnemyType.ChosenEnemyType enemyType;
        public Transform spawnPosition;
        public bool isProtected;

        public EnemySpawnConfig(EnemyType.ChosenEnemyType enemyType, Transform spawnPosition, bool isProtected)
        {
            this.enemyType = enemyType;
            this.spawnPosition = spawnPosition;
            this.isProtected = isProtected;
        }
    }

    [System.Serializable]
    public class BossSpawnConfig
    {
        public EnemyType.ChosenEnemyType bossType;
        public Transform spawnPosition;
        public bool isProtected;

        // Add whatever boss-specific fields you need, e.g:
        public int phase;
        public float enrageTimer;
        public List<EnemySpawnConfig> minionWaves;
    }
}

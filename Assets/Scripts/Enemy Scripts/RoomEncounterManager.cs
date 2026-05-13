using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;
using static RoomEncounterManager;
using System.Collections;

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
        bpmInteract = GameObject.FindWithTag("RhythmManager").GetComponent<BPMInteract>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void InitializeSpawnableGroups()
    {
        for (int i = 0; i < spawnableGroups.Count; i++)
        {
            spawnableGroups[i].InitializeGroup();
        }
    }

    public void ActivateSpawnableGroup(int index)
    {
        if (index < 0 || index >= spawnableGroups.Count)
        {
            Debug.LogError("Invalid spawnable group index: " + index);
            return;
        }

        spawnableGroups[index].SetGroupActivationState(true);
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
            string enemyName = nameof(enemy);
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
    public struct SpawnableGroup
    {
        [SerializeField] List<Enemy> enemies;
        [SerializeField] List<BreakableObject> breakableObjects;
        [SerializeField] RoomEncounterManager enounterMan;
        bool isActive;
        GameObject door;

        SpawnableGroup(List<Enemy> enemies, List<BreakableObject> breakableObjects, RoomEncounterManager eMan, GameObject door)
        {
            this.enemies = enemies;
            this.breakableObjects = breakableObjects;
            this.enounterMan = eMan;
            this.door = door;
            isActive = false;
        }

        public void InitializeGroup()
        {
            for (int i = 0; i < enemies.Count; i++)
            {
                enemies[i].Initialize(enounterMan);
            }

            for (int i = 0; i < breakableObjects.Count; i++)
            {
                breakableObjects[i].Initialize(enounterMan);
            }
        }

        public void SetGroupActivationState(bool state)
        {
            for (int i = 0; i < enemies.Count; i++)
            {
                enemies[i]. SetActivationState(state);
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
        }

        public void AddBeatToGroup()
        {
            for (int i = 0; i < enemies.Count; i++)
            {
                if (!enemies[i].IsDead())
                    enemies[i].enemyInstance.AddToBeatCount();
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
    public struct Enemy 
    {
        [SerializeField] Transform spawnPoint;
        [SerializeField] EnemyType enemyType;
        public EnemyBase enemyInstance;
        RoomEncounterManager encounterManager;
        bool isDead;

        Enemy(Transform spawnPoint, EnemyType enemyType, RoomEncounterManager eMan)
        {
            this.spawnPoint = spawnPoint;
            this.enemyType = enemyType;
            encounterManager = eMan;
            enemyInstance = Instantiate(encounterManager.GetEnemyType(enemyType), spawnPoint.position, Quaternion.identity);
            isDead = false;
        }

        public void Initialize(RoomEncounterManager eMan)
        {
            encounterManager = eMan;
            enemyInstance = Instantiate(encounterManager.GetEnemyType(enemyType), spawnPoint.position, Quaternion.identity);
            enemyInstance.ManagerDeathEvent.AddListener(HandleEnemyDeath);
            enemyInstance.Initialize(encounterManager.player, encounterManager.pulseManager, encounterManager.projectilePool, encounterManager, false);
            isDead = false;
        }

        public void SetActivationState(bool state)
        {
            enemyInstance.gameObject.SetActive(state);
            enemyInstance.SetIsActive(state);
        }

        public void HandleEnemyDeath()
        {
            isDead = true;
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
    }

    [System.Serializable]
    public struct BreakableObject
    {
        [SerializeField] Transform spawnPoint;
        [SerializeField] BreakableObjectType objectType;
        public BreakableObjectBase breakableObjInstance;
        RoomEncounterManager encounterManager;
        bool isDead;

        BreakableObject(Transform spawnPoint, BreakableObjectType objectType, RoomEncounterManager eMan)
        {
            this.spawnPoint = spawnPoint;
            this.objectType = objectType;
            encounterManager = eMan;
            breakableObjInstance = Instantiate(encounterManager.GetBreakableObjectType(objectType), spawnPoint.position, Quaternion.identity);
            breakableObjInstance.Initialize();
            isDead = false;
        }

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

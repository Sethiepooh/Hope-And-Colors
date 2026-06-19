using System.Collections;
using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;
using System.Reflection;

public class PatternedMissileLauncher : MonoBehaviour
{
    [Header("References")]
    [SerializeField] JadeMissile missilePrefab;
    [SerializeField] ProjectilePool missilePool;
    [SerializeField] GameObject player;

    [Header("Missile Settings")]
    [SerializeField] int startupDelayBeats;
    [SerializeField] Transform[] missileSpawnPoints;
    [SerializeField] GameObject[] telegraphObjects;
    [SerializeField] MissilePattern[] missilePatterns;

    Vector2 missileDirection = Vector2.right;
    List<MissileData> currentPattern = new List<MissileData>();
    [SerializeField] bool active;

    private void Start()
    {
        for (int i = 0; i < telegraphObjects.Length; i++)
        {
            telegraphObjects[i].SetActive(false);
        }
    }

    public void ToggleActivate(bool state)
    {
        active = state;
        if (active)
        {
            GetNewPattern();
        }
        else
        {
            for (int i = 0; i < telegraphObjects.Length; i++)
            {
                telegraphObjects[i].SetActive(false);
            }
        }
    }

    public void AddBeatToAllMissiles()
    {
        if(!active) return;

        if (startupDelayBeats > 0)
        {
            startupDelayBeats--;
            return;
        }

        if (CheckAllFired())
        {
            foreach(MissileData missile in currentPattern)
            {
                missile.currentBeat = 0;
                missile.fired = false;
            }

            GetNewPattern();            
        }

        foreach (MissileData missile in currentPattern)
        {
            if (!missile.fired)
            {
                missile.currentBeat++;
                int missileIndex = currentPattern.IndexOf(missile);
                if (missile.currentBeat == missile.beatDelay -1)
                {
                    ToggleTelegraph(missileIndex, true);
                }

                if(missile.currentBeat == missile.beatDelay)
                {
                    ToggleTelegraph(missileIndex, false);
                    FireMissile(missileIndex);
                    missile.fired = true;
                }
            }
        }
    }

    void GetNewPattern()
    {
        int patternIndex = Random.Range(0, missilePatterns.Length);

        currentPattern.Clear();

        for (int i = 0; i < missilePatterns[patternIndex].patternData.Length; i++)
        {
            currentPattern.Add(missilePatterns[patternIndex].patternData[i]);
        }
    }

    public bool CheckAllFired()
    {
        foreach (MissileData missile in currentPattern)
        {
            if (!missile.fired)
            {
                return false;
            }
        }
        return true;
    }

    void FireMissile(int missileIndex)
    {
        JadeMissile missile = Instantiate(missilePrefab, missileSpawnPoints[missileIndex].position, Quaternion.LookRotation(Vector3.forward, missileDirection));
        missile.Initialize(player, missilePool);
        missile.Fire(missileDirection);
    }

    void ToggleTelegraph(int missileIndex, bool toggle)
    {
        telegraphObjects[missileIndex].SetActive(toggle);
    }
}

[System.Serializable]
class MissileData
{
    [HideInInspector] public int currentBeat;
    public int beatDelay;
    public bool fired;
}

[System.Serializable]
class MissilePattern
{
    public MissileData[] patternData;
}

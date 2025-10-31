using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;

public class PulseManager : MonoBehaviour
{
    List<GameObject> entitiesToPulse = new List<GameObject>();
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void AddEntity(GameObject entity)
    {
        entitiesToPulse.Add(entity);
    }

    public void RemoveEntity(GameObject entity)
    {
        entitiesToPulse.Remove(entity);
    }

    public void PulseAll()
    {
        foreach (GameObject entity in entitiesToPulse)
        {
            if (entity != null)
                entity.GetComponent<BeatPulse>().Pulse();
        }
    }
}

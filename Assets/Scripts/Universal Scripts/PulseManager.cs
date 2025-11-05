using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;

public class PulseManager : MonoBehaviour
{
    public List<GameObject> entitiesToPulse = new List<GameObject>();
    public List<GameObject> entitiesToFlash = new List<GameObject>();

    public void AddEntity(GameObject entity, List<GameObject> list)
    {
        list.Add(entity);
    }

    public void RemoveEntity(GameObject entity, List<GameObject> list)
    {
        list.Remove(entity);
    }

    public void PulseAll()
    {
        foreach (GameObject entity in entitiesToPulse)
        {
            if (entity != null)
                entity.GetComponent<BeatPulse>().Pulse();
        }
    }

    public void FlashAll()
    {
        foreach(GameObject entity in entitiesToFlash)
        {
            if (entity != null)
                entity.GetComponent<ChangeOnBeat>().ChangeColor();
        }
    }
}

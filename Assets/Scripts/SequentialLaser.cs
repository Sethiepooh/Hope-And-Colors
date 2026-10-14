using System.Collections;
using UnityEngine;

public class SequentialLaser : MonoBehaviour
{
    [SerializeField] GameObject[] laserParts;
    [SerializeField] float delayBetweenParts = 0.5f;
    [SerializeField] int beatInterval = 4; // Number of beats between each part activation
    int currentBeat = 0;
    bool isLaserActive = false;

    public void ToggleLaserActivation()
    {
        if(currentBeat % beatInterval == 0)
        {
            StartCoroutine(ToggleLaser());
        }
        currentBeat++;
    }

    IEnumerator ToggleLaser()
    {
        if (isLaserActive)
        {
            for (int i = 0; i < laserParts.Length; i++)
            {
                laserParts[i].SetActive(false);
                yield return new WaitForSeconds(delayBetweenParts);
            }
        }
        else
        {
            // Activate laser parts in order
            for (int i = 0; i < laserParts.Length; i++)
            {
                laserParts[i].SetActive(true);
                yield return new WaitForSeconds(delayBetweenParts);
            }
        }

        isLaserActive = !isLaserActive;
    }
}

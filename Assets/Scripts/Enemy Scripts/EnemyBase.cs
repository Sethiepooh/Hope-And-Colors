using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class EnemyBase : MonoBehaviour
{
    public bool active = false;

    public bool empowered = false;
    public abstract void Attack();
    public abstract void AddToBeatCount();

    IEnumerator ResetActive(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        active = true;
    }

    public void ResetActiveForEnemy(float sec)
    {
        StartCoroutine(ResetActive(sec));
    }
}

using UnityEngine;

public abstract class EnemyBase : MonoBehaviour
{
    public bool death = false;
    public abstract void Attack();
    public abstract void AddToBeatCount();
}

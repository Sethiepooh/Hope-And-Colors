using UnityEngine;

public abstract class EnemyBase : MonoBehaviour
{
    public bool death;
    public abstract void Attack();
    public abstract void AddToBeatCount();
}

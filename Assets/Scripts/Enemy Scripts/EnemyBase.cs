using UnityEngine;

public abstract class EnemyBase : MonoBehaviour
{
    public bool active = false;
    public abstract void Attack();
    public abstract void AddToBeatCount();
}

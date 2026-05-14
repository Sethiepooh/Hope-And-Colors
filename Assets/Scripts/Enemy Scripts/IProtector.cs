using UnityEngine;

public interface IProtector
{
    EnemyBase protectedEnemyBase { get; set; }

    public void InitializeProteciton(EnemyBase enemy);
}

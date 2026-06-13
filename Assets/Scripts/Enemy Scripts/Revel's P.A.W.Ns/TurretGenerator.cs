using UnityEngine;

public class TurretGenerator : EnemyBase, IProtector
{
    public EnemyBase protectedEnemyBase { get; set; }

    private void Start()
    {
        health.onDeathEvent += DeactivateGenerator;
    }

    public override void AddToBeatCount()
    {
        // No implementation needed for this enemy
    }

    public override void Attack()
    {
        // No implementation needed for this enemy
    }

    public void InitializeProteciton(EnemyBase enemy)
    {
        protectedEnemyBase = enemy;
        protectedEnemyBase.GetComponent<Turret>().AddGenerator(this.gameObject);
        Debug.Log("Initializing protection for " + protectedEnemyBase.name);
    }

    public void DeactivateGenerator()
    {
        if(protectedEnemyBase != null)
        {
            transform.gameObject.SetActive(false);
            protectedEnemyBase.GetComponent<Turret>().DeactivateTurret();
        }
    }    
}

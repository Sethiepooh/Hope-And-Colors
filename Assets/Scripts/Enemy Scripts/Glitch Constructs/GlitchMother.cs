using UnityEngine;

public class GlitchMother : EnemyBase
{
    [Header("Attack Stats")]
    [SerializeField] int damage = 5;
    [SerializeField] Transform projectileSpawn;
    int beatCount = 0;
    bool slash = false;

    [Header("Movement Stats")]
    [SerializeField] float moveSpeed = 3.0f;
    Rigidbody2D rb;
    GameObject player;
    [SerializeField] LayerMask playerLayer;

    EnemyManager enemyManager;
    PulseManager pulseManager;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public override void Attack()
    {
        throw new System.NotImplementedException();
    }

    public override void AddToBeatCount()
    {
        throw new System.NotImplementedException(); 
    }
}

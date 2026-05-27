using UnityEngine;
using static RoomEncounterManager;

public class BossActivator : MonoBehaviour
{
    [SerializeField] RoomEncounterManager roomEncounterManager;
    [SerializeField] EnemyBase bossInstance;   // your scene-placed boss
    [SerializeField] Transform bossSpawnPoint;
    [SerializeField] int targetGroupIndex;
    [SerializeField] EnemyType.ChosenEnemyType bossType;


    private void Start()
    {
        ActivateBossFight();
    }

    void ActivateBossFight()
    {
        var config = new BossSpawnConfig(bossInstance, bossSpawnPoint, false, bossType);
        roomEncounterManager.AddBossToSpawnableGroup(targetGroupIndex, config);
        roomEncounterManager.ToggleSpawnableGroupActivation(targetGroupIndex, true);
    }
}

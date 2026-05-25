using UnityEngine;

public interface IBossInitialization 
{
    public interface IBossInitializable
    {
        void InitializeBoss(RoomEncounterManager eMan, RoomEncounterManager.BossSpawnConfig config);
    }
}

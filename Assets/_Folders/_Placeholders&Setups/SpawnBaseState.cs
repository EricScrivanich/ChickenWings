
public abstract class SpawnBaseState 
{
    public abstract void EnterState(SpawnStateManager spawner);
    public abstract void ExitState(SpawnStateManager spawner);
    // public abstract void UpdateState(SpawnStateManager spawner);
    public abstract void SetSpeedAndPos(SpawnStateManager spawner);
    public abstract void SetNewIntensity(SpawnStateManager spawner,RandomSpawnIntensity spawnIntensity);
    public abstract void SetupHitTarget(SpawnStateManager spawner);
    public abstract void RingsFinished(SpawnStateManager spawner, bool isCorrect);


    // Start is called before the first frame update


}

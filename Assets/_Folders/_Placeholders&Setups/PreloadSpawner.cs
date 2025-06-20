using UnityEngine;

public class PreloadSpawner : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private float waveTime = 0;
    private float timeToWait = LevelRecordManager.TimePerStep;

    [SerializeField] private LevelData levelData;

    private ushort currentSpawnStep;
    private ushort finalSpawnStep;
    private ushort startingSpawnStep;
    [SerializeField] private LoadingScreen loadingScreen;

    private SpawnStateManager spawnStateManager;

    private float total;

    public void EnablePreloadSpawner(ushort current, ushort finalSpawnStep, SpawnStateManager s)
    {
        this.finalSpawnStep = finalSpawnStep;
        this.currentSpawnStep = current;
        this.startingSpawnStep = current;
        this.spawnStateManager = s;


        waveTime = 0;
        enabled = true;

        loadingScreen.StartLoadingTextSeq();

        loadingScreen.SetLoadingBarValue(0);

    }

    public void DestoryLoadingScreen()
    {
        if (loadingScreen != null)
        {
            loadingScreen.DestroyImmediately();
        }
    }
    private void Update()
    {
        waveTime += Time.deltaTime;
        if (waveTime >= timeToWait)
        {
            levelData.NextSpawnStep(currentSpawnStep);


            currentSpawnStep++;
            waveTime = waveTime - timeToWait; // do this to keep it ver accurate

            if (waveTime >= timeToWait)
            {
                levelData.NextSpawnStep(currentSpawnStep);


                currentSpawnStep++;
                if (currentSpawnStep >= finalSpawnStep)
                {
                    Finish();
                    return;
                }
                waveTime = waveTime - timeToWait; // do this to keep it ver accurate


                if (waveTime >= timeToWait)
                {
                    levelData.NextSpawnStep(currentSpawnStep);


                    currentSpawnStep++;
                    if (currentSpawnStep >= finalSpawnStep)
                    {
                        Finish();
                        return;
                    }
                    waveTime = waveTime - timeToWait; // do this to keep it ver accurate
                }
            }

            if (waveTime >= timeToWait)
            {
                levelData.NextSpawnStep(currentSpawnStep);


                currentSpawnStep++;
                if (currentSpawnStep >= finalSpawnStep)
                {
                    Finish();
                    return;
                }
                waveTime = waveTime - timeToWait; // do this to keep it ver accurate


                if (waveTime >= timeToWait)
                {
                    levelData.NextSpawnStep(currentSpawnStep);


                    currentSpawnStep++;
                    if (currentSpawnStep >= finalSpawnStep)
                    {
                        Finish();
                        return;
                    }
                    waveTime = waveTime - timeToWait; // do this to keep it ver accurate
                }

                if (waveTime >= timeToWait)
                {
                    levelData.NextSpawnStep(currentSpawnStep);


                    currentSpawnStep++;
                    if (currentSpawnStep >= finalSpawnStep)
                    {
                        Finish();
                        return;
                    }
                    waveTime = waveTime - timeToWait; // do this to keep it ver accurate
                }
            }
        }

        float p = ((float)currentSpawnStep - (float)startingSpawnStep) / ((float)finalSpawnStep - (float)startingSpawnStep);
        loadingScreen.SetLoadingBarValue(p);






    }

    public void Finish()
    {
        enabled = false;
        loadingScreen.StopLoad();
        spawnStateManager.FinishPreload();
    }
}

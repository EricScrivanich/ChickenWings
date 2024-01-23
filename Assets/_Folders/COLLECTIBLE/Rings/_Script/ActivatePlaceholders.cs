using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActivatePlaceholders : MonoBehaviour
{
    public RingPool Pool;

    public int index = 1;
    [SerializeField] private List<GameObject> ringSetups;
    
    // Start is called before the first frame update
    private void Start()
    {
        if (!Pool.RingType[index].Testing)
        {
            ResetSetups();
        }
        ChooseSetup(true);




    }

    private void ResetSetups()
    {
        foreach(GameObject setup in ringSetups)
        {
            setup.SetActive(false);
        }
    }

    private void ChooseSetup(bool completedSequence)
    {
        if (!Pool.RingType[index].Testing)
        {
        ResetSetups();
        int randomIndex = Random.Range(0, ringSetups.Count);
        ringSetups[randomIndex].SetActive(true);
        }
        Pool.RingType[index].ringEvent.OnSpawnRings?.Invoke(completedSequence);


    }

    private void OnEnable() {
        foreach (var ringId in Pool.RingType)
        {
            Pool.RingType[index].ringEvent.OnCreateNewSequence += ChooseSetup;

        }
    
    }

    private void OnDisable() 
    {
        foreach (var ringId in Pool.RingType)
        {
            Pool.RingType[index].ringEvent.OnCreateNewSequence -= ChooseSetup;

        }
        
        
    }
        
    }


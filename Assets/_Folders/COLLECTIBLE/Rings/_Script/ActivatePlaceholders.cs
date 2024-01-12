using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActivatePlaceholders : MonoBehaviour
{
    public RingID RedID;
    [SerializeField] private List<GameObject> ringSetups;
    
    // Start is called before the first frame update
    private void Start()
    {
        RedID.ringEvent.OnSpawnRings?.Invoke(true);


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
        if (!RedID.Testing)
        {
        ResetSetups();
        int randomIndex = Random.Range(0, ringSetups.Count);
        ringSetups[randomIndex].SetActive(true);
        }
        RedID.ringEvent.OnSpawnRings?.Invoke(completedSequence);


    }

    private void OnEnable() {
        RedID.ringEvent.OnCreateNewSequence += ChooseSetup;

    }

    private void OnDisable() 
    {
        RedID.ringEvent.OnCreateNewSequence -= ChooseSetup;
        
        
    }
        
    }


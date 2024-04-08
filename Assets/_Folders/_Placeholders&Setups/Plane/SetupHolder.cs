using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SetupHolder", menuName = "PlaceholderScriptableObjects/SetupHolder", order = 2)]
public class SetupHolder : ScriptableObject
{
    public List<PlaceholderDataCollection> Setups;

    public PlaceholderDataCollection GetRandomSetup()
    {
        if (Setups == null || Setups.Count == 0)
        {
            Debug.LogWarning("Setups list is empty or null!");
            return null;
        }

        // Generate a random index within the bounds of the Setups list
        int randomIndex = Random.Range(0, Setups.Count);

        // Return the PlaceholderDataCollection at the random index
        return Setups[randomIndex];
    }
}


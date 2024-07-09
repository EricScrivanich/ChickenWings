using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CreateAssetMenu(fileName = "CollectableSpawnSetup", menuName = "Setups/CollectableSpawnSetup")]
public class SpawnSetupCollectable : ScriptableObject
{
    public CollectableData[] collectableSetups;
    public Vector2 triggerObjectPosition;
    public float triggerObjectSpeed;
    public float triggerObjectXCordinateTrigger;
    [SerializeField] private GameObject RingPrefab;

    // Start is called before the first frame update
    public void SpawnCollectables(CollectablePoolManager manager, bool finalTrigger)
    {
        if (collectableSetups.Length > 0)
        {
            foreach (var collectable in collectableSetups)
            {
                collectable.InitializeCollectable(manager, finalTrigger);
            }
        }
    }

    public void PlaceSetup(Transform parent)
    {
        if (collectableSetups.Length > 0)
        {
            foreach (var collectable in collectableSetups)
            {
                if (collectable is RingSO ringVar)
                {
                    foreach (var ring in ringVar.data)
                    {
                        // Instantiate the RingPrefab
                        GameObject ringObject = PrefabUtility.InstantiatePrefab(RingPrefab) as GameObject;


                        if (ringObject != null)
                        {
                            ringObject.transform.SetParent(parent, false);
                            // Set the position, rotation, and scale
                            ringObject.transform.position = ring.position;
                            ringObject.transform.rotation = ring.rotation;
                            ringObject.transform.localScale = ring.scale;

                            // Set the speed on the RingMovement script
                            RingMovement ringMovement = ringObject.GetComponent<RingMovement>();
                            if (ringMovement != null)
                            {
                                ringMovement.speed = ring.speed;
                            }

                            // Ensure the new object is properly recorded in the hierarchy
                            Undo.RegisterCreatedObjectUndo(ringObject, "Create RingPrefab");
                        }
                    }
                }
            }
        }
    }
}

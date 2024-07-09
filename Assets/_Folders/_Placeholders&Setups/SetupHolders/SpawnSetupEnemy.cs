using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CreateAssetMenu(fileName = "EnemySpawnSetup", menuName = "Setups/EnemySpawnSetup")]
public class SpawnSetupEnemy : ScriptableObject
{


    public Vector2 triggerObjectPosition;
    public EnemyData[] enemySetups;
    public float triggerObjectSpeed;



    public float triggerObjectXCordinateTrigger;

    [SerializeField] private GameObject tenderizerPigPrefab;
    [SerializeField] private GameObject jetpackPigPrefab;
    [SerializeField] private GameObject normalPigPrefab;
    [SerializeField] private GameObject pilotPigPrefab;


    public void SpawnEnemies(EnemyPoolManager manager)
    {
        Debug.Log(this.name);
        if (enemySetups.Length > 0)
        {
            foreach (var enemy in enemySetups)
            {
                enemy.InitializeEnemy(manager);
            }
        }

    }


    public void PlaceSetup(Transform parent)
    {
        if (enemySetups.Length > 0)
        {
            foreach (var enemy in enemySetups)
            {
                if (enemy is JetPackPigSO pigVar)
                {
                    foreach (var pig in pigVar.data)
                    {
                        // Instantiate the RingPrefab
                        GameObject pigObject = PrefabUtility.InstantiatePrefab(jetpackPigPrefab) as GameObject;


                        if (pigObject != null)
                        {
                            pigObject.transform.SetParent(parent, false);
                            // Set the position, rotation, and scale
                            pigObject.transform.position = pig.position;

                            pigObject.transform.localScale = pig.scale;

                            // Set the speed on the RingMovement script
                            JetPackPigMovement movement = pigObject.GetComponent<JetPackPigMovement>();
                            if (movement != null)
                            {
                                movement.speed = pig.speed;
                            }

                            // Ensure the new object is properly recorded in the hierarchy
                            Undo.RegisterCreatedObjectUndo(pigObject, "Create RingPrefab");
                        }
                    }
                }

                else if (enemy is TenderizerPigSO pigVar2)
                {
                    foreach (var pig in pigVar2.data)
                    {
                        // Instantiate the RingPrefab
                        GameObject pigObject = PrefabUtility.InstantiatePrefab(tenderizerPigPrefab) as GameObject;


                        if (pigObject != null)
                        {
                            pigObject.transform.SetParent(parent, false);
                            // Set the position, rotation, and scale
                            pigObject.transform.position = pig.position;

                            pigObject.transform.localScale = pig.scale;

                            // Set the speed on the RingMovement script
                            TenderizerPig movement = pigObject.GetComponent<TenderizerPig>();
                            if (movement != null)
                            {
                                movement.speed = pig.speed;
                                movement.hasHammer = pig.hasHammer;
                            }

                            // Ensure the new object is properly recorded in the hierarchy
                            Undo.RegisterCreatedObjectUndo(pigObject, "Create RingPrefab");
                        }
                    }

                }

                else if (enemy is NormalPigSO pigVar3)
                {
                    foreach (var pig in pigVar3.data)
                    {
                        // Instantiate the RingPrefab
                        GameObject pigObject = PrefabUtility.InstantiatePrefab(tenderizerPigPrefab) as GameObject;


                        if (pigObject != null)
                        {
                            pigObject.transform.SetParent(parent, false);
                            // Set the position, rotation, and scale
                            pigObject.transform.position = pig.position;

                            pigObject.transform.localScale = pig.scale;

                            // Set the speed on the RingMovement script
                            PigMovementBasic movement = pigObject.GetComponent<PigMovementBasic>();
                            if (movement != null)
                            {
                                movement.xSpeed = pig.speed;

                            }

                            // Ensure the new object is properly recorded in the hierarchy
                            Undo.RegisterCreatedObjectUndo(pigObject, "Create RingPrefab");
                        }
                    }

                }
            }
        }
    }


}

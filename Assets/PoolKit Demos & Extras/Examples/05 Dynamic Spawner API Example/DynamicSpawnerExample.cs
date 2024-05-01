//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
//
//	DynamicSpawnerExample.cs
//
//	An example script showing various methods to dynamically change a PoolKit Spawner at runtime.
//
//	PoolKit For Unity, Created By Melli Georgiou
//	© 2018 Hell Tap Entertainment LTD
//
//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HellTap.PoolKit;

public class DynamicSpawnerExample : MonoBehaviour {

	// Reference to the spawner
	Spawner spawner = null;
	
	[Header("Add Prefab To Spawner")]
	public string spawnerToFind = "Spawner";
	public GameObject addPrefabToSpawner = null;
	public float addPrefabToSpawnerRandomWeight = 100f;

	[Header("Replace Prefab In Spawner")]
	public GameObject replacePrefabInSpawner = null;

	// ==================================================================================================================
	//	START
	//	We use a simple Start coroutine to demonstrate various modifications to a spawner
	// ==================================================================================================================
	
	IEnumerator Start () {

		// Get PoolKit to find the spawner  ...
		spawner = PoolKit.GetSpawner( spawnerToFind );
		if( spawner != null && spawner.CanSpawn() == true ){

			// Add the new prefab to spawner
			spawner.AddPrefabToSpawner( addPrefabToSpawner, addPrefabToSpawnerRandomWeight );

			// Set new random weights
			spawner.SetRandomWeightOfPrefab( addPrefabToSpawner, 50f );
			spawner.SetRandomWeightOfVector3Position( 0, 50f );
			spawner.SetRandomWeightOfTransformPosition( 0, 25f );

			// Set new random weights (prefabs that don't exist or are out of range)
			spawner.SetRandomWeightOfPrefab( gameObject, 50f );
			spawner.SetRandomWeightOfVector3Position( 1, 50f );
			spawner.SetRandomWeightOfTransformPosition( 2, 25f );

			// Wait 4 seconds ... 
			yield return new WaitForSeconds( 4f );
			Debug.Log(" The Spawner Is Now Removing The Prefab: " + addPrefabToSpawner.name );

			// Remove the new prefab from the spawner ...
			spawner.RemovePrefabFromSpawner( addPrefabToSpawner );

			// Wait 4 seconds ... 
			yield return new WaitForSeconds( 4f );
			Debug.Log(" The Spawner Is Now Replacing The Prefab: " + replacePrefabInSpawner.name + " with: " + addPrefabToSpawner.name );

			// Replace the old prefab from the spawner with the new one ...
			spawner.ReplacePrefabInSpawner( replacePrefabInSpawner, addPrefabToSpawner );

			// Wait 4 seconds ... 
			yield return new WaitForSeconds( 4f );
			Debug.Log(" Adding 7 New Instances The Prefab: " + addPrefabToSpawner.name );

			// Explicilty Create 7 new prefabs
			//Debug.Log("Create 7!");
			for( int i = 0; i < 7; i++ ){
				spawner.Spawn( addPrefabToSpawner );
			}

			/*
			// Wait 4 seconds ... 
			yield return new WaitForSeconds( 4f );

			// Explicilty Create 5 old prefabs (which do not exist anymore)
			// NOTE: This should result in warning messages because this object doesn't exist!
			for( int i = 0; i < 7; i++ ){
				spawner.Spawn( replacePrefabInSpawner );
			}
			*/

		}
	}
	


}

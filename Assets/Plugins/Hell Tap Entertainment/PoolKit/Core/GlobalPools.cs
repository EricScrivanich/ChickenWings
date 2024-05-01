using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HellTap.PoolKit {

	// [CreateAssetMenu()]
	public class GlobalPools : ScriptableObject {

		[Header("Add Pool Prefabs To The List Below")]
		public GameObject[] globalPools = new GameObject[0];

		// ==================================================================================================================
		//	CREATE GLOBAL POOLS
		//	This is called by PoolKit.cs when the game starts
		// ==================================================================================================================
		
		internal void Create(){

			// If no global pools are setup, end early.
			if( globalPools.Length == 0 ){ return; }
			
			// Setup the PoolKit Global Pools GameObject
			GameObject globalPoolGO = new GameObject("PoolKit Global Pools");

			// Setup the PoolKitSetup object and configure it to be a Global Pool Group
			PoolKitSetup pks = globalPoolGO.AddComponent<PoolKitSetup>();
			pks.updatePoolKitSettings = false;
			pks.dontDestroyOnLoad = true;

			// Just to make sure something doesn't go wrong with execution timing, do Don't Destroy On Load
			DontDestroyOnLoad( globalPoolGO );
			int poolCount = 1;

			// Loop through the global pools
			for( int i = 0; i < globalPools.Length; i++ ){

				// Make sure this global pool is valid
				if( globalPools[i] != null && globalPools[i].GetComponent<Pool>() != null && 
					globalPools[i].GetComponent<Pool>().dontDestroyOnLoad == true 
				){
					GameObject poolGO = Instantiate( globalPools[i], Vector3.zero, Quaternion.identity, globalPoolGO.transform );
					if(poolGO!=null){ 
						poolGO.name = "Global Pool " + poolCount.ToString() +": " + globalPools[i].GetComponent<Pool>().poolName; 
						poolCount++;
					}
				}
			}

			// if pool count is 1 (in this case it means no pools were created ) destroy the PoolKit globalPoolGO
			if( poolCount == 1 && globalPoolGO != null ){ Destroy( globalPoolGO ); }
		}
	}
}
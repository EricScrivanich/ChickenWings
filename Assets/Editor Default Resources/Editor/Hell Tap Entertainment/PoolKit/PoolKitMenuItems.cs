#if UNITY_EDITOR

	// Use HellTap Namespace
	namespace HellTap.PoolKit {

		using System.Collections;
		using System.Collections.Generic;
		using System.IO;
		using UnityEngine;
		using UnityEditor;
		using System.Reflection;

		public class PoolKitMenuItems : MonoBehaviour {

			/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
			//	CREATE SPAWNER
			//	Creates a new Spawner in the scene
			/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
		
			[MenuItem( "GameObject/PoolKit/Create Spawner", false, 10)]
			static void CreateSpawner()
			{

				// Instantiate it in the scene
				GameObject spawnerGO = new GameObject("PoolKit Spawner");
				spawnerGO.AddComponent<Spawner>();

				// Register Undo
				Undo.RegisterCreatedObjectUndo(spawnerGO, "Create Spawner");

				// Select the gameObject
				Selection.activeObject = spawnerGO;
						
			}

			/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
			//	CREATE POOL
			//	Creates a new local Pool in the scene
			/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
		
			[MenuItem( "GameObject/PoolKit/Create Local Pool", false, 10 )]
			static void CreatePool()
			{
				// Instantiate it in the scene
				GameObject poolGO = new GameObject("PoolKit Pool");
				poolGO.AddComponent<Pool>();

				// Register Undo
				Undo.RegisterCreatedObjectUndo(poolGO, "Create Pool");

				// Select the gameObject
				Selection.activeObject = poolGO;
						
			}

			/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
			//	CREATE GLOBAL POOL
			//	Creates a new global Pool in the scene
			/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
		
			[MenuItem( "GameObject/PoolKit/Create Global Pool", false, 10 )]
			static void CreateGlobalPool()
			{
				// Instantiate it in the scene
				GameObject poolGO = new GameObject("PoolKit Global Pool");
				Pool pool = poolGO.AddComponent<Pool>();
				pool.dontDestroyOnLoad = true;

				// Register Undo
				Undo.RegisterCreatedObjectUndo(poolGO, "Create Global Pool");

				// Select the gameObject
				Selection.activeObject = poolGO;
						
			}

			/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
			//	CREATE GLOBAL POOL GROUP
			//	Creates a new Pool in the scene, inside a global Poop Group
			/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
		
			[MenuItem( "GameObject/PoolKit/Create Global Pool Group", false, 10 )]
			static void CreateGlobalPoolGroup()
			{
				// If we've selected a GameObject that already has a PoolKitSetup component (and its global) add the new pool!
				if( Selection.activeGameObject != null && 
					Selection.activeGameObject.GetComponent<PoolKitSetup>() != null &&
					Selection.activeGameObject.GetComponent<PoolKitSetup>()
				){

					// Instantiate it in the scene
					GameObject poolGO = new GameObject("PoolKit Global Pool");
					Pool pool = poolGO.AddComponent<Pool>();
					pool.dontDestroyOnLoad = true;

					// Make the pool a child of the currently selected global group
					poolGO.transform.parent = Selection.activeGameObject.transform;

					// Register Undo
					Undo.RegisterCreatedObjectUndo(poolGO, "Create Global Pool Group");

					// Select the gameObject
					Selection.activeObject = poolGO;

				// Search the scene for a PoolKitSetup that is set to global and ask the user if they want to use that?
				} else {
					
					// Find all PoolKitSetup objects in the scene
					PoolKitSetup[] pksArray = FindObjectsOfType<PoolKitSetup>();
					PoolKitSetup globalGroup = null;	// <- place holder for a PoolKitSetup set to global

					// Find the first pksArray set to global?
					for( int i = 0; i<pksArray.Length; i++ ){
						if( pksArray[i].dontDestroyOnLoad == true ){
							globalGroup = pksArray[i];
							break;
						}
					}

					// Ask the user to 
					if( globalGroup != null && 
						EditorUtility.DisplayDialog("Add Pool To Existing Global Pool Group?","PoolKit has found an existing Global Pool Group on a gameObject named '" + globalGroup.gameObject.name + "'. Would you like to add a new Pool to the group or create a completely new one?", "Use Existing Group [Recommended]", "Create New Group")
					){

						// Instantiate it in the scene
						GameObject poolGO = new GameObject("PoolKit Global Pool");
						Pool pool = poolGO.AddComponent<Pool>();
						pool.dontDestroyOnLoad = true;

						// Make the pool a child of the currently selected global group
						poolGO.transform.parent = globalGroup.gameObject.transform;

						// Register Undo
						Undo.RegisterCreatedObjectUndo(poolGO, "Create Global Pool Group");

						// Select the gameObject
						Selection.activeObject = poolGO;
					
					// Create a new group
					} else {

						// Instantiate it in the scene
						GameObject poolKitSetupGO = new GameObject("PoolKit - Global Pool Group");
						PoolKitSetup pks = poolKitSetupGO.AddComponent<PoolKitSetup>();

						// Make sure dont destroy on load is set to true, and dont update poolKit settings by default
						pks.dontDestroyOnLoad = true;
						pks.updatePoolKitSettings = false;

						// Register Undo
						Undo.RegisterCreatedObjectUndo(poolKitSetupGO, "Create Global Pool Group");

						// Instantiate it in the scene
						GameObject poolGO = new GameObject("PoolKit Global Pool");
						Pool pool = poolGO.AddComponent<Pool>();
						pool.dontDestroyOnLoad = true;

						// Make the pool a child of the global group
						poolGO.transform.parent = poolKitSetupGO.transform;

						// Register Undo
						Undo.RegisterCreatedObjectUndo(poolGO, "Create Global Pool Group");

						// Select the gameObject
						Selection.activeObject = poolGO;

					}
				}
			}

			/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
			//	CREATE POOLKIT SETUP
			//	Creates a new PoolKit Setup object in the scene
			/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
			/*
			[MenuItem( "GameObject/PoolKit/Create PoolKit Setup Object", false, 10 )]
			static void CreatePoolKitSetup()
			{

				// Instantiate it in the scene
				GameObject poolKitSetupGO = new GameObject("PoolKit Setup");
				poolKitSetupGO.AddComponent<PoolKitSetup>();

				// Register Undo
				Undo.RegisterCreatedObjectUndo(poolKitSetupGO, "Create PoolKit Setup");

				// Select the gameObject
				Selection.activeObject = poolKitSetupGO;
						
			}
			*/


			/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
			//	SETUP GLOBAL POOLS
			//	Opens the "PoolKit Global Pools" asset, or creates a new one if it doesnt exist
			/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
		
			[MenuItem( "GameObject/PoolKit/Setup Global Pools", false, 10 )]
			static void SetupGlobalPools()
			{

				// Helpers
				bool _assetDatabaseNeedsRefreshing = false;
				string prefix = Application.dataPath +"/";
				//Debug.Log( prefix + "Plugins" );
				
				// Create Plugins Folder
				if( Directory.Exists( prefix + "Plugins/" ) == false ){
					Directory.CreateDirectory( prefix + "Plugins/" );
					_assetDatabaseNeedsRefreshing = true;
				}

				// Create Plugins / Hell Tap Entertainment Folder
				if( Directory.Exists( prefix + "Plugins/Hell Tap Entertainment/" ) == false ){
					Directory.CreateDirectory( prefix + "Plugins/Hell Tap Entertainment/" );
					_assetDatabaseNeedsRefreshing = true;
				}

				// Create Plugins / Hell Tap Entertainment Folder / PoolKit
				if( Directory.Exists( prefix + "Plugins/Hell Tap Entertainment/PoolKit/" ) == false ){
					Directory.CreateDirectory( prefix + "Plugins/Hell Tap Entertainment/PoolKit/" );
					_assetDatabaseNeedsRefreshing = true;
				}

				// Create Plugins / Hell Tap Entertainment Folder / PoolKit / Global
				if( Directory.Exists( prefix + "Plugins/Hell Tap Entertainment/PoolKit/Global/" ) == false ){
					Directory.CreateDirectory( prefix + "Plugins/Hell Tap Entertainment/PoolKit/Global/" );
					_assetDatabaseNeedsRefreshing = true;
				}

				// Create Plugins / Hell Tap Entertainment Folder / PoolKit / Global / Resources
				if( Directory.Exists( prefix + "Plugins/Hell Tap Entertainment/PoolKit/Global/Resources/" ) == false ){
					Directory.CreateDirectory( prefix + "Plugins/Hell Tap Entertainment/PoolKit/Global/Resources/" );
					_assetDatabaseNeedsRefreshing = true;
				}

				// Create Plugins / Hell Tap Entertainment Folder / PoolKit / Global / PoolKit Global Pools
				if( File.Exists( prefix + "Plugins/Hell Tap Entertainment/PoolKit/Global/Resources/PoolKit Global Pools.asset" ) == false ){

					// Create a new instance of the Global Pools Asset
					GlobalPools globalPoolsAsset = ScriptableObject.CreateInstance<GlobalPools>();

					// Use the AssetDatabase to save it (NOTE: We only need a filepath from Assets for this )
					AssetDatabase.CreateAsset( globalPoolsAsset, "Assets/Plugins/Hell Tap Entertainment/PoolKit/Global/Resources/PoolKit Global Pools.asset" );
					AssetDatabase.SaveAssets();

					// Update the database
					_assetDatabaseNeedsRefreshing = true;

					// Select the asset
					Selection.activeObject = globalPoolsAsset;
				}

				// If we need to update the AssetDatabase, do it now
				if( _assetDatabaseNeedsRefreshing ){
					AssetDatabase.Refresh();
				
				// This means the asset already exists, select it
				} else {
					Selection.activeObject = AssetDatabase.LoadAssetAtPath("Assets/Plugins/Hell Tap Entertainment/PoolKit/Global/Resources/PoolKit Global Pools.asset", typeof(GlobalPools) );
				}

			}

		}
	}

#endif
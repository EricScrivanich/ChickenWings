//////////////////////////////////////////////////////////////////////////////////////////////////////////////////
//
//	PoolKitSetup.cs
//	This is a component that can be used to easily change global PoolKit settings without the API.
//
//	PoolKit For Unity, Created By Melli Georgiou
//	© 2018 Hell Tap Entertainment LTD
//
//////////////////////////////////////////////////////////////////////////////////////////////////////////////////

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

// Using the HellTap.PoolKit namespace
namespace HellTap.PoolKit {
	public class PoolKitSetup : MonoBehaviour {

		[Header("PoolKit Settings")]
		public bool updatePoolKitSettings = true;
		public PoolKit.RenameFormat renameObjectsInPool = PoolKit.RenameFormat.EasyToReadObjectNameWithPoolKitAndIndex;
		public bool onlyRenameObjectsInEditor = true;	// Optimizes performance in builds.
		public bool debugPoolKit = false;

		[Header("Options")]
		public bool dontDestroyOnLoad = false;

		// Use this for initialization
		void Awake(){
			
			// Update Settings if enabled
			if( updatePoolKitSettings ){
			
				// Settings
				PoolKit.renameObjectsInPool = renameObjectsInPool;
				PoolKit.onlyRenameObjectsInEditor = onlyRenameObjectsInEditor;

				// Debugging
				PoolKit.debugPoolKit = debugPoolKit;
			}

			// Otherwise, just handle the Global Pool Group settings
			if( dontDestroyOnLoad ){ 

				// Make sure this transform is parented to anything
				if( transform.parent != null ){ transform.parent = null; }

				// Set Dont Destroy On Load
				DontDestroyOnLoad(gameObject); 
			}
		}
	}
}

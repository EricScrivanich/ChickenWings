//////////////////////////////////////////////////////////////////////////////////////////////////////////////////
//
//	DespawnerEvent.cs
//	This is the base class to help the despawner receive events from other gameObjects. 
//	It is both a huge performance improvement (GC only for needed physics) and allows the despawner to work with child colliders.
//	NOTE: This must be setup in the Editor as attempting to add this at runtime will cause it to be destroyed.
//
//	PoolKit For Unity, Created By Melli Georgiou
//	© 2018 Hell Tap Entertainment LTD
//
//////////////////////////////////////////////////////////////////////////////////////////////////////////////////

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HellTap.PoolKit {

	[DisallowMultipleComponent]
	public class DespawnerEvent : MonoBehaviour {

		// Despawner
		public Despawner despawner = null;

		// Start
		bool hasStarted = false;
		void Start(){ 
			if(hasStarted){ return; }
			if(despawner==null){ Destroy(this); }
			hasStarted = true;
		}
	}
}
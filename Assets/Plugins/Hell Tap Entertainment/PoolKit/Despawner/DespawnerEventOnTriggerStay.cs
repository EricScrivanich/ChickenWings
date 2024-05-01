//////////////////////////////////////////////////////////////////////////////////////////////////////////////////
//
//	DespawnerEventOnTriggerStay.cs
//	Specific helper component for the Despawner
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
	public class DespawnerEventOnTriggerStay : DespawnerEvent {

		// ON TRIGGER STAY
		void OnTriggerStay( Collider other ){
			
			// Make sure we can track collisions in this method ...
			if( despawner != null && despawner.despawnMode == Despawner.DespawnMode.AfterPhysicsTriggerEvent && despawner.useTriggerStay &&

				// Make sure the collision is valid ...
				despawner.CheckLayersAndTags( other.gameObject )
			){
				despawner.lastCollisionPoint = other.transform.position;
				despawner.lastCollidedPhysicsGameObject = other.gameObject;
				despawner.Despawn(true);
			}
		}

	}
}
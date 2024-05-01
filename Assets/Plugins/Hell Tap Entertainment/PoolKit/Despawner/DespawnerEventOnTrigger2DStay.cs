//////////////////////////////////////////////////////////////////////////////////////////////////////////////////
//
//	DespawnerEventOnTrigger2DStay.cs
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
	public class DespawnerEventOnTrigger2DStay : DespawnerEvent {

		// ON TRIGGER STAY 2D
		void OnTriggerStay2D( Collider2D other ){
			
			// Make sure we can track collisions in this method ...
			if( despawner != null && despawner.despawnMode == Despawner.DespawnMode.AfterPhysicsTrigger2DEvent && despawner.useTriggerStay2D &&

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
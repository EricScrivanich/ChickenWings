﻿//////////////////////////////////////////////////////////////////////////////////////////////////////////////////
//
//	DespawnerEventOnCollisionStay.cs
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
	public class DespawnerEventOnCollisionStay : DespawnerEvent {

		// ON COLLISION STAY
		void OnCollisionStay( Collision collision ){
			
			// Make sure we can track collisions in this method ...
			if( despawner != null && despawner.despawnMode == Despawner.DespawnMode.AfterPhysicsCollisionEvent && despawner.useCollisionExit &&

				// Make sure the collision is valid ...
				despawner.CheckLayersAndTags( collision.gameObject )
			){
				despawner.lastCollisionPoint = collision.contacts[0].point;
				despawner.lastCollidedPhysicsGameObject = collision.gameObject;
				despawner.Despawn(true);
			}
		}

	}
}
//////////////////////////////////////////////////////////////////////////////////////////////////////////////////
//
//	DespawnerEventOnCollision2DExit.cs
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
	public class DespawnerEventOnCollision2DExit : DespawnerEvent {

		// Helpers
		ContactPoint2D[] _cachedContacts = new ContactPoint2D[1];	// <- we're only interested in the first contact (this fixes a weird Collision2D bug).

		// ON COLLISION EXIT 2D
		void OnCollisionExit2D( Collision2D collision ){

			// Cache the first contact and the number of collisions available
			/*int collisionContactCount =*/ collision.GetContacts( _cachedContacts );
			
			// Make sure we can track collisions in this method ...
			if( despawner != null && despawner.despawnMode == Despawner.DespawnMode.AfterPhysicsCollision2DEvent && despawner.useCollisionExit2D &&

				// Make sure the collision is valid ...
				despawner.CheckLayersAndTags( collision.gameObject )
			){
				despawner.lastCollisionPoint = _cachedContacts[0].point;
				despawner.lastCollidedPhysicsGameObject = collision.gameObject;
				despawner.Despawn(true);
			}
		}

	}
}
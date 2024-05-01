//////////////////////////////////////////////////////////////////////////////////////////////////////////////////
//
//	DespawnerEventRaycast3D.cs
//	Specific helper component for the Despawner
//
//	PoolKit For Unity, Created By Melli Georgiou
//	© 2019 Hell Tap Entertainment LTD
//
//////////////////////////////////////////////////////////////////////////////////////////////////////////////////

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HellTap.PoolKit {

	[DisallowMultipleComponent]
	public class DespawnerEventRaycast3D : DespawnerEvent {

		// HELPERS
		// The size of the array determines how many raycasts will occur
		internal RaycastHit[] _raycastHitResults = new RaycastHit[30];
		int _numberOfRaycastHits = 0;

		// UPDATE
		void Update(){
			
			// Make sure we can track collisions in this method ...
			if( despawner != null && despawner.despawnMode == Despawner.DespawnMode.AfterPhysicsRaycastEvent ){

				#if UNITY_EDITOR
					// If the user changes the raycast hit array length, update it in real-time, but only in the Editor.
					if( _raycastHitResults.Length != despawner.raycast3DmaxHits ){ 
						_raycastHitResults = new RaycastHit[despawner.raycast3DmaxHits];
					}
				#endif

				// Raycast from this object
				_numberOfRaycastHits = Physics.RaycastNonAlloc ( 
					
					transform.position, 						// Position of the raycast
					despawner.raycast3DDirection.normalized,	// Direction of the raycast (was transform.forward)
					_raycastHitResults, 						// Size of raycast array
					despawner.raycast3DDistance, 				// Length of raycast (was Mathf.Infinity)
					despawner.filterLayers,						// Layermask
					despawner.queryTriggerInteraction			// QueryTriggerInteraction.UseGlobal
				);
				

				// Loop through the objects we hit and find the first validly hit object
				for( int i = 0; i < _numberOfRaycastHits; i++ ){
					
					// Make sure the hit object has a collider (to access the gameObject) and that it matches our layers and tags 
					if (	_raycastHitResults[i].collider != null &&
							despawner.CheckLayersAndTags( _raycastHitResults[i].collider.gameObject )
					){
		
						// Setup the despawner values and despawn now
						despawner.lastCollisionPoint = _raycastHitResults[i].point;
						despawner.lastCollidedPhysicsGameObject = _raycastHitResults[i].collider.gameObject;
						despawner.Despawn(true);

						//#if UNITY_EDITOR
						//	Debug.DrawLine(transform.position, _raycastHitResults[i].point, Color.green, 0.5f, true);
						//#endif

						// End now
						return;
					}
				}
				
			}
		}
	}
}
//////////////////////////////////////////////////////////////////////////////////////////////////////////////////
//
//	DespawnerEventOnPhysicsOverlap.cs
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
	public class DespawnerEventOnPhysicsOverlap : DespawnerEvent {

		//public Despawner despawner = null;						// We need to set the Despawner here!
		Transform m_Transform;									// Cached Transform
		Collider[] _resultsBuffer = new Collider[100];			// This sets up the collider buffer
		Collider2D[] _resultsBuffer2D = new Collider2D[100];	// This sets up the collider buffer
		int m_HitDetect = 0;									// This tells us how many collisions there were
		bool using2D = false;									// Helper that checks whether we should 2D or 3D results
		Vector3 liveScale = Vector3.one;						// We scale the colliders based on the current 

		// Cache on awake
		void Awake () {
			m_Transform = transform;
		}
		
		// Update is called once per frame
		void FixedUpdate () {

			// ===============================
			//	PHYSICS DETECTION - SPHERE 3D
			// ===============================
			
			// Overlap Sphere3D
			if( despawner.overlapType == Despawner.OverlapType.Sphere3D ){

				// Calculate live scale for Sphere's based on the largest of the 3 scale axis
				liveScale.x = Mathf.Max( Mathf.Max(m_Transform.lossyScale.x, m_Transform.lossyScale.y), m_Transform.lossyScale.z ) * despawner.overlapRadius;
				liveScale.y = m_Transform.lossyScale.y * despawner.overlapRadius;
				liveScale.z = m_Transform.lossyScale.z * despawner.overlapRadius;

				// Check Collisions
				m_HitDetect = Physics.OverlapSphereNonAlloc( m_Transform.position +  m_Transform.TransformDirection( despawner.overlapOffset ), liveScale.x, _resultsBuffer, despawner.filterLayers, despawner.queryTriggerInteraction );
				using2D = false;

			// ===============================
			//	PHYSICS DETECTION - CIRCLE 2D
			// ===============================

			// Overlap Circle 2D
			} else if( despawner.overlapType == Despawner.OverlapType.Circle2D ){

				// Calculate live scale for Sphere's based on the largest of the 3 scale axis
				liveScale.x = Mathf.Max( Mathf.Max(m_Transform.lossyScale.x, m_Transform.lossyScale.y), m_Transform.lossyScale.z ) * despawner.overlapRadius;
				liveScale.y = m_Transform.lossyScale.y * despawner.overlapRadius;
				liveScale.z = m_Transform.lossyScale.z * despawner.overlapRadius;

				// Check Collisions
				m_HitDetect = Physics2D.OverlapCircleNonAlloc( m_Transform.position +  m_Transform.TransformDirection( despawner.overlapOffset ), liveScale.x, _resultsBuffer2D, despawner.filterLayers );
				using2D = true;

			// ===============================
			//	PHYSICS DETECTION - BOX 3D
			// ===============================

			// Overlap Box 3D
			} else if( despawner.overlapType == Despawner.OverlapType.Box3D ){

				// Setup the scale of the box by using multiplying the scale of the transform with the user settings
				liveScale.x = m_Transform.lossyScale.x * despawner.overlapScale.x;
				liveScale.y = m_Transform.lossyScale.x * despawner.overlapScale.y;
				liveScale.z = m_Transform.lossyScale.x * despawner.overlapScale.z;

				// Check Collisions
				m_HitDetect = Physics.OverlapBoxNonAlloc( m_Transform.position + m_Transform.TransformDirection( despawner.overlapOffset ), liveScale, _resultsBuffer, m_Transform.rotation, despawner.filterLayers, despawner.queryTriggerInteraction );
				using2D = false;
			
			// ===============================
			//	PHYSICS DETECTION - BOX 2D
			// ===============================

			// Overlap Box 2D
			} else if( despawner.overlapType == Despawner.OverlapType.Box2D ){

				// Setup the scale of the box by using multiplying the scale of the transform with the user settings
				liveScale.x = m_Transform.lossyScale.x * despawner.overlapScale.x;
				liveScale.y = m_Transform.lossyScale.x * despawner.overlapScale.y;
				liveScale.z = m_Transform.lossyScale.x * despawner.overlapScale.z; // <- this should get ignored in 2D

				// Check Collisions
				m_HitDetect = Physics2D.OverlapBoxNonAlloc( m_Transform.position + m_Transform.TransformDirection( despawner.overlapOffset ), liveScale, m_Transform.eulerAngles.z, _resultsBuffer2D, despawner.filterLayers );
				using2D = true;
			
			}

			// ====================
			//	FILTER OBJECTS ...
			// ====================

			// If any collisions were detected, loop through the colliders found...
			if (m_HitDetect > 0 ){

				// 3D Colliders ....
				if( using2D == false ){

					// Loop through the results ...
					for( int i = 0; i < m_HitDetect; i++ ){
						if( _resultsBuffer[i].transform != m_Transform &&				// <- Make sure the collider isn't itself
							despawner.CheckLayersAndTags( _resultsBuffer[i].gameObject )
						){
							// If we've found a valid object to collide with, despawn now and stop the loop!
							// We should use the transform for last collision point as we don't have collision data
							despawner.lastCollisionPoint = m_Transform.position;
							despawner.lastCollidedPhysicsGameObject = _resultsBuffer[i].gameObject;
							despawner.Despawn(true);
							return;
						}
					}

				// 2D Colliders
				} else if( using2D == true ){

					// Loop through the results ...
					for( int i = 0; i < m_HitDetect; i++ ){
						if( _resultsBuffer2D[i].transform != m_Transform &&				// <- Make sure the collider isn't itself
							despawner.CheckLayersAndTags( _resultsBuffer2D[i].gameObject )
						){
							// If we've found a valid object to collide with, despawn now and stop the loop!
							// We should use the transform for last collision point as we don't have collision data
							despawner.lastCollisionPoint = m_Transform.position;
							despawner.lastCollidedPhysicsGameObject = _resultsBuffer2D[i].gameObject;
							despawner.Despawn(true);
							return;
						}
					}

				} 
			}

		}		
	}
}

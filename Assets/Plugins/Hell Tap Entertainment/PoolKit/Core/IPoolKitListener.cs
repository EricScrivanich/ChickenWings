//////////////////////////////////////////////////////////////////////////////////////////////////////////////////
//
//	IPoolKitListener.cs
//	This is the interface that allows Monobehaviours to recieve OnSpawn and OnDespawn messages.
//
//	PoolKit For Unity, Created By Melli Georgiou
//	© 2018 Hell Tap Entertainment LTD
//
//////////////////////////////////////////////////////////////////////////////////////////////////////////////////

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HellTap.PoolKit {
	public interface IPoolKitListener {

		// Methods
		void OnSpawn( Pool pool );		// <- you only recieve the pool when you spawn
		void OnDespawn();
		// void OnSpawnUpdate();				// <- this doesn't seem to have a net benefit so might as well take it out
	}
}
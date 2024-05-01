using UnityEngine;
using System.Collections;
using System;
 
namespace HellTap.PoolKit {

	public static class Epoch {
	
		// Get The current Time
		public static int Current(){

			DateTime epochStart = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
			int currentEpochTime = (int)(DateTime.UtcNow - epochStart).TotalSeconds;
			return currentEpochTime;
		}
	
		// Seconds Elapsed Since the current time and the argument time
		public static int SecondsElapsed(int t1){ return Mathf.Abs( Current() - t1 ); }
	
		// Seconds Elapsed Between two time arguments
		public static int SecondsElapsed(int t1, int t2){ return Mathf.Abs( t1 - t2 ); }
	
	}
}
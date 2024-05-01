//////////////////////////////////////////////////////////////////////////////////////////////////////////////////
//
//	Arrays.cs
//	This is a helper class offering generic functionality to builtin Arrays.
//
//	PoolKit For Unity, Created By Melli Georgiou
//	© 2018 Hell Tap Entertainment LTD
//
//////////////////////////////////////////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Using the HellTap.PoolKit namespace
namespace HellTap.PoolKit {
	public static class Arrays {

		// ==================================================================================================================
		//	ADD ITEM
		//	Adds another item to an Array[]
		// ==================================================================================================================

		public static bool AddItem<T>(ref T[] _arr, T item ){

			// Make sure the array is valid
			if( _arr != null ){

				Array.Resize(ref _arr, _arr.Length + 1);
				_arr[_arr.Length - 1] = item;
				return true;

			// Show an error message if no array was sent
			} else { Debug.LogWarning("ARRAYS - AddItem(): Array cannot be null. Skipping."); }	

			// Return false if we couldn't add the item
			return false;
		}

		// Same as the above version but without any checks
		public static void AddItemFastest<T>( ref T[] _arr, T item ){
			Array.Resize(ref _arr, _arr.Length + 1);
			_arr[_arr.Length - 1] = item;
		}

		// ==================================================================================================================
		//	ADD ITEM IF NOT PRESENT
		//	Adds another item to an Array[] as long as it doesn't already exist
		// ==================================================================================================================

		public static bool AddItemIfNotPresent<T>(ref T[] _arr, T item){

			// Make sure the array is valid
			if( _arr != null ){

				// Make sure the item doesn't already exist in the array
				if( ItemExistsAtIndex( ref _arr, ref item ) == -1 ){
					Array.Resize(ref _arr, _arr.Length + 1);
					_arr[_arr.Length - 1] = item;
					return true;
				}

			// Show an error message if no array was sent
			} else if( _arr == null ){ Debug.LogWarning("ARRAYS - AddItemIfNotPresent(): Array cannot be null. Skipping."); }

			// Return false if we couldn't add the item
			return false;	
		}

		// ==================================================================================================================
		//	REMOVE ITEM
		//	Removes a single or all items from an Array[]
		//	To only remove the first found instance of an item, set onlyRemoveFirstInstance to TRUE.
		// ==================================================================================================================

		// Remove Item from Array
		public static bool RemoveItem<T>(ref T[] _arr, ref T item, bool onlyRemoveFirstInstance = false ){
			
			// Make sure the array is valid
			if( _arr != null ){

				// Track if we removed any items
				bool anItemWasRemoved = false;

				// While the array still has that item in one of its slots ...
				while( Arrays.ItemExistsAtIndex( ref _arr, ref item) != -1 ){

					// Cache the Index of the item
					int id = Arrays.ItemExistsAtIndex( ref _arr, ref item);

					// Create a new array 1 item less than the current size
					T[] newArray = new T[ _arr.Length-1 ];

					// Loop through the old array and copy every item except the cached ID
					for( int i = 0; i<newArray.Length; i++ ){
						if(i<id){
							newArray[i] = _arr[i];
						} else if( _arr.Length > 1 ){
							newArray[i] = _arr[i+1];
						}
					}

					// Update the main array
					_arr = newArray;

					// Track that an item was removed
					anItemWasRemoved = true;

					// If we should only remove the first instance, end it after the first item is removed ...
					if( onlyRemoveFirstInstance ){ return true; }
				}

				// If we did remove an item, return true.
				if( anItemWasRemoved ){ return true; }

			// Show an error message if no array was sent
			} else { Debug.LogWarning("ARRAYS - RemoveItem(): Array cannot be null. Skipping."); }

			// Return false if we couldn't remove the item
			return false;
		}

		// ==================================================================================================================
		//	REMOVE FIRST ITEM
		//	Removes an item at a given index
		// ==================================================================================================================

		public static bool RemoveFirstItem<T>( ref T[] _arr ){

			// Make sure the array is valid
		//	if( _arr != null && _arr.Length > 1 ){

				// Make a new array 1 less than the current one
				T[] newArray = new T[_arr.Length-1];

				// Loop through the new array, and copy every item with a +1 offset
				for( int i = 0; i<newArray.Length; i++ ){ newArray[i] = _arr[i+1]; }

				// Update the main array
				_arr = newArray;

				// Item was removed successfully
				return true;

			// Show an error message if no array was sent
		//	} else if( _arr == null ){ Debug.LogWarning("ARRAYS - RemoveFirstItem(): Array cannot be null or must have at least 1 item. Skipping."); }

			// Return false if we couldn't remove the item
		//	return false;	
		}


		// ==================================================================================================================
		//	REMOVE ITEM AT INDEX
		//	Removes an item at a given index
		// ==================================================================================================================

		public static bool RemoveItemAtIndex<T>(ref T[] _arr, int index ){

			// Make sure the array is valid
			if( _arr != null ){

				// Make sure the index is valid
				if( index >= 0 && index < _arr.Length ){

					// Make a new array 1 less than the current one
					T[] newArray = new T[_arr.Length-1];

					// Loop through the old array and copy every item except the index
					for( int i = 0; i<newArray.Length; i++ ){
						if(i<index){
							newArray[i] = _arr[i];
						} else if( _arr.Length > 1 ){
							newArray[i] = _arr[i+1];
						}
					}

					// Update the main array
					_arr = newArray;

					// Item was removed successfully
					return true;

				// Show an error message if the index is out of range
				} else { Debug.LogWarning("ARRAYS - RemoveItemAtIndex(): Index is out of range. Skipping."); }

			// Show an error message if no array was sent
			} else if( _arr == null ){ Debug.LogWarning("ARRAYS - RemoveItemAtIndex(): Array cannot be null. Skipping."); }

			// Return false if we couldn't remove the item
			return false;	
		}

		// ==================================================================================================================
		//	ITEM EXISTS AT INDEX
		//	If an item exists in the array, it will return its ID. if not, it returns -1.
		// ==================================================================================================================
		
		public static int ItemExistsAtIndex<T>( ref T[] _arr, ref T item ){

			// Loop through the index
			if( _arr != null ){
				for( int i = 0; i<_arr.Length; i++ ){
					if( _arr[i].Equals( item ) ){
						return i;
					}
				}

			// Show an error message if no array was sent	
			} else { Debug.LogWarning("ARRAYS - ItemExistsAtIndex(): Array cannot be null. Returning -1."); }

			// If we didn't find anything, return -1
			return -1;
		}

		// ==================================================================================================================
		//	ITEM EXISTS
		//	Simplified bool version of the above, if an item exists true is returned
		// ==================================================================================================================
		
		public static bool ItemExists<T>( T[] _arr, T item ){

			// Loop through the index
			if( _arr != null ){
				for( int i = 0; i<_arr.Length; i++ ){
					if( _arr[i].Equals( item ) ){
						return true;
					}
				}

			// Show an error message if no array was sent	
			} else { Debug.LogWarning("ARRAYS - ItemExistsAtIndex(): Array cannot be null. Returning false."); }

			// If we didn't find anything, return false
			return false;
		}

		// ==================================================================================================================
		//	CONCAT
		//	Combines two of the same types of Array[] together (untested so far)
		// ==================================================================================================================

		public static T[] Concat<T>(this T[] a, T[] b){
			if (a == null) throw new ArgumentNullException("x");
			if (b == null) throw new ArgumentNullException("b");
			int oldLen = a.Length;
			Array.Resize<T>(ref a, a.Length + b.Length);
			Array.Copy(b, 0, a, oldLen, b.Length);
			return a;
		}

		// ==================================================================================================================
		//	COMBINE
		//	Combines two of the same types of Array[] together (untested so far)
		// ==================================================================================================================
		
		public static T[] Combine<T>( T[] a, T[] b ){
			T[] newArray = new T[ a.Length + b.Length ];
			a.CopyTo( newArray, 0 );
			b.CopyTo( newArray, a.Length );
			return newArray;
		}

		// ==================================================================================================================
		//	CLEAR
		//	Removes all entries in an array
		// ==================================================================================================================
		
		public static bool Clear<T>( ref T[] arr ){
			if( arr != null ){
				arr = new T[0];
				return true;
			} else {
				Debug.LogWarning("ARRAYS - Cannot clear an array that is null. Returning null.");
				return false;
			}
		}

		// ==================================================================================================================
		//	SHIFT ITEM
		//	Shifts an item up or down in an array (this method is a bit slow and is designed for use with the inspector)
		// ==================================================================================================================

		public static bool Shift<T>( ref T[] _arr, int id, bool moveUp ){

			// Make sure the event ID we're moving is less than the total amount of _arr events, and ...
			if( _arr!=null && id < _arr.Length /*&& _arr[id] != null*/ &&
				(
					moveUp == true && id > 0 ||					// If we're moving up, this cant be the first ID.
					moveUp == false && id < _arr.Length-1 		// If we're moving down, this can't be the last event ID.
				)
			){

				// Cache the event we're about to move.
				T eventToMove = _arr[id];

				// Backup the old index into an ArrayList and remove the event we're about to move.
				var oldarr = new ArrayList( _arr );
				oldarr.RemoveAt(id);

				// Create a new ArrayList from the _arr events.
				var newArr = new ArrayList();
				newArr.Clear();

				// Loop through the _arr events so we can recreate a new array.
				for( int i = 0; i<oldarr.Count; i++ ){

					// If this the event we're moving ( Moving Up)
					if( i == id-1 && moveUp ){
						newArr.Add( eventToMove );
					}

					// Add old element to this array
					newArr.Add( oldarr[i] );

					// If this the event we're moving ( Moving Down)
					if( i == id && !moveUp ){
						newArr.Add( eventToMove );
					}
				}

				// Convert this array into a builtin list.
				T[] builtinArray = newArr.ToArray( typeof( T ) ) as T[];
				_arr = builtinArray;

				// Return true as the process succeeded!
				return true;
			}

			// Return false if anything goes wrong
			return false;
		}

	}
}
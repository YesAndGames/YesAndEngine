using System;
using UnityEngine;

namespace YesAndEngine.DataStructures {

	// A collection of weighted objects.
	[Serializable]
	public class WeightedCollection<T> : WeightedObject where T : WeightedObject {

		// The array of weighted objects.
		public T[] objects = new T[0];

		// Construct a new WeightedCollection object with no data.
		public WeightedCollection () { }

		// Construct a new WeightedCollection object.
		public WeightedCollection (T[] objects, int weight = 1) : base (weight) {
			this.objects = objects;
		}

		// Calculates the total weight in this collection.
		public int GetTotalWeight () {
			int totalWeight = 0;
			for (int i = 0; i < objects.Length; i++) {
				totalWeight += objects[i].weight;
			}
			return totalWeight;
		}
	}
}

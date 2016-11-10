using UnityEngine;
using System;

namespace YesAndEngine.DataStructures {

	// An object with a "weight", used for chanced pooling.
	[Serializable]
	public class WeightedObject {

		// Integer weight, used to pick by weighted chance.
		public int weight;

		// Default constructor for a new WeightedObject object.
		protected WeightedObject () {
			weight = 1;
		}

		// Construct a new WeightedObject object.
		protected WeightedObject (int weight) {
			this.weight = weight;
		}
	}
}

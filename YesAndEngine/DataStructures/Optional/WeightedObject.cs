using UnityEngine;
using System;

namespace YesAndEngine.DataStructures {

	// An object with a "weight", used for chanced pooling.
	[Serializable]
	public class WeightedObject {

		// Integer weight, used to pick by weighted chance.
		public int weight;

		// Construct a new WeightedObject object.
		protected WeightedObject (int weight = 1) {
			this.weight = weight;
		}
	}
}

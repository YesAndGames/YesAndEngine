using UnityEngine;
using System;
using System.Collections;

namespace YesAndEditor.Exporting {

	// An exportable resource interface.
	public interface IExportableResource<T> {

		// Serialize the data into exportable format.
		IExportableResource<T> Serialize (T data);
	}
}
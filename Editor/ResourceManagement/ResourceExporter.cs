using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace YesAndEditor.Exporting {

	// A resource exporter facilitates exporting C# data structures to markup output.
	public class ResourceExporter<T> {

		// Attribute overrides.
		private readonly XmlAttributeOverrides overrides;

		// Data to be serialized and exported.
		private readonly T data;

		// Constructor for a resource exporter.
		public ResourceExporter (T data, XmlAttributeOverrides overrides = null) {
			this.data = data;
			this.overrides = overrides == null ? new XmlAttributeOverrides () : overrides;
		}

		// Export to the specified path.
		public void Export (string path) {

			// Serialize the data.
			MemoryStream memoryStream = new MemoryStream ();
			XmlSerializer xs = new XmlSerializer (typeof (T), overrides);
			XmlTextWriter xmlTextWriter = new XmlTextWriter (memoryStream, Encoding.UTF8);
			xs.Serialize (xmlTextWriter, data);

			// Build the string output from the stream.
			memoryStream = (MemoryStream)xmlTextWriter.BaseStream;
			UTF8Encoding encoding = new UTF8Encoding ();
			string output = encoding.GetString (memoryStream.ToArray ());

			// Make sure the file exists.
			if (!File.Exists (path)) {
				Directory.CreateDirectory (Path.GetDirectoryName (path));
				File.Create (path);
			}

			// Write the string output to a file at the specified path.
			FileStream fs = new FileStream (path, FileMode.OpenOrCreate);
			StreamWriter writer = new StreamWriter (fs);
			writer.Write (output);
			writer.Close ();
			fs.Close ();
		}
	}
}

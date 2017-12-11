using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Xml.Linq;
using System.Linq;
using System;
using System.Diagnostics;

namespace System.Resources.Models
{
	public class ResxResource : ResourceBase
	{
		static readonly ITracer tracer = Tracer.Get<ResxResource> ();
		public List<StringTitleDefinition> Data { get; private set; }

		public XDocument Document { get; private set; }

		public ResxResource () : base ("Translations.resx.template")
		{
			Document = XDocument.Parse (resourceData);
			Data = new List<StringTitleDefinition> ();
		}

		public void Load (IEnumerable<StringTitleDefinition> content)
		{
			Data.Clear ();
			foreach (var item in content)
				Data.Add (item);
			Refresh ();
		}

		public void Refresh ()
		{
			var toRemoveElements = Document.Root.Elements ("data")
			                              .ToList ();
			
			foreach (var item in toRemoveElements) {
				item.Remove ();
			}

			foreach (var element in Data) {
				Document.Root.Add (element.ToXElement ());
			}
		}

		public void AddMissingDefinitions (List<StringTitleDefinition> data, bool removeDifferences = false)
		{
			var toAdd = new List<StringTitleDefinition> ();

			if (removeDifferences) {
				var toRemove = Data.Where (p => !data.Any (p2 => p2.ObjectID == p.ObjectID))
								   .ToList ();
				foreach (var item in toRemove)
					Data.Remove (item);
			}

			foreach (var definition in data) {
				if (!Data.Exists (s => definition.ObjectID == s.ObjectID)) {
					toAdd.Add (definition);
				};
			}
			Data.AddRange (toAdd);

			Refresh ();
		}

		public void Load (XDocument document)
		{
			Data.Clear ();
			Document = document;
			var root = Document.Descendants ("root").FirstOrDefault ();

			StringTitleDefinition stringDefinition;
			foreach (var data in root.Elements ("data")) {
				var objectId = data.Attribute ("name").Value;
				stringDefinition = new StringTitleDefinition (objectId);
				stringDefinition.Title = data.Descendants ()
								 .FirstOrDefault ()
								 .Value;
				Data.Add (stringDefinition);
			}
		}

		public void Load (string fileName)
		{
			Load (XDocument.Load (fileName));
		}

		/// <summary>
		/// Save all procecesses definitions of this instance in the target destination.
		/// </summary>
		/// <returns>The save.</returns>
		/// <param name="fileName">Destination file.</param>

		public override void Save (string filePath, bool forceOverride = false)
		{
			if (File.Exists (filePath)) {
				if (forceOverride)
					File.Delete (filePath);
				else
					throw new Exception ("there is a file with the same name");
			}
			Document.Save (filePath);
		}
	}
}

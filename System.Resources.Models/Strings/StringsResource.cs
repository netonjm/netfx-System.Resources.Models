using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace System.Resources.Models
{
	public class StringsResource : ResourceBase
	{
		readonly Dictionary<string, StringTitleDefinition> definitions;
		public Dictionary<string, StringTitleDefinition> Definitions => definitions;

		public StringsResource ()
		{
			definitions = new Dictionary<string, StringTitleDefinition> ();
		}

		/// <summary>
		/// Reset and procecesses all possible definitions of this instance.
		/// </summary>
		public void Reset ()
		{
			definitions.Clear ();

			var lines = resourceData.Split (new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries)
							 .Select (s => s.Trim ());
			foreach (var line in lines) {
				if (line.StartsWith ("/*", StringComparison.Ordinal) && line.EndsWith ("*/", StringComparison.Ordinal)) {
					ProcessDataCommentedLine (line);
				}
				else if (line.StartsWith ("\"", StringComparison.Ordinal)) {
					ProcessDataLine (line);
				}
			}
		}

		/// <summary>
		/// Save all procecesses definitions of this instance in the target destination.
		/// </summary>
		/// <returns>The save.</returns>
		/// <param name="fileName">Destination file.</param>
		public override void Save (string filePath, bool forceOverride = false)
		{

			StringBuilder builder = new StringBuilder ();
			int line = 1;
			foreach (var item in definitions) {
				if (line > 1) {
					builder.AppendLine ();
				}
				builder.AppendLine (item.Value.ToStringDefinition ());
				line++;
			}

			File.WriteAllText (filePath, builder.ToString ());
		}

		/// <summary>
		/// Processes the data line with this format: "3IN-sU-3Bg.title" = "Spelling";
		/// </summary>
		/// <param name="line">Line.</param>
		void ProcessDataLine (string line)
		{
			try {
				var propertyData = line.Split (new char[] { '=' }, StringSplitOptions.RemoveEmptyEntries);
				var title = propertyData[0].Trim () //remove empty spaces
										   .Trim ('"'); //remove quotes

				var objectId = title.Split (new char[] { '.' }, StringSplitOptions.RemoveEmptyEntries)[0];
				var definition = GetOrCreate (objectId);

				var value = propertyData[1].Trim () //remove empty spaces
										   .Trim (';') //remove semicolon character
										   .Trim ('"'); //remove quotes

				definition.Title = value;
			}
			catch (Exception ex) {
				Console.WriteLine (ex);
			}
		}

		/// <summary>
		/// Processes the data commented line with this format: /* Class = "NSMenuItem"; title = "Transformations"; ObjectID = "2oI-Rn-ZJC"; */
		/// </summary>
		/// <param name="line">Line.</param>
		void ProcessDataCommentedLine (string line)
		{
			var dataCommented = GetDataCommented (line);
			//we get the object id
			string objectId;
			if (!dataCommented.TryGetValue ("ObjectID", out objectId))
				return;

			var definition = GetOrCreate (objectId);
			foreach (var data in dataCommented) {
				switch (data.Key) {
					case "Class":
						definition.Class = data.Value.Trim ('"'); ;
						break;
					case "title":
						definition.Comment = data.Value.Trim ('"');
						break;
				}
			}
		}

		StringTitleDefinition GetOrCreate (string objectId)
		{
			//ask if exists in our definitions
			StringTitleDefinition definition;
			if (!definitions.TryGetValue (objectId, out definition)) {
				definition = new StringTitleDefinition (objectId);
				definitions.Add (objectId, definition);
			}
			return definition;
		}

		public void Merge (List<StringTitleDefinition> data, bool decoding)
		{
			foreach (var item in data) {
				try {
					var definition = definitions.FirstOrDefault (s => s.Value.ResxID == item.ResxID);
					definition.Value.Title = decoding ? item.GetDecodedTitle () : item.Title;
				}
				catch (Exception ex) {
					Console.WriteLine (ex);
				}
			}
		}

		public void AddMissingDefinitions (List<StringTitleDefinition> data, bool removeDifferences = false)
		{
			var toAdd = new List<StringTitleDefinition> ();

			if (removeDifferences) {
				foreach (var item in Definitions.Values.Except (data).ToList ())
					Definitions.Remove (item.ObjectID);
			}

			foreach (var definition in data) {
				StringTitleDefinition def;
				if (!Definitions.TryGetValue (definition.ObjectID, out def)) {
					toAdd.Add (definition);
				};
			}
			foreach (var item in toAdd) {
				Definitions.Add (item.ObjectID, item);
			}
		}

		Dictionary<string, string> GetDataCommented (string comment)
		{
			comment = comment.Substring (2, comment.Length - 5);
			var data = new Dictionary<string, string> ();
			var properties = comment.Split (new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
			foreach (var property in properties) {
				try {
					var propertyData = property.Split ('=');
					var name = propertyData[0].Trim ();
					var value = propertyData[1].Trim ().Trim ('"');
					data.Add (name, value);
				}
				catch (Exception ex) {
					Console.WriteLine (ex);
				}
			}
			return data;
		}

		public void FromResourceId (string resource, Assembly assembly)
		{
			if (assembly == null)
				throw new TargetException ("assembly refered is null");
			var data = TemplateHelper.GetManifestResource (assembly, resource);
			FromData (data);
		}

		public void FromData (string data)
		{
			resourceData = data;
			Reset ();
		}

		public void Load (IEnumerable<StringTitleDefinition> model)
		{
			definitions.Clear ();
			foreach (var item in model) {
				definitions.Add (item.ObjectID, item);
			}
		}

		public void Load (string fileName)
		{
			//Read saved file and reload manager
			FromData (File.ReadAllText (fileName));
		}
	}
}

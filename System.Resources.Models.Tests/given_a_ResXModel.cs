using System.Collections.Generic;
using NUnit.Framework;
using System.Linq;
using System.IO;

namespace System.Resources.Models.Tests
{
	[TestFixture ()]
	public class given_a_ResXModel : BaseTest
	{
		readonly ResxResource model;

		public given_a_ResXModel ()
		{
			model = new ResxResource ();
		}

		[Test ()]
		public void when_model_loads_then_schema_persists ()
		{
			model.Load (Resources.DefaultResourcesResX);

			var count = model.Data.Count ();

			var definitions = new List<StringTitleDefinition> {
				new StringTitleDefinition("1") { Class = "Class1", Comment = "Comment1", Title = "Title1" },
				new StringTitleDefinition("2") { Class = "Class2", Comment = "Comment2", Title = "Title2" },
				new StringTitleDefinition("4") { Class = "Class5", Comment = "Comment5", Title = "Title5" }
			};

			model.AddMissingDefinitions (definitions);

			Assert.AreEqual ((count + definitions.Count), model.Data.Count, "AddMissingDefinitions is not adding correctly new elements");

			var tmpFile = Path.Combine (Resources.TmpDirectory, "tmpFile.resx");
			if (File.Exists (tmpFile)) {
				File.Delete (tmpFile);
			}
			model.Save (tmpFile);
			model.Load (tmpFile);
			if (File.Exists (tmpFile)) {
				File.Delete (tmpFile);
			}
			Assert.IsTrue (model.Document.Root.Elements ().Any (s => s.Name.LocalName == "schema"), "#1 Schema is not in the current temporal file");
			Assert.AreEqual ((count + definitions.Count), model.Data.Count, "We are not storing correcly the elements.");
		}

		[Ignore ("")]
		[Test ()]
		public void when_model_loads_then_result_contains_definition ()
		{
			var content = new StringTitleDefinition ("id") {
				Class = "Class",
				Comment = "Comment",
				Title = "Title"
			};
			model.Load (new List<StringTitleDefinition> { content });
			var definition = content.ToResXDefinition (false).Replace ("\t", "");
			var result = model.Result.Replace ("\t", "");
			Assert.IsTrue (result.Contains (definition), "#1");
		}

		[Test ()]
		public void when_definitions_present_then_no_dupplicated_elements ()
		{
			model.Load (new List<StringTitleDefinition> {
				new StringTitleDefinition("1") { Class = "Class1", Comment = "Comment1", Title = "Title1" },
				new StringTitleDefinition("2") { Class = "Class2", Comment = "Comment2", Title = "Title2" },
				new StringTitleDefinition("4") { Class = "Class5", Comment = "Comment5", Title = "Title5" }
			});
			var initialDefinitions = model.Data.ToList ();

			var definitions = new List<StringTitleDefinition> {
				new StringTitleDefinition("1") { Class = "Class1", Comment = "Comment1", Title = "Title1" },
				new StringTitleDefinition("2") { Class = "Class2", Comment = "Comment2", Title = "Title2" },
			};

			model.Load (definitions);
			model.AddMissingDefinitions (initialDefinitions, false);
			Assert.AreEqual (3, model.Data.Count, "#1");
			Assert.IsTrue (model.Data.Any (s => s.ObjectID == "1"), "#2");
			Assert.IsTrue (model.Data.Any (s => s.ObjectID == "2"), "#3");
			Assert.IsTrue (model.Data.Any (s => s.ObjectID == "4"), "#4");
		}

		[Test ()]
		public void when_definitions_present_then_remove_differences ()
		{
			model.Load (new List<StringTitleDefinition> {
				new StringTitleDefinition("1") { Class = "Class1", Comment = "Comment1", Title = "Title1" },
				new StringTitleDefinition("4") { Class = "Class5", Comment = "Comment5", Title = "Title5" }
			});
			var initialDefinitions = model.Data.ToList ();

			var definitions = new List<StringTitleDefinition> {
				new StringTitleDefinition("1") { Class = "Class1", Comment = "Comment1", Title = "Title1" },
				new StringTitleDefinition("2") { Class = "Class2", Comment = "Comment2", Title = "Title2" },
				new StringTitleDefinition("6") { Class = "Class6", Comment = "Comment6", Title = "Title6" },
				new StringTitleDefinition("7") { Class = "Class7", Comment = "Comment6", Title = "Title7" }
			};

			model.Load (definitions);
			model.AddMissingDefinitions (initialDefinitions, true);
			Assert.AreEqual (2, model.Data.Count, "#1");
			Assert.IsTrue (model.Data.Any (s => s.ObjectID == "1"), "#2");
			Assert.IsTrue (model.Data.Any (s => s.ObjectID == "4"), "#3");
		}

		[Test ()]
		public void when_load_resx_then_fill_data ()
		{
			model.Load (Resources.DefaultResourcesResX);
			Assert.IsTrue (model.Data.Count > 0, "#1");
		}
	}
}

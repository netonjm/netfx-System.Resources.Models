using NUnit.Framework;
using System.Reflection;
using System.Collections.Generic;
using System;
using System.IO;
using System.Linq;

namespace System.Resources.Models.Tests
{
	[TestFixture ()]
	public class given_a_StringsModel
	{
		readonly string currentOutputDirectory;
		readonly StringsResource manager;
		readonly Assembly executingAssembly;

		public given_a_StringsModel ()
		{
			executingAssembly = Assembly.GetExecutingAssembly ();
			manager = new StringsResource ();
			currentOutputDirectory = Path.GetDirectoryName (executingAssembly.Location);
		}

		[Test ()]
		public void when_definitions_present_then_can_add_elements ()
		{
			var strings = new List<Tuple<string, string, string, string>> {
				new Tuple<string, string, string, string>("1UK-8n-QPP", "NSMenuItem", "Customize Toolbar…", "Customize Toolbar…2"),
				new Tuple<string, string, string, string>("1Xt-HY-uBw", "NSMenuItem", "LocalizationApp", "LocalizationApp2"),
				new Tuple<string, string, string, string>("1b7-l0-nxx", "NSMenu", "Find", "Find2")
			};

			manager.FromResourceId ("Main.strings", executingAssembly);

			var definitions = manager.Definitions;

			Assert.AreEqual (strings.Count, definitions.Count, "#1");

			foreach (var item in strings) {
				StringTitleDefinition definition;
				Assert.True (definitions.TryGetValue (item.Item1, out definition), "#{item.Item1}-1");

				Assert.AreEqual (item.Item2, definition.Class, $"#{item.Item2}-1");
				Assert.AreEqual (item.Item3, definition.Comment, $"#{item.Item3}-2");
				Assert.AreEqual (item.Item4, definition.Title, $"#{item.Item4}-3");
			}
		}

		[Test ()]
		public void when_definitions_present_then_no_dupplicated_elements ()
		{

			string randomSnippet = @"/* Class = ""NSMenu""; title = ""Random1""; ObjectID = ""2Xt-HY-uBw""; */
""2Xt-HY-uBw.title"" = ""Random2"";

/* Class = ""NSMenu""; title = ""Found""; ObjectID = ""1b7-l0-RR""; */
""1b7-l0-RR.title"" = ""Found4"";";

			manager.FromData (randomSnippet);

			var initialDefinitions = manager.Definitions.Values.ToList ();

			randomSnippet = @"/* Class = ""NSMenu""; title = ""Random1""; ObjectID = ""3Xt-HY-uBw""; */
""3Xt-HY-uBw"" = ""Random3"";

/* Class = ""NSMenu""; title = ""Found""; ObjectID = ""1b7-l0-RR""; */
""1b7-l0-RR.title"" = ""Found4"";

/* Class = ""NSMenu5""; title = ""Found""; ObjectID = ""1b7-l0-R2""; */
""1b7-l0-R2.title"" = ""Found5"";";

			manager.FromData (randomSnippet);
			manager.AddMissingDefinitions (initialDefinitions, false);

			Assert.AreEqual (4, manager.Definitions.Count, "#1");
			StringTitleDefinition dev;
			Assert.IsTrue (manager.Definitions.TryGetValue ("2Xt-HY-uBw", out dev), "#2");
			Assert.IsTrue (manager.Definitions.TryGetValue ("1b7-l0-RR", out dev), "#3");
			Assert.IsTrue (manager.Definitions.TryGetValue ("3Xt-HY-uBw", out dev), "#4");
			Assert.IsTrue (manager.Definitions.TryGetValue ("1b7-l0-R2", out dev), "#5");
		}

		[Test ()]
		public void when_definitions_present_then_remove_differences ()
		{

			string randomSnippet = @"/* Class = ""NSMenu""; title = ""Random1""; ObjectID = ""2Xt-HY-uBw""; */
""2Xt-HY-uBw.title"" = ""Random2"";

/* Class = ""NSMenu""; title = ""Found""; ObjectID = ""1b7-l0-RR""; */
""1b7-l0-RR.title"" = ""Found4"";";

			manager.FromData (randomSnippet);

			var initialDefinitions = manager.Definitions.Values.ToList ();

			randomSnippet = @"/* Class = ""NSMenu""; title = ""Random1""; ObjectID = ""3Xt-HY-uBw""; */
""3Xt-HY-uBw"" = ""Random3"";

/* Class = ""NSMenu""; title = ""Found""; ObjectID = ""1b7-l0-RR""; */
""1b7-l0-RR.title"" = ""Found4"";

/* Class = ""NSMenu5""; title = ""Found""; ObjectID = ""1b7-l0-R2""; */
""1b7-l0-R2.title"" = ""Found5"";";

			manager.FromData (randomSnippet);
			manager.AddMissingDefinitions (initialDefinitions, true);

			Assert.AreEqual (2, manager.Definitions.Count, "#1");
			StringTitleDefinition dev;
			Assert.IsTrue (manager.Definitions.TryGetValue ("2Xt-HY-uBw", out dev), "#2");
			Assert.IsTrue (manager.Definitions.TryGetValue ("1b7-l0-RR", out dev), "#3");
		}

		[Test ()]
		public void when_save_then_single_element_persists ()
		{
			string randomSnippet = @"/* Class = ""NSMenu""; title = ""Random1""; ObjectID = ""2Xt-HY-uBw""; */
""2Xt-HY-uBw.title"" = ""Random2"";
";
			manager.FromData (randomSnippet);
			var outputFileInfo = new FileInfo (Path.Combine (currentOutputDirectory, "testFile.strings"));

			if (outputFileInfo.Exists) {
				outputFileInfo.Delete ();
			}

			manager.Save (outputFileInfo.FullName);

			outputFileInfo.Refresh ();
			Assert.True (outputFileInfo.Exists, "#1");

			var processedSnippetData = File.ReadAllText (outputFileInfo.FullName);
			Assert.AreEqual (randomSnippet, processedSnippetData, "#1");

			outputFileInfo.Delete ();
			outputFileInfo.Refresh ();
			Assert.False (outputFileInfo.Exists, "#2");
		}

		[Test ()]
		public void when_save_then_multiple_element_persists ()
		{
			string randomSnippet = @"/* Class = ""NSMenuItem""; title = ""Customize Toolbar…""; ObjectID = ""1UK-8n-QPP""; */
""1UK-8n-QPP.title"" = ""Customize Toolbar…2"";

/* Class = ""NSMenuItem""; title = ""LocalizationApp""; ObjectID = ""1Xt-HY-uBw""; */
""1Xt-HY-uBw.title"" = ""LocalizationApp2"";

/* Class = ""NSMenu""; title = ""Find""; ObjectID = ""1b7-l0-nxx""; */
""1b7-l0-nxx.title"" = ""Find2"";
";

			manager.FromData (randomSnippet);
			var outputFileInfo = new FileInfo (Path.Combine (currentOutputDirectory, "testFile.strings"));

			if (outputFileInfo.Exists) {
				outputFileInfo.Delete ();
			}

			manager.Save (outputFileInfo.FullName);

			outputFileInfo.Refresh ();
			Assert.True (outputFileInfo.Exists, "#1");

			var processedSnippetData = File.ReadAllText (outputFileInfo.FullName);
			Assert.AreEqual (randomSnippet, processedSnippetData, "#1");

			outputFileInfo.Delete ();
			outputFileInfo.Refresh ();
			Assert.False (outputFileInfo.Exists, "#2");
		}

		[Test ()]
		public void when_adding_stringtiledefinitions_then_stores_elements ()
		{
			string randomSnippet = @"/* Class = ""NSMenu""; title = ""Random1""; ObjectID = ""2Xt-HY-uBw""; */
""2Xt-HY-uBw.title"" = ""Random2"";

/* Class = ""NSMenu""; title = ""Found""; ObjectID = ""1b7-l0-RR""; */
""1b7-l0-RR.title"" = ""Found4"";";

			manager.FromData (randomSnippet);

			var definitions = manager.Definitions;
			definitions.Add ("II-OO-UU", new StringTitleDefinition ("II-OO-UU") {
				Class = "NSValue",
				Comment = "Comment something",
				Title = "Test title"
			});

			definitions.Add ("JJ-FF-UU", new StringTitleDefinition ("JJ-FF-UU") {
				Class = "NSString",
				Comment = "This is a comment",
				Title = "This is a title"
			});


			var fileInfo = new FileInfo (Path.Combine (currentOutputDirectory, "tmpTest.strings"));
			if (fileInfo.Exists)
				fileInfo.Delete ();

			manager.Save (fileInfo.FullName);

			//Read saved file and reload manager
			var newData = File.ReadAllText (fileInfo.FullName);
			manager.FromData (newData);

			foreach (var item in manager.Definitions.Values) {
				StringTitleDefinition definition;
				Assert.True (definitions.TryGetValue (item.ObjectID, out definition), "#{item.ObjectID}-1");

				Assert.AreEqual (item.Class, definition.Class, $"#{item.ObjectID}-2");
				Assert.AreEqual (item.Comment, definition.Comment, $"#{item.ObjectID}-3");
				Assert.AreEqual (item.Title, definition.Title, $"#{item.ObjectID}-4");
			}

			fileInfo.Delete ();
			fileInfo.Refresh ();
			Assert.False (fileInfo.Exists, "#2");
		}
	}
}

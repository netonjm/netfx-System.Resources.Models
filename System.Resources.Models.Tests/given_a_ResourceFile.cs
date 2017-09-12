using System;
using System.IO;
using NUnit.Framework;
using System.Linq;

namespace System.Resources.Models.Tests
{
	[TestFixture ()]
	public class given_a_ResourceFile : BaseTest
	{
		[Test ()]
		public void when_gets_from_path_then_loads_resources_with_languages ()
		{
			var resourceFiles = ResourceFile.FromPath (Resources.PropertiesDirectory, false);
			Assert.AreEqual (2, resourceFiles.Count, "#1");
			Assert.AreEqual (2, resourceFiles[0].Languages.Count (), "#2");
			Assert.AreEqual (2, resourceFiles[1].Languages.Count (), "#3");
		}

		[Test ()]
		public void when_gets_from_path_then_loads_resources_recursively ()
		{
			var resourceFiles = ResourceFile.FromPath (Resources.PropertiesDirectory, true);
			Assert.AreEqual (4, resourceFiles.Count, "#1");
		}
	}
}

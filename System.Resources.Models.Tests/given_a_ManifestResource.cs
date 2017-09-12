using NUnit.Framework;
using System.IO;
using System.Linq;

namespace System.Resources.Models.Tests
{
	[TestFixture ()]
	public class given_a_ManifestResource : BaseTest
	{
		[Test ()]
		public void when_loads_resource_then_contains_data ()
		{
			var template = TemplateHelper.GetManifestResource (Resources.ExecutingAssembly, "Main.strings");
			Assert.IsNotNull (template);
			Assert.IsNotEmpty (template);
		}
	}
}

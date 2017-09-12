using System.IO;
using System.Reflection;
using System;

namespace System.Resources.Models.Tests
{
	public class Resources
	{
		public Assembly ExecutingAssembly {
			get; private set;
		}

		public DirectoryInfo OutputDirectory {
			get; private set;
		}

		public DirectoryInfo PropertiesDirectory {
			get; private set;
		}

		public DirectoryInfo BaseTestsDirectory {
			get; private set;
		}

		public string DefaultResourcesResX {
			get; private set;
		}

		public string TmpDirectory {
			get; private set;
		}


		public Resources ()
		{
			ExecutingAssembly = Assembly.GetExecutingAssembly ();
			OutputDirectory = new DirectoryInfo (Path.GetDirectoryName (Assembly.GetExecutingAssembly ().Location));
			PropertiesDirectory = new DirectoryInfo (Path.Combine (OutputDirectory.FullName, "Properties"));
			DefaultResourcesResX = Path.Combine (PropertiesDirectory.FullName, "Resources.resx");
			TmpDirectory = Path.Combine (OutputDirectory.FullName, "tmp");

			BaseTestsDirectory = OutputDirectory.Parent.Parent;
		}
	}
}

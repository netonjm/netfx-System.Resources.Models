using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace System.Resources.Models
{
	public class ResourceFile
	{
		readonly public List<LanguageResourceFileInfo> Languages;

		public ResourceFileInfo FileInfo {
			get;
			private set;
		}

		public ResourceFile (ResourceFileInfo resourceFileInfo)
		{
			FileInfo = resourceFileInfo;
			Languages = new List<LanguageResourceFileInfo> ();
		}

		public static List<ResourceFile> FromPath (DirectoryInfo directoryPath, bool recursively)
		{
			var resources = new List<ResourceFile> ();
			if (recursively) {
				foreach (var subdirectory in directoryPath.GetDirectories ())
					resources.AddRange (FromPath (subdirectory, true));
			}

			resources.AddRange (FromPath (directoryPath));
			return resources;
		}

		static List<ResourceFile> FromPath (DirectoryInfo directoryPath)
		{
			var resourceFiles = directoryPath.EnumerateFiles ("*.resx")
											 .Select (s => ResourceFileInfo.FromFileInfo (s));

			var resources = new List<ResourceFile> ();
			ResourceFile resourceFile;
			var mainResourceFiles = resourceFiles.Where (s => s.GetType () == typeof (ResourceFileInfo));
			foreach (var mainResourceFile in mainResourceFiles) {
				resourceFile = new ResourceFile (mainResourceFile);
				var languages = resourceFiles.OfType<LanguageResourceFileInfo> ()
											 .Where (s => s.ResourceName == mainResourceFile.ResourceName);
				resourceFile.Languages.AddRange (languages);
				resources.Add (resourceFile);
			}
			return resources;
		}
	}
}

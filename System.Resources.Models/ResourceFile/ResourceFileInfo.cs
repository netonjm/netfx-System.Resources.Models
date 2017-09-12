using System.IO;

namespace System.Resources.Models
{
	public class LanguageResourceFileInfo : ResourceFileInfo
	{
		public string Language {
			get;
			private set;
		}

		protected internal LanguageResourceFileInfo (string fullname, string name, string resourceName, string language)
		: base (fullname, name, resourceName)
		{
			Language = language;
		}
	}

	public class ResourceFileInfo
	{
		public string Fullname {
			get;
			private set;
		}

		public string Name {
			get;
			private set;
		}

		public string ResourceName {
			get;
			private set;
		}

		protected ResourceFileInfo (string fullname, string name, string resourceName)
		{
			Fullname = fullname;
			Name = name;
			ResourceName = resourceName;
		}

		public static ResourceFileInfo FromFileInfo (FileInfo fileInfo)
		{
			var fullname = fileInfo.FullName;
			var name = fileInfo.Name;

			var resourceName = Path.GetFileNameWithoutExtension (name); //removes .resx
			if (Path.HasExtension (resourceName)) {
				//is a language
				var language = Path.GetExtension (resourceName).Substring (1);
				resourceName = Path.GetFileNameWithoutExtension (resourceName);
				return new LanguageResourceFileInfo (fullname, name, resourceName, language);
			}
			return new ResourceFileInfo (fullname, name, resourceName);
		}
	}
}

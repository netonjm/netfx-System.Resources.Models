using System.Reflection;
using System.IO;

namespace System.Resources.Models
{
	public static class TemplateHelper
	{
		public static string GetManifestResource (Assembly assembly, string resource)
		{
			var resources = assembly.GetManifestResourceNames ();
			using (var stream = assembly.GetManifestResourceStream (resource)) {
				using (TextReader tr = new StreamReader (stream)) {
					return tr.ReadToEnd ();
				};
			}
		}
	}
}

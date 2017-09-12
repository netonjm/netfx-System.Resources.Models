using System;
using System.Reflection;
using System.Collections.Generic;
using System.IO;

namespace System.Resources.Models
{
	public abstract class ResourceBase
	{
		protected string resourceData;
		protected string resourceResult;

		public string Result => resourceResult;

		protected ResourceBase ()
		{
		}

		protected ResourceBase (string resourceId)
		{
			resourceData = TemplateHelper.GetManifestResource (Assembly.GetExecutingAssembly (), resourceId);
		}

		protected string ComputeTemplate (string template, Dictionary<string, string> elements, string errorMessage = "current data don't correspond to elements keys")
		{
			foreach (var item in elements) {
				var index = template.IndexOf (item.Key, StringComparison.Ordinal);
				if (index == -1)
					throw new IndexOutOfRangeException (errorMessage);
				template = template.Replace (item.Key, item.Value);
			}
			return template;
		}

		public virtual void Save (string filePath, bool forceOverride = false)
		{
			if (File.Exists (filePath)) {
				if (forceOverride)
					File.Delete (filePath);
				else
					throw new Exception ("there is a file with the same name");
			}
			File.WriteAllText (filePath, resourceResult);
		}
	}
}

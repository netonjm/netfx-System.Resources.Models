using System.IO;
using System.Reflection;

namespace System.Resources.Models.Tests
{
	public class BaseTest
	{
		protected readonly Resources Resources;

		public BaseTest ()
		{
			Resources = new Resources ();

			if (!Directory.Exists (Resources.TmpDirectory))
				Directory.CreateDirectory (Resources.TmpDirectory);
		}
	}
}

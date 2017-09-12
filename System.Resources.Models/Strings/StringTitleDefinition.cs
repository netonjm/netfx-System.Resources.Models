using System;
using System.Text;
using System.Xml.Linq;

namespace System.Resources.Models
{
	public class StringTitleDefinition
	{
		public string ResxID { get; private set; }
		public string ObjectID { get; private set; }
		public string Class { get; set; }
		public string Title { get; set; }
		public string Comment { get; set; }

		public StringTitleDefinition (string objectID)
		{
			ObjectID = objectID;
			ResxID = objectID.Replace ("-", "");
		}

		public string ToStringDefinition ()
		{
			return string.Concat ($"/* Class = \"{Class}\"; title = \"{Comment}\"; ObjectID = \"{ObjectID}\"; */",
								 Environment.NewLine,
								 $"\"{ObjectID}.title\" = \"{Title}\";");
		}

		public string GetDecodedTitle ()
		{
			return System.Net.WebUtility.HtmlDecode (Title);
		}

		public string GetEncodedTitle ()
		{
			return System.Net.WebUtility.HtmlEncode (Title);
		}

		public string ToResXDefinition (bool includeComments)
		{
			StringBuilder element = new StringBuilder ();
			if (includeComments) {
				element.AppendLine ($"\t<!-- {Comment} -->");
			}
			element.AppendLine ($"\t<data name=\"{ResxID}\" xml:space=\"preserve\">");
			element.AppendLine ($"\t\t<value>{GetEncodedTitle ()}</value>");
			element.Append ("\t</data>");
			return element.ToString ();
		}

		public XElement ToXElement ()
		{
			StringBuilder element = new StringBuilder ();
			element.AppendLine ($"<data name=\"{ResxID}\" xml:space=\"preserve\">");
			element.AppendLine ($"<value>{GetEncodedTitle ()}</value>");
			element.Append ("</data>");
			return XElement.Parse (element.ToString ()); ;
		}
	}
}

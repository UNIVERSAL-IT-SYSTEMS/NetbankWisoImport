using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace ConsoleApplication1
{
	public class Category
	{
		[XmlElement("Keyword")]
		public List<Keyword> Keywords { get; set; }
		[XmlAttribute]
		public string Name { get; set; }
	}

	public class Keyword
	{
		/// <summary>
		/// Comment
		/// </summary>
		[XmlAttribute]
		public string Name { get; set; }

		/// <summary>
		/// Comment
		/// </summary>
		[XmlAttribute]
		public string Empfänger { get; set; }
	}
}

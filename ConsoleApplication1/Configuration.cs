using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ConsoleApplication1
{
	public class Configuration
	{
		/// <summary>
		/// Comment
		/// </summary>
		public DateTime? LastWrite { get; set; }

		public List<Category> Categories { get; set; }
	}
}

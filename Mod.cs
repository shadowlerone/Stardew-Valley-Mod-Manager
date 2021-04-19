using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Names
{
	class Mod
	{
		public String Name { get; set; }
		public String Author { get; set; }
		public String Version { get; set; }
		public String Description { get; set; }
		public String UniqueID { get; set; }
		public String EntryDll { get; set; }
		public String MinimumApiVersion { get; set; }
		public String[] UpdateKeys { get; set; }
		public String FilePath { get; set; }
	}
}

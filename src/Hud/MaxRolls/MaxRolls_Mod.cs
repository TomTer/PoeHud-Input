using System;
using System.Collections.Generic;
using System.Xml.Linq;

namespace PoeHUD.Hud.MaxRolls
{
	public class MaxRolls_Mod
	{
		public readonly string name;
		public readonly string type;
		public readonly bool multi;
		public List<MaxRolls_ModLevel> modLevel;

		public MaxRolls_Mod(XElement m)
		{
			name = m.Attribute("readable").Value;
			type = m.Attribute("type").Value;
			modLevel = new List<MaxRolls_ModLevel>();
			foreach (XElement level in m.Elements())
			{
				modLevel.Add(new MaxRolls_ModLevel(level));
			}
			if (modLevel[0].min2 != null && modLevel[0].max2 != null)
				multi = true;
		}

		public class MaxRolls_ModLevel
		{
			public readonly string min;
			public readonly string max;
			public readonly string min2;
			public readonly string max2;
			public readonly int lvl;
			public readonly int ilvl;
			public MaxRolls_ModLevel(XElement ml)
			{
				this.ilvl = Convert.ToInt32(ml.Attribute("ilvl").Value);
				this.min = ml.Element("min").Value;
				this.max = ml.Element("max").Value;
				if (ml.Element("min2") != null && ml.Element("max2") != null)
				{
					this.min2 = ml.Element("min2").Value;
					this.max2 = ml.Element("max2").Value;
				}
				else
				{
					this.min2 = null;
					this.max2 = null;
				}
			}
		}

	}
}
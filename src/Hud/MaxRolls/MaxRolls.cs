using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using PoeHUD.Controllers;
using PoeHUD.Framework;
using PoeHUD.Poe;
using PoeHUD.Poe.Files;

namespace PoeHUD.Hud.MaxRolls
{

	public class RollValue
	{
		public readonly int Tier = -1;
		private readonly int TotalTiers = 1;
		public readonly ModsDat.ModType AffixType;
		public readonly bool IsCrafted;
		public readonly String AffixText;
		public readonly Color TextColor;
		public readonly ModsDat.ModRecord TheMod;

		public readonly int[] StatValue;


		public RollValue(ItemMod mod, FsController fs, int iLvl)
		{
			string name = mod.RawName;
			TheMod = fs.Mods.records[name];
			AffixType = TheMod.AffixType;
			AffixText = String.IsNullOrEmpty(TheMod.UserFriendlyName) ? TheMod.Key : TheMod.UserFriendlyName;
			IsCrafted = TheMod.Domain == 10;
			StatValue = new int[] {mod.Value1, mod.Value2, mod.Value3, mod.Value4 };

			int subOptimalTierDistance = 0;

			List<ModsDat.ModRecord> allTiers;
			if (fs.Mods.recordsByTier.TryGetValue(Tuple.Create(TheMod.Group, TheMod.AffixType), out allTiers))
			{
				bool tierFound = false;
				TotalTiers = 0;
				//AllTiersRange = new[]{new IntRange(), new IntRange(), new IntRange()};
				foreach (var tn in allTiers)
				{
					// still not filtering out some mods. (like a.spd from gloves projected onto rings)
					if (tn.StatNames[0] != TheMod.StatNames[0] || tn.StatNames[1] != TheMod.StatNames[1] 
					    ||tn.StatNames[2] != TheMod.StatNames[2] || tn.StatNames[3] != TheMod.StatNames[3])
						continue;

					TotalTiers++;
					if (tn.Equals(TheMod))
					{
						Tier = TotalTiers;
						tierFound = true;
					}
					if (!tierFound && tn.MinLevel <= iLvl)
						subOptimalTierDistance++;

					
				}
			}
			double hue;
			if (TotalTiers == 1)
				hue = 180;
			else
				hue = 120 - Math.Min(subOptimalTierDistance, 3)*40;

			TextColor = ColorUtils.ColorFromHsv(hue, TotalTiers == 1 ? 0 : 1,1);

		}

		internal bool CouldHaveTiers()
		{
			return TotalTiers > 1;
		}
	}
}
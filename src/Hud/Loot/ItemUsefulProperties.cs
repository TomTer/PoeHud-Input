using System;
using System.Collections.Generic;
using System.Drawing;
using PoeHUD.Game;

namespace PoeHUD.Hud.Loot
{
	public class ItemUsefulProperties {

		public string Name;
		public string DisplayName { get { return (Quality > 0 ? "Superior " : String.Empty) + Name; } }
		public bool IsCurrency;
		public bool IsSkillGem;
		public ItemRarity Rarity;
		public bool WorthChrome;

		public bool IsCraftingBase;
		
		public int NumSockets;
		public int NumLinks;

		public int ItemLevel;
		public int Quality;
		public int MapLevel;
		public bool IsVaalFragment;

		public bool IsWorthAlertingPlayer(HashSet<string> currencyNames)
		{			
			if( Rarity == ItemRarity.Rare && Settings.GetBool("ItemAlert.Rares"))
				return true;
			if( Rarity == ItemRarity.Unique && Settings.GetBool("ItemAlert.Uniques"))
				return true;
			if(( MapLevel > 0  || IsVaalFragment ) && Settings.GetBool("ItemAlert.Maps"))
				return true;
			if( NumLinks >= Settings.GetInt("ItemAlert.MinLinks"))
				return true;
			if( IsCurrency && Settings.GetBool("ItemAlert.Currency")) {
				if (currencyNames == null) {
					if( !Name.Contains("Portal") && Name.Contains("Wisdom") )
						return true;
				}
				else if (currencyNames.Contains(Name))
					return true;
			}

			if (IsSkillGem && Settings.GetBool("ItemAlert.SkillGems")) return true;
			if (IsSkillGem && Settings.GetBool("ItemAlert.QualitySkillGems") && Quality >= Settings.GetInt("ItemAlert.QualitySkillGemsLevel")) return true;
			if (WorthChrome && Settings.GetBool("ItemAlert.RGB")) return true;
			if (NumSockets >= Settings.GetInt("ItemAlert.MinSockets")) return true;

			return IsCraftingBase;
		}

		internal AlertDrawStyle GetDrawStyle()
		{
			System.Drawing.Color color = Color.White;
			switch(this.Rarity) {
				case ItemRarity.White : color = Color.White; break;
				case ItemRarity.Magic: color = HudSkin.MagicColor; break;
				case ItemRarity.Rare: color = HudSkin.RareColor; break;
				case ItemRarity.Unique: color = HudSkin.UniqueColor; break;
			}
			if( IsSkillGem )
				color = HudSkin.SkillGemColor;
			if (IsCurrency)
				color = HudSkin.CurrencyColor;

			int iconIndex = -1;
			if (WorthChrome)
				iconIndex = 1;
			if (NumSockets == 6)
				iconIndex = 0;
			if (IsCraftingBase)
				iconIndex = 2;
			if (NumLinks == 6)
				iconIndex = 3;

			return new AlertDrawStyle()
			{
				color = color,
				FrameWidth = MapLevel > 0 || IsVaalFragment ? 1 : 0,
				Text = DisplayName,
				IconIndex = iconIndex
			};
		}
	}
}

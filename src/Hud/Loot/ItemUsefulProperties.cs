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
		public Rarity Rarity;
		public bool WorthChrome;

		public bool IsCraftingBase;
		
		public int NumSockets;
		public int NumLinks;

		public int ItemLevel;
		public int Quality;
		public int MapLevel;
		public bool IsVaalFragment;

		public bool IsWorthAlertingPlayer(ItemAlerter.ItemAlertSettings Settings, HashSet<string> currencyNames)
		{			
			if( Rarity == Rarity.Rare && Settings.AlertOfRares)
				return true;
			if( Rarity == Rarity.Unique && Settings.AlertOfUniques)
				return true;
			if(( MapLevel > 0  || IsVaalFragment ) && Settings.AlertOfMaps)
				return true;
			if( NumLinks >= Settings.Sockets.MinLinksToAlert)
				return true;
			if( IsCurrency && Settings.AlertOfCurrency) {
				if (currencyNames == null) {
					if( !Name.Contains("Portal") && Name.Contains("Wisdom") )
						return true;
				}
				else if (currencyNames.Contains(Name))
					return true;
			}

			if (IsSkillGem && Settings.AlertOfGems.Enabled && Quality >= Settings.AlertOfGems.MinQuality) return true;
			if (WorthChrome && Settings.Sockets.AlertOfRgb) return true;
			if (NumSockets >= Settings.Sockets.MinSocketsToAlert) return true;

			return IsCraftingBase;
		}

		internal AlertDrawStyle GetDrawStyle()
		{
			System.Drawing.Color color = Color.White;
			switch(this.Rarity) {
				case Rarity.White : color = Color.White; break;
				case Rarity.Magic: color = HudSkin.MagicColor; break;
				case Rarity.Rare: color = HudSkin.RareColor; break;
				case Rarity.Unique: color = HudSkin.UniqueColor; break;
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

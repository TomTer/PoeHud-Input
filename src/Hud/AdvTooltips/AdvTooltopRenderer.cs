using System.Collections.Generic;
using System.Drawing;
using PoeHUD.Framework;
using PoeHUD.Hud.MaxRolls;
using PoeHUD.Poe;
using PoeHUD.Poe.EntityComponents;
using PoeHUD.Poe.FileSystem;
using PoeHUD.Poe.UI;
using PoeHUD.Settings;
using SlimDX.Direct3D9;

namespace PoeHUD.Hud.AdvTooltips
{
	public class AdvTooltopRenderer : HUDPluginBase
	{
		public class RollsSettings : SettingsForModule
		{
			public Setting<bool> ShowItemMods = new Setting<bool>("Mod Rolls", true);
			public Setting<bool> ShowItemLevel = new Setting<bool>("Item Level", true);

			public RollsSettings() : base("Item Tooltips") {}
		}
	
		private Entity _lastHovered;
		private List<RollValue> _mods;
		public RollsSettings Settings = new RollsSettings();

		public override void OnEnable()
		{
			this._mods = new List<RollValue>();
			this._lastHovered = null;
		}

		public override void OnDisable()
		{
		}

		public override SettingsForModule SettingsNode
		{
			get { return Settings; }
		}

		public override void Render(RenderingContext rc, Dictionary<UiMountPoint, Vec2> mountPoints)
		{
			Element uiHover = this.model.Internal.IngameState.ElementUnderCursor;

			Tooltip tooltip = uiHover.AsObject<InventoryItemIcon>().Tooltip;
			if (tooltip == null)
				return;
			Element childAtIndex1 = tooltip.GetChildAtIndex(0);
			if (childAtIndex1 == null)
				return;
			Element childAtIndex2 = childAtIndex1.GetChildAtIndex(1);
			if (childAtIndex2 == null)
				return;
			Rect clientRect = childAtIndex2.GetClientRect();


			Entity poeEntity = uiHover.AsObject<InventoryItemIcon>().Item;
			if (poeEntity.Address == 0 || !poeEntity.IsValid)
				return;

			if (this._lastHovered == null || this._lastHovered.ID != poeEntity.ID) {
				this._lastHovered = poeEntity;

				this._mods = new List<RollValue>();
				//List<Poe_ItemMod> impMods = lastHovered.GetComponent<Mods>().ImplicitMods;
				List<ItemMod> expMods = poeEntity.GetComponent<Mods>().ItemMods;
				int ilvl = poeEntity.GetComponent<Mods>().ItemLevel;
				foreach (ItemMod item in expMods)
					this._mods.Add(new RollValue(item, model.Files, ilvl));
				
			}
			if( Settings.ShowItemMods)
				RenderRolls(rc, clientRect);
			if (Settings.ShowItemLevel)
				RenderItemLevel(rc, clientRect);


		}

		private void RenderRolls(RenderingContext rc, Rect clientRect)
		{
			int yPosTooltil = clientRect.Y + clientRect.H + 5;
			int i = yPosTooltil + 4;
			// Implicit mods
			//foreach (Poe_ItemMod item in impMods)
			//{
			//    rc.AddTextWithHeight(new Vec2(clientRect.X, yPos), item.Name, Color.Yellow, 9, DrawTextFormat.Left);
			//    rc.AddTextWithHeight(new Vec2(clientRect.X + clientRect.W - 10, yPos), item.Level.ToString(), Color.White, 6, DrawTextFormat.Left);
			//    yPos += 20;
			//}

			foreach (RollValue item in this._mods)
			{
				i = drawStatLine(rc, item, clientRect, i);
				i += 4;
			}
			if (i > yPosTooltil + 4)
			{
				Rect helpRect = new Rect(clientRect.X + 1, yPosTooltil, clientRect.W, i - yPosTooltil);
				rc.AddBox(helpRect, Color.FromArgb(220, Color.Black));
			}
		}


		private void RenderItemLevel(RenderingContext rc, Rect clientRect)
		{
			string text = _lastHovered.GetComponent<Mods>().ItemLevel.ToString();
#if DEBUG
			text += " @ " + _lastHovered.Address.ToString("X8");
#endif
			rc.AddTextWithHeight(new Vec2(clientRect.X + 2, clientRect.Y + 2), text, Color.White, 14, DrawTextFormat.Left);
		}

		private static int drawStatLine(RenderingContext rc, RollValue item, Rect clientRect, int yPos)
		{
			const int leftRuler = 50;

			bool isUniqAffix = item.AffixType == ModsDat.ModType.Hidden;
			string prefix = item.AffixType == ModsDat.ModType.Prefix
				? "[P]"
				: item.AffixType == ModsDat.ModType.Suffix ? "[S]" : "[?]";
			if (!isUniqAffix)
			{
				if( item.CouldHaveTiers())
					prefix += " T" + item.Tier + " ";

				rc.AddTextWithHeight(new Vec2(clientRect.X + 5, yPos), prefix, Color.White, 8, DrawTextFormat.Left);
				var textSize = rc.AddTextWithHeight(new Vec2(clientRect.X + leftRuler, yPos), item.AffixText, item.TextColor, 8,
					DrawTextFormat.Left);
				yPos += textSize.Y;
			}

			for (int iStat = 0; iStat < 4; iStat++)
			{
				IntRange range = item.TheMod.StatRange[iStat];
				if(range.Min == 0 && range.Max == 0)
					continue;

				var theStat = item.TheMod.StatNames[iStat];
				int val = item.StatValue[iStat];
				float percents = range.GetPercentage(val);
				bool noSpread = !range.HasSpread();

				double hue = 120 * percents;
				if (noSpread) hue = 300;
				if (percents > 1) hue = 180;
				
				Color col = ColorUtils.ColorFromHsv(hue, 1, 1);

				string line2 = string.Format(noSpread ? "{0}" : "{0} [{1}]", theStat, range);

				rc.AddTextWithHeight(new Vec2(clientRect.X + leftRuler, yPos), line2, Color.White, 8, DrawTextFormat.Left);

				string sValue = theStat.ValueToString(val);
				var txSize = rc.AddTextWithHeight(new Vec2(clientRect.X + leftRuler - 5, yPos), sValue,
					col, 8,
					DrawTextFormat.Right);

				yPos += txSize.Y;
			}
			return yPos;
		}
	}
}
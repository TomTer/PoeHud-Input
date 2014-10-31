using System.Collections.Generic;
using System.Drawing;
using PoeHUD.Framework;
using PoeHUD.Poe;
using PoeHUD.Poe.EntityComponents;
using PoeHUD.Poe.UI;
using SlimDX.Direct3D9;

namespace PoeHUD.Hud.MaxRolls
{
    public class ItemRollsRenderer : HUDPluginBase
    {
        private Entity poeEntity;
        private List<MaxRolls_Current> mods;
        public override void OnEnable()
        {
            MaxRolls.Initialize();
            this.mods = new List<MaxRolls_Current>();
            this.poeEntity = null;
        }

        public override void OnDisable()
        {
        }

		public override void Render(RenderingContext rc, Dictionary<UiMountPoint, Vec2> mountPoints)
        {
            if (!Settings.GetBool("Tooltip") || !Settings.GetBool("Tooltip.ShowItemMods"))
                return;
            Element uiHover = this.model.Internal.IngameState.UIHover;
            Entity poeEntity = uiHover.AsObject<InventoryItemIcon>().Item;
            if (poeEntity.address == 0 || !poeEntity.IsValid)
                return;
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
            Rect headerRect = childAtIndex1.GetChildAtIndex(0).GetClientRect();
            if (this.poeEntity == null || this.poeEntity.ID != poeEntity.ID) {
                this.mods = new List<MaxRolls_Current>();
                //List<Poe_ItemMod> impMods = poeEntity.GetComponent<Mods>().ImplicitMods;
                List<ItemMod> expMods = poeEntity.GetComponent<Mods>().ItemMods;
                int ilvl = poeEntity.GetComponent<Mods>().ItemLevel;
                foreach (ItemMod item in expMods)
                {
                    this.mods.Add(new MaxRolls_Current(item.Name, item.Level, ilvl));
                }
                this.poeEntity = poeEntity;
            }
            int tooltipBotY=clientRect.Y + clientRect.H;
            int i = tooltipBotY;
			// Implicit mods
            //foreach (Poe_ItemMod item in impMods)
            //{
            //    rc.AddTextWithHeight(new Vec2(clientRect.X, i), item.Name, Color.Yellow, 9, DrawTextFormat.Left);
            //    rc.AddTextWithHeight(new Vec2(clientRect.X + clientRect.W - 10, i), item.Level.ToString(), Color.White, 6, DrawTextFormat.Left);
            //    i += 20;
            //}
            foreach (MaxRolls_Current item in this.mods)
            {
                rc.AddTextWithHeight(new Vec2(clientRect.X, i), item.name, item.color, 8, DrawTextFormat.Left);
                i += 20;
                rc.AddTextWithHeight(new Vec2(clientRect.X + clientRect.W - 100, i), item.max, Color.White, 8, DrawTextFormat.Left);
                rc.AddTextWithHeight(new Vec2(clientRect.X + 30, i), item.curr, Color.White, 8, DrawTextFormat.Left);
                i += 20;
                if (item.curr2 != null && item.max2 != null)
                {
                    rc.AddTextWithHeight(new Vec2(clientRect.X + clientRect.W - 100, i), item.max2, Color.White, 8, DrawTextFormat.Left);
                    rc.AddTextWithHeight(new Vec2(clientRect.X + 30, i), item.curr2, Color.White, 8, DrawTextFormat.Left);
                    i += 20;
                }
            }
            if (i > tooltipBotY)
            {
                Rect helpRect = new Rect(clientRect.X + 1, tooltipBotY, clientRect.W, i - tooltipBotY);
                rc.AddBox(helpRect, Color.FromArgb(220, Color.Black));
            }
        }
    }
}
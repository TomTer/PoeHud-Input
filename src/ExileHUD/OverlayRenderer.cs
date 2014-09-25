using System;
using System.Collections.Generic;
using ExileHUD.ExileBot;

namespace ExileHUD.ExileHUD
{
	public class OverlayRenderer
	{
		private List<HUDPlugin> hudRenderers;
		private PathOfExile poe;
		public XPHRenderer XphRenderer;
		public PreloadAlert PreloadAlert;
		public MinimapRenderer MinimapRenderer;
		private int tickCount;
        public OverlayRenderer(PathOfExile poe, RenderingContext rc)
        {
            AreaChangeEvent event2 = null;
            Action<HUDPlugin> action = null;
            this.poe = poe;
            if (event2 == null)
            {
                event2 = area => this.tickCount = 6;
            }
            poe.CurrentArea.OnAreaChange += event2;
            this.hudRenderers = new List<HUDPlugin>();
            this.hudRenderers.Add(new HealthBarRenderer());
            this.hudRenderers.Add(new ItemAlerter());
            this.hudRenderers.Add(this.MinimapRenderer = new MinimapRenderer());
            this.hudRenderers.Add(new ItemLevelRenderer());
			this.hudRenderers.Add(new ItemRollsRenderer());
            this.hudRenderers.Add(new DangerAlert());
            this.hudRenderers.Add(this.XphRenderer = new XPHRenderer());
            this.hudRenderers.Add(new ClientHacks());
            this.hudRenderers.Add(this.PreloadAlert = new PreloadAlert());
            if (Settings.GetBool("Window.ShowIngameMenu"))
            {
#if !DEBUG
                this.hudRenderers.Add(new Menu());
#endif
            }
            rc.OnRender += new RenderCallback(this.rc_OnRender);
            if (action == null)
            {
                action = x => x.Init(poe, this);
            }
            this.hudRenderers.ForEach(action);
        }

		private void rc_OnRender(RenderingContext rc)
		{
			if (!Settings.GetBool("Window.RequireForeground") || this.poe.Window.IsForeground())
			{
				this.tickCount++;
				if (this.tickCount > 6)
				{
					this.poe.Update();
					this.tickCount = 0;
				}
				if (!this.poe.InGame || this.poe.Player == null)
				{
					return;
				}
				foreach (HUDPlugin current in this.hudRenderers)
				{
					current.Render(rc);
				}
			}
		}
		public bool Detach()
		{
			foreach (HUDPlugin current in this.hudRenderers)
			{
				current.OnDisable();
			}
			return false;
		}
	}
}

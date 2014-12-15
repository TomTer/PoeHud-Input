using PoeHUD.Settings;

namespace PoeHUD.Hud.Loot
{
	public class ItemSettings :SettingsForModule
	{
		public Setting<bool> ShowItemLevel = new Setting<bool>("ShowItemLevel", true);

		public ItemSettings(): base("Tooltip")
		{
		}
	}
}
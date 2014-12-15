namespace PoeHUD.Settings
{
	public class SettingsForModule : SettingsBlock
	{
		public string Group;

		public SettingsForModule(string moduleName) : base(moduleName) {}
		public readonly Setting<bool> Enabled = new Setting<bool>(" ", true);
		public GlobalSettings Global;
		public override void AttachToParent(SettingsRoot settingsRoot)
		{
			base.AttachToParent(settingsRoot);
			Global = settingsRoot.Global;
		}

		public static implicit operator bool(SettingsForModule inst) {
			return inst.Enabled.Value;
		}
	}

	public class GlobalSettings : SettingsBlock
	{
		public GlobalSettings() : base("Window") {}

		public Setting<bool> RequireForeground = new Setting<bool>("RequireForeground", true);
		public Setting<bool> ShowIngameMenu = new Setting<bool>("ShowIngameMenu", true);
		public Setting<string> WindowName = new Setting<string>("Name", "ExileHUD");
	}
}
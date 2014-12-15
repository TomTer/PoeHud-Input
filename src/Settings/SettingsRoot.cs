using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace PoeHUD.Settings
{
	public class SettingsRoot : SettingsBlock
	{
		public GlobalSettings Global = new GlobalSettings();
		private readonly string filename;

		public SettingsRoot(string fileName) : base("Root")
		{
			this.filename = fileName;
			AddModule(Global);
		}

		public void AddModule(SettingsBlock settingsNode)
		{
			settingsNode.AttachToParent(this);
			this.AllMembers.Add(settingsNode.Key, settingsNode);
			settingsNode.SetObserver(UpdateBridge);
		}

		protected override void EnsureDictionaryIsFilled()
		{
		}

		public void ReadFromFile()
		{
			if( !File.Exists(filename) )
				return;
			char[] eq = {'='};

			string[] allLines = File.ReadAllLines(filename);
			foreach (var line in allLines.Where(s => !String.IsNullOrWhiteSpace(s)))
			{
				var parts = line.Split(eq, 2);
				var combinedPath = parts[0].Trim();
				var value = parts[1].Trim();
				
				LoadValue(combinedPath, value);
			}
		}

		public override IEnumerable<string> GetKeyValuePairStrings()
		{
			// add nothing on this level
			return AllMembers.Values.SelectMany(iSet => iSet.GetKeyValuePairStrings());
		}

		public void SaveSettings()
		{
			File.WriteAllLines(filename, this.GetKeyValuePairStrings());
		}
	}
}
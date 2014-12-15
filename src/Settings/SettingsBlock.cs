using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using SlimDX.Direct3D10;

namespace PoeHUD.Settings
{
	public class SettingsBlock : ISetting
	{
		public SettingsBlock(string name) {
			BlockName = name;
		}
		public readonly string BlockName;
		private Action<string, object> _updateObserver;
		protected readonly IDictionary<string, ISetting> AllMembers = new SortedDictionary<string, ISetting>(StringComparer.OrdinalIgnoreCase);

		public IEnumerable<ISetting> Members { get { return AllMembers.Values; } }


		protected virtual void EnsureDictionaryIsFilled()
		{
			if (AllMembers.Count > 0)
				return;
			foreach (var fv in this.GetType().GetFields().Select(field => field.GetValue(this)).OfType<ISetting>())
			{
				AllMembers.Add(fv.Key, fv);
			}
		}

		public void UpdateBridge(string fieldName, object newValue)
		{
			if (null != _updateObserver)
			{
				string keyUp = string.IsNullOrWhiteSpace(fieldName) ? BlockName : string.Concat(BlockName, ".", fieldName);
				_updateObserver(keyUp, newValue);
			}
		}

		public string Key { get { return BlockName; }}
		public string ValueToString()
		{
			throw new InvalidOperationException("Block is a composite value");
		}

		public void SetObserver(Action<string, object> observer)
		{
			_updateObserver = observer;
			EnsureDictionaryIsFilled();
			foreach (ISetting iSet in AllMembers.Values)
			{
				iSet.SetObserver(UpdateBridge);
			}
		}

		public virtual IEnumerable<string> GetKeyValuePairStrings()
		{
			foreach (string s in AllMembers.Values.SelectMany(iSet => iSet.GetKeyValuePairStrings()))
			{
				if (s.StartsWith(" ="))
					yield return String.Concat(Key, s);
				else
					yield return String.Concat(Key, ".", s);
			}
		}

		public void LoadValue(string combinedPath, string value)
		{
			var ixDotInPath = combinedPath.IndexOf('.');
			var p1 = ixDotInPath > 0 ? combinedPath.Substring(0, ixDotInPath) : combinedPath;
			var p2 = ixDotInPath > 0 ? combinedPath.Substring(ixDotInPath + 1) : " ";
			ISetting module;
			if (AllMembers.TryGetValue(p1, out module))
				module.LoadValue(p2, value);
			else 
				System.Diagnostics.Debug.WriteLine("No member named '" + p1 + "' in module " + Key);
		}

		public virtual void AttachToParent(SettingsRoot settingsRoot)
		{
			EnsureDictionaryIsFilled();
			foreach (var m in AllMembers.Values)
			{
				var mb = m as SettingsBlock;
				if( mb != null)
					mb.AttachToParent(settingsRoot);
			}
		}
	}
}
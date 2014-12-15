using System;
using System.Collections.Generic;

namespace PoeHUD.Settings
{
	public interface ISetting
	{
		string Key { get; }

		void SetObserver(Action<string, object> observer);

		IEnumerable<string> GetKeyValuePairStrings();
		void LoadValue(string p2, string value);
	}
}
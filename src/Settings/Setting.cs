using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;

namespace PoeHUD.Settings
{
	public class Setting<T> : ISetting
	{
		public string Key { get; private set; }
		private T _default;
		private T _value;

		public virtual T Value { 
			get { return _value; }
			set {
				_value = value; 
				if (null != _updateObserver) 
					_updateObserver(Key, _value); 
			}
		}
		public virtual T Default
		{
			get { return _default; }
			set { _value = value; _default = value; }
		}

		public static implicit operator T(Setting<T> myClassInstance)
		{
			return myClassInstance._value;
		}

		public void Reset() { Value = _default; }
		public Setting(string key, T defaultValue = default(T)) {
			Key = key;
			_default = defaultValue;
			_value = defaultValue;
		}

		public void SetObserver(Action<string, object> observer) { _updateObserver = observer; }
		public virtual IEnumerable<string> GetKeyValuePairStrings()
		{
			//if( Default.Equals(_value))
			//	yield break;

			string v =  _value.ToString();
			if (_value is Color)
				v = ((Color)(object)_value).ToArgb().ToString("X");

			
			yield return String.Concat(String.IsNullOrWhiteSpace(Key) ? "" : Key, " = ", v);
		}

		public void LoadValue(string p2, string value)
		{
			if (!String.IsNullOrWhiteSpace(p2))
			{
				System.Diagnostics.Debug.WriteLine("trying to assign subSetting " + p2 + " to leaf " + Key);
				return;
			}

			object box = null;
			switch (typeof(T).Name)
			{
				case "Boolean": box = bool.Parse(value); break;
				case "Int32": box = int.Parse(value); break;
				case "Float": box = float.Parse(value); break;
				case "String": box = value; break;
				case "Color": box = System.Drawing.Color.FromArgb(int.Parse(value, NumberStyles.HexNumber)); break;
			}
			_value = (T) box;
		}

		private Action<string, object> _updateObserver;
	}

	public class SettingIntRange : Setting<int>
	{
		public readonly int Min;
		public readonly int Max;

		public SettingIntRange(string key, int min, int max, int defaultValue = 0)
			: base(key, defaultValue)
		{
			Max = max;
			Min = min;
		}

		public virtual int Value
		{
			get { return base.Value; }
			set
			{
				if (value < Min)
					base.Value = Min;
				else if (value > Max)
					base.Value = Max;
				else
					base.Value = value;
			}
		}

	}
}
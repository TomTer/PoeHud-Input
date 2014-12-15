using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using PoeHUD.Controllers;
using PoeHUD.Poe;
using PoeHUD.Poe.UI;
using System.ComponentModel.DataAnnotations;

namespace PoeHUD.Hud
{
	public class IdcScriptMaker
	{
		public readonly GameController game;

		public IdcScriptMaker(GameController gameController)
		{
			game = gameController;
		}

		public string GetBaseAddressScript()
		{
			StringBuilder sb = new StringBuilder();

			sb.MakeName(game.Internal.Address, "TheGame").AppendLine();
			sb.MakeName(game.Internal.IngameState.Address, "IngameState").AppendLine();

			foreach (var tt in EnumProperties<RemoteMemoryObject>(game.Internal.IngameState))
				sb.MakeName(tt.Item2, "igs_" + tt.Item1).AppendLine();

			foreach (var tt in EnumProperties<Element>(game.Internal.IngameState.IngameUi))
				sb.MakeName(tt.Item2, "ui_" + tt.Item1).AppendLine();
			foreach (var tt in EnumProperties<Element>(game.Internal.IngameState.IngameUi.InventoryPanel))
				sb.MakeName(tt.Item2, "ui_inv_" + tt.Item1).AppendLine();
			foreach (var tt in EnumProperties<Element>(game.Internal.IngameState.IngameUi.Chat))
				sb.MakeName(tt.Item2, "ui_cht_" + tt.Item1).AppendLine();


			sb.MakeName(game.Player.Address, "Player").AppendLine();
			return sb.ToString();
		}

		public string GetFlaskAddressScript()
		{
			StringBuilder sb = new StringBuilder();

			var flasks = game.Internal.IngameState.IngameUi.InventoryPanel.FlasksFrame;
			for (int i = 0; i < 5; i++)
			{
				var f1 = flasks.GetItemAt(i);
				if (f1 == null) continue;
				foreach (KeyValuePair<string, int> component in f1.EnumComponents())
				{
					sb.MakeName( component.Value, "fl_" + (i + 1) + "_" + component.Key);
				}
			}
			return sb.ToString();
		}

		IEnumerable<Tuple<string, int>> EnumProperties<T>(object instance) where T : RemoteMemoryObject
		{
			Type theClass = instance.GetType();
			Type tNeeded = typeof (T);
			object[] emptyArr = {};
			foreach (PropertyInfo p in theClass.GetProperties(BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Public))
			{

				var fnget = p.GetGetMethod();
				var rt = fnget.ReturnType;
				if (!tNeeded.IsAssignableFrom(rt))
					continue;

				var pa = p.GetCustomAttributes(typeof(DisplayAttribute), false);
				if( pa.Length > 0)
				{
					var a1 = pa[0] as DisplayAttribute;
					if (a1.GroupName == "Hidden")
						continue;
				}

				T value = fnget.Invoke(instance, emptyArr) as T;
				yield return Tuple.Create(p.Name, value.Address);
			}
		}
	}

	static class Idcextensions{

		public static StringBuilder MakeName(this StringBuilder sb, int ea, string name)
		{
			sb.AppendFormat("MakeName(0x{0:X8}, \"{1}\");", ea, name);
			return sb;
		}
	}
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using PoeHUD.Controllers;
using PoeHUD.Poe;
using PoeHUD.Poe.UI;

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

			sb.MakeName(game.Internal.address, "TheGame").AppendLine();
			sb.MakeName(game.Internal.IngameState.address, "IngameState").AppendLine();
			sb.MakeName(game.Internal.IngameState.Data.address, "IngameData").AppendLine();
			sb.MakeName(game.Internal.IngameState.IngameUi.address, "IngameUi").AppendLine();
			sb.MakeName(game.Internal.IngameState.UIRoot.address, "UiRoot").AppendLine();

			foreach (var tt in EnumProperties<Element>(game.Internal.IngameState.IngameUi))
				sb.MakeName(tt.Item2, "ui_" + tt.Item1).AppendLine();
			foreach (var tt in EnumProperties<Element>(game.Internal.IngameState.IngameUi.InventoryPanel))
				sb.MakeName(tt.Item2, "ui_inv_" + tt.Item1).AppendLine();



			sb.MakeName(game.Player.Address, "Player").AppendLine();
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
				if( !rt.IsAssignableFrom(tNeeded) )
					continue;
				T value = fnget.Invoke(instance, emptyArr) as T;
				yield return Tuple.Create(p.Name, value.address);
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

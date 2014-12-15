using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PoeHUD.Poe.EntityComponents
{
	public static class ComponentNames
	{
		public static readonly Dictionary<Type, string> Map = new Dictionary<Type, string>() {
			{typeof(Actor), "Actor"},
			{typeof(AreaTransition), "AreaTransition"}, 
			{typeof(Armour), "Armour"},
			{typeof(Base), "Base"}, 
			{typeof(Charges), "Charges"},
			{typeof(Chest), "Chest"}, 
			{typeof(Flask), "Flask"},
			{typeof(Mods), "Mods"}, 
			{typeof(Map), "Map"},
			{typeof(Life), "Life"}, 
			{typeof(Monster), "Monster"},
			{typeof(NPC), "NPC"}, 
			{typeof(ObjectMagicProperties), "ObjectMagicProperties"},
			{typeof(Player), "Player"}, 
			{typeof(Positioned), "Positioned"},
			{typeof(Quality), "Quality"}, 
			{typeof(Render), "Render"},
			{typeof(SkillGem), "SkillGem"}, 
			{typeof(Stats), "Stats"},
			{typeof(Sockets), "Sockets"},
			{typeof(Targetable), "Targetable"}, 
			{typeof(Weapon), "Weapon"}, 
			{typeof(WorldItem), "WorldItem"}
		};
	}
}

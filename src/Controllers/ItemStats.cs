using System;
using System.Collections.Generic;
using PoeHUD.Game;
using PoeHUD.Poe;
using PoeHUD.Poe.EntityComponents;

namespace PoeHUD.Controllers
{
	public class ItemStats
	{
		private class StatTranslator
		{
			private delegate void AddStat(ItemStats stats, ItemMod m);
			private Dictionary<string, ItemStats.StatTranslator.AddStat> dict;
			public StatTranslator()
			{
				this.dict = new Dictionary<string, ItemStats.StatTranslator.AddStat>();
				this.dict.Add("Dexterity", this.Single(ItemStat.Dexterity));
				this.dict.Add("Strength", this.Single(ItemStat.Strength));
				this.dict.Add("Intelligence", this.Single(ItemStat.Intelligence));
				this.dict.Add("IncreasedMana", this.Single(ItemStat.AddedMana));
				this.dict.Add("IncreasedLife", this.Single(ItemStat.AddedHP));
				this.dict.Add("IncreasedEnergyShield", this.Single(ItemStat.AddedES));
				this.dict.Add("IncreasedEnergyShieldPercent", this.Single(ItemStat.AddedESPercent));
				this.dict.Add("ColdResist", this.Single(ItemStat.ColdResistance));
				this.dict.Add("FireResist", this.Single(ItemStat.FireResistance));
				this.dict.Add("LightningResist", this.Single(ItemStat.LightningResistance));
				this.dict.Add("ChaosResist", this.Single(ItemStat.ChaosResistance));
				this.dict.Add("AllResistances", this.MultipleSame(new ItemStat[]
				{
					ItemStat.ColdResistance,
					ItemStat.FireResistance,
					ItemStat.LightningResistance
				}));
				this.dict.Add("CriticalStrikeChance", this.Single(ItemStat.CritChance));
				this.dict.Add("LocalCriticalMultiplier", this.Single(ItemStat.CritMultiplier));
				this.dict.Add("MovementVelocity", this.Single(ItemStat.MovementSpeed));
				this.dict.Add("ItemFoundRarityIncrease", this.Single(ItemStat.Rarity));
				this.dict.Add("ItemFoundQuantityIncrease", this.Single(ItemStat.Quantity));
				this.dict.Add("ManaLeech", this.Single(ItemStat.ManaLeech));
				this.dict.Add("LifeLeech", this.Single(ItemStat.LifeLeech));
				this.dict.Add("AddedLightningDamage", this.Average(ItemStat.AddedLightningDamage));
				this.dict.Add("AddedColdDamage", this.Average(ItemStat.AddedColdDamage));
				this.dict.Add("AddedFireDamage", this.Average(ItemStat.AddedFireDamage));
				this.dict.Add("AddedPhysicalDamage", this.Average(ItemStat.AddedPhysicalDamage));
				this.dict.Add("WeaponElementalDamage", this.Single(ItemStat.WeaponElementalDamagePercent));
				this.dict.Add("FireDamagePercent", this.Single(ItemStat.FireDamagePercent));
				this.dict.Add("ColdDamagePercent", this.Single(ItemStat.ColdDamagePercent));
				this.dict.Add("LightningDamagePercent", this.Single(ItemStat.LightningDamagePercent));
				this.dict.Add("SpellDamage", this.Single(ItemStat.SpellDamage));
				this.dict.Add("SpellDamageAndMana", this.Dual(ItemStat.SpellDamage, ItemStat.AddedMana));
				this.dict.Add("SpellCriticalStrikeChance", this.Single(ItemStat.SpellCriticalChance));
				this.dict.Add("IncreasedCastSpeed", this.Single(ItemStat.CastSpeed));
				this.dict.Add("ProjectileSpeed", this.Single(ItemStat.ProjectileSpeed));
				this.dict.Add("LocalIncreaseSocketedMinionGemLevel", this.Single(ItemStat.MinionSkillLevel));
				this.dict.Add("LocalIncreaseSocketedFireGemLevel", this.Single(ItemStat.FireSkillLevel));
				this.dict.Add("LocalIncreaseSocketedColdGemLevel", this.Single(ItemStat.ColdSkillLevel));
				this.dict.Add("LocalIncreaseSocketedLightningGemLevel", this.Single(ItemStat.LightningSkillLevel));
				this.dict.Add("LocalAddedPhysicalDamage", this.Average(ItemStat.LocalPhysicalDamage));
				this.dict.Add("LocalIncreasedPhysicalDamagePercent", this.Single(ItemStat.LocalPhysicalDamagePercent));
				this.dict.Add("LocalAddedColdDamage", this.Average(ItemStat.LocalAddedColdDamage));
				this.dict.Add("LocalAddedFireDamage", this.Average(ItemStat.LocalAddedFireDamage));
				this.dict.Add("LocalAddedLightningDamage", this.Average(ItemStat.LocalAddedLightningDamage));
				this.dict.Add("LocalCriticalStrikeChance", this.Single(ItemStat.LocalCritChance));
				this.dict.Add("LocalIncreasedAttackSpeed", this.Single(ItemStat.LocalAttackSpeed));
				this.dict.Add("LocalIncreasedEnergyShield", this.Single(ItemStat.LocalES));
				this.dict.Add("LocalIncreasedEvasionRating", this.Single(ItemStat.LocalEV));
				this.dict.Add("LocalIncreasedPhysicalDamageReductionRating", this.Single(ItemStat.LocalArmor));
				this.dict.Add("LocalIncreasedEvasionRatingPercent", this.Single(ItemStat.LocalEVPercent));
				this.dict.Add("LocalIncreasedEnergyShieldPercent", this.Single(ItemStat.LocalESPercent));
				this.dict.Add("LocalIncreasedPhysicalDamageReductionRatingPercent", this.Single(ItemStat.LocalArmorPercent));
				this.dict.Add("LocalIncreasedArmourAndEvasion", this.MultipleSame(new ItemStat[]
				{
					ItemStat.LocalArmorPercent,
					ItemStat.LocalEVPercent
				}));
				this.dict.Add("LocalIncreasedArmourAndEnergyShield", this.MultipleSame(new ItemStat[]
				{
					ItemStat.LocalArmorPercent,
					ItemStat.LocalESPercent
				}));
				this.dict.Add("LocalIncreasedEvasionAndEnergyShield", this.MultipleSame(new ItemStat[]
				{
					ItemStat.LocalEVPercent,
					ItemStat.LocalESPercent
				}));
			}
			public void Translate(ItemStats stats, ItemMod m)
			{
				if (!this.dict.ContainsKey(m.Name))
				{
					return;
				}
				this.dict[m.Name](stats, m);
			}
			private ItemStats.StatTranslator.AddStat Single(ItemStat stat)
			{
				return delegate(ItemStats x, ItemMod m)
				{
					x.AddToMod(stat, (float)m.Value1);
				};
			}
			private ItemStats.StatTranslator.AddStat Average(ItemStat stat)
			{
				return delegate(ItemStats x, ItemMod m)
				{
					x.AddToMod(stat, (float)(m.Value1 + m.Value2) / 2f);
				};
			}
			private ItemStats.StatTranslator.AddStat Dual(ItemStat s1, ItemStat s2)
			{
				return delegate(ItemStats x, ItemMod m)
				{
					x.AddToMod(s1, (float)m.Value1);
					x.AddToMod(s2, (float)m.Value2);
				};
			}
			private ItemStats.StatTranslator.AddStat MultipleSame(params ItemStat[] stats)
			{
				return delegate(ItemStats x, ItemMod m)
				{
					ItemStat[] stats2 = stats;
					for (int i = 0; i < stats2.Length; i++)
					{
						ItemStat stat = stats2[i];
						x.AddToMod(stat, (float)m.Value1);
					}
				};
			}
		}
		protected Poe.Entity item;
		protected float[] stats;
		private static ItemStats.StatTranslator translate;
		public ItemStats(Poe.Entity item)
		{
			this.item = item;
			if (ItemStats.translate == null)
			{
				ItemStats.translate = new ItemStats.StatTranslator();
			}
			this.stats = new float[Enum.GetValues(typeof(ItemStat)).Length];
			this.ParseSockets();
			this.ParseExplicitMods();
			if (item.HasComponent<Weapon>())
			{
				this.ParseWeaponStats();
			}
		}
		private void ParseWeaponStats()
		{
			Weapon component = this.item.GetComponent<Weapon>();
			float num = (float)(component.DamageMin + component.DamageMax) / 2f + this.GetStat(ItemStat.LocalPhysicalDamage);
			num *= 1f + (this.GetStat(ItemStat.LocalPhysicalDamagePercent) + (float)this.item.GetComponent<Quality>().ItemQuality) / 100f;
			this.AddToMod(ItemStat.AveragePhysicalDamage, num);
			float num2 = 1f / ((float)component.AttackTime / 1000f);
			num2 *= 1f + this.GetStat(ItemStat.LocalAttackSpeed) / 100f;
			this.AddToMod(ItemStat.AttackPerSecond, num2);
			float num3 = (float)component.CritChance / 100f;
			num3 *= 1f + this.GetStat(ItemStat.LocalCritChance) / 100f;
			this.AddToMod(ItemStat.WeaponCritChance, num3);
			float num4 = this.GetStat(ItemStat.LocalAddedColdDamage) + this.GetStat(ItemStat.LocalAddedFireDamage) + this.GetStat(ItemStat.LocalAddedLightningDamage);
			this.AddToMod(ItemStat.AverageElementalDamage, num4);
			this.AddToMod(ItemStat.DPS, (num + num4) * num2);
			this.AddToMod(ItemStat.PhysicalDPS, num * num2);
		}
		private void ParseArmorStats()
		{
		}
		private void ParseExplicitMods()
		{
			foreach (ItemMod current in this.item.GetComponent<Mods>().ItemMods)
			{
				ItemStats.translate.Translate(this, current);
			}
			this.AddToMod(ItemStat.ElementalResistance, this.GetStat(ItemStat.LightningResistance) + this.GetStat(ItemStat.FireResistance) + this.GetStat(ItemStat.ColdResistance));
			this.AddToMod(ItemStat.TotalResistance, this.GetStat(ItemStat.ElementalResistance) + this.GetStat(ItemStat.TotalResistance));
		}
		private void ParseSockets()
		{
		}
		private void AddToMod(ItemStat stat, float value)
		{
			this.stats[(int)stat] += value;
		}
		public float GetStat(ItemStat stat)
		{
			return this.stats[(int)stat];
		}
		public ItemType GetSlot()
		{
			return ItemType.All;
		}
	}
}

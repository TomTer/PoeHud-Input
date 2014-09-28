using System;

namespace ExileHUD.ExileBot
{
	public class AreaInstance
	{
		public readonly int RealLevel;
		public readonly int NominalLevel;
		public readonly String Name;
		public readonly int Act;
		public readonly bool IsTown;
		public readonly bool HasWaypoint;
		public readonly int Hash;

		public TimeSpan TimeSpent = new TimeSpan(0);

		public AreaInstance(Poe_Area area, int hash, int realLevel)
		{
			this.Hash = hash;
			this.RealLevel = realLevel;
			this.NominalLevel = area.NominalLevel;
			this.Name = area.Name;
			this.Act = area.Act;
			this.IsTown = area.IsTown;
			this.HasWaypoint = area.HasWaypoint;
		}

		public override string ToString()
		{
			return String.Format("{0} ({1}) #{2}", Name, RealLevel, Hash);
		}

		public string DisplayName { get { return String.Concat(Name, " (", RealLevel, ")"); } }

		internal void AddTimeSpent(TimeSpan delta)
		{
			TimeSpent += delta;
		}

		public string TimeString {
			get {
				int allsec = (int)TimeSpent.TotalSeconds;
				int secs = allsec % 60;
				int mins = allsec / 60;
				int hours = mins / 60;
				mins = mins % 60;
				return String.Format(hours > 0 ? "{0}:{1:00}:{2:00}" : "{1}:{2:00}", hours, mins, secs);
			}
		}
	}
}
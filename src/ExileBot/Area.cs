namespace ExileHUD.ExileBot
{
	public class Area
	{
		private int lastAddress;
		private Poe_Area current;
		private PathOfExile Poe;
		public event AreaChangeEvent OnAreaChange;
		public string RawName
		{
			get
			{
				if (this.current != null)
				{
					return this.current.RawName;
				}
				return "";
			}
		}
		public string Name
		{
			get
			{
				if (this.current != null)
				{
					return this.current.Name;
				}
				return "";
			}
		}
		public int Act
		{
			get
			{
				if (this.current != null)
				{
					return this.current.Act;
				}
				return 1;
			}
		}
		public bool IsTown
		{
			get
			{
				return this.current != null && this.current.IsTown;
			}
		}
		public bool HasWaypoint
		{
			get
			{
				return this.current != null && this.current.HasWaypoint;
			}
		}
		public Area(PathOfExile poe)
		{
			this.Poe = poe;
			poe.OnUpdate += new UpdateEvent(this.poe_OnUpdate);
		}
		private void poe_OnUpdate()
		{
			Poe_Area currentArea = this.Poe.Internal.game.IngameState.Data.CurrentArea;
			if (currentArea.address == this.lastAddress)
			{
				return;
			}
			this.current = currentArea;
			if (this.OnAreaChange != null)
			{
				this.OnAreaChange(this);
			}
			this.lastAddress = this.current.address;
		}
		public bool TestString(string s)
		{
			return this.RawName == s || this.RawName.StartsWith(s) || s == this.Name;
		}
	}
}

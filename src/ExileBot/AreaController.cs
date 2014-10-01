using System;
using PoeHUD.Poe;

namespace PoeHUD.ExileBot
{
	public class AreaController
	{
		public int Level { get; private set; }
		public int Hash { get; private set; }
		private PathOfExile Poe;
		public event Action<AreaController> OnAreaChange;

		public AreaInstance CurrentArea { get; private set; }

		// dict is wrong 'cause hash is wrong
		// public Dictionary<int, AreaInstance> AreasVisited = new Dictionary<int, AreaInstance>();

		public AreaController(PathOfExile poe)
		{
			this.Poe = poe;
			poe.OnUpdate += this.poe_OnUpdate;
		}
		private void poe_OnUpdate()
		{
			var igsd = this.Poe.Internal.game.IngameState.Data;
			AreaTemplate clientsArea = igsd.CurrentArea;
			int curAreaHash = igsd.CurrentAreaHash;

			if (CurrentArea != null && curAreaHash == CurrentArea.Hash)
				return;

			// try to find the new area in our dictionary
			AreaInstance //area;
			//if (!AreasVisited.TryGetValue(curAreaHash, out area)) {
				area = new AreaInstance(clientsArea, curAreaHash, igsd.CurrentAreaLevel);
			// }

			CurrentArea = area;

			this.OnAreaChange(this);
		}
	}
}

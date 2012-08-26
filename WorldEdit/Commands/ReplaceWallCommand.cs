using System;
using Terraria;
using TShockAPI;

namespace WorldEdit.Commands
{
	public class ReplaceWallCommand : WECommand
	{
		private byte wall1;
		private byte wall2;

		public ReplaceWallCommand(int x, int y, int x2, int y2, int plr, byte wall1, byte wall2) :
			base(x, y, x2, y2, plr)
		{
			this.wall1 = wall1;
			this.wall2 = wall2;
		}

		public override void Execute()
		{
			Tools.PrepareUndo(x, y, x2, y2, plr);
			int edits = 0;
			if (wall1 != wall2)
			{
				for (int i = x; i <= x2; i++)
				{
					for (int j = y; j <= y2; j++)
					{
						if (selectFunc(i, j, plr) && Main.tile[i, j].wall == wall1)
						{
							Main.tile[i, j].wall = wall2;
							edits++;
						}
					}
				}
				ResetSection();
			}

			string wallName1 = wall1 == 0 ? "air" : "wall " + wall1;
			string wallName2 = wall2 == 0 ? "air" : "wall " + wall2;
			TShock.Players[plr].SendMessage(String.Format("Replaced {0} with {1}. ({2})",
				wallName1, wallName2, edits), Color.Green);
		}
	}
}
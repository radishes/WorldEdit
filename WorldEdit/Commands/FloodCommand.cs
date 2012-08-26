using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Terraria;
using TShockAPI;

namespace WorldEdit.Commands
{
	public class FloodCommand : WECommand
	{
		private bool lava;

		public FloodCommand(int x, int y, int x2, int y2, int plr, bool lava)
			: base(x, y, x2, y2, plr)
		{
			this.lava = lava;
		}

		public override void Execute()
		{
			Tools.PrepareUndo(x, y, x2, y2, plr);
			int edits = 0;
			for (int i = x; i <= x2; i++)
			{
				for (int j = y; j <= y2; j++)
				{
					if (!Main.tile[i, j].active || !Main.tileSolid[Main.tile[i, j].type])
					{
						Main.tile[i, j].lava = lava;
						Main.tile[i, j].liquid = 255;
						edits++;
					}
				}
			}
			ResetSection();
			TShock.Players[plr].SendMessage(String.Format("Flooded nearby area. ({0})", edits), Color.Green);
		}
	}
}
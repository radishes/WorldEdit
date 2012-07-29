using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Terraria;
using TShockAPI;

namespace WorldEdit.Commands
{
    public class SetWallCommand : WECommand
    {
        public SetWallCommand(int x, int y, int x2, int y2, int plr, byte wall)
            : base(x, y, x2, y2, plr)
        {
            data = wall;
        }

        public override void Execute()
        {
            Tools.PrepareUndo(x, y, x2, y2, plr);
            int edits = 0;
            for (int i = x; i <= x2; i++)
            {
                for (int j = y; j <= y2; j++)
                {
                    if (selectFunc(i, j, plr) && Main.tile[i, j].wall != data)
                    {
                        Main.tile[i, j].wall = data;
                        TSPlayer.All.SendTileSquare(i, j, 1);
                        edits++;
                    }
                }
            }

            string wallName = "wall " + data;
            if (data == 0)
            {
                wallName = "air";
            }
            TShock.Players[plr].SendMessage(String.Format("Set walls to {0}. ({1})", wallName, edits), Color.Yellow);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Terraria;
using TShockAPI;

namespace WorldEdit.Commands
{
    public class ReplaceWallCommand : WECommand
    {
        public ReplaceWallCommand(int x, int y, int x2, int y2, int plr, byte wall1, byte wall2) :
            base(x, y, x2, y2, plr)
        {
            data = wall1;
            data2 = wall2;
        }

        public override void Execute()
        {
            Tools.PrepareUndo(x, y, x2, y2, plr);
            int edits = 0;
            if (data != data2)
            {
                for (int i = x; i <= x2; i++)
                {
                    for (int j = y; j <= y2; j++)
                    {
                        if (selectFunc(i, j, plr) && Main.tile[i, j].wall == data)
                        {
                            Main.tile[i, j].wall = data2;
                            TSPlayer.All.SendTileSquare(i, j, 1);
                            edits++;
                        }
                    }
                }
            }

            string wallName1 = "wall " + data;
            if (data == 0)
            {
                wallName1 = "air";
            }
            string wallName2 = "wall " + data2;
            if (data2 == 0)
            {
                wallName2 = "air";
            }
            TShock.Players[plr].SendMessage(String.Format("Replaced {0} with {1}. ({2})", wallName1, wallName2, edits), Color.Yellow);
        }
    }
}

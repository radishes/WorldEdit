using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Terraria;
using TShockAPI;

namespace WorldEdit.Commands
{
    public class ReplaceCommand : WECommand
    {
        private static string[] SpecialTileNames = { "air", "lava", "water", "wire", "no wire" };

        public ReplaceCommand(int x, int y, int x2, int y2, int plr, byte tile1, byte tile2)
            : base(x, y, x2, y2, plr)
        {
            data = tile1;
            data2 = tile2;
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
                        if (selectFunc(i, j, plr) &&
                            ((data < 149 && Main.tile[i, j].active && Main.tile[i, j].type == data)
                            || (data == 149 && !Main.tile[i, j].active)
                            || (data == 150 && Main.tile[i, j].lava && Main.tile[i, j].liquid > 0)
                            || (data == 151 && !Main.tile[i, j].lava && Main.tile[i, j].liquid > 0)
                            || (data == 152 && Main.tile[i, j].wire)))
                        {
                            SetTile(i, j, data2);
                            TSPlayer.All.SendTileSquare(i, j, 1);
                            edits++;
                        }
                    }
                }
            }

            string tileName1 = "tile " + data;
            if (data > 148)
            {
                tileName1 = SpecialTileNames[data - 149];
            }
            string tileName2 = "tile " + data2;
            if (data2 > 148)
            {
                tileName2 = SpecialTileNames[data2 - 149];
            }
            TShock.Players[plr].SendMessage(String.Format("Replaced {0} with {1}. ({2})", tileName1, tileName2, edits), Color.Yellow);
        }
    }
}

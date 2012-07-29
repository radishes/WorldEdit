using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Terraria;
using TShockAPI;

namespace WorldEdit.Commands
{
    public class SetCommand : WECommand
    {
        private static string[] SpecialTileNames = { "air", "lava", "water", "wire", "no wire" };

        public SetCommand(int x, int y, int x2, int y2, int plr, byte tile)
            : base(x, y, x2, y2, plr)
        {
            data = tile;
        }

        public override void Execute()
        {
            Tools.PrepareUndo(x, y, x2, y2, plr);
            int edits = 0;
            for (int i = x; i <= x2; i++)
            {
                for (int j = y; j <= y2; j++)
                {
                    if (selectFunc(i, j, plr) &&
                        ((data < 149 && (!Main.tile[i, j].active || Main.tile[i, j].type != data))
                        || (data == 149 && (Main.tile[i, j].active || Main.tile[i, j].liquid > 0))
                        || (data == 150)
                        || (data == 151)
                        || (data == 152 && !Main.tile[i, j].wire)
                        || (data == 153 && Main.tile[i, j].wire)))
                    {
                        SetTile(i, j, data);
                        TSPlayer.All.SendTileSquare(i, j, 1);
                        edits++;
                    }
                }
            }

            string tileName = "tile " + data;
            if (data >= 149)
            {
                tileName = SpecialTileNames[data - 149];
            }
            TShock.Players[plr].SendMessage(String.Format("Set tiles to {0}. ({1})", tileName, edits), Color.Yellow);
        }
    }
}

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Terraria;
using TShockAPI;

namespace WorldEdit.Commands
{
    public class CopyCommand : WECommand
    {
        public CopyCommand(int x, int y, int x2, int y2, int plr)
            : base(x, y, x2, y2, plr)
        {
        }

        public override void Execute()
        {
            int xLen = x2 - x + 1;
            int yLen = y2 - y + 1;
            Tile[,] tiles = new Tile[xLen, yLen];
            for (int i = 0; i < xLen; i++)
            {
                for (int j = 0; j < yLen; j++)
                {
                    tiles[i, j] = Main.tile[i + x, j + y];
                }
            }

            Tools.SaveClipboard(tiles, plr);
            TShock.Players[plr].SendMessage(String.Format("Copied selection to clipboard. ({0})", xLen * yLen), Color.Yellow);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Terraria;
using TShockAPI;

namespace WorldEdit.Commands
{
    public class FlipCommand : WECommand
    {
        public FlipCommand(int plr, byte direction)
            : base(0, 0, 0, 0, plr)
        {
            data = direction;
        }

        public override void Execute()
        {
            Tile[,] tiles = Tools.LoadClipboard(plr);
            int maxX = tiles.GetLength(0);
            int maxY = tiles.GetLength(1);
            int realI = 0;
            int realJ = 0;
            Tile[,] flippedTiles = new Tile[maxX, maxY];

            for (int i = 0; i < maxX; i++)
            {
                for (int j = 0; j < maxY; j++)
                {
                    realI = (data & 1) == 1 ? maxX - i - 1 : i;
                    realJ = (data & 2) == 2 ? maxY - j - 1 : j;
                    flippedTiles[realI, realJ] = tiles[i, j];
                }
            }
            Tools.SaveClipboard(flippedTiles, plr);
            TShock.Players[plr].SendMessage(String.Format("Flipped clipboard. ({0})", maxX * maxY), Color.Yellow);
        }
    }
}

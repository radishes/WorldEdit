using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Terraria;
using TShockAPI;

namespace WorldEdit.Commands
{
    public class PasteCommand : WECommand
    {
        public PasteCommand(int x, int y, int plr)
            : base(x, y, 0, 0, plr)
        {
        }

        public override void Execute()
        {
            Tile[,] tiles = Tools.LoadClipboard(plr);
            int xLen = tiles.GetLength(0);
            int yLen = tiles.GetLength(1);
            Tools.PrepareUndo(x, y, x + xLen, y + yLen, plr);

            for (int i = 0; i < xLen; i++)
            {
                for (int j = 0; j < yLen; j++)
                {
                    Main.tile[i + x, j + y] = tiles[i, j];
                }
            }
            ResetSection();
            TShock.Players[plr].SendMessage(String.Format("Pasted clipboard to selection. ({0})", xLen * yLen), Color.Yellow);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Terraria;
using TShockAPI;

namespace WorldEdit.Commands
{
    public class FixGrassCommand : WECommand
    {
        public FixGrassCommand(int x, int y, int x2, int y2, int plr)
            : base(x, y, x2, y2, plr)
        {
        }

        public override void Execute()
        {
            Tools.PrepareUndo(x, y, x2, y2, plr);
            int edits = 0;
            for (int i = x; i <= x2; i++)
            {
                for (int j = y; j <= y2; j++)
                {
                    if (Main.tile[i, j].type == 2 || Main.tile[i, j].type == 23 || Main.tile[i, j].type == 60)
                    {
                        if (TileSolid(i - 1, j - 1) && TileSolid(i - 1, j) && TileSolid(i - 1, j + 1)
                            && TileSolid(i, j - 1) && TileSolid(i, j + 1)
                            && TileSolid(i + 1, j) && TileSolid(i + 1, j) && TileSolid(i + 1, j + 1))
                        {
                            Main.tile[i, j].type = (Main.tile[i, j].type == 60) ? (byte)59 : (byte)0;
                            edits++;
                        }
                    }
                }
            }
            ResetSection();
            TShock.Players[plr].SendMessage(String.Format("Fixed nearby grass. ({0})", edits), Color.Green);
        }
        private bool TileSolid(int x, int y)
        {
            if (x < 0 || y < 0 || x >= Main.maxTilesX || y >= Main.maxTilesY)
            {
                return true;
            }
            return Main.tile[x, y].active && Main.tileSolid[Main.tile[x, y].type];
        }
    }
}

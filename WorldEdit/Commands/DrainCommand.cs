using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Terraria;
using TShockAPI;

namespace WorldEdit.Commands
{
    public class DrainCommand : WECommand
    {
        public DrainCommand(int x, int y, int x2, int y2, int plr)
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
                    if (Main.tile[i, j].liquid > 0)
                    {
                        Main.tile[i, j].lava = false;
                        Main.tile[i, j].liquid = 0;
                        edits++;
                    }
                }
            }
            ResetSection();
            TShock.Players[plr].SendMessage(String.Format("Drained nearby area. ({0})", edits), Color.Green);
        }
    }
}

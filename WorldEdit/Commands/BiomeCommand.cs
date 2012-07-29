using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Terraria;
using TShockAPI;

namespace WorldEdit.Commands
{
    public class BiomeCommand : WECommand
    {
        public BiomeCommand(int x, int y, int x2, int y2, int plr, byte biome1, byte biome2)
            : base(x, y, x2, y2, plr)
        {
            data = biome1;
            data2 = biome2;
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
                        if (selectFunc(i, j, plr) && Main.tile[i, j].active)
                        {
                            for (int k = 0; k < WorldEdit.BiomeConversions[data].Length; k++)
                            {
                                if (Main.tile[i, j].type == WorldEdit.BiomeConversions[data][k])
                                {
                                    if (WorldEdit.BiomeConversions[data2][k] == 255)
                                    {
                                        Main.tile[i, j].active = false;
                                        Main.tile[i, j].type = 0;
                                    }
                                    else
                                    {
                                        Main.tile[i, j].type = WorldEdit.BiomeConversions[data2][k];
                                    }
                                    edits++;
                                    break;
                                }
                            }
                        }
                    }
                }
                ResetSection();
            }
            string msg = String.Format("Converted {0} to {1}. ({2})", WorldEdit.BiomeNames[data], WorldEdit.BiomeNames[data2], edits);
            TShock.Players[plr].SendMessage(msg, Color.Yellow);
        }
    }
}

using System;
using Terraria;
using TShockAPI;

namespace WorldEdit.Commands
{
    public class SetCommand : WECommand
    {
        private static string[] SpecialTileNames = { "air", "lava", "water", "wire", "no wire" };

        private byte tile;

        public SetCommand(int x, int y, int x2, int y2, int plr, byte tile)
            : base(x, y, x2, y2, plr)
        {
            this.tile = tile;
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
                        ((tile < 149 && (!Main.tile[i, j].active || Main.tile[i, j].type != tile))
                        || (tile == 149 && (Main.tile[i, j].active || Main.tile[i, j].liquid > 0))
                        || (tile == 150)
                        || (tile == 151)
                        || (tile == 152 && !Main.tile[i, j].wire)
                        || (tile == 153 && Main.tile[i, j].wire)))
                    {
                        SetTile(i, j, tile);
                        edits++;
                    }
                }
            }
            ResetSection();

            string tileName = "tile " + tile;
            if (tile >= 149)
            {
                tileName = SpecialTileNames[tile - 149];
            }
            TShock.Players[plr].SendMessage(String.Format("Set tiles to {0}. ({1})", tileName, edits), Color.Green);
        }
    }
}

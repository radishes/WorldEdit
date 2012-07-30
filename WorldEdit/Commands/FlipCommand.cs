using System;
using System.IO;
using Terraria;
using TShockAPI;

namespace WorldEdit.Commands
{
    public class FlipCommand : WECommand
    {
        private byte direction;

        public FlipCommand(int plr, byte direction)
            : base(0, 0, 0, 0, plr)
        {
            this.direction = direction;
        }

        public override void Execute()
        {
            string clipboardPath = Path.Combine("worldedit", String.Format("clipboard-{0}.dat", plr));
            Tile[,] tiles = Tools.LoadWorldData(clipboardPath);
            int lenX = tiles.GetLength(0);
            int lenY = tiles.GetLength(1);
            bool flipX = (direction & 1) == 1;
            bool flipY = (direction & 2) == 2;
            int endX = flipX ? -1 : lenX;
            int endY = flipY ? -1 : lenY;
            int incX = flipX ? -1 : 1;
            int incY = flipY ? -1 : 1;

            using (BinaryWriter writer = new BinaryWriter(new FileStream(clipboardPath, FileMode.Create)))
            {
                writer.Write(lenX);
                writer.Write(lenY);
                for (int i = flipX ? lenX - 1 : 0; i != endX; i += incX)
                {
                    for (int j = flipY ? lenY - 1 : 0; j != endY; j += incY)
                    {
                        Tools.WriteTile(writer, tiles[i, j]);
                    }
                }
            }
            TShock.Players[plr].SendMessage(String.Format("Flipped clipboard."), Color.Green);
        }
    }
}

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Terraria;
using TShockAPI;

namespace WorldEdit.Commands
{
    public class SaveCommand : WECommand
    {
        private string name;

        public SaveCommand(int plr, string name)
            : base(0, 0, 0, 0, plr)
        {
            this.name = name;
        }

        public override void Execute()
        {
            string path = Path.Combine("worldedit", String.Format("{0}.schematic", name));
            Tile[,] tiles = Tools.LoadClipboard(plr);
            Tools.SaveWorldData(tiles, path);
            TShock.Players[plr].SendMessage(String.Format("Saved clipboard to schematic \"{0}\".", name), Color.Green);
        }
    }
}

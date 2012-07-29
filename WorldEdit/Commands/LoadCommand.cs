using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Terraria;
using TShockAPI;

namespace WorldEdit.Commands
{
    public class LoadCommand : WECommand
    {
        private string name;

        public LoadCommand(int plr, string name)
            : base(0, 0, 0, 0, plr)
        {
            this.name = name;
        }

        public override void Execute()
        {
            string path = Path.Combine("worldedit", String.Format("{0}.schematic", name));
            Tile[,] tiles = Tools.LoadWorldData(path);
            Tools.SaveClipboard(tiles, plr);
            TShock.Players[plr].SendMessage(String.Format("Loaded schematic \"{0}\" to clipboard.", name), Color.Green);
        }
    }
}

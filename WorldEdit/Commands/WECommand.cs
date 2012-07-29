using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Terraria;
using TShockAPI;

namespace WorldEdit.Commands
{
    public abstract class WECommand
    {
        public byte data;
        public byte data2;
        public int plr;
        public Func<int, int, int, bool> selectFunc = (x, y, plr) => true;
        public int x;
        public int x2;
        public int y;
        public int y2;

        public WECommand(int x, int y, int x2, int y2, int plr)
        {
            this.plr = plr;
            int select = WorldEdit.Players[plr].select;
            if (select >= 0)
            {
                selectFunc = WorldEdit.Selections[select];
            }
            this.x = x;
            this.x2 = x2;
            this.y = y;
            this.y2 = y2;
        }

        public abstract void Execute();
        public void ResetSection()
        {
            int lowX = Netplay.GetSectionX(x);
            int highX = Netplay.GetSectionX(x2);
            int lowY = Netplay.GetSectionY(y);
            int highY = Netplay.GetSectionY(y2);
            foreach (ServerSock sock in Netplay.serverSock)
            {
                for (int i = lowX; i <= highX; i++)
                {
                    for (int j = lowY; j <= highY; j++)
                    {
                        sock.tileSection[i, j] = false;
                    }
                }
            }
        }
        public void SetTile(int i, int j, byte tile)
        {
            switch (tile)
            {
                case 149:
                    Main.tile[i, j].active = false;
                    Main.tile[i, j].lava = false;
                    Main.tile[i, j].liquid = 0;
                    Main.tile[i, j].type = 0;
                    break;
                case 150:
                    Main.tile[i, j].active = false;
                    Main.tile[i, j].lava = true;
                    Main.tile[i, j].liquid = 255;
                    Main.tile[i, j].type = 0;
                    break;
                case 151:
                    Main.tile[i, j].active = false;
                    Main.tile[i, j].lava = false;
                    Main.tile[i, j].liquid = 255;
                    Main.tile[i, j].type = 0;
                    break;
                case 152:
                    Main.tile[i, j].wire = true;
                    break;
                case 153:
                    Main.tile[i, j].wire = false;
                    break;
                default:
                    Main.tile[i, j].active = true;
                    Main.tile[i, j].lava = false;
                    Main.tile[i, j].liquid = 0;
                    Main.tile[i, j].type = tile;
                    break;
            }
        }
    }
}

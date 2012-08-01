using System;
using Terraria;

namespace WorldEdit.Commands
{
    public abstract class WECommand
    {
        protected int plr;
        protected Func<int, int, int, bool> selectFunc = (x, y, plr) => true;
        protected int x;
        protected int x2;
        protected int y;
        protected int y2;

        protected WECommand(int x, int y, int x2, int y2, int plr)
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

        public void Clamp()
        {
            if (x < 0)
            {
                x = 0;
            }
            if (y < 0)
            {
                y = 0;
            }
            if (x2 >= Main.maxTilesX)
            {
                x2 = Main.maxTilesX - 1;
            }
            if (y2 >= Main.maxTilesY)
            {
                y2 = Main.maxTilesY - 1;
            }
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
                    Main.tile[i, j].frameX = -1;
                    Main.tile[i, j].frameY = -1;
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
                    Main.tile[i, j].frameX = -1;
                    Main.tile[i, j].frameY = -1;
                    Main.tile[i, j].lava = false;
                    Main.tile[i, j].liquid = 0;
                    Main.tile[i, j].type = tile;
                    break;
            }
        }
    }
}

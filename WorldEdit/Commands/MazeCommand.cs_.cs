using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Terraria;
using TShockAPI;


namespace WorldEdit.Commands
{

    public class MazeCommand : WECommand
    {
        public MazeCommand(int x, int y, int x2, int y2, int plr)
            : base(x, y, x2, y2, plr)
        {
        }

        public override void Execute()
        {
            Tools.PrepareUndo(x, y, x2, y2, plr);
            int plr_x = TShock.Players[plr].TileX;
            int plr_y = TShock.Players[plr].TileY;
            TShock.Players[plr].SendMessage(String.Format("Maze creation initiated at {0},{1}", plr_x, plr_y), Color.Green);


            Stack cellStack = new Stack();
            cellStack.Push(new Point(plr_x, plr_y));

            List<Point> visitedCells = new List<Point>(new Point[] {  });

            int size = 3;
            int wallSize = 1;
            int totalSize = wallSize + size;
            Point up = new Point(0, -totalSize);
            Point down = new Point(0, totalSize);
            Point left = new Point(-totalSize, 0);
            Point right = new Point(totalSize, 0);
            List<Point> dirs = new List<Point>(new Point[] { up, right, down, left });

            int loops = 0;
            System.IO.StreamWriter file = new System.IO.StreamWriter("maze_debug.txt", true);
            file.WriteLine(String.Format("========= Starting Maze ========"));
            Random rng = new Random();
            while (cellStack.Count > 0)
            {
                // this is my attempt at a "recursive backtracker" maze algorithm
                // see http://www.astrolog.org/labyrnth/algrithm.htm
                file.WriteLine(String.Format("\ncellStack.count: {0}", cellStack.Count));
                if (loops++ > 1000000)
                {
                    TShock.Players[plr].SendMessage(String.Format("Exceeded maximum loop iterations; exiting maze creation loop."), Color.Red);
                    break;
                }

                // 0 = up; 1 = right; 2 = down; 3 = left
                List<int> rndDirs = new List<int>(new int[] { 0, 1, 2, 3 });
                Tools.Shuffle(rndDirs, rng);
                string ddd = "";
                foreach (int zz in rndDirs)
                {
                    ddd += ". " + zz.ToString();
                }

                file.WriteLine(String.Format("Random directions list is now: {0}", ddd));

                Point p = (Point)cellStack.Pop();
                visitedCells.Add(new Point(p.X, p.Y));
                file.WriteLine(String.Format("* Currently at ({0},{1})", p.X, p.Y));

                foreach (int d in rndDirs)
                {
                    Point p2 = new Point(p.X+dirs[d].X, p.Y+dirs[d].Y); // sample tile in a random adjacent cell

                    file.WriteLine(String.Format("Trying direction: {0}; p2 = {1}, {2}", d, p2.X, p2.Y));
                    // look at a sample tile in the currently selected random direction
                    bool cnt = false; // continue flag
                    foreach (Point c in visitedCells)
                    {
                        if (Tools.ArePointsEqual(c, p2))
                        {
                            file.WriteLine(String.Format("Tried direction {0} but found it already visited. Trying next direction.", d));
                            cnt = true;
                            break;
                        }   
                    }
                    if (cnt)
                        continue;
                    if (Tools.PointInRect(x, y, x2-(totalSize*2), y2-(totalSize*2), p2) && Main.tile[p2.X, p2.Y].active )

                    { // a valid unvisited cell, begin excavation
                        //TShock.Players[plr].SendMessage(String.Format("SELECTED direction {0}", d), Color.Red);
                        file.WriteLine(String.Format("Selected direction {0}!", d));
                        Point p3 = new Point(p2.X, p2.Y); // amount to adjust cell by to create "doorway" between cells
                        Point offset = new Point();
                        if (d == 0)
                        {
                            offset.X = size;
                            offset.Y = totalSize;
                        }
                        if (d == 1)
                        {
                            p3.X -= wallSize;
                            offset.X = totalSize;
                            offset.Y = size;
                        }
                        if (d == 2)
                        {
                            p3.Y -= wallSize;
                            offset.X = size;
                            offset.Y = totalSize;
                        }
                        if (d == 3)
                        {
                            offset.X = totalSize;
                            offset.Y = size;
                        }

                        file.WriteLine(String.Format("Destroying tiles in direction {0}.", d));
                        for (int i = p2.X; i < p2.X + offset.X; i++)
                        {
                            for (int j = p2.Y; j < p2.Y + offset.Y; j++)
                            {
                                //file.WriteLine(String.Format("Destroying tiles in direction {0} ({1},{2}).", d, i, j));
                                Main.tile[i, j].active = false;
                                Main.tile[i, j].type = 0;
                                //Main.tile[i, j].lava = false;
                                //Main.tile[i, j].liquid = 0;
                                //Main.tile[i, j].wall = 0;
                                //Main.tile[i, j].wire = false;
                            }
                        }
                        file.WriteLine(String.Format("Pushing onto stack.", d));
                        cellStack.Push(p);
                        cellStack.Push(p2); // push next cell onto stack
                        break; // next cell has been found; moving on to it now
                    }
                     
                } // if we don't find an available direction to travel in, just fall through.
                // we've already popped the current cell, so we're taking a step back to the previous cell.
                
            }
            TShock.Players[plr].SendMessage(String.Format("Maze creation complete."), Color.LimeGreen);
            ResetSection();
            file.Close();
        }

    }
}

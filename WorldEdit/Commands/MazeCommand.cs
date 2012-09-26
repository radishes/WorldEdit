// maze module added by radishes

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Terraria;
using TShockAPI;


namespace WorldEdit.Commands
{

    public struct Rect
    {
        public Point p;
        public Point offset;

        public Rect(Point p, Point offset)
        {
            this.p = p;
            this.offset = offset;
        }
    }

    public class MazeCommand : WECommand
    {

        private int tunnelWidth = 3;
        private int wallWidth = 3;
        private int totalWidth;
        private int algorithm = 0;
        private int parameter = 0;

        private Point up, down, left, right;
        private List<Point> dirs;

        private Random rng;


        public MazeCommand(int x, int y, int x2, int y2, int plr, int tunnelWidth, int wallWidth, int algorithm, int parameter)
            : base(x, y, x2, y2, plr)
        {
            this.tunnelWidth = tunnelWidth;
            this.wallWidth = wallWidth;
            this.algorithm = algorithm;
            this.parameter = parameter;

            this.totalWidth = this.wallWidth + this.tunnelWidth;
            this.up = new Point(0, -this.totalWidth);
            this.down = new Point(0, this.totalWidth);
            this.left = new Point(-this.totalWidth, 0);
            this.right = new Point(this.totalWidth, 0);
            this.dirs = new List<Point>(new Point[] { this.up, this.right, this.down, this.left });

            this.rng = new Random();
        }
        
        public override void Execute()
        {
            // assumes x,y and x2,y2 have been normalized so that x,y is top-left
            Tools.PrepareUndo(x, y, x2, y2, plr);

            switch (algorithm)
            {
                case 0:
                    TShock.Players[plr].SendMessage(String.Format("0: Recursive Backtracker maze creation initiated!"), Color.Green);
                    RecursiveBacktracker();
                    break;
                case 1:
                    TShock.Players[plr].SendMessage(String.Format("1: Growing Tree maze creation initiated!"), Color.Green);
                    GrowingTree();
                    break;
                default:
                    TShock.Players[plr].SendMessage(String.Format("Invalid algorithm specified."), Color.Red);
                    break;
            }
            TShock.Players[plr].SendMessage(String.Format("Maze creation complete."), Color.LimeGreen);
            ResetSection();

        } //Execute()


        private Rect MakeOffset(Point p, int dir)
        {
            Point p2 = new Point(p.X, p.Y); // amount to adjust cell by to create "doorway" between cells
            Point offset = new Point();
            if (dir == 0)
            {
                offset.X = this.tunnelWidth;
                offset.Y = this.totalWidth;
            }
            if (dir == 1)
            {
                p2.X -= this.wallWidth;
                offset.X = this.totalWidth;
                offset.Y = this.tunnelWidth;
            }
            if (dir == 2)
            {
                p2.Y -= this.wallWidth;
                offset.X = this.tunnelWidth;
                offset.Y = this.totalWidth;
            }
            if (dir == 3)
            {
                offset.X = this.totalWidth;
                offset.Y = this.tunnelWidth;
            }
            Rect rect = new Rect(p2, offset);
            rect.p = p2;
            rect.offset = offset;
            return rect;
        }

        private void NukeTiles(Point p, Point offset)
        {
            for (int i = p.X; i < p.X + offset.X; i++)
            {
                for (int j = p.Y; j < p.Y + offset.Y; j++)
                {
                    Main.tile[i, j].active = false;
                    Main.tile[i, j].type = 0;
                    //Main.tile[i, j].lava = false;
                    //Main.tile[i, j].liquid = 0;
                    //Main.tile[i, j].wall = 0;
                    //Main.tile[i, j].wire = false;
                    }
            }
        }

        private bool TestPointForTunnel(Point p)
        { // check that the point is within the selected area, and is active (has a tile on it)
            return (Tools.PointInRect(x, y, x2 - (this.totalWidth * 2), y2 - (this.totalWidth * 2), p) && Main.tile[p.X, p.Y].active);
        }

        private Point MoveCell(Point currentCell, Point moveOffset)
        {
            return new Point(currentCell.X + moveOffset.X, currentCell.Y + moveOffset.Y);
        }

        private void GrowingTree()
        { // "Growing Tree" maze algorithm.
            // see http://www.astrolog.org/labyrnth/algrithm.htm
            int plr_x = TShock.Players[plr].TileX;
            int plr_y = TShock.Players[plr].TileY;
            List<Point> visitedCells = new List<Point>(new Point[] { });
            visitedCells.Add(new Point(plr_x, plr_y));
            Point p = visitedCells.First();
            int totalWidth = this.tunnelWidth + this.wallWidth;
            NukeTiles(p, new Point(this.tunnelWidth, this.tunnelWidth)); // carve out starting cell
            bool spoken = false;

            System.IO.StreamWriter file = new System.IO.StreamWriter("maze_debug.txt", true);
            file.WriteLine(String.Format("========= Starting Maze ========"));
            file.WriteLine(String.Format("Parameters: {0} {1} {2} {3}", this.tunnelWidth, this.wallWidth, this.algorithm, this.parameter));

            while (visitedCells.Count() > 0)
            {
                int loops = 0;
                if (loops++ > 1000000)
                { // safety net
                    TShock.Players[plr].SendMessage(String.Format("Exceeded maximum loop iterations; aborting maze creation loop."), Color.Red);
                    break;
                }

                file.WriteLine(String.Format("----------"));

                switch (this.parameter)
                {
                    case 1: // take a random cell
                        p = visitedCells[rng.Next(0, visitedCells.Count())];
                        if (!spoken)
                            TShock.Players[plr].SendMessage(String.Format("Maze Variant 1: Random Cell"), Color.Green);
                        break;
                    case 2: // pick randomly from recent cells
                        int history = 10;
                        int count = visitedCells.Count();
                        int earliest;
                        if (count < history)
                            earliest = 0;
                        else
                            earliest = count - history;
                        p = visitedCells[rng.Next(earliest, count)];
                        if (!spoken)
                            TShock.Players[plr].SendMessage(String.Format("Maze Variant 2: Random Recent Cell"), Color.Green);
                        break;
                    case 3: // usually pick the newest cell, but sometimes pick randomly
                        double randomChance = 0.25; // 0.1 = 10% chance to pick a random cell
                        double dice = rng.NextDouble();
                        if (dice <= randomChance)
                            p = visitedCells[rng.Next(0, visitedCells.Count())];
                        else
                            p = visitedCells.Last();
                        if (!spoken)
                            TShock.Players[plr].SendMessage(String.Format("Maze Variant 3: Last Or Random"), Color.Green);
                        break;
                    case 4: // take the first cell (sucks)
                        p = visitedCells.First();
                        if (!spoken)
                            TShock.Players[plr].SendMessage(String.Format("Maze Variant 4: First Cell"), Color.Green);
                        break;
                    case 5: // take the middle cell (sucks)
                        p = visitedCells[(int)Math.Ceiling((float)(visitedCells.Count / 2))];
                        if (!spoken)
                            TShock.Players[plr].SendMessage(String.Format("Maze Variant 5: Middle Cell"), Color.Green);
                        break;
                    default: // take the last cell
                        // includes 0
                        p = visitedCells.Last();
                        if (!spoken)
                            TShock.Players[plr].SendMessage(String.Format("Maze Variant 0: Last Cell (default)"), Color.Green);
                        break;    
                }
                spoken = true;
                file.WriteLine(String.Format("*** p is ({0},{0})", p.X, p.Y));

                List<int> rndDirs = new List<int>(new int[] { 0, 1, 2, 3 });
                Tools.Shuffle(rndDirs, this.rng);

                ////*
                string ddd = "";
                foreach (int zz in rndDirs)
                    ddd += " - " + zz.ToString();
                file.WriteLine(String.Format("Random directions list is now: {0}", ddd));
                ///*/

                foreach (int d in rndDirs)
                {
                    file.WriteLine(String.Format("Trying direction {0}", d));
                    Point p2 = MoveCell(p, this.dirs[d]);

                    if (TestPointForTunnel(p2))
                    {
                        file.WriteLine(String.Format("Successfully tunnelled direction {0}!", d));
                        Rect rect = MakeOffset(p2, d);
                        NukeTiles(rect.p, rect.offset);
                        visitedCells.Add(p2);
                        break;
                    }
                    else
                    {
                        file.WriteLine(String.Format("Found direction {0} to be no good.", d));
                        if (d == rndDirs.Last()) // if no directions were tunnel-able
                        {
                            file.WriteLine(String.Format("BEFORE visitedCells.Count: {0}", visitedCells.Count()));
                            visitedCells.Remove(p); // remove this cell from the list
                            file.WriteLine(String.Format("Removed visited cell p -- {0},{1}", p.X, p.Y));
                            file.WriteLine(String.Format("AFTER visitedCells.Count: {0}", visitedCells.Count()));
                            break; // shouldn't matter but just in case
                        }
                    }
                }

            }
            file.Close();
            
        }

        private void RecursiveBacktracker()
        {
            // this is my attempt at a "recursive backtracker" maze algorithm
            // see http://www.astrolog.org/labyrnth/algrithm.htm
            int plr_x = TShock.Players[plr].TileX;
            int plr_y = TShock.Players[plr].TileY;
            Stack cellStack = new Stack();
            cellStack.Push(new Point(plr_x, plr_y));

            List<Point> visitedCells = new List<Point>(new Point[] { });


            int loops = 0;
            System.IO.StreamWriter file = new System.IO.StreamWriter("maze_debug.txt", true);
            file.WriteLine(String.Format("========= Starting Maze ========"));
            while (cellStack.Count > 0)
            {
                file.WriteLine(String.Format("cellStack.count: {0}", cellStack.Count));
                if (loops++ > 1000000)
                { // safety net
                    TShock.Players[plr].SendMessage(String.Format("Exceeded maximum loop iterations; aborting maze creation loop."), Color.Red);
                    break;
                }

                // 0 = up; 1 = right; 2 = down; 3 = left
                List<int> rndDirs = new List<int>(new int[] { 0, 1, 2, 3 });
                Tools.Shuffle(rndDirs, rng);
                string ddd = "";
                foreach (int zz in rndDirs)
                {
                    ddd += " - " + zz.ToString();
                }

                file.WriteLine(String.Format("Random directions list is now: {0}", ddd));

                Point p = (Point)cellStack.Pop();
                visitedCells.Add(new Point(p.X, p.Y));
                file.WriteLine(String.Format("*** Currently at ({0},{1})", p.X, p.Y));

                foreach (int d in rndDirs)
                {
                    Point p2 = MoveCell(p, this.dirs[d]); // sample tile in a random adjacent cell

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
                    if (TestPointForTunnel(p2))
                    { // a valid unvisited cell, begin excavation
                        file.WriteLine(String.Format("Selected direction {0}!", d));
                        Rect rect = MakeOffset(p2, d);


                        file.WriteLine(String.Format("Destroying tiles in direction {0}.", d));
                        NukeTiles(rect.p, rect.offset);
                        file.WriteLine(String.Format("Pushing onto stack.", d));
                        cellStack.Push(p);
                        cellStack.Push(p2); // push next cell onto stack
                        break; // next cell has been found; moving on to it now
                    }

                } // if we don't find an available direction to travel in, just fall through.
                // we've already popped the current cell, so we're taking a step back to the previous cell.

            }
            file.Close();
        } // end RecursiveBacktracker()

    }
}

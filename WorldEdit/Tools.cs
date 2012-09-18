﻿using System;
using System.Collections.Generic;
using System.IO;
using Terraria;

namespace WorldEdit
{
	public static class Tools
	{
		public static List<byte> GetTileByName(string tile)
		{
			byte ID;
			if (byte.TryParse(tile, out ID) && ID < 149 &&
				!Main.tileFrameImportant[ID] && !WorldEdit.InvalidTiles.Contains(ID))
			{
				return new List<byte> { ID };
			}

			List<byte> list = new List<byte>();
			foreach (KeyValuePair<string, byte> kv in WorldEdit.TileNames)
			{
				if (kv.Key == tile)
				{
					return new List<byte> { kv.Value };
				}
				if (kv.Key.StartsWith(tile))
				{
					list.Add(kv.Value);
				}
			}
			return list;
		}
		public static List<byte> GetWallByName(string wall)
		{
			byte ID;
			if (byte.TryParse(wall, out ID) && ID < 32)
			{
				return new List<byte> { ID };
			}

			List<byte> list = new List<byte>();
			foreach (KeyValuePair<string, byte> kv in WorldEdit.WallNames)
			{
				if (kv.Key == wall)
				{
					return new List<byte> { kv.Value };
				}
				if (kv.Key.StartsWith(wall))
				{
					list.Add(kv.Value);
				}
			}
			return list;
		}
		public static bool HasClipboard(int plr)
		{
			return File.Exists(Path.Combine("worldedit", String.Format("clipboard-{0}.dat", plr)));
		}
		public static Tile[,] LoadWorldData(string path)
		{
			Tile[,] tile;
			using (BinaryReader reader = new BinaryReader(new FileStream(path, FileMode.Open)))
			{
				int xLen = reader.ReadInt32();
				int yLen = reader.ReadInt32();
				tile = new Tile[xLen, yLen];

				for (int i = 0; i < xLen; i++)
				{
					for (int j = 0; j < yLen; j++)
					{
						tile[i, j] = ReadTile(reader);
					}
				}
				return tile;
			}
		}
		public static void LoadWorldSection(string path)
		{
			using (BinaryReader reader = new BinaryReader(new FileStream(path, FileMode.Open)))
			{
				int x = reader.ReadInt32();
				int y = reader.ReadInt32();
				int xLen = reader.ReadInt32();
				int yLen = reader.ReadInt32();

				for (int i = 0; i < xLen; i++)
				{
					for (int j = 0; j < yLen; j++)
					{
						Main.tile[i + x, j + y] = ReadTile(reader);
						Main.tile[i + x, j + y].skipLiquid = true;
					}
				}
				ResetSection(x, y, x + xLen, y + yLen);
			}
		}
		public static void PrepareUndo(int x, int y, int x2, int y2, int plr)
		{
			WorldEdit.Players[plr].redoLevel = -1;
			WorldEdit.Players[plr].undoLevel++;
			string path = Path.Combine("worldedit", String.Format("undo-{0}-{1}.dat", plr, WorldEdit.Players[plr].undoLevel));
			SaveWorldSection(x, y, x2, y2, path);

			foreach (string fileName in Directory.EnumerateFiles("worldedit", String.Format("redo-{0}-*.dat", plr)))
			{
				File.Delete(fileName);
			}
		}
		public static Tile ReadTile(BinaryReader reader)
		{
			Tile tile = new Tile();
			byte flags = reader.ReadByte();
			if ((flags & 1) == 1)
			{
				byte type = reader.ReadByte();
				tile.active = true;
				tile.type = type;
				if (Main.tileFrameImportant[type])
				{
					tile.frameNumber = reader.ReadByte();
					tile.frameX = reader.ReadInt16();
					tile.frameY = reader.ReadInt16();
				}
				else
				{
					tile.frameX = -1;
					tile.frameY = -1;
				}
			}
			if ((flags & 2) == 2)
			{
				tile.wall = reader.ReadByte();
			}
			if ((flags & 4) == 4)
			{
				tile.liquid = reader.ReadByte();
			}
			if ((flags & 8) == 8)
			{
				tile.lava = true;
			}
			if ((flags & 16) == 16)
			{
				tile.wire = true;
			}
			return tile;
		}
		public static void Redo(int plr)
		{
			WorldEdit.Players[plr].undoLevel++;
			string redoPath = Path.Combine("worldedit", String.Format("redo-{0}-{1}.dat", plr, WorldEdit.Players[plr].redoLevel));
			string undoPath = Path.Combine("worldedit", String.Format("undo-{0}-{1}.dat", plr, WorldEdit.Players[plr].undoLevel));
			WorldEdit.Players[plr].redoLevel--;
			using (BinaryReader reader = new BinaryReader(new FileStream(redoPath, FileMode.Open)))
			{
				int x = reader.ReadInt32();
				int y = reader.ReadInt32();
				int x2 = x + reader.ReadInt32() - 1;
				int y2 = y + reader.ReadInt32() - 1;
				SaveWorldSection(x, y, x2, y2, undoPath);
			}
			LoadWorldSection(redoPath);
			File.Delete(redoPath);
		}
		public static void ResetSection(int x, int y, int x2, int y2)
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
		public static void SaveWorldData(Tile[,] tiles, string path)
		{
			using (BinaryWriter writer = new BinaryWriter(new FileStream(path, FileMode.Create)))
			{
				int xLen = tiles.GetLength(0);
				int yLen = tiles.GetLength(1);
				writer.Write(xLen);
				writer.Write(yLen);
				for (int i = 0; i < xLen; i++)
				{
					for (int j = 0; j < yLen; j++)
					{
						WriteTile(writer, tiles[i, j] ?? new Tile());
					}
				}
			}
		}
		public static void SaveWorldSection(int x, int y, int x2, int y2, string path)
		{
			using (BinaryWriter writer = new BinaryWriter(new FileStream(path, FileMode.Create)))
			{
				writer.Write(x);
				writer.Write(y);
				writer.Write(x2 - x + 1);
				writer.Write(y2 - y + 1);
				for (int i = x; i <= x2; i++)
				{
					for (int j = y; j <= y2; j++)
					{
						WriteTile(writer, Main.tile[i, j]);
					}
				}
			}
		}
		public static void WriteTile(BinaryWriter writer, Tile tile)
		{
			byte flags = 0;
			if (tile.active)
			{
				flags |= 1;
			}
			if (tile.wall != 0)
			{
				flags |= 2;
			}
			if (tile.liquid > 0)
			{
				flags |= 4;
			}
			if (tile.lava)
			{
				flags |= 8;
			}
			if (tile.wire)
			{
				flags |= 16;
			}

			writer.Write(flags);
			if ((flags & 1) == 1)
			{
				writer.Write(tile.type);
				if (Main.tileFrameImportant[tile.type])
				{
					writer.Write(tile.frameNumber);
					writer.Write(tile.frameX);
					writer.Write(tile.frameY);
				}
			}
			if ((flags & 2) == 2)
			{
				writer.Write(tile.wall);
			}
			if ((flags & 4) == 4)
			{
				writer.Write(tile.liquid);
			}
		}
		public static void Undo(int plr)
		{
			WorldEdit.Players[plr].redoLevel++;
			string redoPath = Path.Combine("worldedit", String.Format("redo-{0}-{1}.dat", plr, WorldEdit.Players[plr].redoLevel));
			string undoPath = Path.Combine("worldedit", String.Format("undo-{0}-{1}.dat", plr, WorldEdit.Players[plr].undoLevel));
			WorldEdit.Players[plr].undoLevel--;
			using (BinaryReader reader = new BinaryReader(new FileStream(undoPath, FileMode.Open)))
			{
				int x = reader.ReadInt32();
				int y = reader.ReadInt32();
				int x2 = x + reader.ReadInt32() - 1;
				int y2 = y + reader.ReadInt32() - 1;
				SaveWorldSection(x, y, x2, y2, redoPath);
			}
			LoadWorldSection(undoPath);
			File.Delete(undoPath);
		}

        public static void Shuffle<T>(this IList<T> list, Random rng)
        {
            int n = list.Count;
            while (n > 1)
            {
                n--;
                int k = rng.Next(n + 1);
                T value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
        }

        public static bool PointInRect(int x, int y, int x2, int y2, Point p)
        {
            if (p.X >= x &&
                p.Y >= y &&
                p.X <= x2 &&
                p.Y <= y2)
                return true;
            else
                return false;
        }

        public static bool ArePointsEqual(Point p1, Point p2)
        {
            if ((p1.X == p2.X) && (p1.Y == p2.Y))
                return true;
            else
                return false;
        }


	}
}
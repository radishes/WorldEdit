using System;
using Terraria;
using TShockAPI;

namespace WorldEdit.Commands
{
	public class BiomeCommand : WECommand
	{
		private byte biome1;
		private byte biome2;

		public BiomeCommand(int x, int y, int x2, int y2, int plr, byte biome1, byte biome2)
			: base(x, y, x2, y2, plr)
		{
			this.biome1 = biome1;
			this.biome2 = biome2;
		}

		public override void Execute()
		{
			Tools.PrepareUndo(x, y, x2, y2, plr);
			int edits = 0;
			if (biome1 != biome2)
			{
				for (int i = x; i <= x2; i++)
				{
					for (int j = y; j <= y2; j++)
					{
						if (selectFunc(i, j, plr) && Main.tile[i, j].active)
						{
							for (int k = 0; k < WorldEdit.BiomeConversions[biome1].Length; k++)
							{
								if (Main.tile[i, j].type == WorldEdit.BiomeConversions[biome1][k])
								{
									if (WorldEdit.BiomeConversions[biome2][k] == 255)
									{
										Main.tile[i, j].active = false;
										Main.tile[i, j].type = 0;
									}
									else
									{
										Main.tile[i, j].type = WorldEdit.BiomeConversions[biome2][k];
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
			string msg = String.Format("Converted {0} to {1}. ({2})", WorldEdit.BiomeNames[biome1], WorldEdit.BiomeNames[biome2], edits);
			TShock.Players[plr].SendMessage(msg, Color.Green);
		}
	}
}

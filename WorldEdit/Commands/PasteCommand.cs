using System;
using System.IO;
using Terraria;
using TShockAPI;

namespace WorldEdit.Commands
{
	public class PasteCommand : WECommand
	{
		public PasteCommand(int x, int y, int plr)
			: base(x, y, 0, 0, plr)
		{
			string clipboardPath = Path.Combine("worldedit", String.Format("clipboard-{0}.dat", plr));
			using (BinaryReader reader = new BinaryReader(new FileStream(clipboardPath, FileMode.Open)))
			{
				x2 = x + reader.ReadInt32() - 1;
				y2 = y + reader.ReadInt32() - 1;
			}
		}

		public override void Execute()
		{
			string clipboardPath = Path.Combine("worldedit", String.Format("clipboard-{0}.dat", plr));
			using (BinaryReader reader = new BinaryReader(new FileStream(clipboardPath, FileMode.Open)))
			{
				reader.ReadInt64();
				Tools.PrepareUndo(x, y, x2, y2, plr);
				for (int i = x; i <= x2; i++)
				{
					for (int j = y; j <= y2; j++)
					{
						Main.tile[i, j] = Tools.ReadTile(reader);
					}
				}
			}
			ResetSection();
			TShock.Players[plr].SendMessage(String.Format("Pasted clipboard to selection."), Color.Green);
		}
	}
}
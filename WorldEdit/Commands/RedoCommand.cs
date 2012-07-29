using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TShockAPI;

namespace WorldEdit.Commands
{
    public class RedoCommand : WECommand
    {
        public RedoCommand(int plr)
            : base(0, 0, 0, 0, plr)
        {
        }

        public override void Execute()
        {
            Tools.Redo(plr);
            TShock.Players[plr].SendMessage("Redid last action.", Color.Yellow);
        }
    }
}

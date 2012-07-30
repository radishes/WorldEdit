using TShockAPI;

namespace WorldEdit.Commands
{
    public class UndoCommand : WECommand
    {
        public UndoCommand(int plr)
            : base(0, 0, 0, 0, plr)
        {
        }

        public override void Execute()
        {
            Tools.Undo(plr);
            TShock.Players[plr].SendMessage("Undid last action.", Color.Green);
        }
    }
}

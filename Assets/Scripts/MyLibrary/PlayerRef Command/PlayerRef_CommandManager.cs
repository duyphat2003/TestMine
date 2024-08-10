using System.Collections.Generic;
using UnityEngine;

namespace MyLibrary.PlayerPref_Command
{
    public class PlayerRef_CommandManager
    {
        private Stack<IPlayerPrefCommand> commandHistory = new Stack<IPlayerPrefCommand>();

        public void ExecuteCommand(IPlayerPrefCommand command)
        {
            command.Execute();
            commandHistory.Push(command);
        }

        public void UndoLastCommand()
        {
            if (commandHistory.Count > 0)
            {
                IPlayerPrefCommand lastCommand = commandHistory.Pop();
                lastCommand.Undo();
            }
        }
    }
}

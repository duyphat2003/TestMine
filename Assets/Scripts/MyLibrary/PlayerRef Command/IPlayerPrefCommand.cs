using UnityEngine;

namespace MyLibrary.PlayerPref_Command
{
    public interface  IPlayerPrefCommand
    {
        void Execute();
        void Undo();
    }
}
using UnityEngine;

namespace MyLibrary.PlayerPref_Command
{
    /// <summary>
    /// Khai báo lệnh cần thực hiện
    /// </summary>
    public interface  IPlayerPrefCommand
    {
        void Execute();
        void Undo();
    }
}
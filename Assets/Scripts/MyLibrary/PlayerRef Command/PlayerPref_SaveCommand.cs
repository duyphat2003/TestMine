using System.IO;
using UnityEngine;

namespace MyLibrary.PlayerPref_Command
{
    public class PlayerPref_SaveCommand : IPlayerPrefCommand
    {
        private string filePath;
        private string content;

        public PlayerPref_SaveCommand(string filePath, string content)
        {
            this.filePath = filePath;
            this.content = content;
        }

        public void Execute()
        {
            File.WriteAllText(filePath, content);
            Debug.Log($"Data saved to {filePath}");
        }

        public void Undo()
        {
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
                Debug.Log($"Data deleted from {filePath}");
            }
        }
    }
    }

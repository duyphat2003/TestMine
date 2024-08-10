using System.IO;
using UnityEngine;

namespace MyLibrary.PlayerPref_Command
{
    public class PlayerPref_LoadCommand : IPlayerPrefCommand
    {
        private string filePath;
        private string loadedContent;
        private string previousContent;
        public PlayerPref_LoadCommand(string filePath)
        {
            this.filePath = filePath;
        }

        public void Execute()
        {
            if (File.Exists(filePath))
            {
                previousContent = loadedContent;
                loadedContent = File.ReadAllText(filePath);
                Debug.Log($"Data loaded from {filePath}");
            }
            else
            {
                Debug.LogError("File not found!");
            }
        }

        public string GetLoadedContent()
        {
            return loadedContent;
        }

        public void Undo()
        {
 
        }
    }
}
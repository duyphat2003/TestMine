using UnityEngine;
using MyLibrary.PlayerPref_Command;
using MyLibrary.Model;
using System.IO;
using System.Linq;
using System.Collections.Generic;

public class PlayerPref_DatabaseManager : MonoBehaviour
{
    public static PlayerPref_DatabaseManager Instance;
    PlayerRef_CommandManager playerRef_CommandManager;

    
    void Start()
    {
        if(Instance)
            Destroy(gameObject);
        else
            Instance = this;

        playerRef_CommandManager = new PlayerRef_CommandManager();
        DontDestroyOnLoad(gameObject);
    }

    public void SaveProp(Prop prop)
    {
        // Lấy tất cả các tệp và sắp xếp chúng
        string[] sortedFilePaths = GetSortedFilePaths(Application.persistentDataPath + "/Prop");

        // Tạo tên tệp mới và lưu tệp mới với tên không trùng
        string uniqueFileName = GenerateUniqueFileName(Application.persistentDataPath + "/Prop", prop.name, ".txt");
        string newFilePath = Path.Combine(Application.persistentDataPath + "/Prop", uniqueFileName);
   
        IPlayerPrefCommand savePropCommand = new PlayerPref_SaveCommand(newFilePath, prop.Serialize());
        playerRef_CommandManager.ExecuteCommand(savePropCommand);
    }

    public void SaveInventory(Inventory[] inventory)
    {
        foreach(Inventory item in inventory)
        {
            IPlayerPrefCommand saveItemCommand = new PlayerPref_SaveCommand(Application.persistentDataPath + $"/Inventory/{item.index}", item.Serialize());
            playerRef_CommandManager.ExecuteCommand(saveItemCommand);
        }
    }

    public void SavePlayer(Player player)
    {
        string playerPath = Application.persistentDataPath + "/Player/Player.txt";
        IPlayerPrefCommand savePlayerCommand = new PlayerPref_SaveCommand(playerPath, player.Serialize());
        playerRef_CommandManager.ExecuteCommand(savePlayerCommand);
    }

    List<string> GetAllFiles(string directoryPath)
    {
        List<string> filePaths = new List<string>();

        if (Directory.Exists(directoryPath))
        {
            // Lấy tất cả các tệp trong thư mục
            string[] files = Directory.GetFiles(directoryPath);

            // Thêm các tệp vào danh sách
            filePaths.AddRange(files);
        }
        else
        {
            Debug.LogWarning($"Directory does not exist: {directoryPath}");
        }

        return filePaths;
    }

    public List<Prop> LoadProp()
    {
        List<Prop> props= new List<Prop>();
        // Lấy tất cả các tệp từ thư mục và in ra tên tệp
        List<string> filePaths = GetAllFiles(Application.persistentDataPath + "/Prop");
        
        foreach (string filePath in filePaths)
        {
            // Tải dữ liệu Prop
            IPlayerPrefCommand loadPropCommand = new PlayerPref_LoadCommand(filePath);
            playerRef_CommandManager.ExecuteCommand(loadPropCommand);
            Prop loadedProp = Prop.Deserialize(((PlayerPref_LoadCommand)loadPropCommand).GetLoadedContent());
            Debug.Log($"Loaded Prop Position: {loadedProp.name}, {loadedProp.x}, {loadedProp.y}, {loadedProp.z}");
            Debug.Log($"File found: {filePath}");
            props.Add(loadedProp);
        }

        return props;
    }

    public List<Inventory> LoadInventory()
    {
        List<Inventory> inventory = new List<Inventory>();
        // Lấy tất cả các tệp từ thư mục và in ra tên tệp
        List<string> filePaths = GetAllFiles(Application.persistentDataPath + "/Inventory");
        
        foreach (string filePath in filePaths)
        {
            // Tải dữ liệu Prop
            IPlayerPrefCommand loadPropCommand = new PlayerPref_LoadCommand(filePath);
            playerRef_CommandManager.ExecuteCommand(loadPropCommand);
            Inventory item = Inventory.Deserialize(((PlayerPref_LoadCommand)loadPropCommand).GetLoadedContent());
            Debug.Log($"Loaded Prop Position: {item.name}, {item.index}, {item.amount}");
            Debug.Log($"File found: {filePath}");
            inventory.Add(item);
        }
        return inventory;
    }

    public Player LoadPlayer()
    {
        // Tải dữ liệu Player
        IPlayerPrefCommand loadPlayerCommand = new PlayerPref_LoadCommand(Application.persistentDataPath + "/Player/Player.txt");
        playerRef_CommandManager.ExecuteCommand(loadPlayerCommand);
        Player loadedPlayer = Player.Deserialize(((PlayerPref_LoadCommand)loadPlayerCommand).GetLoadedContent());
        Debug.Log($"Loaded Player Position: {loadedPlayer.x}, {loadedPlayer.y}, {loadedPlayer.z}, {loadedPlayer.a}, {loadedPlayer.b}, {loadedPlayer.c}");
        return loadedPlayer;
    }



    string[] GetSortedFilePaths(string directoryPath)
    {
        if (Directory.Exists(directoryPath))
        {
            string[] filePaths = Directory.GetFiles(directoryPath);
            System.Array.Sort(filePaths, (x, y) => string.Compare(Path.GetFileName(x), Path.GetFileName(y)));
            return filePaths;
        }
        else
        {
            Debug.LogWarning($"Directory does not exist: {directoryPath}");
            return new string[0];
        }
    }

    string GenerateUniqueFileName(string directoryPath, string baseFileName, string extension)
    {
        // Lấy tất cả tên tệp hiện có
        string[] existingFileNames = Directory.GetFiles(directoryPath)
                                              .Select(file => Path.GetFileNameWithoutExtension(file))
                                              .ToArray();

        string fileName = baseFileName;
        string filePath = Path.Combine(directoryPath, fileName + extension);
        int count = 1;

        // Nếu tên tệp đã tồn tại, thêm số vào tên tệp mới
        while (existingFileNames.Contains(fileName))
        {
            fileName = $"{baseFileName}_{count}";
            filePath = Path.Combine(directoryPath, fileName + extension);
            count++;
        }

        return fileName + extension;
    }
}
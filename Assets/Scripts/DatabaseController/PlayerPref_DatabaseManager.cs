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
    [SerializeField] GameObject playerPrefab;    
    void Start()
    {
        if(Instance)
            Destroy(gameObject);
        else
            Instance = this;

        playerRef_CommandManager = new PlayerRef_CommandManager();
        DontDestroyOnLoad(gameObject);

        props = new List<Prop>();
        inventory = new List<Inventory>();
        player = new Player();

        hasDataProp = true;
        hasDataInventory = true;
        hasDataPlayer = true;

        LoadInventory();
        LoadProp();
        LoadPlayer();
        

        if(hasDataPlayer)
        {
            Instantiate(playerPrefab, new Vector3(player.x, player.y, player.z), Quaternion.Euler(player.a, player.b, player.c));
        }
        else
        {
            Instantiate(playerPrefab, transform.position, Quaternion.identity);
        }
        
        if(PlayerPref_DatabaseManager.Instance.hasDataProp)
        {
            foreach(var prop in PlayerPref_DatabaseManager.Instance.props)
            {
                GameObject prefab = Resources.Load<GameObject>($"Props/{prop.name}");
                GameObject clone = Instantiate(prefab, new Vector3(prop.x, prop.y, prop.z), Quaternion.Euler(prop.a, prop.b, prop.c));
                clone.GetComponent<PropInfo>().name = prop.name;
                clone.GetComponent<PropInfo>().index = prop.index;
                clone.GetComponent<PropInfo>().hasData = true;
            }
        }
    }

    public List<Prop> props;
    public List<Inventory> inventory;
    public Player player;
    public bool hasDataProp;
    public bool hasDataInventory;
    public bool hasDataPlayer;
    public void SaveProp()
    {
        foreach(Prop prop in props)
        {
            if (!Directory.Exists(Application.persistentDataPath + "/Prop"))
            {
                Directory.CreateDirectory(Application.persistentDataPath + "/Prop");
            }
            IPlayerPrefCommand savePropCommand = new PlayerPref_SaveCommand(Application.persistentDataPath + $"/Prop/{prop.index}.txt", prop.Serialize());
            playerRef_CommandManager.ExecuteCommand(savePropCommand);
        }
    }

    public void SaveInventory()
    {
         string directoryPath = Application.persistentDataPath + "/Inventory";
        if (!Directory.Exists(directoryPath))
        {
            Directory.CreateDirectory(directoryPath);
        }
        foreach(Inventory item in inventory)
        {
            IPlayerPrefCommand saveItemCommand = new PlayerPref_SaveCommand(Application.persistentDataPath + $"/Inventory/{item.index}.txt", item.Serialize());
            playerRef_CommandManager.ExecuteCommand(saveItemCommand);
        }
    }

    public void SavePlayer()
    {
        string directoryPath = Application.persistentDataPath + "/Player";
        if (!Directory.Exists(directoryPath))
        {
            Directory.CreateDirectory(directoryPath);
        }
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

    public void LoadProp()
    {
        string propDirectory = Application.persistentDataPath + "/Prop";
        
        // Kiểm tra xem thư mục có tồn tại và có tệp không
        if (Directory.Exists(propDirectory) && Directory.GetFiles(propDirectory).Length > 0)
        {
            List<string> filePaths = GetAllFiles(propDirectory);

            foreach (string filePath in filePaths)
            {
                // Tải dữ liệu Prop
                IPlayerPrefCommand loadPropCommand = new PlayerPref_LoadCommand(filePath);
                playerRef_CommandManager.ExecuteCommand(loadPropCommand);
                Prop loadedProp = Prop.Deserialize(((PlayerPref_LoadCommand)loadPropCommand).GetLoadedContent());
                Debug.Log($"Loaded Prop Position: {loadedProp.name}, {loadedProp.index}, {loadedProp.x}, {loadedProp.y}, {loadedProp.z}, {loadedProp.a}, {loadedProp.b}, {loadedProp.c}");
                Debug.Log($"File found: {filePath}");
                props.Add(loadedProp);
            }
        }
        else
        {
            Debug.LogWarning("No Prop data files found.");
            hasDataProp = false;
        }
    }

    public void LoadInventory()
    {
        string inventoryDirectory = Application.persistentDataPath + "/Inventory";

        // Kiểm tra xem thư mục có tồn tại và có tệp không
        if (Directory.Exists(inventoryDirectory) && Directory.GetFiles(inventoryDirectory).Length > 0)
        {
            List<string> filePaths = GetAllFiles(inventoryDirectory);

            foreach (string filePath in filePaths)
            {
                // Tải dữ liệu Inventory
                IPlayerPrefCommand loadInventoryCommand = new PlayerPref_LoadCommand(filePath);
                playerRef_CommandManager.ExecuteCommand(loadInventoryCommand);
                Inventory item = Inventory.Deserialize(((PlayerPref_LoadCommand)loadInventoryCommand).GetLoadedContent());
                Debug.Log($"Loaded Inventory Item: {item.name}, {item.index}, {item.amount}");
                Debug.Log($"File found: {filePath}");
                inventory.Add(item);
            }
        }
        else
        {
            Debug.LogWarning("No Inventory data files found.");
            hasDataInventory = false;
        }
    }

    public void LoadPlayer()
    {
        string playerFilePath = Application.persistentDataPath + "/Player/Player.txt";

        // Kiểm tra xem tệp Player có tồn tại không
        if (File.Exists(playerFilePath))
        {
            // Tải dữ liệu Player
            IPlayerPrefCommand loadPlayerCommand = new PlayerPref_LoadCommand(playerFilePath);
            playerRef_CommandManager.ExecuteCommand(loadPlayerCommand);
            player = Player.Deserialize(((PlayerPref_LoadCommand)loadPlayerCommand).GetLoadedContent());
            Debug.Log($"Loaded Player Position: {player.x}, {player.y}, {player.z}, {player.a}, {player.b}, {player.c}");
        }
        else
        {
            Debug.LogWarning("Player data file not found.");
            hasDataPlayer = false;
        }
    }


    public void ResetContent()
    {
        playerRef_CommandManager.UndoLastCommand();
    }


}
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using DG.Tweening;
using MyLibrary.Model;
using MyLibrary.PlayerFacade;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    [SerializeField] Camera cam;
    [SerializeField] float rangePick = 100f;
    public LayerMask itemMask;
    SpectatorCameraFacade spectatorCameraFacade;
    [SerializeField] SpectatorCameraProperties spectatorCameraProperties;
    
    CharacterController characterController;

    [SerializeField] GameObject menu;
    
    void OpenMenu()
    {
        isEdit = true;
        menu.SetActive(true);
    }

    public void CloseMenu()
    {
        isEdit = false;
        menu.SetActive(false);
    }

    public void ResetFile()
    {
         // Kiểm tra xem thư mục có tồn tại không
        if (Directory.Exists(Application.persistentDataPath + "/Prop"))
        {
            // Lấy tất cả các file trong thư mục
            string[] files = Directory.GetFiles(Application.persistentDataPath + "/Prop");

            // Xóa từng file
            foreach (string file in files)
            {
                // Xóa thuộc tính chỉ đọc (Read-Only) nếu có
                FileAttributes attributes = File.GetAttributes(file);
                if ((attributes & FileAttributes.ReadOnly) == FileAttributes.ReadOnly)
                {
                    File.SetAttributes(file, attributes & ~FileAttributes.ReadOnly);
                }

                // Xóa file
                File.Delete(file);
            }

            Debug.Log("All files deleted successfully.");
        }
         // Kiểm tra xem thư mục có tồn tại không
        if (Directory.Exists(Application.persistentDataPath + "/Inventory"))
        {
            // Lấy tất cả các file trong thư mục
            string[] files = Directory.GetFiles(Application.persistentDataPath + "/Inventory");

            // Xóa từng file
            foreach (string file in files)
            {
                // Xóa thuộc tính chỉ đọc (Read-Only) nếu có
                FileAttributes attributes = File.GetAttributes(file);
                if ((attributes & FileAttributes.ReadOnly) == FileAttributes.ReadOnly)
                {
                    File.SetAttributes(file, attributes & ~FileAttributes.ReadOnly);
                }

                // Xóa file
                File.Delete(file);
            }

            Debug.Log("All files deleted successfully.");
        }
         // Kiểm tra xem thư mục có tồn tại không
        if (Directory.Exists(Application.persistentDataPath + "/Player"))
        {
            // Lấy tất cả các file trong thư mục
            string[] files = Directory.GetFiles(Application.persistentDataPath + "/Player");

            // Xóa từng file
            foreach (string file in files)
            {
                // Xóa thuộc tính chỉ đọc (Read-Only) nếu có
                FileAttributes attributes = File.GetAttributes(file);
                if ((attributes & FileAttributes.ReadOnly) == FileAttributes.ReadOnly)
                {
                    File.SetAttributes(file, attributes & ~FileAttributes.ReadOnly);
                }

                // Xóa file
                File.Delete(file);
            }
            playerPref_DatabaseManager.props = new List<Prop>();
            playerPref_DatabaseManager.inventory = new List<Inventory>();
            playerPref_DatabaseManager.player = new Player();
            Debug.Log("All files deleted successfully.");
        }
                // Lấy tên scene hiện tại
        string currentSceneName = SceneManager.GetActiveScene().name;
        
        // Load lại scene hiện tại
        SceneManager.LoadScene(currentSceneName);
    }

    public void ExitGame()
    {
        WaitingForSaving();
        Debug.Log("Quit");
        Application.Quit();
    }

    void WaitingForSaving()
    {
        Debug.Log("Saving Inventory");
        playerPref_DatabaseManager.SaveInventory();
        Debug.Log("Done Inventory");

        Debug.Log("Saving Player");
        playerPref_DatabaseManager.SavePlayer();
        Debug.Log("Done Player");

        Debug.Log("Saving Prop");
        playerPref_DatabaseManager.SaveProp();
        Debug.Log("Done Prop");
    }

    void Awake()
    {
        characterController = GetComponent<CharacterController>();
    }

    void Start()
    {
        playerPref_DatabaseManager = FindObjectOfType<PlayerPref_DatabaseManager>();
        if(playerPref_DatabaseManager.hasDataPlayer)
        {
            transform.SetPositionAndRotation(new Vector3(playerPref_DatabaseManager.player.x, playerPref_DatabaseManager.player.y, playerPref_DatabaseManager.player.z), 
                                Quaternion.Euler(playerPref_DatabaseManager.player.a, playerPref_DatabaseManager.player.b, playerPref_DatabaseManager.player.c));
        }

        Cursor.lockState = isEdit ? CursorLockMode.None : CursorLockMode.Locked;
        Cursor.visible = isEdit;
        spectatorCameraFacade = new SpectatorCameraFacade(spectatorCameraProperties);

        spectatorCameraProperties.spectatorCameraRestoreObjectProperties.gridInfos = GetComponentsInChildren<GridInfo>().ToList();
    }
    bool isMove;
    bool isEdit;
    public bool isDrag;
    Vector3 currentRotate;
    void Update()
    {
        SetEdit();
        IntertactObject();
        Movement();
        if(isMove)
            flySource.Play();
        else
            flySource.Stop();
        UpdatePosition();
    }

    void UpdatePosition()
    {
        playerPref_DatabaseManager.player.x = transform.position.x;
        playerPref_DatabaseManager.player.y = transform.position.y;
        playerPref_DatabaseManager.player.z = transform.position.z;
        playerPref_DatabaseManager.player.a = transform.rotation.x;
        playerPref_DatabaseManager.player.b = transform.rotation.y;
        playerPref_DatabaseManager.player.c = transform.rotation.z;
    }

    void SetEdit()
    {
        Cursor.lockState = isEdit ? CursorLockMode.None : CursorLockMode.Locked;
        Cursor.visible = isEdit;
        Vector3 mousePosition = Input.mousePosition;
        mousePosition.z = rangePick;
        mousePosition = cam.ScreenToWorldPoint(mousePosition);
        Debug.DrawRay(cam.transform.position, mousePosition - cam.transform.position, Color.blue);

        if(MyCustomKeyboard.KEY_R || MyCustomKeyboard.KEY_ESC)
        {
            isEdit = !isEdit;
            if(MyCustomKeyboard.KEY_ESC)
            {
                OpenMenu();
            }
        }
    }
    [SerializeField] AudioSource removeSource;
    [SerializeField] GameObject vfxPrefab;
    [SerializeField] PlayerPref_DatabaseManager playerPref_DatabaseManager;
    GameObject worldPoint;
    bool isRotate;
    void IntertactObject()
    {
        if(!isEdit)
            return;

        if(worldPoint != null && !isRotate)
        {
            worldPoint.GetComponent<Rigidbody>().isKinematic = false;
            worldPoint = null;
        }
        
        if(MyCustomKeyboard.MOUSE_R)
        {
            Ray ray = cam.ScreenPointToRay(Input.mousePosition);
            if(Physics.Raycast(ray, out RaycastHit hitInfo, rangePick, itemMask, QueryTriggerInteraction.Ignore))
            {
                spectatorCameraFacade.ConstructRestoreObject(hitInfo.collider.gameObject.GetComponent<PropInfo>().prop.name);
                Debug.Log(hitInfo.collider.gameObject.GetComponent<PropInfo>().prop.name);
                removeSource.Play();
                Instantiate(vfxPrefab, hitInfo.collider.gameObject.transform.position, Quaternion.identity);
                hitInfo.collider.gameObject.GetComponent<BoxCollider>().enabled = false;
                hitInfo.collider.gameObject.transform.DOScale(Vector3.zero, 1).SetEase(Ease.InOutQuad).OnComplete(() =>
                {
                    playerPref_DatabaseManager.props.Remove(playerPref_DatabaseManager.props.FirstOrDefault(x => x.index == hitInfo.collider.gameObject.GetComponent<PropInfo>().prop.index));
                    playerPref_DatabaseManager.DeleteProp(hitInfo.collider.gameObject.GetComponent<PropInfo>().prop.index);
                    Destroy(hitInfo.collider.gameObject);
                });
                
                
            }        
        }
    }

    void Movement()
    {
        if(MyCustomKeyboard.KEY_W)
        {
            spectatorCameraProperties.spectatorCameraMovementProperties.direction = DirectionMovement.FORWARD;
            isMove = true;
        }

        if(MyCustomKeyboard.KEY_S)
        {
            spectatorCameraProperties.spectatorCameraMovementProperties.direction = DirectionMovement.BACKWARD;
            isMove = true;
        }

        if(MyCustomKeyboard.KEY_A)
        {
            spectatorCameraProperties.spectatorCameraMovementProperties.direction = DirectionMovement.LEFT;
            isMove = true;
        }

        if(MyCustomKeyboard.KEY_D)
        {
            spectatorCameraProperties.spectatorCameraMovementProperties.direction = DirectionMovement.RIGHT;
            isMove = true;
        }

        if(MyCustomKeyboard.KEY_E)
        {
            spectatorCameraProperties.spectatorCameraMovementProperties.direction = DirectionMovement.UP;
            isMove = true;
        }

        if(MyCustomKeyboard.KEY_Q)
        {
            spectatorCameraProperties.spectatorCameraMovementProperties.direction = DirectionMovement.DOWN;
            isMove = true;
        }

        if(isEdit || (!MyCustomKeyboard.KEY_W && !MyCustomKeyboard.KEY_S && !MyCustomKeyboard.KEY_A && !MyCustomKeyboard.KEY_D && !MyCustomKeyboard.KEY_E && !MyCustomKeyboard.KEY_Q))
        {
            isMove = false;
        }

        System.Tuple<Vector3, Vector3> direction = spectatorCameraFacade.ConstructMovement(GetComponent<Transform>(), MyCustomKeyboard.KEY_LSHIFT);
        Vector3 stopMove = spectatorCameraFacade.DestroyMovement();
        if(!isEdit)
            currentRotate = direction.Item2;
        
        characterController.Move(isMove ? direction.Item1 : stopMove);
        transform.eulerAngles = !isEdit ? direction.Item2 : currentRotate;
    }


    void OnDestroy()
    {
        WaitingForSaving();
    }

    void OnApplicationQuit()
    {
        WaitingForSaving();
    }

    void OnDisable()
    {
        WaitingForSaving();
    }

    private void OnApplicationPause(bool pauseStatus) {
        if(pauseStatus)
        {
            WaitingForSaving();
        }
    }

    public AudioSource flySource;
    public AudioSource impactSource;
    public AudioClip flySound; // Âm thanh bước chân
    public AudioClip impactSound;

    void OnCollisionEnter(Collision other)
    {
        if(other.gameObject.CompareTag("Ground") || other.gameObject.CompareTag("Object"))
        impactSource.Play();
    }
}

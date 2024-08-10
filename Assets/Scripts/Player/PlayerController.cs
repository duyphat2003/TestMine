using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MyLibrary.Model;
using MyLibrary.PlayerFacade;
using UnityEngine;

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

    public void ExitGame()
    {
        WaitingForSaving();
        Debug.Log("Quit");
        Application.Quit();
    }

    void WaitingForSaving()
    {
        Debug.Log("Reset Data");
        
        Debug.Log("Done Reset Data");

        Debug.Log("Saving Inventory");
        PlayerPref_DatabaseManager.Instance.SaveInventory();
        Debug.Log("Done Inventory");

        Debug.Log("Saving Player");
        PlayerPref_DatabaseManager.Instance.SavePlayer();
        Debug.Log("Done Player");

        Debug.Log("Saving Prop");
        PlayerPref_DatabaseManager.Instance.SaveProp();
        Debug.Log("Done Prop");
    }

    void Awake()
    {
        characterController = GetComponent<CharacterController>();
    }

    void Start()
    {
        stepTimer = stepInterval;
        if(PlayerPref_DatabaseManager.Instance.hasDataPlayer)
        {
            transform.SetPositionAndRotation(new Vector3(PlayerPref_DatabaseManager.Instance.player.x, PlayerPref_DatabaseManager.Instance.player.y, PlayerPref_DatabaseManager.Instance.player.z), 
                                Quaternion.Euler(PlayerPref_DatabaseManager.Instance.player.a, PlayerPref_DatabaseManager.Instance.player.b, PlayerPref_DatabaseManager.Instance.player.c));
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
        UpdatePosition();
    }

    void UpdatePosition()
    {
        PlayerPref_DatabaseManager.Instance.player.x = transform.position.x;
        PlayerPref_DatabaseManager.Instance.player.y = transform.position.y;
        PlayerPref_DatabaseManager.Instance.player.z = transform.position.z;
        PlayerPref_DatabaseManager.Instance.player.a = transform.rotation.x;
        PlayerPref_DatabaseManager.Instance.player.b = transform.rotation.y;
        PlayerPref_DatabaseManager.Instance.player.c = transform.rotation.z;
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
    [SerializeField] AudioSource removeSound;
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
                PlayerPref_DatabaseManager.Instance.props.Remove(hitInfo.collider.gameObject.GetComponent<PropInfo>().prop);
                PlayerPref_DatabaseManager.Instance.DeleteProp(hitInfo.collider.gameObject.GetComponent<PropInfo>().prop.index);
                Destroy(hitInfo.collider.gameObject);
            }        
        }
        isRotate = MyCustomKeyboard.MOUSE_L;
        if(MyCustomKeyboard.MOUSE_L && !isDrag)
        {
            Ray ray = cam.ScreenPointToRay(Input.mousePosition);
            if(Physics.Raycast(ray, out RaycastHit hitInfo, rangePick, itemMask, QueryTriggerInteraction.Ignore))
            {
                worldPoint = hitInfo.collider.gameObject;
                isRotate = true;
                if(worldPoint.CompareTag("Object"))
                {
                    float threshold = 0.5f;

                    // Check the direction of the surface normal to determine the hit surface
                    if (Vector3.Dot(hitInfo.normal, Vector3.up) > threshold) {
                        spectatorCameraProperties.spectatorCameraRotateObjectProperties.rotateDirection = RotateDirection.UP;
                        Debug.Log("Hit top surface");
                    }
                    else if (Vector3.Dot(hitInfo.normal, Vector3.down) > threshold) {
                        spectatorCameraProperties.spectatorCameraRotateObjectProperties.rotateDirection = RotateDirection.DOWN;
                        Debug.Log("Hit bottom surface");
                    }
                    else if (Vector3.Dot(hitInfo.normal, Vector3.left) > threshold) {
                        spectatorCameraProperties.spectatorCameraRotateObjectProperties.rotateDirection = RotateDirection.LEFT;
                        Debug.Log("Hit left surface");
                    }
                    else if (Vector3.Dot(hitInfo.normal, Vector3.right) > threshold) {
                        spectatorCameraProperties.spectatorCameraRotateObjectProperties.rotateDirection = RotateDirection.RIGHT;
                        Debug.Log("Hit right surface");
                    }
                    else if (Vector3.Dot(hitInfo.normal, Vector3.forward) > threshold) {
                        spectatorCameraProperties.spectatorCameraRotateObjectProperties.rotateDirection = RotateDirection.FORWARD;
                        Debug.Log("Hit front surface");
                    }
                    else if (Vector3.Dot(hitInfo.normal, Vector3.back) > threshold) {
                        spectatorCameraProperties.spectatorCameraRotateObjectProperties.rotateDirection = RotateDirection.BACKWARD;
                        Debug.Log("Hit back surface");
                    }
                    worldPoint.GetComponent<Rigidbody>().isKinematic = true;
                    worldPoint.transform.rotation *= spectatorCameraFacade.ConstructRotateObject();
                    Debug.Log(worldPoint.GetComponent<PropInfo>().name);
                }
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

    public AudioSource audioSource;
    public AudioClip footstepSound; // Âm thanh bước chân
    public AudioClip rotateSound;
    public float stepInterval = 0.5f; // Khoảng thời gian giữa các bước chân
    public float rotateThreshold = 0.1f; // Ngưỡng để xác định khi nào phát âm thanh quay
    private Vector3 lastRotation; // Lưu trữ hướng quay trước đó
    private float stepTimer;
    void SoundMovement()
    {
        // Kiểm tra xem nhân vật có đang di chuyển không
        if (characterController.isGrounded && characterController.velocity.magnitude > 0.1f)
        {
            stepTimer -= Time.deltaTime;

            if (stepTimer <= 0f)
            {
                // Phát âm thanh bước chân
                audioSource.PlayOneShot(footstepSound);
                stepTimer = stepInterval; // Đặt lại thời gian giữa các bước chân
            }
        }
        else
        {
            // Đặt lại bộ đếm thời gian khi nhân vật dừng di chuyển
            stepTimer = stepInterval;
        }

         // Âm thanh quay
        Vector3 currentRotation = transform.eulerAngles;
        if (Vector3.Distance(currentRotation, lastRotation) > rotateThreshold)
        {
            audioSource.PlayOneShot(rotateSound);
            lastRotation = currentRotation; // Cập nhật hướng quay hiện tại
        }
    }
}

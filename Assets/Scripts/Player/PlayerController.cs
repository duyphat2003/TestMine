using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
    
    [SerializeField] List<GridInfo> gridInfos;
    CharacterController characterController;

    void Awake()
    {
        characterController = GetComponent<CharacterController>();
    }

    void Start()
    {
        Player player = PlayerPref_DatabaseManager.Instance.LoadPlayer();
        transform.SetPositionAndRotation(new Vector3(player.x, player.y, player.z), Quaternion.Euler(player.a, player.b, player.c));
        
        gridInfos = GetComponentsInChildren<GridInfo>().ToList();
        List<Inventory> inventory = PlayerPref_DatabaseManager.Instance.LoadInventory();
        
        for (int i = 0; i < gridInfos.Count(); i++)
        {
            Inventory item = inventory.FirstOrDefault(x => x.index == i);
            gridInfos[i].gridProperties.name = item != null ? item.name : "";
            gridInfos[i].gridProperties.amount = item != null ? item.amount : 0;
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
    }

    void SetEdit()
    {
        Cursor.lockState = isEdit ? CursorLockMode.None : CursorLockMode.Locked;
        Cursor.visible = isEdit;
        Vector3 mousePosition = Input.mousePosition;
        mousePosition.z = rangePick;
        mousePosition = cam.ScreenToWorldPoint(mousePosition);
        Debug.DrawRay(cam.transform.position, mousePosition - cam.transform.position, Color.blue);

        if(MyCustomKeyboard.KEY_R)
        {
            isEdit = !isEdit;
        }
    }
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
                spectatorCameraFacade.ConstructRestoreObject(hitInfo.collider.gameObject.GetComponent<PropInfo>().name);
                Debug.Log(hitInfo.collider.gameObject.GetComponent<PropInfo>().name);
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
}

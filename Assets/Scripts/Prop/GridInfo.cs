using System.Collections;
using System.Collections.Generic;
using System.Linq;
using MyLibrary.Model;
using TMPro;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class GridInfo : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public GridProperties gridProperties;
    [SerializeField] Image image; 
    [SerializeField] Sprite imageNull; // Khi không có item nào
    [SerializeField] TextMeshProUGUI amountText;

    [SerializeField] GameObject prefabInstantiate;
    [SerializeField] private List<GameObject> propPrefabs;
    [SerializeField] private RectTransform UIDragElement;
    [SerializeField] private RectTransform Canvas;


    private Vector2 mOriginalLocalPointerPosition;
    private Vector3 mOriginalPanelLocalPosition;
    private Vector2 mOriginalPosition;
    GameObject prop;
    List<Sprite> nameSprites;

    // Start is called before the first frame update
    void Start()
    {
        if(PlayerPref_DatabaseManager.Instance.hasDataInventory)
        {
            int index = PlayerPref_DatabaseManager.Instance.inventory.FindIndex(x => x.index == gridProperties.index);
            gridProperties.name = PlayerPref_DatabaseManager.Instance.inventory[index].name; 
            gridProperties.amount = PlayerPref_DatabaseManager.Instance.inventory[index].amount; 
        }
        else
        {
            gridProperties.name = "";
            gridProperties.amount = 0;

            Inventory inventory = new Inventory();
            inventory.name = gridProperties.name;
            inventory.amount = gridProperties.amount;
            inventory.index = gridProperties.index;

            PlayerPref_DatabaseManager.Instance.inventory.Add(inventory);
        }

        mOriginalPosition = UIDragElement.localPosition;
        nameSprites = Resources.LoadAll<Sprite>("PropSprite").ToList();
        propPrefabs = Resources.LoadAll<GameObject>("Props").ToList();

        Sprite sprite = nameSprites.FirstOrDefault(element => element.name == gridProperties.name);
        prop = propPrefabs.FirstOrDefault(element => element.name == gridProperties.name);
        
        if(gridProperties.amount == 0)
        {
            image.sprite = imageNull;
            amountText.text = "0";
            gridProperties.name = "";
        }
        else
        {
            image.sprite = sprite;
            prefabInstantiate = prop;
            amountText.text = gridProperties.amount.ToString();
        }
    }

    void Update()
    {
        if(gridProperties.amount == 0)
        {
            gridProperties.name = "";
            image.sprite = imageNull;
            prefabInstantiate = null;
        }
        amountText.text = gridProperties.amount.ToString();
        int index = PlayerPref_DatabaseManager.Instance.inventory.FindIndex(x => x.index == gridProperties.index);
        PlayerPref_DatabaseManager.Instance.inventory[index].name = gridProperties.name; 
        PlayerPref_DatabaseManager.Instance.inventory[index].amount = gridProperties.amount; 
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if(string.IsNullOrEmpty(gridProperties.name)) return;
        GetComponentInParent<PlayerController>().isDrag = true;
        mOriginalPanelLocalPosition = UIDragElement.localPosition;

        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            Canvas,
            eventData.position,
            eventData.pressEventCamera,
            out mOriginalLocalPointerPosition);
    }

    public void OnDrag(PointerEventData eventData)
    {
        if(string.IsNullOrEmpty(gridProperties.name)) return;
        GetComponentInParent<PlayerController>().isDrag = true;
        Vector2 localPointerPosition;
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
            Canvas,
            eventData.position,
            eventData.pressEventCamera,
            out localPointerPosition))
        {

            Vector3 offsetToOriginal = localPointerPosition - mOriginalLocalPointerPosition;

            UIDragElement.localPosition = mOriginalPanelLocalPosition + offsetToOriginal;
        }
    }

    public IEnumerator Coroutine_MoveUIElement(RectTransform r, Vector2 targetPosition, float duration = 0.1f)
    {
        float elapsedTime = 0;
        Vector2 startingPos = r.localPosition;

        while (elapsedTime < duration)
        {
        r.localPosition = Vector2.Lerp(startingPos, targetPosition, (elapsedTime / duration));
        elapsedTime += Time.deltaTime;

        yield return new WaitForEndOfFrame();
        }

        r.localPosition = targetPosition;
    }

    [SerializeField] Camera cam;
    public void OnEndDrag(PointerEventData eventData)
    {
        StartCoroutine(Coroutine_MoveUIElement(UIDragElement, mOriginalPosition, 0.5f));

        RaycastHit hit;
        Ray ray = cam.ScreenPointToRay(eventData.position);

        if (Physics.Raycast(ray, out hit, 1000.0f))
        {
            GameObject worldPoint = hit.collider.gameObject;
            if(worldPoint.CompareTag("Object"))
            {
                float threshold = 0.5f;

                // Check the direction of the surface normal to determine the hit surface
                if (Vector3.Dot(hit.normal, Vector3.up) > threshold) {
                    directionHit = DirectionHit.TOP;
                    Debug.Log("Hit top surface");
                }
                else if (Vector3.Dot(hit.normal, Vector3.down) > threshold) {
                    directionHit = DirectionHit.BOTTOM;
                    Debug.Log("Hit bottom surface");
                }
                else if (Vector3.Dot(hit.normal, Vector3.left) > threshold) {
                    directionHit = DirectionHit.LEFT;
                    Debug.Log("Hit left surface");
                }
                else if (Vector3.Dot(hit.normal, Vector3.right) > threshold) {
                    directionHit = DirectionHit.RIGHT;
                    Debug.Log("Hit right surface");
                }
                else if (Vector3.Dot(hit.normal, Vector3.forward) > threshold) {
                    directionHit = DirectionHit.FRONT;
                    Debug.Log("Hit front surface");
                }
                else if (Vector3.Dot(hit.normal, Vector3.back) > threshold) {
                    directionHit = DirectionHit.BACK;
                    Debug.Log("Hit back surface");
                }
            }

            CreateObject(worldPoint, hit.point + Vector3.up * 0.5f);
        }
        GetComponentInParent<PlayerController>().isDrag = false;
    }

    DirectionHit directionHit;
    public float checkRadius = 1f;
    public LayerMask layerMask;  
    public void CreateObject(GameObject target, Vector3 point)
    {
        if (prefabInstantiate == null)
        {
        Debug.Log("No prefab to instantiate");
        return;
        }
        // Xác định vị trí của chuỗi trên vật thể
        if (PositionWithinCell(target))
        {
            if( target.CompareTag("Object"))
            {
                switch(directionHit)
                {
                    case DirectionHit.TOP:
                    point += new Vector3(0,1,0);
                    break;

                    case DirectionHit.BOTTOM:
                    point -= new Vector3(0,1,0);
                    break;

                    case DirectionHit.LEFT:
                    point -= new Vector3(1,0,0);
                    break;

                    case DirectionHit.RIGHT:
                    point += new Vector3(1,0,0);
                    break;
                    
                    case DirectionHit.FRONT:
                    point += new Vector3(0,0,1);
                    break;

                    case DirectionHit.BACK:
                    point -= new Vector3(0,0,1);
                    break;
                }
            }
            // Xác định xung quang trống hay không
            bool isSpaceFree = !Physics.CheckSphere(point, checkRadius, layerMask);

            if (isSpaceFree) {
                // Nếu không có đối tượng nào khác, thực hiện việc spawn
                GameObject obj = Instantiate(prefabInstantiate, point, Quaternion.identity);
                obj.transform.localScale = Vector3.zero;
                
                // Phóng to Cube từ (0,0,0) đến (1,1,1) mượt mà
                while (obj.transform.localScale != targetScale)
                {
                    obj.GetComponent<Rigidbody>().isKinematic = obj.transform.localScale != targetScale;
                    obj.transform.localScale = Vector3.Lerp(obj.transform.localScale, targetScale, scaleSpeed * Time.deltaTime);
                }
                obj.GetComponent<Rigidbody>().isKinematic = obj.transform.localScale != targetScale;
                int index = PlayerPref_DatabaseManager.Instance.props.Count() <= 0 ? 0 : FindMaxIndex(PlayerPref_DatabaseManager.Instance.props);
                
                Prop prop = new()
                {
                    name = gridProperties.name,
                    index = index,
                    x = transform.position.x,
                    y = transform.position.y,
                    z = transform.position.z,
                    a = transform.rotation.x,
                    b = transform.rotation.y,
                    c = transform.rotation.z
                };
                obj.GetComponent<PropInfo>().prop = prop;
                PlayerPref_DatabaseManager.Instance.props.Add(prop);
                gridProperties.amount--;

                Instantiate(vfxPrefab, obj.transform.position, Quaternion.identity);
                placeSound.Play();
                Debug.Log("Spawned object at position: " + point);
            } else {
                Debug.Log("Cannot spawn object, space is occupied.");
            }
            
            
        }
    }
    [SerializeField] AudioSource placeSound;
    [SerializeField] GameObject vfxPrefab;
    public float scaleSpeed = 2.0f; // Tốc độ phóng to
    private Vector3 targetScale = new Vector3(1, 1, 1);
    int FindMaxIndex(List<Prop> list)
    {
        list = list.OrderBy(p => p.index).ToList();

        return list.Max(p => p.index) + 1;
    }

    private bool PositionWithinCell(GameObject target) => target.CompareTag("Ground") || target.CompareTag("Object");
}

// Xác định vị trí của chuỗi trên vật thể
public enum DirectionHit
{
    TOP,
    BOTTOM,
    LEFT,
    RIGHT,
    FRONT,
    BACK,
}

[System.Serializable]
public class GridProperties : System.IComparable<GridProperties>
{
    public string name;
    public int amount;
    public int index;


    public int CompareTo(GridProperties other)
    {
        return name.CompareTo(other.name);
    }
}

public class GridPropertiesComparer : IComparer<GridProperties>
{
    public int Compare(GridProperties x, GridProperties y)
    {
        return x.name.CompareTo(y.name);
    }
    
}

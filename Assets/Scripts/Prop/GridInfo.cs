using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using MyLibrary.Model;
using TMPro;
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
    [SerializeField] PlayerPref_DatabaseManager playerPref_DatabaseManager;

    void Start()
    {
        playerPref_DatabaseManager = FindObjectOfType<PlayerPref_DatabaseManager>();
        if(playerPref_DatabaseManager.hasDataInventory)
        {
            int index = playerPref_DatabaseManager.inventory.FindIndex(x => x.index == gridProperties.index);
            gridProperties.name = playerPref_DatabaseManager.inventory[index].name; 
            gridProperties.amount = playerPref_DatabaseManager.inventory[index].amount; 
        }
        else
        {
            if(gridProperties.index == 0)
                gridProperties.name = "Green Cube";
            else if(gridProperties.index == 1)
                gridProperties.name = "Blue Cube";
            else if(gridProperties.index == 2)
                gridProperties.name = "Red Cube";
            gridProperties.amount = 10;

            Inventory inventory = new Inventory();
            inventory.name = gridProperties.name;
            inventory.amount = gridProperties.amount;
            inventory.index = gridProperties.index;

            playerPref_DatabaseManager.inventory.Add(inventory);
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
        if(playerPref_DatabaseManager.inventory.Count() == 0) return;    
        if(gridProperties.amount == 0)
        {
            gridProperties.name = "";
            image.sprite = imageNull;
            prefabInstantiate = null;
        }
        else
        {
            Sprite sprite = nameSprites.FirstOrDefault(element => element.name == gridProperties.name);
            image.sprite = sprite;
        }
        amountText.text = gridProperties.amount.ToString();
        int index = playerPref_DatabaseManager.inventory.FindIndex(x => x.index == gridProperties.index);
        playerPref_DatabaseManager.inventory[index].name = gridProperties.name; 
        playerPref_DatabaseManager.inventory[index].amount = gridProperties.amount; 
    }

    [SerializeField] GameObject previewCube;
    [SerializeField] GameObject previewCubePrefab;
    
    public void OnBeginDrag(PointerEventData eventData)
    {
        previewPoint = Vector3.zero;
        if(string.IsNullOrEmpty(gridProperties.name)) return;
        GetComponentInParent<PlayerController>().isDrag = true;
        previewCube = Instantiate(previewCubePrefab);
    }
    Vector3 currentPoint;
    public LayerMask canMask;
    public void OnDrag(PointerEventData eventData)
    {
        if(string.IsNullOrEmpty(gridProperties.name)) return;
        GetComponentInParent<PlayerController>().isDrag = true;


        RaycastHit hit;
        Ray ray = cam.ScreenPointToRay(eventData.position);
        
        if (Physics.Raycast(ray, out hit, 1000.0f, canMask))
        {
            Snap(hit.collider.gameObject, hit.point);
            previewCube.transform.DOMove(currentPoint, 0.05f)
                 .SetEase(Ease.InOutQuad);
            previewPoint = currentPoint;
        }
    }

    public int tileSize = 1;
    public Vector3 tileoffset = Vector3.zero;
    void Snap(GameObject target, Vector3 hit)
    {
        Vector3 currentPosition = hit;
        float snappedX = Mathf.Round(currentPosition.x  / tileSize) * tileSize + tileoffset.x;
        float snappedZ = Mathf.Round(currentPosition.z / tileSize) * tileSize + tileoffset.z;
        float snappedy = Mathf.Round(currentPosition.y / tileSize) * tileSize + tileoffset.y; // Preserve the original y-coordinate
        Vector3 snappedPosition = new Vector3(snappedX, snappedy, snappedZ);
        currentPoint = snappedPosition;
        if (previewCube.GetComponent<PreviewBox>().isTrigger) 
        {
            currentPoint.y += 0.5f;
        }
    }

    [SerializeField] Camera cam;
    public void OnEndDrag(PointerEventData eventData)
    {
        bool isTrigger = previewCube.GetComponent<PreviewBox>().isTrigger;
        Destroy(previewCube);
        if(isTrigger)
            return;

        GameObject obj = Instantiate(prefabInstantiate, previewPoint, Quaternion.identity); 
        obj.transform.localScale = Vector3.zero;
                
        // Sử dụng DOTween để phóng to đối tượng từ (0,0,0) đến (1,1,1) mượt mà
        obj.transform.DOScale(targetScale, scaleSpeed).SetEase(Ease.InOutQuad);

        int index = playerPref_DatabaseManager.props.Count() <= 0 ? 0 : FindMaxIndex(playerPref_DatabaseManager.props);
        
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
        playerPref_DatabaseManager.props.Add(prop);
        gridProperties.amount--;

        Instantiate(vfxPrefab, obj.transform.position, Quaternion.identity);
        placeSound.Play();
        Debug.Log("Spawned object at position: " + previewPoint);
        GetComponentInParent<PlayerController>().isDrag = false;
    }
    Vector3 previewPoint = Vector3.zero;
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

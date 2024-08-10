using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class GridInfo : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public GridProperties gridProperties;
    [SerializeField] Image image; 
    [SerializeField] Sprite imageNull; 
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
    }

    GameObject clone;
    public void OnBeginDrag(PointerEventData eventData)
    {
        if(string.IsNullOrEmpty(gridProperties.name)) return;
        clone = Instantiate(prop, eventData.position, Quaternion.identity);
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
        Vector2 localPointerPosition;
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
            Canvas,
            eventData.position,
            eventData.pressEventCamera,
            out localPointerPosition))
        {

            Vector3 offsetToOriginal = localPointerPosition - mOriginalLocalPointerPosition;

            UIDragElement.localPosition = mOriginalPanelLocalPosition + offsetToOriginal;
            clone.transform.position = eventData.position;
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
            bool isSpaceFree = !Physics.CheckSphere(point, checkRadius, layerMask);

            if (isSpaceFree) {
                // Nếu không có đối tượng nào khác, thực hiện việc spawn
                GameObject obj = Instantiate(prefabInstantiate, point, Quaternion.identity);
                obj.GetComponent<PropInfo>().name = gridProperties.name;
                gridProperties.amount--;
                Debug.Log("Spawned object at position: " + point);
                Destroy(clone);
            } else {
                Debug.Log("Cannot spawn object, space is occupied.");
            }
            
            
        }
    }

    private bool PositionWithinCell(GameObject target) => target.CompareTag("Ground") || target.CompareTag("Object");
}

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

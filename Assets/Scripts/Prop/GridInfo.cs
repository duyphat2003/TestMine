using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
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

    List<Sprite> nameSprites;

    // Start is called before the first frame update
    void Start()
    {
        mOriginalPosition = UIDragElement.localPosition;
        nameSprites = Resources.LoadAll<Sprite>("PropSprite").ToList();
        propPrefabs = Resources.LoadAll<GameObject>("Props").ToList();

        Sprite sprite = nameSprites.FirstOrDefault(element => element.name == gridProperties.name);
        GameObject prop = propPrefabs.FirstOrDefault(element => element.name == gridProperties.name);
        
        if(gridProperties.amount == 0)
        {
            image.sprite = imageNull;
            amountText.text = "0";
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

    public bool AddItem(string name)
    {
        if(image.sprite && (name != gridProperties.name || gridProperties.amount >= 10))
        {
            return false;
        }

        if(!image.sprite)
        {
            Sprite sprite = nameSprites.FirstOrDefault(element => element.name == name);
            GameObject prop = propPrefabs.FirstOrDefault(element => element.name == gridProperties.name);
            gridProperties.name = name;
            image.sprite = sprite;
            prefabInstantiate = prop;
        }

        gridProperties.amount++;
        return true;
    }

    public bool RemoveItem(string name)
    {
        if(!image.sprite) return false;

        gridProperties.amount--;
        return true;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        mOriginalPanelLocalPosition = UIDragElement.localPosition;

        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            Canvas,
            eventData.position,
            eventData.pressEventCamera,
            out mOriginalLocalPointerPosition);
    }

    public void OnDrag(PointerEventData eventData)
    {
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

    public void OnEndDrag(PointerEventData eventData)
    {
        StartCoroutine(Coroutine_MoveUIElement(UIDragElement, mOriginalPosition, 0.5f));

        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(eventData.position);

        if (Physics.Raycast(ray, out hit, 1000.0f))
        {
            Vector3 worldPoint = hit.point;

            CreateObject(worldPoint);
        }
    }

    public void CreateObject(Vector3 position)
    {
        if (prefabInstantiate == null)
        {
        Debug.Log("No prefab to instantiate");
        return;
        }

        if (PositionWithinCell(position))
        {
            GameObject obj = Instantiate(prefabInstantiate, position, Quaternion.identity);
        }
    }

    private bool PositionWithinCell(Vector3 pos)
    {
        // Placeholder logic: Always return true. Implement your own logic here.
        return true;
    }
}

[System.Serializable]
public class GridProperties
{
    public string name;
    public int amount;
}

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GridInfo : MonoBehaviour
{

    public GridProperties gridProperties;
    [SerializeField] Image image; 
    [SerializeField] Sprite imageNull; 
    [SerializeField] TextMeshProUGUI amountText;
    List<Sprite> nameSprites;

    // Start is called before the first frame update
    void Start()
    {
        nameSprites = Resources.LoadAll<Sprite>("PropSprite").ToList();
        Sprite sprite = nameSprites.FirstOrDefault(element => element.name == gridProperties.name);
        if(gridProperties.amount == 0)
        {
            image.sprite = imageNull;
            amountText.text = "0";
        }
        else
        {
            image.sprite = sprite;
            amountText.text = gridProperties.amount.ToString();
        }
    }

    void Update()
    {
        if(gridProperties.amount == 0)
        {
            gridProperties.name = "";
            image.sprite = imageNull;
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
            gridProperties.name = name;
            image.sprite = sprite;
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
}

[System.Serializable]
public class GridProperties
{
    public string name;
    public int amount;
}

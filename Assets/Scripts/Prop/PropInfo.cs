using System.Collections.Generic;
using System.Linq;
using MyLibrary.Model;
using UnityEngine;

public class PropInfo : MonoBehaviour
{
    public string propName;
    public int index = -1;
    public bool hasData = false;

    void Start()
    {
        if (hasData)
        {
            return;
        }
        
        index = PlayerPref_DatabaseManager.Instance.props.Count() <= 0 ? 0 : FindMaxIndex(PlayerPref_DatabaseManager.Instance.props);

        Prop prop = new Prop();
        prop.name = propName;
        prop.index = index;
        prop.x = transform.position.x;
        prop.y = transform.position.y;
        prop.z = transform.position.z;
        prop.a = transform.rotation.x;
        prop.b = transform.rotation.y;
        prop.c = transform.rotation.z;
       
        PlayerPref_DatabaseManager.Instance.props.Add(prop);
        Debug.Log("Reset Data");
        PlayerPref_DatabaseManager.Instance.ResetContent();
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
    void OnDestroy()
    {
        Prop propCLone = PlayerPref_DatabaseManager.Instance.props.First(x => x.index == index);
        PlayerPref_DatabaseManager.Instance.props.Remove(propCLone);
        Debug.Log("Reset Data");
        PlayerPref_DatabaseManager.Instance.ResetContent();
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

    void Update()
    {
        PlayerPref_DatabaseManager.Instance.props.FirstOrDefault(x => x.index == index).x = transform.position.x;
        PlayerPref_DatabaseManager.Instance.props.FirstOrDefault(x => x.index == index).y = transform.position.y;
        PlayerPref_DatabaseManager.Instance.props.FirstOrDefault(x => x.index == index).z = transform.position.z;
        PlayerPref_DatabaseManager.Instance.props.FirstOrDefault(x => x.index == index).a = transform.rotation.x;
        PlayerPref_DatabaseManager.Instance.props.FirstOrDefault(x => x.index == index).b = transform.rotation.y;
        PlayerPref_DatabaseManager.Instance.props.FirstOrDefault(x => x.index == index).c = transform.rotation.z;
    }

    int FindMaxIndex(List<Prop> list)
    {
        list = list.OrderBy(p => p.index).ToList();

        return list.Max(p => p.index) + 1;
    }
}
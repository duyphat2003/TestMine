using System.Collections.Generic;
using System.Linq;
using MyLibrary.Model;
using UnityEngine;

public class PropInfo : MonoBehaviour
{
    public Prop prop;
   

    void Update()
    {
        PlayerPref_DatabaseManager.Instance.props.FirstOrDefault(x => x.index == prop.index).x = transform.position.x;
        PlayerPref_DatabaseManager.Instance.props.FirstOrDefault(x => x.index == prop.index).y = transform.position.y;
        PlayerPref_DatabaseManager.Instance.props.FirstOrDefault(x => x.index == prop.index).z = transform.position.z;
        PlayerPref_DatabaseManager.Instance.props.FirstOrDefault(x => x.index == prop.index).a = transform.rotation.x;
        PlayerPref_DatabaseManager.Instance.props.FirstOrDefault(x => x.index == prop.index).b = transform.rotation.y;
        PlayerPref_DatabaseManager.Instance.props.FirstOrDefault(x => x.index == prop.index).c = transform.rotation.z;
    }


}
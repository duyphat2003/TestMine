using System.Collections.Generic;
using System.Linq;
using MyLibrary.Model;
using UnityEngine;

public class PropInfo : MonoBehaviour
{
    public Prop prop;
    [SerializeField] PlayerPref_DatabaseManager playerPref_DatabaseManager;

    void Start()
    {
        playerPref_DatabaseManager = FindObjectOfType<PlayerPref_DatabaseManager>();
    }
    void Update()
    {
        if(playerPref_DatabaseManager == null) return;
        playerPref_DatabaseManager.props.FirstOrDefault(x => x.index == prop.index).x = transform.position.x;
        playerPref_DatabaseManager.props.FirstOrDefault(x => x.index == prop.index).y = transform.position.y;
        playerPref_DatabaseManager.props.FirstOrDefault(x => x.index == prop.index).z = transform.position.z;
        playerPref_DatabaseManager.props.FirstOrDefault(x => x.index == prop.index).a = transform.rotation.x;
        playerPref_DatabaseManager.props.FirstOrDefault(x => x.index == prop.index).b = transform.rotation.y;
        playerPref_DatabaseManager.props.FirstOrDefault(x => x.index == prop.index).c = transform.rotation.z;
    }


}
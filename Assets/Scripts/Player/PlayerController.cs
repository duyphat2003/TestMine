using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] Camera cam;
    public LayerMask itemMask;
    SpectatorCameraFacade spectatorCameraFacade;
    [SerializeField] SpectatorCameraProperties spectatorCameraProperties;

    void Start()
    {
        spectatorCameraFacade = new SpectatorCameraFacade(spectatorCameraProperties);
    }

    void Update()
    {
        
    }
}

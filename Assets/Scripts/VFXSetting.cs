using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VFXSetting : MonoBehaviour
{
    void Update()
    {
        if (GetComponent<ParticleSystem>().isStopped)
        {
            Debug.Log("Particle System đã hoàn thành.");
            // Thực hiện các hành động sau khi Particle System hoàn thành
            Destroy(gameObject);
        }
    }
}

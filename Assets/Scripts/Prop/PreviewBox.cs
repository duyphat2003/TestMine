using UnityEngine;

public class PreviewBox : MonoBehaviour
{
    public bool isTrigger;
    [SerializeField] Material[] materials;

    void Start()
    {
        isTrigger = false;
    }

    void Update()
    {
        GetComponent<MeshRenderer>().material = isTrigger ? materials[1] : materials[0];
    }
    void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.CompareTag("Object"))
            isTrigger = true;
    }

    void OnTriggerStay(Collider other)
    {
        isTrigger = true;
            
    }

    void OnTriggerExit(Collider other)
    {
        isTrigger = false;
            
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    
    [SerializeField] private float followSpeed = 0.1f; //Que tan rapido sigue la camara al pj

    [SerializeField] private Vector3 offset; //La camara no se aleje del pj
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
            transform.position = Vector3.Lerp(transform.position, PJ.Instance.transform.position + offset, followSpeed);
    }
}

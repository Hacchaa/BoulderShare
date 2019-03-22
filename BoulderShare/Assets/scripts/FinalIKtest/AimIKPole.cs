using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AimIKPole : MonoBehaviour
{
    [SerializeField] Transform pelvis;
    [SerializeField] Transform head;
    [SerializeField] float length = 5.0f;

    // Update is called once per frame
    void Update()
    {
        transform.position = (head.position - pelvis.position).normalized * length +  head.position;
    }
}

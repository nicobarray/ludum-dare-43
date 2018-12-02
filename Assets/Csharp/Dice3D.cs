using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dice3D : MonoBehaviour
{
    // Use this for initialization
    void Start()
    {
        GetComponent<Rigidbody>().AddExplosionForce(10, Vector3.left, 45, 1, ForceMode.VelocityChange);
    }
}

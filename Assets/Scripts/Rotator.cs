using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotator : MonoBehaviour
{
    [SerializeField]private int rotationX;
    [SerializeField]private int rotationY;
    [SerializeField]private int rotationZ;
    [SerializeField] private int rotateSpeed;
    void Update()
    {
        transform.Rotate(new Vector3(rotationX, rotationY, rotationZ) * rotateSpeed * Time.deltaTime);
    }

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileGameLogic : MonoBehaviour
{
    public float selfDestructionDelay = 3f;


    void Start()
    {
        Destroy (gameObject, selfDestructionDelay);
    }

    void Update()
    {
    }
}

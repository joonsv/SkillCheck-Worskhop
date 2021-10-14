using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveEnemy : MonoBehaviour
{
    [SerializeField]private float maxMoveDistance = 10;
    //Set this to your objects initial position when game starts.
    Vector3 origin;
    [SerializeField]private float speed = 10;
    void Start()
    {
        origin = this.GetComponent<Rigidbody>().position;
    }

    void FixedUpdate()
    {
        Vector3 destination = origin;
        destination.y = (transform.position.y > origin.y + maxMoveDistance) ? origin.y : origin.y + maxMoveDistance;
        transform.position = Vector3.MoveTowards(transform.position, destination, speed * Time.deltaTime);
    }
}

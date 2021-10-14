using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveEnemy : MonoBehaviour
{
    [SerializeField]private float maxMoveDistance = 10;
    //Set this to your objects initial position when game starts.
    Rigidbody rb;
    Vector3 origin;
    [SerializeField]private float speed = 10;
    // Start is called before the first frame update
    void Start()
    {
        origin = this.GetComponent<Rigidbody>().position;
    }


    void Update()
    {
        Vector3 destination = origin;
        destination.y = (transform.position.y > origin.y + maxMoveDistance) ? origin.y : origin.y + maxMoveDistance;
        //destination.y = (transform.position.x > origin.x + maxMoveDistance) ? origin.x : origin.x + maxMoveDistance;
        transform.position = Vector3.MoveTowards(transform.position, destination, speed * Time.deltaTime);
    }
}

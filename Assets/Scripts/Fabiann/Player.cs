using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    [SerializeField] private int DebugValue;
    [SerializeField] private float Speed;

    private float horizontal;
    private float vertical;
    private float mouseX;
    private float mouseY;
    private Vector3 deltaPostition;
    private int skillPoints;
    private int breadCrumb;

    private Rigidbody rbody;

    private void Start()
    {
        rbody = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        Vector3 movement = new Vector3(horizontal, 0.0f, vertical);
        movement.Normalize();
        transform.position = transform.position + (movement * Speed * Time.deltaTime);
    }
    void OnMove(InputValue movementValue)
    {
        Vector2 movementVector = movementValue.Get<Vector2>();

        horizontal = movementVector.x;
        vertical = movementVector.y;
    }

    void OnLook(InputValue MousveMovementValue)
    {
        Vector2 movementVector = MousveMovementValue.Get<Vector2>();

        mouseX = movementVector.x;
        mouseY = movementVector.y;
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("SkillPoint"))
        {
            other.gameObject.SetActive(false);
            skillPoints += 1;
        }
        if (other.gameObject.CompareTag("BreadCrumb"))
        {
            other.gameObject.SetActive(false);
            breadCrumb += 1;
        }


    }
}

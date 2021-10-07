using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class FirstPersonPlayerController : MonoBehaviour
{
    [SerializeField, Range(10f, 100f)] private float mouseSensitivity = 70f; 
    [SerializeField] private float WalkSpeed;
    [SerializeField] private float MinRotation;
    [SerializeField] private float MaxRotation;

    private float horizontal;
    private float vertical;
    private float mouseX;
    private float mouseY;
    private Vector3 deltaPostition;
    private Quaternion deltaRotation;
    private Quaternion QuatMaxRotation;
    private Quaternion QuatMinRotation;

    private float cameraRotation;

    private Rigidbody rbody;
    Transform _cameraTransform;

    private void Start()
    {
        rbody = GetComponent<Rigidbody>();
        _cameraTransform = Camera.main.transform;
        QuatMaxRotation = Quaternion.Euler(new Vector3(MaxRotation, 0, 0));
        QuatMinRotation = Quaternion.Euler(new Vector3(MinRotation, 0, 0));
    }

    private void Update()
    {
        if(_cameraTransform.rotation.x < QuatMaxRotation.x && _cameraTransform.rotation.x > QuatMinRotation.x)
        {
            //_cameraTransform.Rotate(-Vector3.right * mouseY * mouseSensitivity * Time.deltaTime);
            //_cameraTransform.Rotate(QuatMaxRotation);
        }
        cameraRotation = _cameraTransform.rotation.x;
        
    }

    private void FixedUpdate()
    {
        deltaPostition = ((transform.forward * vertical) + (transform.right * horizontal)) * WalkSpeed * Time.deltaTime;
        rbody.MovePosition(rbody.position + deltaPostition);

        deltaRotation = Quaternion.Euler(Vector3.up * mouseX * mouseSensitivity * Time.deltaTime);
        rbody.MoveRotation(rbody.rotation * deltaRotation);
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
}

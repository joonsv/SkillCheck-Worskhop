using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class FirstPersonPlayerController : Abilities
{
    [SerializeField, Range(10f, 100f)] private float mouseSensitivity = 40f; 
    [SerializeField] private float WalkSpeed = 10;
    [SerializeField] private float MinCameraRotation = -90;
    [SerializeField] private float MaxCameraRotation = 90;
    [SerializeField] private GameObject mainMenu;
    private GameObject Reader;

    [SerializeField] private bool JumpIsUnlocked;
    [SerializeField] private bool DoubleJumpIsUnlocked;
    [SerializeField] private bool SuperJumpIsUnlocked;
    [SerializeField] private bool DashIsUnlocked;
    [SerializeField] private bool FireIsUnlocked;
    [SerializeField] private bool LargeMagazineIsUnlocked;
    
    private float horizontal;
    private float vertical;
    private float mouseX;
    private float mouseY;
    private Vector3 deltaPostition;
    private Vector3 DashMoveDirection;
    private Quaternion deltaRotation;
    private Quaternion QuatMaxRotation;
    private Quaternion QuatMinRotation;
    private Camera _cameraTransform;
    

    public object RangeStatus { get; private set; }

    private void Start()
    {
        rbody = GetComponent<Rigidbody>();
        _cameraTransform = Camera.main;
        Reader = gameObject;
    }

    private void Update()
    {
        CameraHandler();
        JumpIsUnlocked = Reader.GetComponent<SkillTreeReader>().IsSkillUnlocked(0);
        DashIsUnlocked = Reader.GetComponent<SkillTreeReader>().IsSkillUnlocked(1);
        SuperJumpIsUnlocked = Reader.GetComponent<SkillTreeReader>().IsSkillUnlocked(2);
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
        //This is bound to the "wasd" keys
        Vector2 movementVector = movementValue.Get<Vector2>();

        horizontal = movementVector.x;
        vertical = movementVector.y;
    }

    void OnLook(InputValue MousveMovementValue)
    {
        //This is bound to the mouse.
        Vector2 movementVector = MousveMovementValue.Get<Vector2>();

        mouseX = movementVector.x;
        mouseY = movementVector.y;
    }

    private void OnJump()
    {
        //This is bound to the "Space" key.
        NormalJump(JumpIsUnlocked);
        MidAirJump(DoubleJumpIsUnlocked);
    }

    private void OnSuperJump()
    {
        //This is currently bound to the 'R' key.
        SuperJump(SuperJumpIsUnlocked);
    }

    private void OnMenu()
    {
        //This is bound to the "Esc" key.
        print("dasd");
        if (mainMenu != null)
        {
            if (!mainMenu.activeSelf)
            {
                mainMenu.SetActive(true);
            }
            else
            {
                mainMenu.SetActive(false);
            }
        }
    }

    private void OnFire()
    {
        //This is bound to the left mouse button.
        Fire(FireIsUnlocked, LargeMagazineIsUnlocked);
    }

    private void OnDash()
    {
        //This is bound to the shift key
        Dash(DashIsUnlocked);
    }

    private void CameraHandler()
    {
        if (_cameraTransform.transform.rotation.x >= MaxCameraRotation && mouseY > 0)
        {
            _cameraTransform.transform.Rotate(new Vector3(-mouseY, 0, 0) * mouseSensitivity * Time.deltaTime);
        }
        else if (_cameraTransform.transform.rotation.x <= MinCameraRotation && mouseY < 0)
        {
            _cameraTransform.transform.Rotate(new Vector3(-mouseY, 0, 0) * mouseSensitivity * Time.deltaTime);
        }
        else if (_cameraTransform.transform.rotation.x < MaxCameraRotation && _cameraTransform.transform.rotation.x > MinCameraRotation)
        {
            _cameraTransform.transform.Rotate(new Vector3(-mouseY, 0, 0) * mouseSensitivity * Time.deltaTime);
        }
        _cameraTransform.transform.eulerAngles = new Vector3(_cameraTransform.transform.eulerAngles.x, transform.eulerAngles.y, 0.0f);
    }

    private void OnCollisionStay(Collision other)
    {
        if (other.gameObject.CompareTag("Surface"))
        {
            isGrounded = true;
            DoubleJump = true;
        }
    }

    private void OnCollisionExit(Collision other)
    {
        if (other.gameObject.CompareTag("Surface"))
        {
            isGrounded = false;
        }
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class FirstPersonPlayerController : MonoBehaviour
{
    [SerializeField, Range(10f, 100f)] private float mouseSensitivity = 40f; 
    [SerializeField] private float WalkSpeed = 10;
    [SerializeField] private float MinCameraRotation = -90;
    [SerializeField] private float MaxCameraRotation = 90;
    [SerializeField] private float JumpHeight = 7;
    [SerializeField] private GameObject projectile;
    [SerializeField] private Transform shootingPoint;
    [SerializeField] private int maxCubes = 3;
    [SerializeField] private int LargeMagazineMaxCubes = 10;
    [SerializeField] private float shootingSpeed = 100;
    [SerializeField] private float rotationSpeed = 10;
    [SerializeField] private float SuperJumpModifier = 2;
    [SerializeField] private float MaxDashTime = 0.4f;
    [SerializeField] private float DashDistance = 20f;
    [SerializeField] private GameObject mainMenu;

    [SerializeField] private bool JumpIsUnlocked;
    [SerializeField] private bool DoubleJumpIsUnlocked;
    [SerializeField] private bool SuperJumpIsUnlocked;
    [SerializeField] private bool DashIsUnlocked;
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

    private bool DoubleJump;
    private bool isGrounded;
    private bool fire = false;

    private Rigidbody rbody;
    private Rigidbody ProjectileRBody;
    private Camera _cameraTransform;

    public object RangeStatus { get; private set; }

    private void Start()
    {
        rbody = GetComponent<Rigidbody>();
        _cameraTransform = Camera.main;
        ProjectileRBody = projectile.GetComponent<Rigidbody>();
    }

    private void Update()
    {
        CameraHandler();
        
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

    private void OnJump()
    {
        if (JumpIsUnlocked)
        {
            NormalJump();
        }
        if (DoubleJumpIsUnlocked)
        {
            MidAirJump();
        }
    }

    private void OnSuperJump()
    {
        if (SuperJumpIsUnlocked)
        {
            SuperJump();
        }
    }

    private void NormalJump()
    {
        if (isGrounded)
        {
            Vector3 Jump = new Vector3(0.0f, JumpHeight, 0.0f);
            rbody.AddForce(Jump, ForceMode.Impulse);
        }
    }

    private void MidAirJump()
    {
        //Debug.Log("Double Jump attempt");
        if (DoubleJump == true && isGrounded == false)
        {
            //Debug.Log("Succes");
            Vector3 Jump = new Vector3(0.0f, JumpHeight, 0.0f);
            rbody.AddForce(Jump, ForceMode.Impulse);
            DoubleJump = false;
        }
    }
    private void OnMenu()
    {
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

    private void SuperJump()
    {
        if (isGrounded)
        {
            Vector3 Jump = new Vector3(0.0f, JumpHeight*SuperJumpModifier, 0.0f);
            rbody.AddForce(Jump, ForceMode.Impulse);
        }
    }

    private void OnFire()
    {
        if (GameObject.FindGameObjectsWithTag("Projectile").Length < maxCubes)
        {
            fire = !fire;
            if (LargeMagazineIsUnlocked)
            {
                maxCubes = LargeMagazineMaxCubes;
            }
        }
        if (fire)
        {
            fire = !fire;
            Rigidbody projectileInstance;
            projectileInstance =
                Instantiate(ProjectileRBody,
                shootingPoint.position,
                shootingPoint.rotation);
            projectileInstance.AddForce(shootingSpeed * shootingPoint.forward);
            projectileInstance.AddRelativeTorque(0, rotationSpeed, 0);
        }
    }

    private void OnDash()
    {
        if (DashIsUnlocked)
        {
            Dash();
        }
    }

    private void Dash()
    {
        rbody.constraints |= RigidbodyConstraints.FreezePositionY;
        rbody.velocity = rbody.transform.forward * DashDistance;
        Invoke("StopDash", MaxDashTime);
    }

    private void StopDash()
    {
        rbody.constraints &= ~RigidbodyConstraints.FreezePositionY;
        rbody.velocity = Vector3.zero;
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

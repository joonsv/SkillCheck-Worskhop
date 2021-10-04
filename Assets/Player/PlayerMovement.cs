using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
public class PlayerMovement : MonoBehaviour
{
    Vector2 moveDirection;

    bool jumping = false;

    BoxCollider boxCollider;

    float lookDirection;

    public float maxCubes = 3;

    public Transform shootingPoint;

    public float shootingSpeed = 100;

    public float rotationSpeed = 10;

    bool fire = false;

    public Rigidbody projectile;

    public float moveSpeed = 10;

    private float baseJumpSpeed;

    public float jumpSpeed = 200;

    public float lookSpeed = 5;

    public float boxColliderDistanceToFloor = 0.05f;
    void Start()
    {
        baseJumpSpeed = jumpSpeed;
        boxCollider = gameObject.GetComponent<BoxCollider>();
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        moveDirection = context.ReadValue<Vector2>();
    }

    void Move(Vector2 direction)
    {
        Vector3 vector3 =
            transform.right * direction.x + transform.forward * direction.y;
        this
            .gameObject
            .GetComponent<Rigidbody>()
            .MovePosition(transform.position +
            vector3 * Time.deltaTime * moveSpeed);
        // transform.Translate(direction.x * moveSpeed * Time.deltaTime, 0, direction.y * moveSpeed * Time.deltaTime);
    }

    public void OnLook(InputAction.CallbackContext context)
    {
        lookDirection = context.ReadValue<float>();
    }

    void Look(float direction)
    {
        transform
            .Rotate(Vector3.up, lookDirection * lookSpeed * Time.deltaTime);
    }

    bool testShooter;

    public void OnFire()
    {
        if (GameObject.FindGameObjectsWithTag("projectile").Length < maxCubes)
        {
            fire = !fire;
        }
    }

    public void Fire()
    {
        if (fire)
        {
            fire = !fire;
            Rigidbody projectileInstance;
            projectileInstance =
                Instantiate(projectile,
                shootingPoint.position,
                shootingPoint.rotation);
            projectileInstance.AddForce(shootingSpeed * shootingPoint.forward);
            projectileInstance.AddRelativeTorque(0, rotationSpeed, 0);
        }
    }

    public void OnJump()
    {
        jumping =
            Physics
                .Raycast(boxCollider.bounds.center,
                Vector3.down,
                boxCollider.bounds.extents.y + boxColliderDistanceToFloor);
    }

    public void Jump()
    {
        if (SkillTreeReader.Instance.IsSkillUnlocked(1))
        {
            jumpSpeed = baseJumpSpeed * 2f;
        }
        if (jumping)
        {
            jumping = !jumping;
            gameObject
                .GetComponent<Rigidbody>()
                .AddForce(transform.up * jumpSpeed);
        }
    }

    void Update()
    {
        Jump();
        Fire();
    }

    void FixedUpdate()
    {
        Move(moveDirection);
        Look(lookDirection);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Abilities : MonoBehaviour
{
    [SerializeField] private float JumpHeight = 7;
    [SerializeField] private GameObject projectile;
    [SerializeField] private Transform shootingPoint;
    [SerializeField] private int DefaultmaxCubes = 3;
    [SerializeField] private int LargeMagazineMaxCubes = 10;
    [SerializeField] private float shootingSpeed = 100;
    [SerializeField] private float rotationSpeed = 10;
    [SerializeField] private float SuperJumpModifier = 2;
    [SerializeField] private float MaxDashTime = 0.4f;
    [SerializeField] private float DashDistance = 20f;
    [SerializeField] private float DashCooldown = 2f;

    private bool DashIsAvailable = true;
    private bool DoubleJump;
    private bool isGrounded;
    private bool fire = false;

    private int maxCubes;

    private Rigidbody rbody;
    private Rigidbody ProjectileRBody;

    private void Start()
    {
        ProjectileRBody = projectile.GetComponent<Rigidbody>();
        maxCubes = DefaultmaxCubes;
        rbody = GetComponent<Rigidbody>();
    }

    protected void NormalJump(bool JumpIsUnlocked)
    {
        if (JumpIsUnlocked)
        {
            if (isGrounded)
            {
                Vector3 Jump = new Vector3(0.0f, JumpHeight, 0.0f);
                rbody.AddForce(Jump, ForceMode.Impulse);
            }
        }
    }

    protected void MidAirJump(bool DoubleJumpIsUnlocked)
    {
        if (DoubleJumpIsUnlocked)
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
    }

    protected void SuperJump(bool SuperJumpIsUnlocked)
    {
        if (SuperJumpIsUnlocked)
        {
            if (isGrounded)
            {
                Vector3 Jump = new Vector3(0.0f, JumpHeight * SuperJumpModifier, 0.0f);
                rbody.AddForce(Jump, ForceMode.Impulse);
            }
        }
    }

    protected void Fire(bool FireIsUnlocked, bool LargeMagazineIsUnlocked)
    {
        if (FireIsUnlocked)
        {
            if (GameObject.FindGameObjectsWithTag("Projectile").Length < maxCubes)
            {
                fire = !fire;
                if (LargeMagazineIsUnlocked)
                {
                    maxCubes = LargeMagazineMaxCubes;
                }
                else
                {
                    maxCubes = DefaultmaxCubes;
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
    }

    protected void Dash(bool DashIsUnlocked)
    {
        if (DashIsUnlocked)
        {
            if (DashIsAvailable)
            {
                DashIsAvailable = false;
                rbody.constraints |= RigidbodyConstraints.FreezePositionY;
                rbody.velocity = rbody.transform.forward * DashDistance;
                Invoke("StopDash", MaxDashTime);
                Invoke("ResetDash", DashCooldown);
            }
        }
    }

    private void StopDash()
    {
        rbody.constraints &= ~RigidbodyConstraints.FreezePositionY;
        rbody.velocity = Vector3.zero;
    }

    private void ResetDash()
    {
        DashIsAvailable = true;
    }
}

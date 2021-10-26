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
    protected bool DoubleJump;
    protected bool isGrounded;
    private bool fire = false;

    private int maxCubes;

    protected Rigidbody rbody;
    private Rigidbody ProjectileRBody;

    private void OnEnable()
    {
        ProjectileRBody = projectile.GetComponent<Rigidbody>();
    }

    protected void NormalJump(bool JumpIsUnlocked)
    {
        if (JumpIsUnlocked)
        {
            //Make your own function
        }
    }

    protected void MidAirJump(bool DoubleJumpIsUnlocked)
    {
        if (DoubleJumpIsUnlocked)
        {
            //Make your own function
        }
    }

    protected void SuperJump(bool SuperJumpIsUnlocked)
    {
        if (SuperJumpIsUnlocked)
        {
            //Make your own function
        }
    }

    protected void Fire(bool FireIsUnlocked, bool LargeMagazineIsUnlocked)
    {
        if (FireIsUnlocked)
        {
            //Make your own function
        }
    }

    protected void Dash(bool DashIsUnlocked)
    {
        if (DashIsUnlocked)
        {
            //Make your own function
        }
    }
}

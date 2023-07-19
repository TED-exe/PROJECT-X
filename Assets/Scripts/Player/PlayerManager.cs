using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PlayerManager : MonoBehaviour
{
    private PlayerCrouchingSystem crouchingSystem;
    private PlayerMovement movementSystem;
    private PlayerRotateSystem rotateSystem;
    private ControllHolder controllHolder;
    private PlayerPullObjectSystem pullObjectSystem;

    [Header("TRANSFORM")]
    [SerializeField] private Transform tr_raycastCaster;
    [SerializeField] private Transform tr_playerCollider;
    [Header("FLOAT")]
    [SerializeField] private so_floatValue f_crouchSpeed;
    [SerializeField] private so_floatValue f_walkSpeed;
    [SerializeField] private so_floatValue f_runSpeed;
    [SerializeField] private so_floatValue f_pullingSpeed;
    [SerializeField] private so_floatValue f_moveSpeed;
    [SerializeField] private so_floatValue f_playerHeight;
    [Header("BOOL")]
    [SerializeField] private so_boolValue b_isCrouching;
    [SerializeField] private so_boolValue b_isPulling;
    [SerializeField] private so_boolValue b_canRotate;
    private bool b_isGround;

    private EnumsHolder.MovementState state;

    private void Awake()
    {
        f_playerHeight.value = GetComponentInChildren<CapsuleCollider>().height;
        pullObjectSystem = GetComponent<PlayerPullObjectSystem>();
        crouchingSystem = GetComponent<PlayerCrouchingSystem>();
        movementSystem = GetComponent<PlayerMovement>();
        rotateSystem = GetComponent<PlayerRotateSystem>();
        controllHolder = GetComponent<ControllHolder>();

        BaseSetUp(); //base game set up;
    }
    private void Update()
    {
        b_isGround = movementSystem.GroundCheck(tr_raycastCaster); // chechk Ground
        movementSystem.SpeedControll(); // controll max player speed
        movementSystem.HandleDrag(b_isGround); // pulling the player to the ground (change in air and on ground)
        movementSystem.MyInput(controllHolder); // take velocity from movement button
        movementSystem.OnSlopeOffGravity(tr_raycastCaster);// turn off gravity while slope
        StateHandler(); // state controller
        if (b_canRotate.value) rotateSystem.RotatePlayer(controllHolder, movementSystem.CalculatePlayerDirection()); // rotate player (free and locked)
        crouchingSystem.MyInput(controllHolder, tr_raycastCaster); // take input crouching
        crouchingSystem.Crouching(tr_playerCollider); // crouching
        pullObjectSystem.MyInput(controllHolder, tr_raycastCaster);// pulling Object
    }
    private void FixedUpdate()
    {
        movementSystem.MovePlayer(tr_raycastCaster); // player movement
    }
    private void BaseSetUp()
    {
        crouchingSystem.BaseSetUp(tr_playerCollider); // base set Up for crouching
        b_canRotate.value = true;
        b_isPulling.value = false;
        b_isCrouching.value = false;
    }
    private void StateHandler() // przeniesc to potem do player managera
    {
        //state pulling
        if (b_isGround && (Input.GetKey(controllHolder.kc_allowToPullKey) && b_isPulling.value))
        {
            f_moveSpeed.value = f_pullingSpeed.value;
            state = EnumsHolder.MovementState.pullingObject;
            return;
        }
        // state crouching
        else if (b_isGround && (Input.GetKey(controllHolder.kc_crouchKey) || b_isCrouching.value))
        {
            f_moveSpeed.value = f_crouchSpeed.value;
            state = EnumsHolder.MovementState.crouching;
            return;
        }
        // state running
        else if (b_isGround && Input.GetKey(controllHolder.kc_runKey))
        {
            f_moveSpeed.value = f_runSpeed.value;
            state = EnumsHolder.MovementState.running;
            return;
        }
        // state walking
        else if (b_isGround)
        {
            f_moveSpeed.value = f_walkSpeed.value;
            state = EnumsHolder.MovementState.walking;
            return;
        }
        // state air
        else if (!b_isGround)
        {
            state = EnumsHolder.MovementState.air;
            return;
        }
    }
}

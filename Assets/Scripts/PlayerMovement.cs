using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class PlayerMovement : MonoBehaviour
{
    [Header("MOVEMENT")]
    [SerializeField] private Transform tr_playerCollider;
    [SerializeField] private float f_walkSpeed;
    [SerializeField] private float f_runSpeed;

    [Header("SLOPE MOVEMENT")]
    [SerializeField] private float f_maxSlopeAngle;
    private RaycastHit rh_slopeHit;

    [Header("ROTATION")]
    [SerializeField] private LayerMask lm_groundCollideWithRay;
    [SerializeField] private float f_freeRotateSpeed;
    [SerializeField] private float f_closedRotateSpeed;

    [Header("CROUCH")]
    [SerializeField] private LayerMask lm_roofColideWithRay;
    [SerializeField] private float f_crouchSpeed;
    [SerializeField] private float f_crouchYScale;
    [SerializeField] private float f_crouchTransitionSpeed;
    private RaycastHit rh_crouchingRaycastHit;
    private float f_startYScale;
    private float f_currentHeight;
    private bool b_isCrouching;

    [Header("GROUND CHECK")]
    [SerializeField] private LayerMask lm_whatIsWalkingGround;
    [SerializeField] private Transform tr_raycastCaster;
    [SerializeField] private float f_groundDrag;
    [SerializeField] private float f_LeanghtToAddInRay;
    private float f_playerHeight;
    private bool b_grounded;

    private float f_horizontal;
    private float f_vertical;
    private float f_moveSpeed;

    Vector3 v3_moveDirection;

    private Rigidbody rb;
    private ControllHolder controllHolder;

    private EnumsHolder.MovementState state;



    private void Awake()
    {
        f_playerHeight = GetComponentInChildren<CapsuleCollider>().height;
        f_startYScale = tr_playerCollider.localScale.y;
        f_currentHeight = f_startYScale;//GetComponentInChildren<CapsuleCollider>().height; 
        rb = GetComponent<Rigidbody>();
        controllHolder = GetComponent<ControllHolder>();
    }
    private void Update()
    {
        DEBUGS();

        // ground check (bool)
        b_grounded = Physics.Raycast(tr_raycastCaster.position, Vector3.down, f_playerHeight * 0.5f + f_LeanghtToAddInRay, lm_whatIsWalkingGround);

        MyInput();
        SpeedControll();
        RotatePlayer();
        StateHandler();
        Crouching();

        //handle drag 
        if (b_grounded)
            rb.drag = f_groundDrag;
        else
            rb.drag = 0;
    }

    private void FixedUpdate()
    {
        MovePlayer();
    }
    private void StateHandler()
    {
        // state running
        if (b_grounded && Input.GetKey(controllHolder.kc_runKey))
        {
            f_moveSpeed = f_runSpeed;
            state = EnumsHolder.MovementState.running;
            return;
        }
        // state crouching
        else if (b_grounded && (Input.GetKey(controllHolder.kc_crouchKey) || b_isCrouching))
        {
            f_moveSpeed = f_crouchSpeed;
            state = EnumsHolder.MovementState.crouching;
            return;
        }
        // state walking
        else if (b_grounded)
        {
            f_moveSpeed = f_walkSpeed;
            state = EnumsHolder.MovementState.walking;
            return;
        }
        // state air
        else if (!b_grounded)
        {
            state = EnumsHolder.MovementState.air;
            return;
        }

        //turn off gravity while slope
        rb.useGravity = !OnSlope();
    }
    private void MyInput()
    {
        // get Input from InputHolder
        f_vertical = controllHolder.f_verticalInput;
        f_horizontal = controllHolder.f_horizontalInput;

        // crouching
        var crouchDelta = Time.deltaTime * f_crouchTransitionSpeed;
        if (Input.GetKey(controllHolder.kc_crouchKey))
        {
            f_currentHeight = Mathf.Lerp(f_currentHeight, f_crouchYScale, crouchDelta);
            f_playerHeight = GetComponentInChildren<CapsuleCollider>().height;
            b_isCrouching = true;
        }
        else if (!Input.GetKey(controllHolder.kc_crouchKey))
        {
            //check u can stay in this room. need to add layer;
            if (!Physics.Raycast(tr_raycastCaster.position, Vector3.up, f_playerHeight * 0.5f + f_LeanghtToAddInRay + (f_startYScale - f_crouchYScale - 0.1f), lm_roofColideWithRay))
            {
                f_currentHeight = Mathf.Lerp(f_currentHeight, f_startYScale, crouchDelta);
                f_playerHeight = GetComponentInChildren<CapsuleCollider>().height;
                b_isCrouching = false;
            }

        }
    }
    private void Crouching()
    {
        tr_playerCollider.localScale = new Vector3(tr_playerCollider.localScale.x, f_currentHeight, tr_playerCollider.localScale.z);

        if (b_isCrouching)
        {
            if (Physics.Raycast(tr_raycastCaster.position, Vector3.up, out rh_crouchingRaycastHit, f_playerHeight * 0.5f + f_LeanghtToAddInRay + (f_startYScale - f_crouchYScale - 0.1f), lm_roofColideWithRay))
            {
                Debug.Log("mleko");
                var material = rh_crouchingRaycastHit.collider.GetComponent<Renderer>().material;
                material.SetFloat("Opacity", 0f);
                Debug.Log(material.name);
            }
        }
    }
    private void RotatePlayer()
    {
        // if hold button rotate player is free (you can move and look on other side)
        if (Input.GetKey(controllHolder.kc_allowToRotatePlayerKey))
        {
            var (succes, position) = GetMousePosition();
            if (succes)
            {
                //calculate look direction (without Y axis)
                var direction = position - transform.position;
                direction.y = 0;

                transform.forward = Vector3.Lerp(transform.forward, direction, f_freeRotateSpeed * Time.deltaTime);
            }
        }
        else
        {
            // if freelooking off player look in move direction;
            if (v3_moveDirection != Vector3.zero)
            {
                Quaternion toRotate = Quaternion.LookRotation(v3_moveDirection, Vector3.up);
                transform.rotation = Quaternion.RotateTowards(transform.rotation, toRotate, f_closedRotateSpeed * Time.deltaTime);
            }
        }
    }
    private (bool succes, Vector3 position) GetMousePosition()
    {
        //ray from camera to mouse pointer;
        var ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out var hitInfo, Mathf.Infinity, lm_groundCollideWithRay))
            //if ray hit in groun send mouse position
            return (succes: true, position: hitInfo.point);
        else
            //else send vector3(0,0,0)
            return (succes: false, Position: Vector3.zero);
    }
    private void MovePlayer()
    {
        // on slope
        if (OnSlope())
        {
            rb.AddForce(GetSlopeMoveDirection() * f_moveSpeed * 20f, ForceMode.Force);

            if (rb.velocity.y > 0)
                rb.AddForce(Vector3.down * 80f, ForceMode.Force);
        }

        // calculate Direction;
        v3_moveDirection = new Vector3(f_horizontal, 0f, f_vertical);

        rb.AddForce(v3_moveDirection.normalized * f_moveSpeed * 10, ForceMode.Force);
    }
    private void SpeedControll()
    {
        Vector3 flatVel = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
        if (flatVel.magnitude > f_moveSpeed)
        {
            Vector3 limitedVel = flatVel.normalized * f_moveSpeed;
            rb.velocity = new Vector3(limitedVel.x, rb.velocity.y, limitedVel.z);
        }
    }
    private bool OnSlope()
    {
        if (Physics.Raycast(tr_raycastCaster.position, Vector3.down, out rh_slopeHit, f_playerHeight * 0.5f + f_LeanghtToAddInRay))
        {
            float angle = Vector3.Angle(Vector3.up, rh_slopeHit.normal);
            return angle < f_maxSlopeAngle && angle != 0;
        }
        return false;
    }
    private Vector3 GetSlopeMoveDirection()
    {
        return Vector3.ProjectOnPlane(v3_moveDirection, rh_slopeHit.normal).normalized;
    }

    private void DEBUGS()
    {
        //Debug.Log(Input.GetKey(controllHolder.kc_crouchKey));
        //Debug.Log(f_moveSpeed);
        Debug.Log(Physics.Raycast(tr_raycastCaster.position, Vector3.up, f_playerHeight * 0.5f + f_LeanghtToAddInRay + (f_startYScale - f_crouchYScale - 0.1f), lm_roofColideWithRay));
        //if (state == EnumsHolder.MovementState.crouching)
        //   Debug.DrawLine(tr_raycastCaster.position, tr_raycastCaster.position + new Vector3(0, f_playerHeight * 0.5f + f_LeanghtToAddInRay + (f_startYScale - f_crouchYScale - 0.1f), 0), Color.red);
        //else
        //  Debug.DrawLine(tr_raycastCaster.position, tr_raycastCaster.position + new Vector3(0, f_playerHeight * 0.5f + f_LeanghtToAddInRay, 0), Color.red);
    }
}

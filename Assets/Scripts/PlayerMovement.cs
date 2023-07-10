using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UIElements;

public class PlayerMovement : MonoBehaviour
{
    [Header("MOVEMENT")]
    [SerializeField] private float f_walkSpeed;
    [SerializeField] private float f_runSpeed;

    [Header("ROTATION")]
    [SerializeField] private LayerMask lm_groundCollideWithRay;
    [SerializeField] private float f_freeRotateSpeed;
    [SerializeField] private float f_closedRotateSpeed;
    [SerializeField] private Transform tr_cameraFollowThis;

    [Header("GROUND CHECK")]
    [SerializeField] private LayerMask lm_whatIsWalkingGround; // przypisać
    [SerializeField] private Transform tr_raycastCaster; // przypisać
    private float f_playerHeight; // dp wykorzystania
    private bool grouinded; // do wykorzystania

    private float f_horizontal;
    private float f_vertical;
    private float f_moveSpeed;

    Vector3 v3_moveDirection;

    private Rigidbody rb;
    private ControllHolder controllHolder;

    private void Awake()
    {
        f_playerHeight = GetComponentInChildren<CapsuleCollider>().height; // sprawdzić czy to na pewno to bo nie jestem pewien
        rb = GetComponent<Rigidbody>();
        controllHolder = GetComponent<ControllHolder>();
    }
    private void Update()
    {
        MyInput();
        Debug.Log(rb.velocity.magnitude);
        SpeedControll();
        RotatePlayer();
    }

    // zastanowić sie na enumami (statetami)
    // zrobić slope movement https://www.youtube.com/watch?v=xCxSjgYTw9c&t=141s (tu to jest)
    private void GroundCheck() // dkończyć to SPRAWDZANIA CZY DOTYKASZ ZIEMI.
    {
        
    }
    private void FixedUpdate()
    {
        MovePlayer();
    }
    private void MyInput()
    {
        // get Input from InputHolder
        f_vertical = controllHolder.f_verticalInput;
        f_horizontal = controllHolder.f_horizontalInput;
    }
    private void RotatePlayer()
    {
        if (Input.GetKey(controllHolder.kc_allowToRotatePlayerKey)) // if hold button rotate player is free (you can move and look on other side)
        {
            Debug.Log("mleko");
            var (succes, position) = GetMousePosition();
            if (succes)
            {
                var direction = position - transform.position; //calculate look direction (without Y axis)
                direction.y = 0;

                transform.forward = Vector3.Lerp(transform.forward, direction, f_freeRotateSpeed * Time.deltaTime);
            }
        }
        else
        {
            Debug.Log("mleko2");
            if (v3_moveDirection != Vector3.zero)
            {
                Quaternion toRotate = Quaternion.LookRotation(v3_moveDirection, Vector3.up);
                transform.rotation = Quaternion.RotateTowards(transform.rotation, toRotate, f_closedRotateSpeed * Time.deltaTime);
            }
        }
        tr_cameraFollowThis.rotation = Quaternion.identity;
    }
    private (bool succes, Vector3 position) GetMousePosition()
    {
        var ray = Camera.main.ScreenPointToRay(Input.mousePosition); //ray from camera to mouse pointer;

        if (Physics.Raycast(ray, out var hitInfo, Mathf.Infinity, lm_groundCollideWithRay))
            return (succes: true, position: hitInfo.point); //if ray hit in groun send mouse position
        else
            return (succes: false, Position: Vector3.zero); //else send vector3(0,0,0)
    }
    private void MovePlayer()
    {
        // change speed (run and walk)
        if (Input.GetKey(controllHolder.kc_runKey))
            f_moveSpeed = f_runSpeed;
        else
            f_moveSpeed = f_walkSpeed;

        // calculate Direction;
        v3_moveDirection = new Vector3(f_horizontal, 0f, f_vertical);

        rb.AddForce(v3_moveDirection.normalized * f_moveSpeed, ForceMode.Force);
    }
    private void SpeedControll()
    {
        Vector3 flatVel = new Vector3(rb.velocity.x, 0f, rb.velocity.y);
        if(flatVel.magnitude > f_moveSpeed)
        {
            Vector3 limitedVel = flatVel.normalized * f_moveSpeed;
            rb.velocity = new Vector3(limitedVel.x, rb.velocity.y, limitedVel.z);
        }
    } //dostosować do slope movementu
}

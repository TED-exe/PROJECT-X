using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class PlayerMovement : MonoBehaviour
{
    [Header("MOVEMENT SETTING")]
    [SerializeField] private so_floatValue f_moveSpeed;
    [SerializeField] private so_vectorValue v3_moveDirection;
    [Header("SLOPE MOVEMENT SETTING")]
    [SerializeField] private float f_maxSlopeAngle;
    private RaycastHit rh_slopeHit;

    [Header("GROUND CHECK SETTING")]
    [SerializeField] private so_floatValue f_playerHeight;
    [SerializeField] private LayerMask lm_whatIsWalkingGround;
    [SerializeField] private float f_groundDrag;
    [SerializeField] private float f_LeanghtToAddInRay;

    private float f_horizontal;
    private float f_vertical;

    private Rigidbody rb;
    public void BaseSetUp()
    {
        v3_moveDirection.v3_value = Vector3.zero;
        rb = GetComponent<Rigidbody>();
    }
    public bool GroundCheck(Transform raycastCaster)
    {
        // ground check (bool)
        return Physics.Raycast(raycastCaster.position, Vector3.down, f_playerHeight.value * 0.5f + 0.2f, lm_whatIsWalkingGround);
    }
    public void HandleDrag(bool grounded)
    {
        //handle drag 
        if (grounded)
            rb.drag = f_groundDrag;
        else
            rb.drag = 0;
    }
    public void MyInput(ControllHolder controllHolder)
    {
        // get Input from InputHolder
        f_vertical = controllHolder.f_verticalInput;
        f_horizontal = controllHolder.f_horizontalInput;
    }
    public void MovePlayer(Transform raycastCaster)
    {

        // on slope
        if (OnSlope(raycastCaster))
        {
            rb.AddForce(GetSlopeMoveDirection() * f_moveSpeed.value * 20f, ForceMode.Force);

            if (rb.velocity.y > 0)
                rb.AddForce(Vector3.down * 80f, ForceMode.Force);
        }

        v3_moveDirection.v3_value = new Vector3(f_horizontal, 0f, f_vertical);

        rb.AddForce(v3_moveDirection.v3_value.normalized * f_moveSpeed.value * 10, ForceMode.Force);
    }
    public void SpeedControll()
    {
        Vector3 flatVel = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
        if (flatVel.magnitude > f_moveSpeed.value)
        {
            Vector3 limitedVel = flatVel.normalized * f_moveSpeed.value;
            rb.velocity = new Vector3(limitedVel.x, rb.velocity.y, limitedVel.z);
        }
    }
    private bool OnSlope(Transform raycastCaster)
    {
        if (Physics.Raycast(raycastCaster.position, Vector3.down, out rh_slopeHit, f_playerHeight.value * 0.5f + f_LeanghtToAddInRay))
        {
            float angle = Vector3.Angle(Vector3.up, rh_slopeHit.normal);
            return angle < f_maxSlopeAngle && angle != 0;
        }
        return false;
    }
    public void OnSlopeOffGravity(Transform raycastCaster)
    {
        rb.useGravity = !OnSlope(raycastCaster);
    }

    private Vector3 GetSlopeMoveDirection()
    {
        return Vector3.ProjectOnPlane(v3_moveDirection.v3_value, rh_slopeHit.normal).normalized;
    }
}

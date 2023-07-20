using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerRotateSystem : MonoBehaviour
{
    [Header("ROTATION")]
    [SerializeField] private LayerMask lm_groundCollideWithRay;
    [SerializeField] private float f_freeRotateSpeed;
    [SerializeField] private float f_closedRotateSpeed;
    [SerializeField] private so_vectorValue v3_moveDirection;

    public void RotatePlayer(ControllHolder controllHolder)
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
            if (v3_moveDirection.v3_value != Vector3.zero)
            {
                Quaternion toRotate = Quaternion.LookRotation(v3_moveDirection.v3_value, Vector3.up);
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
}

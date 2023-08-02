using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCrouchingSystem : MonoBehaviour
{
    [Header("CROUCH SETTING")]
    [SerializeField] private LayerMask lm_roofColideWithRay;
    [SerializeField] private float f_crouchYScale;
    [SerializeField] private float f_crouchTransitionSpeed;
    [SerializeField] private so_floatValue f_playerHeight;
    [SerializeField] private so_boolValue b_isCrouching;
    private float f_startYScale;
    private float f_currentHeight;
    

    public void BaseSetUp(Transform playerCollider)
    {
        f_startYScale = playerCollider.localScale.y;
        f_currentHeight = f_startYScale;
    }
    //TO DO: zamienić to na collider a nie na wykrywanie przez raycast (2 raycasty tutaj zabiorą zdecydowanie więcej fps niż jeden collider trigger)
    // narazie niech tak zostanie, ale potem do optymalizacji!!!!!!!
    public void MyInput(ControllHolder controllHolder, Transform raycastCasterV1, Transform raycastCasterV2 )
    {
        // crouching
        if (Input.GetKey(controllHolder.kc_crouchKey))
        {
            var crouchDelta = Time.deltaTime * f_crouchTransitionSpeed;
            f_currentHeight = Mathf.Lerp(f_currentHeight, f_crouchYScale, crouchDelta);
            f_playerHeight.value = GetComponentInChildren<CapsuleCollider>().height;
            b_isCrouching.value = true;
            return;
        }
        else if (!Input.GetKey(controllHolder.kc_crouchKey))
        {
            //check u can stay in this room. need to add layer;
            if (!Physics.Raycast(raycastCasterV1.position, Vector3.up, f_playerHeight.value * 0.5f + 0.2f + (f_startYScale - f_crouchYScale - 0.1f), lm_roofColideWithRay))
            {
                if(!Physics.Raycast(raycastCasterV2.position, Vector3.up, f_playerHeight.value * 0.5f + 0.2f + (f_startYScale - f_crouchYScale - 0.1f), lm_roofColideWithRay))
                {
                    var crouchDelta = Time.deltaTime * f_crouchTransitionSpeed;
                    f_currentHeight = Mathf.Lerp(f_currentHeight, f_startYScale, crouchDelta);
                    f_playerHeight.value = GetComponentInChildren<CapsuleCollider>().height;
                    b_isCrouching.value = false;
                }
            }
        }
    }

    //change player Height
    public void Crouching(Transform playerCollider)
    {
        playerCollider.localScale = new Vector3(playerCollider.localScale.x, f_currentHeight, playerCollider.localScale.z);
    }
}

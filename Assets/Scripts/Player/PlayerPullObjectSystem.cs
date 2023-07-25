using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPullObjectSystem : MonoBehaviour
{
    [Header("PULL SETTING")]
    [SerializeField] private float f_pullRange;
    [SerializeField] private so_floatValue f_moveSpeed;
    [SerializeField] private so_boolValue b_isPulling;
    [SerializeField] private so_boolValue b_canRotate;
    [SerializeField] private so_vectorValue v3_moveDirection;
    [SerializeField] private LayerMask lm_objectAbleToPull;
    private Transform tr_holder;
    public bool b_stayInColider;
    private RaycastHit rh_hitedObjectByRay;

    public void MyInput(ControllHolder controllHolder, Transform raycastCaster)
    {
        if (Input.GetKeyDown(controllHolder.kc_allowToPullKey))
            StartPullObject(raycastCaster);

        else if((Input.GetKeyUp(controllHolder.kc_allowToPullKey)))
            StopPullObject();

    }
    private void StopPullObject()
    {
        if (tr_holder == null)
            return;

        tr_holder.SetParent(null);
        tr_holder = null;
        b_canRotate.value = true;
        b_isPulling.value = false;
    }
    private void StartPullObject(Transform raycastCaster)
    {
        var(succes, objectToPull) = CheckObject(raycastCaster);
        tr_holder = objectToPull;

        if (!succes)
        {;
            return;
        }

        if(b_stayInColider)
        {
            tr_holder.SetParent(transform);
            b_canRotate.value = false;
            b_isPulling.value = true;
        }
    }
    private (bool succes, Transform objectToPull) CheckObject(Transform raycastCaster)
    {
        if (!Physics.Raycast(raycastCaster.position, raycastCaster.forward, out rh_hitedObjectByRay, f_pullRange, lm_objectAbleToPull))
            return (succes: false, objectToPull: null);

        return (succes: true, objectToPull: rh_hitedObjectByRay.transform);
    }
}

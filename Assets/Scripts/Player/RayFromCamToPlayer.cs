using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RayFromCamToPlayer : MonoBehaviour
{
    [Header("FADE OUT/FADE IN OBJECT")]
    [SerializeField] private LayerMask lm_objectCanToFadeOut;

    private Transform tr_camLookAtThis;
    private RaycastHit rh_objectHitByRay;
    private GameObject holder;


    private void Awake()
    {
        tr_camLookAtThis = GetComponentInChildren<CinemachineVirtualCamera>().LookAt;
    }

    private void Update()
    {
        
        var (success, hitGameobject) = GetGameobjectOnRay();
        if(!success)
        {
            if (holder != null)
            {
                if (holder.TryGetComponent(out ObjectFadeOut fadeOutOff))
                {
                    fadeOutOff.b_doFade = false;
                    holder = null;
                }
            }
            return;
        }
        holder = hitGameobject;
        if(hitGameobject.TryGetComponent(out ObjectFadeOut fadeOutOn))
        {
            fadeOutOn.b_doFade = true;
        }

    }
    private (bool success, GameObject hitGameobject) GetGameobjectOnRay()
    {
        var direction = Camera.main.transform.position - tr_camLookAtThis.position;
        var length = Vector3.Distance(Camera.main.transform.position, tr_camLookAtThis.position);
        if(Physics.Raycast(Camera.main.transform.position, -    direction,out rh_objectHitByRay, length, lm_objectCanToFadeOut))
        {
            return (success: true, hitGameobject: rh_objectHitByRay.collider.gameObject);
        }
        return (success: false, hitGameobject: gameObject);
    }
}

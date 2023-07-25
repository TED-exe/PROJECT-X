using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerFadingBlockingObject : MonoBehaviour
{
    [Header("FADING SETTING")]
    [SerializeField] private LayerMask lm_objectToFading;
    [SerializeField] private Transform tr_target;
    [SerializeField] private Camera cam_cameraToCastRay;
    [Range(0f, 1f)]
    [SerializeField] private float f_fadedAlpha;
    [Range(0f, 5f)]
    [SerializeField] private float f_fadeSpeed;
    [SerializeField] private bool b_retainShadows;
    [SerializeField] private Vector3 v3_targetPositionOffset = Vector3.up;
    [Header("ONLYREAD DATA")]
    [SerializeField] private List<ObjectFadeOut> objectBlockingTarget = new List<ObjectFadeOut>();
    private Dictionary<ObjectFadeOut, Coroutine> runningCoroutine = new Dictionary<ObjectFadeOut, Coroutine>();
    private RaycastHit[] rh_hit = new RaycastHit[10];


    private void Awake()
    {
        cam_cameraToCastRay = Camera.main;
    }
    private void Start()
    {
        StartCoroutine(CheckForObject());
    }

    IEnumerator CheckForObject()
    {
        while (true)
        {
            int hits = Physics.RaycastNonAlloc(cam_cameraToCastRay.transform.position,
                (tr_target.position + v3_targetPositionOffset - cam_cameraToCastRay.transform.position).normalized,
                rh_hit,
                Vector3.Distance(cam_cameraToCastRay.transform.position, tr_target.position + v3_targetPositionOffset - cam_cameraToCastRay.transform.position),
                lm_objectToFading);

            if(hits > 0 )
            {
                for (int i = 0; i < hits; i++)
                {
                    ObjectFadeOut fadingObject = GetFadingObjectFromHit(rh_hit[i]);
                    if( fadingObject != null && !objectBlockingTarget.Contains(fadingObject))
                    {
                        if(runningCoroutine.ContainsKey(fadingObject))
                        {
                            if (runningCoroutine[fadingObject] != null)
                            {
                                StopCoroutine(runningCoroutine[fadingObject]);
                            }

                            runningCoroutine.Remove(fadingObject);
                        }
                        runningCoroutine.Add(fadingObject, StartCoroutine(FadeOutObject(fadingObject)));
                        objectBlockingTarget.Add(fadingObject);
                    }
                }
            }
            FadeObjectNoLongerBeigHit();
            ClearHit();
            yield return null;
        }
    }

    private IEnumerator FadeOutObject(ObjectFadeOut fadingObject)
    {
        foreach(Material mat in fadingObject.m_materials)
        {
            mat.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
            mat.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
            mat.SetInt("_ZWrite", 0);
            mat.SetInt("_Surface", 1);

            mat.renderQueue = (int)UnityEngine.Rendering.RenderQueue.Transparent;

            mat.SetShaderPassEnabled("DepthOnly", false);
            mat.SetShaderPassEnabled("SHADOWCASTER", b_retainShadows);

            mat.SetOverrideTag("RenderType", "Transparent");

            mat.EnableKeyword("_SURFACE_TYPE_TRANSPARENT");
            mat.EnableKeyword("_ALPHAPREMULTIPLY_ON");
        }

        float time = 0;

        while (fadingObject.m_materials[0].color.a > f_fadedAlpha)
        {
            foreach(Material mat in fadingObject.m_materials)
            {
                if(mat.HasProperty("_BaseColor"))
                {
                    mat.color = new Color(mat.color.r, mat.color.g, mat.color.b, Mathf.Lerp(fadingObject.f_initialObjectAlfa, f_fadedAlpha,time * f_fadeSpeed));
                }
            }

            time += Time.deltaTime;
            yield return null;
        }

        if(runningCoroutine.ContainsKey(fadingObject))
        {
            StopCoroutine(runningCoroutine[fadingObject]);
            runningCoroutine.Remove(fadingObject);
        }
    }

    private IEnumerator FadeInObject(ObjectFadeOut fadingObject)
    {
        float time = 0;

        while (fadingObject.m_materials[0].color.a < fadingObject.f_initialObjectAlfa)
        {
            foreach (Material mat in fadingObject.m_materials)
            {
                if (mat.HasProperty("_BaseColor"))
                {
                    mat.color = new Color(mat.color.r, mat.color.g, mat.color.b, Mathf.Lerp(f_fadedAlpha, fadingObject.f_initialObjectAlfa, time * f_fadeSpeed));
                }
            }

            time += Time.deltaTime;
            yield return null;
        }

        foreach (Material mat in fadingObject.m_materials)
        {
            mat.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
            mat.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.Zero);
            mat.SetInt("_ZWrite", 1);
            mat.SetInt("_Surface", 0);

            mat.renderQueue = (int)UnityEngine.Rendering.RenderQueue.Geometry;

            mat.SetShaderPassEnabled("DepthOnly", true);
            mat.SetShaderPassEnabled("SHADOWCASTER", true);

            mat.SetOverrideTag("RenderType", "Opaque");

            mat.DisableKeyword("_SURFACE_TYPE_TRANSPARENT");
            mat.DisableKeyword("_ALPHAPREMULTIPLY_ON");
        }

        if(runningCoroutine.ContainsKey(fadingObject))
        {
            StopCoroutine(runningCoroutine[fadingObject]);
            runningCoroutine.Remove(fadingObject);
        }
    }

    private void FadeObjectNoLongerBeigHit()
    {
        List<ObjectFadeOut> objectToRemove = new List<ObjectFadeOut>();
        foreach(ObjectFadeOut obj in objectBlockingTarget)
        {
            bool objectIsBeigHit = false;
            for (int i = 0; i < rh_hit.Length; i++)
            {
                ObjectFadeOut hitFadingObject = GetFadingObjectFromHit(rh_hit[i]);
                if(hitFadingObject != null &&   obj == hitFadingObject)
                {
                    objectIsBeigHit=true;
                    break;
                }
            }

            if(!objectIsBeigHit)
            {
                if (runningCoroutine.ContainsKey(obj))
                {
                    if (runningCoroutine[obj] != null)
                    {
                        StopCoroutine(runningCoroutine[obj]);
                    }
                    runningCoroutine.Remove(obj);
                }
                runningCoroutine.Add(obj, StartCoroutine(FadeInObject(obj)));
                objectToRemove.Add(obj);
            }
        }

        foreach(ObjectFadeOut removeObject in objectToRemove)
        {
            objectBlockingTarget.Remove(removeObject);
        }
    }

    private void ClearHit()
    {
        System.Array.Clear(rh_hit, 0, rh_hit.Length);
    }

    private ObjectFadeOut GetFadingObjectFromHit(RaycastHit hit)
    {
        return hit.collider != null ? hit.collider.GetComponent<ObjectFadeOut>() : null;
    }

    /*private void OnDrawGizmos()
    {
        Vector3 direction = (tr_target.position + v3_targetPositionOffset - cam_cameraToCastRay.transform.position).normalized * Vector3.Distance(cam_cameraToCastRay.transform.position, tr_target.position + v3_targetPositionOffset - cam_cameraToCastRay.transform.position);
        Gizmos.color = Color.yellow;
        Gizmos.DrawRay(cam_cameraToCastRay.transform.position , direction);
    }*/
}

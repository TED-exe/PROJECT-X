using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(PlayerFadingBlockingObject))]
public class FadingObjectEditor : Editor
{
    private void OnSceneGUI()
    {
        PlayerFadingBlockingObject pFob = (PlayerFadingBlockingObject)target;
        Handles.color = Color.red;
        Handles.DrawLine(pFob.cam_cameraToCastRay.transform.position, pFob.tr_target.position + pFob.v3_targetPositionOffset);
;    }
}

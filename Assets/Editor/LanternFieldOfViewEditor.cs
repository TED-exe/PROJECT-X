using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(PlayerLanternFieldOfView))]
public class LanternFieldOfViewEditor : Editor
{
    private void OnSceneGUI()
    {
        PlayerLanternFieldOfView pFow = (PlayerLanternFieldOfView)target;
        Handles.color = Color.white;
        if(pFow.b_lanternSwitch)
        {
            Handles.DrawWireArc(pFow.transform.position, Vector3.up, Vector3.forward, 360, pFow.f_playerViewRadius);

            Vector3 viewAngleA = pFow.DirFromAngle(-pFow.f_playerViewCurrentAngle / 2, false);
            Vector3 viewAngleB = pFow.DirFromAngle(pFow.f_playerViewCurrentAngle / 2, false);

            Handles.DrawLine(pFow.transform.position, pFow.transform.position + viewAngleA * pFow.f_playerViewRadius);
            Handles.DrawLine(pFow.transform.position, pFow.transform.position + viewAngleB * pFow.f_playerViewRadius);

            Handles.color = Color.red;
            foreach (var visibleTarget in pFow.tr_visibleTargetList)
            {
                Handles.DrawLine(pFow.transform.position, visibleTarget.position);
            }
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserTriggerButton : MonoBehaviour
{
    [SerializeField] private GameObject go_doorToOpen;
    [SerializeField] private GameEvent ge_onLaserHitCorrect;
    public bool b_isHited;


    private void Update()
    {
        if (b_isHited)
            ge_onLaserHitCorrect.Raise(this, go_doorToOpen);
            b_isHited = false;
    }
}

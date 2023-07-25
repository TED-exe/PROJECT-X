using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserTarget : MonoBehaviour
{
    public bool b_isHit;
    [SerializeField] private GameEvent OnLaserHit;
    [SerializeField] private GameObject go_doorToOpen;

    private void Update()
    {
        if(b_isHit)
        {
            OnLaserHit.Raise(this, go_doorToOpen);
            b_isHit = false;
        }
    }
}

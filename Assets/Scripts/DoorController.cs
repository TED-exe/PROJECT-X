using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorController : MonoBehaviour
{
    
    private Animator doorAnimator;
    private const string s_openTrigger = "Open";

    public void OpenDoor(Component sander, object data)
    {
        if(sander is LaserTarget && data is GameObject)
        {
            var holder = (GameObject)data;
            doorAnimator = holder.GetComponent<Animator>();            doorAnimator.SetTrigger(s_openTrigger);
        }
    }
}

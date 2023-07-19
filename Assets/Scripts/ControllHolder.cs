using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControllHolder : MonoBehaviour
{
    [Header("MOVE BINDS")]
    public KeyCode kc_runKey;
    public KeyCode kc_crouchKey;
    public KeyCode kc_allowToRotatePlayerKey;
    public KeyCode kc_allowToPullKey;


    [HideInInspector] public float f_verticalInput;
    [HideInInspector] public float f_horizontalInput;

    private void Update()
    {
        f_horizontalInput = Input.GetAxis("Horizontal");
        f_verticalInput = Input.GetAxis("Vertical");
    }
}

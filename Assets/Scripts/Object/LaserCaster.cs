using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserCaster : MonoBehaviour
{
    [SerializeField] private bool b_laserSwitch;
    [SerializeField] private Transform tr_laserCaster;
    [SerializeField] private Material m_laserMaterial;
    [SerializeField] private Color c_laserColor;
    [SerializeField] private float f_laserStartWidth;
    [SerializeField] private float f_laserEndWidth;
    [SerializeField] private LayerMask lm_objectStopLaser;

    LaserBeam laserBeam;
    private void Update()
    {
        if(b_laserSwitch)
        {
            if (laserBeam != null)
                Destroy(laserBeam.ga_laserObj);
            laserBeam = new LaserBeam(tr_laserCaster.position, tr_laserCaster.forward, m_laserMaterial, c_laserColor, f_laserStartWidth, f_laserEndWidth, tr_laserCaster, lm_objectStopLaser);
        }
    }

}

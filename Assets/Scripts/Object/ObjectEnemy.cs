using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering.LookDev;
using UnityEngine;

public class ObjectEnemy : MonoBehaviour
{
    [SerializeField] private float f_enemyMaxHP;
    [SerializeField] private bool b_inLight;
    private float f_enemyCurrHP;
    private bool b_loseHP;

    private void Awake()
    {
        f_enemyCurrHP = f_enemyMaxHP;   
    }

    public void EnemyStayInLight(float dmg)
    {
        DealDMG(dmg);
        b_inLight = true;
    }
    public void EnemyStopStayInLight()
    {
        b_inLight = false;
    }

    private void DealDMG(float dmg)
    {
        Debug.Log(dmg + " to " + this.gameObject.name);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHealthSystem : MonoBehaviour
{
    [Header("ENEMY HEALTH SETTING")]
    [SerializeField] private float f_maxHealth;
    [SerializeField] private float f_healthRegenerateSpeed;
    [Header("ONLY READ DATA")]
    [SerializeField] private float f_currHealth;
    [SerializeField] private so_boolValue b_stayInLight;
    //[SerializeField] private GameEvent so_enemyStayInLight;
    //[SerializeField] private GameEvent so_enemyStopStayInLight;
   public void CheckDeath()
    {
        if (f_currHealth <= 0)
            Destroy(gameObject);
    }
    public void BaseSetUp()
    {
        f_currHealth = f_maxHealth;
    }
    public void LoseHealth(float dmg)
    {
        f_currHealth -= dmg * Time.deltaTime;
    }
    public void RegenerateHealth()
    {
        if (!b_stayInLight.value)
        {
            if (f_currHealth < f_maxHealth)
                f_currHealth += f_healthRegenerateSpeed * Time.deltaTime;
            else if(f_currHealth >= f_maxHealth)
                f_currHealth = f_maxHealth;
        }
    }

}

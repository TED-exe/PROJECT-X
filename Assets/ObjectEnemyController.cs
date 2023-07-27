using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectEnemyController : MonoBehaviour
{
    public void EnemyLoseHp(Component sender, object data)
    {
        if(sender is PlayerLanternFieldOfView && data is Transform)
        {
            
        }
    }
}

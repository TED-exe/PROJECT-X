using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnumsHolder : MonoBehaviour
{
    public enum PlayerMovementState
    {
        walking,
        running,
        air,
        crouching,
        pullingObject
    }
    public enum EnemyMovementState
    {
        patroling,
        chasing,
        chasingInLight,
    }
}

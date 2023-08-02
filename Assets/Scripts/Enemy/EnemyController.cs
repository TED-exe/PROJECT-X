using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyController : MonoBehaviour
{
    [Header("FLOAT")]
    [SerializeField] private so_floatValue f_patrollSpeed;
    [SerializeField] private so_floatValue f_chasingSpeed;
    [SerializeField] private so_floatValue f_inLightSpeed;
    [Header("BOOL")]
    [SerializeField] private bool b_patrollingByWayPoints;
    [SerializeField] private bool b_stayInLight;
    [SerializeField] private bool b_isPatrolling;
    [SerializeField] private bool b_isChasing;
    [SerializeField] private bool b_playerInRange;

    private EnemyHealthSystem healthSystem;
    private EnemyWalkSystem walkSystem;
    private EnemyFieldOfView fieldOfView;

    private EnumsHolder.EnemyMovementState state;
    private NavMeshAgent navMeshAgent;

    private void Awake()
    {
        fieldOfView = GetComponent<EnemyFieldOfView>();
        healthSystem = GetComponent<EnemyHealthSystem>();
        walkSystem = GetComponent<EnemyWalkSystem>();
        navMeshAgent = GetComponent<NavMeshAgent>();    

        BaseSetUp();
    }
    private void Update()
    {
        StateHandler();
        healthSystem.CheckDeath();
        healthSystem.RegenerateHealth();
        fieldOfView.DrawEnemyFieldOfView();

        b_playerInRange = fieldOfView.FindPlayerInEnemyFieldOfView();
        if (state == EnumsHolder.EnemyMovementState.patroling && !b_patrollingByWayPoints)
            walkSystem.RandomPatrolling(navMeshAgent);
        else if (state == EnumsHolder.EnemyMovementState.patroling && b_patrollingByWayPoints)
            walkSystem.PatrollingWithWayPoints();
    }
    private void BaseSetUp()
    {
        healthSystem.BaseSetUp();
        walkSystem.BaseSetUp(navMeshAgent, b_patrollingByWayPoints);
        fieldOfView.BaseSetUp();

    }
    public void EnemyStayInLigt(float dmg)
    {
        b_stayInLight = true;
        healthSystem.LoseHealth(dmg);
    }
    public void EnemyStopStayInLight()
    {
        b_stayInLight = false;
        healthSystem.RegenerateHealth();
    }
    private void StateHandler()
    {
        //patrolling State
        if (b_isPatrolling && !b_isChasing && !b_stayInLight)
        {
            navMeshAgent.speed = f_patrollSpeed.value;
            state = EnumsHolder.EnemyMovementState.patroling;
            return;
        }
        //chasing State
        else if (!b_isPatrolling && b_isChasing && !b_stayInLight)
        {
            navMeshAgent.speed = f_chasingSpeed.value;
            state = EnumsHolder.EnemyMovementState.chasing;
            return;
        }
        //chasing inLight
        else if (!b_isPatrolling && b_isChasing && b_stayInLight)
        {
            navMeshAgent.speed = f_inLightSpeed.value;
            state = EnumsHolder.EnemyMovementState.chasingInLight;
            return;
        }
    }
}

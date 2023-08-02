using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.AI;

public class EnemyWalkSystem : MonoBehaviour
{
    [Header("ENEMY WALK SETTING")]
    [SerializeField] private LayerMask lm_whatIsGround;
    [SerializeField] private Vector3 v3_patrollingPoint;
    [SerializeField] private float f_acceleration;
    [SerializeField] private float f_angularSpeed;
    [SerializeField] private float f_patrollingPointRange;
    [SerializeField] private GameObject go_wayPointsHolder;
    [SerializeField] private Vector3[] v3_waypoints;
    [SerializeField] private so_boolValue b_isChasing;
    [SerializeField] private so_boolValue b_isPatrolling;
    private bool b_patrollPointSet;
    private Vector3 v3_currTarget;

    private bool CheckArrayIsNotEmpty()
    {
        int corr = 0;
        foreach (Vector3 t in v3_waypoints)
        {
            if (t != null)
            {
                corr++;
            }
        }
        if (corr == v3_waypoints.Length) { return true; }
        else { return false; }
    }
    public void BaseSetUp(NavMeshAgent navMeshAgent, bool isPatrollingByWaypoint)
    {

        if (isPatrollingByWaypoint)
        {
            v3_waypoints = new Vector3[go_wayPointsHolder.transform.childCount];
            for (int i = 0; i < v3_waypoints.Length; i++)
            {
                v3_waypoints[i] = go_wayPointsHolder.transform.GetChild(i).transform.position;
            }
            if (v3_waypoints.Length > 1 && CheckArrayIsNotEmpty())
            {
                v3_waypoints[0] = transform.position;
                v3_currTarget = v3_waypoints[1];
                return;
            }
            else if (v3_waypoints.Length <= 1)
            {
                Debug.LogError("you need 2 or more waypoint");
                return;
            }
            else if (!CheckArrayIsNotEmpty())
            {
                Debug.LogError("object in array is NULL");
            }
        }
        navMeshAgent = GetComponent<NavMeshAgent>();
        navMeshAgent.acceleration = f_acceleration;
        navMeshAgent.angularSpeed = f_angularSpeed;
    }
    public void RandomPatrolling(NavMeshAgent navMeshAgent)
    {
        if (!b_patrollPointSet)
            SearchWalkPoint();
        if (b_patrollPointSet)
            navMeshAgent.SetDestination(v3_patrollingPoint);

        Vector3 distanceToPoint = transform.position - v3_patrollingPoint;
        if (distanceToPoint.magnitude < 1f)
            b_patrollPointSet = false;
    }
    public void PatrollingWithWayPoints()
    {

    }

    private void SearchWalkPoint()
    {
        float randomZ = Random.Range(-f_patrollingPointRange, f_patrollingPointRange);
        float randomX = Random.Range(-f_patrollingPointRange, f_patrollingPointRange);

        v3_patrollingPoint = new Vector3(transform.position.x + randomX, transform.position.y, transform.position.z + randomZ);
        if (Physics.Raycast(transform.position, Vector3.down, 2f, lm_whatIsGround))
        {
            b_patrollPointSet = true;
        }
    }

    public void Chasing()
    {

    }

}

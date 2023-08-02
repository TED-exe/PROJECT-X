using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyFieldOfView : MonoBehaviour
{
    [Header("ENEMY FIEL OF VIEW SETTING")]
    [Range(1f, 50f)]
    [SerializeField] private float f_FloatToAddInMaxRadius;
    [Range(1f, 100f)]
    [SerializeField] private float f_ViewRadius;
    [Range(1f, 360f)]
    [SerializeField] private float f_ViewAngle;
    [Range(0.1f,1f)]
    [SerializeField] private float f_DeleyToRefresh;
    [Range(0.1f, 1f)]
    [SerializeField] private float f_meshResolution;
    [SerializeField] private float f_edgeDstThreshold;

    [SerializeField] private int i_edgeResolveIteration;

    [SerializeField] private LayerMask lm_targetMask;
    [SerializeField] private LayerMask lm_obstacleMask;

    [SerializeField] private MeshFilter viewMeshFilter;

    private float f_MaxViewRadius;
    private bool isPlayerInFieldOfView;

    
    private Mesh viewMesh;

    public void BaseSetUp()
    {
        viewMesh = new Mesh();
        viewMesh.name = "View Mesh";
        viewMeshFilter.mesh = viewMesh;
        f_MaxViewRadius = f_ViewRadius + f_FloatToAddInMaxRadius;
    }
    IEnumerator FindPlayerWithDeley(float deley)
    {
        while (true)
        {
            yield return new WaitForSeconds(deley);
            FindPlayerInEnemyFieldOfView();
        }
    }
    public void StartCourutine()
    {
        StartCoroutine(FindPlayerWithDeley(f_DeleyToRefresh));
    }
    public void DrawEnemyFieldOfView()
    {
        int stepCount = Mathf.RoundToInt(f_ViewAngle * f_meshResolution);
        float stepAngleSize = f_ViewAngle / stepCount;
        List<Vector3> viewPoints = new List<Vector3>();
        ViewCastInfo oldViewCast = new ViewCastInfo();
        for (int i = 0; i <= stepCount; i++)
        {
            float angle = transform.eulerAngles.y - f_ViewAngle / 2 + stepAngleSize * i;
            ViewCastInfo newViewCast = ViewCast(angle);

            if (i > 0)
            {
                bool edgeDstTresholdExceeded = Mathf.Abs(oldViewCast.dst - newViewCast.dst) > f_edgeDstThreshold;
                if (oldViewCast.hit != newViewCast.hit || (oldViewCast.hit && newViewCast.hit && edgeDstTresholdExceeded))
                {
                    EdgeInfo edge = FindEdge(oldViewCast, newViewCast);
                    if (edge.pointA != Vector3.zero)
                    {
                        viewPoints.Add(edge.pointA);
                    }
                    if (edge.pointB != Vector3.zero)
                    {
                        viewPoints.Add(edge.pointB);
                    }
                }
            }

            viewPoints.Add(newViewCast.point);
            oldViewCast = newViewCast;
        }
        int vertexCount = viewPoints.Count + 1;
        Vector3[] vertices = new Vector3[vertexCount];
        int[] triangles = new int[(vertexCount - 2) * 3];

        vertices[0] = Vector3.zero;
        for (int i = 0; i < vertexCount - 1; i++)
        {
            vertices[i + 1] = transform.InverseTransformPoint(viewPoints[i]);

            if (i < vertexCount - 2)
            {
                triangles[i * 3] = 0;
                triangles[i * 3 + 1] = i + 1;
                triangles[i * 3 + 2] = i + 2;
            }
        }

        viewMesh.Clear();

        viewMesh.vertices = vertices;
        viewMesh.triangles = triangles;
        viewMesh.RecalculateNormals();
        viewMesh.RecalculateNormals();

    }
    private EdgeInfo FindEdge(ViewCastInfo minViewCast, ViewCastInfo maxViewCast)
    {
        float minAngle = minViewCast.angle;
        float maxAngle = maxViewCast.angle;
        Vector3 minPoint = Vector3.zero;
        Vector3 maxPoint = Vector3.zero;

        for (int i = 0; i < i_edgeResolveIteration; i++)
        {
            float angle = (minAngle + maxAngle) / 2;
            ViewCastInfo newViewCast = ViewCast(angle);

            bool edgeDstThresholdExceeded = Mathf.Abs(minViewCast.dst - newViewCast.dst) > f_edgeDstThreshold;
            if (newViewCast.hit == minViewCast.hit && !edgeDstThresholdExceeded)
            {
                minAngle = angle;
                minPoint = newViewCast.point;
            }
            else
            {
                maxAngle = angle;
                maxPoint = newViewCast.point;
            }
        }

        return new EdgeInfo(minPoint, maxPoint);
    }
    private ViewCastInfo ViewCast(float globalAngle)
    {
        Vector3 dir = DirFromAngle(globalAngle, true);
        RaycastHit hit;

        if (Physics.Raycast(transform.position, dir, out hit, f_MaxViewRadius, lm_obstacleMask))
        {
            return new ViewCastInfo(true, hit.point, hit.distance, globalAngle);
        }
        else
            return new ViewCastInfo(false, transform.position + dir * f_MaxViewRadius, f_MaxViewRadius, globalAngle);
    }
    private struct ViewCastInfo
    {
        public bool hit;
        public Vector3 point;
        public float dst;
        public float angle;

        public ViewCastInfo(bool _hit, Vector3 _point, float _dst, float _angle)
        {
            hit = _hit;
            point = _point;
            dst = _dst;
            angle = _angle;
        }
    }
    private struct EdgeInfo
    {
        public Vector3 pointA;
        public Vector3 pointB;

        public EdgeInfo(Vector3 _pointA, Vector3 _pointB)
        {
            pointA = _pointA;
            pointB = _pointB;
        }
    }
    private Vector3 DirFromAngle(float angleInDegrees, bool angleIsGlobal)
    {
        if (!angleIsGlobal)
            angleInDegrees += transform.eulerAngles.y;
        return new Vector3(Mathf.Sin(angleInDegrees * Mathf.Deg2Rad), 0, Mathf.Cos(angleInDegrees * Mathf.Deg2Rad));
    }
    public bool FindPlayerInEnemyFieldOfView()
    {
        
        Collider[] targetPlayer = Physics.OverlapSphere(transform.position, f_ViewRadius, lm_targetMask);

        if (targetPlayer.Length > 0)
        {
            Transform target = targetPlayer[0].transform;
            Vector3 dirToTarget = (target.position - transform.position).normalized;
            if (Vector3.Angle(transform.forward, dirToTarget) < f_ViewAngle / 2)
            {
                float dstToTarget = Vector3.Distance(transform.position, target.position);
                if (!Physics.Raycast(transform.position, dirToTarget, dstToTarget, lm_obstacleMask))
                    isPlayerInFieldOfView = true;
                else
                    isPlayerInFieldOfView = false;
            }
            else
                isPlayerInFieldOfView = false;
        }
        else
            isPlayerInFieldOfView = false;

        return isPlayerInFieldOfView;
    }
}

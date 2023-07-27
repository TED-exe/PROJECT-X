using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLanternFieldOfView : MonoBehaviour
{
    [Header("FIELD OF VIEW SETTINGS")]
    public float f_playerViewRadius;
    [Range(0f, 360f)]
    [SerializeField] private float f_playerViewMaxAngle;
    [Range(0f, 360f)]
    [SerializeField] private float f_playerViewMinAngle;
    [SerializeField] private float f_changeFocusSpeed;
    public Transform tr_LanternLightCaster;


    [SerializeField] private LayerMask lm_targetMask;
    [SerializeField] private LayerMask lm_obstacleMask;
    [Range(0f, 1f)]
    [SerializeField] private float f_deleyToRefreshTarget;
    [Range(0f, 1f)]
    [SerializeField] private float f_meshResolution;
    [SerializeField] private float f_edgeDstThreshold;
    [SerializeField] private int i_edgeResolveIteration;

    [SerializeField] private GameEvent onEnemyInLanternFieldOfView;
    [SerializeField] private GameEvent onEnemyLeaveLanternFieldOfView;

    public MeshFilter viewMeshFilter;
    private Mesh viewMesh;
    private ControllHolder controllHolder;

    [Header("ONLYREAD DATA")]
    [HideInInspector] public float f_playerViewCurrentAngle;
    [HideInInspector] public bool b_lanternSwitch;
    public List<Transform> tr_visibleTargetList = new List<Transform>();
    public void BaseSetUp()
    {
        b_lanternSwitch = true;
        viewMesh = new Mesh();
        viewMesh.name = "View Mesh";
        viewMeshFilter.mesh = viewMesh;
        controllHolder = GetComponent<ControllHolder>();
        f_playerViewCurrentAngle = f_playerViewMaxAngle;
    }
    public void StartCouroutine()
    {
        StartCoroutine(FindTargetWithDeley(f_deleyToRefreshTarget));
    }
    IEnumerator FindTargetWithDeley(float deley)
    {
        while (true)
        {
            yield return new WaitForSeconds(deley);
            FindVisibleTargets();
        }
    }
    public void MyInput()
    {
        if (Input.GetKeyDown(controllHolder.kc_switchLanternLightKey))
        {
            b_lanternSwitch = !b_lanternSwitch;
        }

        if (Input.GetKey(controllHolder.kc_increseLightFocus))
        {
            IncreaseLanternFocus();
            return;
        }
        else if (Input.GetKey(controllHolder.kc_decreaseLightFocus))
        {
            DecreaseLanternFocus();
            return;
        }

    }
    public void DrawFieldOfView()
    {
        if (b_lanternSwitch)
        {
            int stepCount = Mathf.RoundToInt(f_playerViewCurrentAngle * f_meshResolution);
            float stepAngleSize = f_playerViewCurrentAngle / stepCount;
            List<Vector3> viewPoints = new List<Vector3>();
            ViewCastInfo oldViewCast = new ViewCastInfo();

            for (int i = 0; i <= stepCount; i++)
            {
                float angle = transform.eulerAngles.y - f_playerViewCurrentAngle / 2 + stepAngleSize * i;
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
        }
        else
        {
            viewMesh.Clear();
        }
    }
    private void IncreaseLanternFocus()
    {
        if (f_playerViewCurrentAngle < f_playerViewMaxAngle)
            f_playerViewCurrentAngle += f_changeFocusSpeed * Time.deltaTime;

        else if (f_playerViewCurrentAngle >= f_playerViewMaxAngle)
            f_playerViewCurrentAngle = f_playerViewMaxAngle;

    }
    private void DecreaseLanternFocus()
    {
        if (f_playerViewCurrentAngle > f_playerViewMinAngle)
            f_playerViewCurrentAngle -= f_changeFocusSpeed * Time.deltaTime;

        else if (f_playerViewCurrentAngle <= f_playerViewMinAngle)
            f_playerViewCurrentAngle = f_playerViewMinAngle;
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

        if (Physics.Raycast(transform.position, dir, out hit, f_playerViewRadius, lm_obstacleMask))
        {
            return new ViewCastInfo(true, hit.point, hit.distance, globalAngle);
        }
        else
            return new ViewCastInfo(false, transform.position + dir * f_playerViewRadius, f_playerViewRadius, globalAngle);
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
    void FindVisibleTargets()
    {
        TargetStopInteractWithLight();
        Collider[] targetsInViewRadius = Physics.OverlapSphere(transform.position, f_playerViewRadius, lm_targetMask);
        if(b_lanternSwitch)
        {
            for (int i = 0; i < targetsInViewRadius.Length; i++)
            {
                Transform target = targetsInViewRadius[i].transform;
                Vector3 dirToTarget = (target.position - transform.position).normalized;
                if (Vector3.Angle(transform.forward, dirToTarget) < f_playerViewCurrentAngle / 2)
                {
                    float dstToTarget = Vector3.Distance(transform.position, target.position);
                    if (!Physics.Raycast(transform.position, dirToTarget, dstToTarget, lm_obstacleMask))
                    {
                        tr_visibleTargetList.Add(target);
                        TargetInteractWithLight();
                    }
                }
            }
        }
    }

    private void TargetInteractWithLight()
    {
        foreach(Transform target in tr_visibleTargetList)
        {
            target.GetComponent<ObjectEnemy>().EnemyStayInLight(10 - f_playerViewCurrentAngle/10);
        }
    }
    private void TargetStopInteractWithLight()
    {
        foreach (Transform target in tr_visibleTargetList)
        {
            target.GetComponent<ObjectEnemy>().EnemyStopStayInLight();
        }
        tr_visibleTargetList.Clear();
    }
    public Vector3 DirFromAngle(float angleInDegrees, bool angleIsGlobal)
    {
        if (!angleIsGlobal)
            angleInDegrees += transform.eulerAngles.y;
        return new Vector3(Mathf.Sin(angleInDegrees * Mathf.Deg2Rad), 0, Mathf.Cos(angleInDegrees * Mathf.Deg2Rad));
    }
}

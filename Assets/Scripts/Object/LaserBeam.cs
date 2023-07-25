using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserBeam
{
    private Vector3 v3_laserPos;
    private Vector3 v3_laserDir;

    public GameObject go_laserObj;
    LineRenderer lineRenderer;
    List<Vector3> v3_laserIndices = new List<Vector3>();
    public LaserBeam(Vector3 v3_laserPos, Vector3 v3_laserDir, Material m_laserMaterial, Color c_laserColor, float f_laserStartWidth, float f_laserEndWidth, Transform tr_laserParent, LayerMask lm_objectStopLaser)
    {
        this.lineRenderer = new LineRenderer();
        this.go_laserObj = new GameObject();
        this.go_laserObj.name = "Laser Beam";
        this.v3_laserPos = v3_laserPos;
        this.v3_laserDir = v3_laserDir;

        this.lineRenderer = this.go_laserObj.AddComponent(typeof(LineRenderer)) as LineRenderer;
        this.lineRenderer.startWidth = f_laserStartWidth;
        this.lineRenderer.endWidth = f_laserEndWidth;
        this.lineRenderer.material = m_laserMaterial;
        this.lineRenderer.startColor = c_laserColor;
        this.lineRenderer.endColor = c_laserColor;
        this.go_laserObj.transform.SetParent(tr_laserParent);

        // cast ray from point
        CastRay(v3_laserPos, v3_laserDir, lineRenderer, lm_objectStopLaser);
    }

    private void CastRay(Vector3 pos,Vector3 dir, LineRenderer lineRenderer, LayerMask lm_objectStopLaser)
    {

        v3_laserIndices.Add(pos);
        
        Ray ray = new Ray(pos,dir);
        RaycastHit hit;

        if(Physics.Raycast(ray, out hit ,30, lm_objectStopLaser))
        {
            CheckHit(hit, dir, lineRenderer, lm_objectStopLaser);
        }
        else
        {
            v3_laserIndices.Add(ray.GetPoint(30));
            UpdateLaser();
        }
    }
    private void CheckHit(RaycastHit hitInfo, Vector3 direction, LineRenderer lineRenderer, LayerMask lm_objectStopLaser)
    {
        if (hitInfo.collider.gameObject.tag == "mirror")
        {
            Vector3 pos = hitInfo.point;
            Vector3 dir = Vector3.Reflect(direction, hitInfo.normal);

            CastRay(pos, dir, lineRenderer, lm_objectStopLaser);
        }
        else if (hitInfo.collider.gameObject.tag == "target")
        {
            if (hitInfo.collider.TryGetComponent<LaserTarget>(out LaserTarget laserTriggerButton))
            {
                laserTriggerButton.b_isHit = true;
            }

            v3_laserIndices.Add(hitInfo.point);
            UpdateLaser();

        }
        else
        {
            v3_laserIndices.Add(hitInfo.point);
            UpdateLaser();
        }
    }
    private void UpdateLaser()
    {
        int count = 0;
        lineRenderer.positionCount = v3_laserIndices.Count;

        foreach(Vector3 idx in v3_laserIndices)
        {
            lineRenderer.SetPosition(count, idx);
            count++;
        }
    }


}

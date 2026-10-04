using UnityEngine;
public class CPUMoveTargetSearch
{
    private float _changepointdistance = 10;
    private Transform _cpuTransform;

    private GameObject target;
    public CPUMoveTargetSearch(Transform transform,GameObject fastTarget)
    {
        _cpuTransform = transform;
        target = fastTarget;
    }
    public GameObject ChangeTarget()
    {
        if (Vector3.Distance(target.transform.position, _cpuTransform.position) < _changepointdistance)
        {
            target.TryGetComponent<WayPointsSetting>(out WayPointsSetting wayPoints);
            {
                target = wayPoints.OnGetWayPoint();
            }

            Debug.Log(target.name);
        }
        return target;
    }
    public float GetHorizontal()
    {
        float horizontal = 0f;

        float distance = Vector3.Distance(target.transform.position, _cpuTransform.position);

        horizontal = Vector3.Angle(target.transform.position - _cpuTransform.position, _cpuTransform.forward) / 90f;

        //if (distance < 20)
        //{
        //    horizontal=CurveSearch(distance);
        //}
        //else
        //{
        //    Vector3 targetDirection = target.transform.position - _cpuTransform.position;
        //    horizontal = Vector3.Dot(_cpuTransform.right, targetDirection.normalized);
        //}
        return horizontal;
    }
    public float GetAngle()
    {
        Vector3 targetDirection = target.transform.position - _cpuTransform.position;
        float angle = Vector3.Angle(targetDirection, _cpuTransform.forward);
        return angle;
    }
    public Vector3 MoveTarget()
    {
        return target.transform.position - _cpuTransform.position;
    }
    public float CurveSearch(float distance)
    {
        GameObject nextTaeget;
        target.TryGetComponent<WayPointsSetting>(out WayPointsSetting wayPointSettings);
        {
            nextTaeget = wayPointSettings.OnGetWayPoint();
        }
        Vector3 targetDirection = target.transform.position - _cpuTransform.position;
        Vector3 nextTargetDirection = nextTaeget.transform.position - target.transform.position;

        targetDirection = Vector3.Normalize(targetDirection);
        nextTargetDirection = Vector3.Normalize(nextTargetDirection);

        float angle = Vector3.Angle(nextTargetDirection, targetDirection);


        Vector3 crossProduct = Vector3.Cross(targetDirection, nextTargetDirection);

        float sign = (crossProduct.y >= 0) ? 1f : -1f;

        float value = Mathf.Clamp((angle / 90f) * sign, -1f, 1f);
        return value;
    }
}

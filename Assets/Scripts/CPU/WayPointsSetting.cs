using UnityEngine;

public class WayPointsSetting : MonoBehaviour
{
    [SerializeField] private GameObject _myWayPoint;

    public GameObject OnGetWayPoint()
    {
        return _myWayPoint;
    }
}
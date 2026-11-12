using UnityEngine;

public class WayPointsSetting : MonoBehaviour
{
    [SerializeField] private GameObject _nextPoint;

    public GameObject GetNextWaypoint()
    {
        return _nextPoint;
    }
}
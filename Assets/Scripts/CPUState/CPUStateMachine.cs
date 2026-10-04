using UnityEngine;

public class CPUStateMachine : MonoBehaviour
{
    [SerializeField] private Rigidbody _myRigibody = default;
    private Vector3 targetpoint = default;
    [SerializeField] private GameObject _currentTarget = default;


    private CarMove _carMove = default;
    private CPUMoveTargetSearch _searchTarget = default;

    [SerializeField] private float _maxSpeed = 0.0f;
    [SerializeField] private float _maxCurveSpeed = 0.0f;
    [SerializeField] private float _maxDashSpeed = 0.0f;
    [SerializeField] private float _moveForce = 0.0f;
    private CarDriver _driver;
    private void Awake()
    {
        _carMove = new CarMove(_myRigibody, _maxSpeed);
        //_driver = new CarDriver(_carMove, transform, _maxSpeed, _moveForce);
        _searchTarget = new CPUMoveTargetSearch(transform, _currentTarget);
    }
    private void FixedUpdate()
    {

    }
}

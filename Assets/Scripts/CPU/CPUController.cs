using UnityEngine;

public class CPUController : MonoBehaviour
{
    [SerializeField] private Rigidbody _myRigibody = default;
    private Vector3 targetpoint = default;
    [SerializeField]private GameObject _currentTarget = default;

    [SerializeField] private LayerMask _wallLayer = default;

    //
    private CarMove _carMove = default;
    private CPUMoveTargetSearch _searchTarget = default;

    [SerializeField] private float _maxSpeed = 0.0f;
    [SerializeField] private float _maxCurveSpeed = 0.0f;
    [SerializeField] private float _maxDashSpeed = 0.0f;
    private float _moveForce = 0.0f;

    //private enum _runstate
    //{
    //    _nomal,
    //    _dash,
    //    _curve
    //}
    //_runstate _currentrun = _runstate._nomal;

    private float _horizontal;
    private float _vertical;
    private CarDriver _driver;
    private void Awake()
    {
        _carMove = new CarMove(_myRigibody, _maxSpeed);
        //_driver = new CarDriver(_carMove, transform, _maxSpeed, _moveForce);
        _searchTarget = new CPUMoveTargetSearch(transform, _currentTarget);
    }
    //private void DirectionCarMove()
    //{
    //    targetpoint.y = 0;
    //    float angle = Vector3.Angle(targetpoint, transform.forward);
    //    if (angle != 0)
    //    {
    //        Quaternion looktarget = Quaternion.LookRotation(targetpoint);
    //        transform.rotation = Quaternion.Lerp(
    //        transform.rotation,
    //        looktarget,
    //        10 * Time.deltaTime);
    //    }
    //}
    private void FixedUpdate()
    {
        _currentTarget = _searchTarget.ChangeTarget();
        _horizontal = _searchTarget.GetHorizontal();
        //DirectionCarMove();
        //_driver.CarUpdate(_horizontal, 1, 1);
        //switch (_currentrun)
        //{
        //    case _runstate._nomal:
        //        targetpoint = _searchTarget.MoveTarget();
        //        _driver.SpeedLimit(_myRigibody.linearVelocity, _maxSpeed);

        //        if (Physics.Raycast(transform.position, transform.forward,5, _wallLayer))print("aa");
        //        return;
        //    case _runstate._curve:
        //        _driver.SpeedLimit(_myRigibody.linearVelocity, _maxCurveSpeed);

        //        return;
        //    case _runstate._dash:

        //        return;

        //}
    }
}

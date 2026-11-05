using UnityEngine;
using XqLua;
public class CameraTargetMove : MonoBehaviour
{
    [SerializeField] private Rigidbody _playerRigidBody = default;
    [SerializeField] private CarController _carController = default;
    [SerializeField] private float _driftCameraSideMoveSpeed = 10.0f;
    [SerializeField] private float _cameraSideLength = 5.0f;


    private  Vector3 _initialPosition;
    private IDisposableSubscription _onstateChange = default;
    private void OnEnable()
    {
        _onstateChange = _carController._onStateChanged.Subscribe(StateChange);
        _initialPosition = transform.position;
    }
    private void OnDisable()
    {
        _onstateChange.Dispose();
    }
    private void StateChange(CarState oldState, CarState newState)
    {
        Exit(oldState);
        Enter(newState);
    }
    private void Enter(CarState state)
    {
        switch (state)
        {
            case CarState.Idle:
                CameraPositionReset();
                break;
            case CarState.Move:
                CameraPositionReset();
                break;
            case CarState.Drift:
                break;
        }
    }
    private void Exit(CarState state)
    {
        switch (state)
        {
            case CarState.Idle:
                break;
            case CarState.Move:
                break;
            case CarState.Drift:
                break;
        }
    }
    private void LateUpdate()
    {
        switch (_carController._currentState)
        {
            case CarState.Idle:
                RotateTo(_carController.transform.rotation);
                break;
            case CarState.Move:
                MoveCamera(_carController.InputHorizontal * 0.5f);
                MoveRotation();
                break;
            case CarState.Drift:
                MoveCamera(_carController.InputHorizontal);
                MoveRotation();
                break;
        }
    }
    private void CameraPositionReset()
    {
        transform.position = _initialPosition;
    }
    private void MoveCamera(float driftAmount)
    {
        driftAmount *= _cameraSideLength;

        Vector3 targetPosition = _carController.transform.position + transform.right * driftAmount;

        transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * _driftCameraSideMoveSpeed);
    }
    private void MoveRotation()
    {
        Vector3 velocity = _playerRigidBody.linearVelocity;
        if (velocity.sqrMagnitude < 0.01f)
        {
            return;
        }

        RotateTo(Quaternion.LookRotation(velocity.normalized));
    }
    private void RotateTo(Quaternion target)
    {
        transform.rotation = Quaternion.Slerp(
            transform.rotation, target, _driftCameraSideMoveSpeed * Time.deltaTime);
    }
}

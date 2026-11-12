using UnityEngine;
using XqLua;
public class CameraTargetMove : MonoBehaviour
{
    [SerializeField] private Rigidbody _playerRigidBody = default;
    [SerializeField] private float _moveSpeed = 10.0f;
    [SerializeField] private float _rotateSpeed = 10.0f;
    [SerializeField] private float _cameraSideLength = 5.0f;
    private IPlayerStatus _playerStatus;

    private CarState _currentState;
    private  Vector3 _initialPosition;
    private IDisposableSubscription _onstateChange = default;
    public void Initialize(IPlayerStatus carStatus)
    {
        _playerStatus = carStatus;

        _onstateChange = _playerStatus._onStateChanged.Subscribe(StateChange);
        _initialPosition = transform.position;
    }
    private void OnDisable()
    {
        _onstateChange.Dispose();
    }
    private void StateChange(CarState oldState, CarState newState)
    {
        _currentState = newState;
        Exit(oldState);
        Enter(newState);
    }
    private void Enter(CarState state)
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
        switch (_currentState)
        {
            case CarState.Idle:
                RotateTo(_playerStatus._playerTransform.rotation);
                break;
            case CarState.Move:
                MoveCamera(_playerStatus.InputHorizontal * 0.5f);
                MoveRotation();
                break;
            case CarState.Drift:
                MoveCamera(_playerStatus.InputHorizontal);
                MoveRotation();
                break;
            case CarState.Air:
                MoveCamera(_playerStatus.InputHorizontal);
                MoveRotation();
                break;
        }
    }
    private void Update()
    {
        transform.position = _playerStatus._playerTransform.position;
    }
    private void MoveCamera(float driftAmount)
    {
        driftAmount *= _cameraSideLength;

        Vector3 targetPosition = _playerStatus._playerTransform.position + transform.right * driftAmount;

        transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * _moveSpeed);
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
            transform.rotation, target, _rotateSpeed * Time.deltaTime);
    }
}

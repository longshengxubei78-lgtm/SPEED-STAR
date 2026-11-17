using UnityEngine;
using XqLua;
//CarControllerのなかに入れていい。
public class CameraTargetMove : MonoBehaviour
{
    [SerializeField] private Rigidbody _playerRigidbody = default;
    [SerializeField] private float _moveSpeed = 10.0f;
    [SerializeField] private float _rotateSpeed = 10.0f;
    [SerializeField] private float _cameraSideLength = 5.0f;
    private IPlayerData _playerData;

    private CarState _currentState;
    private IDisposableSubscription _onStateChanged = default;
    public void Initialize(IPlayerData carStatus)
    {
        _playerData = carStatus;
        //iranai
        _onStateChanged = _playerData._onStateChanged.Subscribe(StateChange);
    }
    private void OnDisable()
    {
        _onStateChanged.Dispose();
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
    //CarControllerに移動
    private void LateUpdate()
    {
        switch (_currentState)
        {
            case CarState.Idle:
                transform.rotation = Quaternion.Slerp(
            transform.rotation, _playerData._playerTransform.rotation, _rotateSpeed * Time.deltaTime);
                break;
            case CarState.Move:
                MoveCamera(_playerData._inputInfo.horizontal * 0.5f);
                MoveRotation();
                break;
            case CarState.Drift:
                MoveCamera(_playerData._inputInfo.horizontal);
                MoveRotation();
                break;
            case CarState.Air:
                MoveCamera(_playerData._inputInfo.horizontal);
                MoveRotation();
                break;
        }
    }
    private void Update()
    {
        transform.position = _playerData._playerTransform.position;
    }
    private void MoveCamera(float driftAmount)
    {
        driftAmount *= _cameraSideLength;

        Vector3 targetPosition = _playerData._playerTransform.position + transform.right * driftAmount;

        transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * _moveSpeed);
    }
    private void MoveRotation()
    {
        Vector3 velocity = _playerRigidbody.linearVelocity;
        if (velocity.sqrMagnitude < 0.01f)
        {
            return;
        }

        transform.rotation = Quaternion.Slerp(
            transform.rotation, Quaternion.LookRotation(velocity.normalized), _rotateSpeed * Time.deltaTime);
    }
}

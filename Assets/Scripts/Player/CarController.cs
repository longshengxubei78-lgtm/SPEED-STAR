using UnityEngine;
using UnityEngine.InputSystem;
using XqLua;

public enum CarState {
    Idle, 
    Move, 
    Drift,
    Air 
}
//言いたいことはメモにまとめておく
public class CarController : MonoBehaviour, IPlayerStatus
{
    public CarState _currentState = CarState.Idle;

    [SerializeField] private float _maxSpeed = 0.0f;
    [SerializeField] private float _moveForce = 0.0f;
    [SerializeField] private Rigidbody _myRigidbody = default;
    [SerializeField] private PlayerInput playerInput = default;
    [SerializeField] private LayerMask _roadLayer = default;

    private InputReader _inputReader;

    // readonly を外し、Awake で代入する
    private CarMove _driver;
    private GroundDetector _groundDetector;

    private const float _stopSpeedThreshold = 0.1f;
    public const float _inputThreshold = 0.01f;
    //IPlayerStatusの実装
    public float CurrentSpeed => _myRigidbody.linearVelocity.magnitude;
    public Transform _playerTransform => transform;
    public InputInfo _inputInfo => _inputReader.GetInputInfo();
    private Publisher<CarState, CarState> _onStateChange
        = new Publisher<CarState, CarState>();

    public IPublisher<CarState, CarState> _onStateChanged
        => _onStateChange;
    private void Awake()
    {
        _onStateChange = new Publisher<CarState, CarState>();
        _inputReader = new InputReader(playerInput);
        _groundDetector = new GroundDetector(transform , _roadLayer);
        _driver = new CarMove(_myRigidbody, transform, _moveForce, _maxSpeed);
    }
    //誰かから呼んでもらう、GameManagerなど、
    //GameManagerがRacePhaseを管理していて、CarControllerにApplyPhaseを呼ぶ
    //フックが必要ならステートパターン
    public void FixedTick()
    {
        GroundInfo groundInfo = _groundDetector.GetGroundInfo();
        InputInfo inputInfo = _inputReader.GetInputInfo();

        FixedTick(groundInfo, inputInfo);
    }

    private CarState EvaluateNextState(GroundInfo ground, InputInfo input, float speed)
    {
        if (!ground.isGrounded)
        {
            return CarState.Air;
        }
        bool hasMoveInput = false;
        if (Mathf.Abs(input.vertical) > _inputThreshold)
        {
            hasMoveInput = true;
        }

        bool isMoving;
        if (speed > _stopSpeedThreshold)
        {
            isMoving = true;
        }
        else
        {
            isMoving = false;
        }

        if (!hasMoveInput && !isMoving)
        {
            return CarState.Idle;
        }

        if (input.isDrifted && isMoving)
        {
            return CarState.Drift;
        }

        return CarState.Move;
    }

    private void FixedTick(GroundInfo groundInfo, InputInfo inputInfo)
    {
        float currentSpeed = CurrentSpeed;
        CarState nextState = EvaluateNextState(groundInfo, inputInfo, currentSpeed);
        switch (_currentState)
        {
            case CarState.Idle:
                if (nextState != _currentState)
                {
                    SwitchState(nextState);
                }
                break;

            case CarState.Move:
                _driver.DriveUpdate(inputInfo, groundInfo);
                if (nextState != _currentState)
                {
                    SwitchState(nextState);
                }
                break;

            case CarState.Drift:
                _driver.DriftUpdate(inputInfo, groundInfo);
                if (nextState != _currentState)
                {
                    SwitchState(nextState);
                }
                break;

            case CarState.Air:
                if (nextState != _currentState)
                {
                    SwitchState(nextState);
                }
                break;
        }
    }
    private void SwitchState(CarState newState)
    {
        CarState oldState = _currentState;
        Exit();
        _currentState = newState;
        Enter();
        _onStateChange.Invoke(oldState, newState);
    }
    private void Enter()
    {
        switch (_currentState)
        {
            case CarState.Idle:
                break;
            case CarState.Move:
                break;
            case CarState.Drift:
                break;
            case CarState.Air:
                break;
        }
    }

    private void Exit()
    {
        switch (_currentState)
        {
            case CarState.Idle:
                break;
            case CarState.Move:
                break;
            case CarState.Drift:
                break;
            case CarState.Air:
                break;
        }
    }
}


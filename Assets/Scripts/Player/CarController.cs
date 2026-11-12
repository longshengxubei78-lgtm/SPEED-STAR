using UnityEngine;
using UnityEngine.InputSystem;
using XqLua;

public enum CarState {
    Idle, 
    Move, 
    Drift,
    Air 
}

public class CarController : MonoBehaviour, IPlayerStatus
{
    public CarState _currentState = CarState.Idle;

    [SerializeField] private float _maxSpeed = 0.0f;
    [SerializeField] private float _moveForce = 0.0f;
    [SerializeField] private Rigidbody _myRigibody = default;
    [SerializeField] private PlayerInput playerInput = default;
    [SerializeField] private LayerMask _roadLayer = default;

    private InputReader _inputReader;
    private bool _canControl;

    // readonly を外し、Awake で代入する
    private CarDriver _driver;
    private GroundDetector _groundDetector;

    private const float _stopSpeedThreshold = 0.1f;
    public const float _inputThreshold = 0.01f;

    //IPlayerStatusの実装
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
        _driver = new CarDriver(_myRigibody, transform, _moveForce, _maxSpeed);
    }

    private void FixedUpdate()
    {
        if (!_canControl)
            return;
        FixedTick();
    }
    private void SwitchState(CarState newState)
    {
        _onStateChange.Invoke(_currentState, newState);

        Exit();
        _currentState = newState;
        Enter();
    }
    private void ApplyPhase(RacePhase phase, RacePhase nextPhase)
    {
        _canControl = nextPhase == RacePhase.Racing;
    }
    public void FixedTick()
    {
        switch (_currentState)
        {
            case CarState.Idle:
                InputInfo input = _inputReader.GetInputInfo();
                if (input.vertical != 0)
                {
                    SwitchState(CarState.Move);
                    return;
                }
                break;

            case CarState.Move:
                if (!_groundDetector.GroundSearch())
                {
                    SwitchState(CarState.Air);
                    return;
                }
                InputInfo moveInput = _inputReader.GetInputInfo();
                if (moveInput.isDrifted)
                {
                    SwitchState(CarState.Drift);
                    return;
                }
                GroundInfo groundInfo = _groundDetector.GetGroundInfo();
                _driver.DriveUpdate(moveInput, groundInfo);

                if (Mathf.Abs(moveInput.vertical) < _inputThreshold &&
                    Mathf.Abs(_driver.GetCurrentSpeed()) < _stopSpeedThreshold)
                {
                    SwitchState(CarState.Idle);
                    return;
                }
                break;

            case CarState.Drift:
                if (!_groundDetector.GroundSearch())
                {
                    SwitchState(CarState.Air);
                    return;
                }
                InputInfo driftInput = _inputReader.GetInputInfo();
                if (Mathf.Abs(driftInput.vertical) < _inputThreshold &&
                    Mathf.Abs(_driver.GetCurrentSpeed()) < _stopSpeedThreshold)
                {
                    SwitchState(CarState.Idle);
                    return;
                }
                if (!driftInput.isDrifted)
                {
                    SwitchState(CarState.Move);
                    return;
                }
                GroundInfo driftGroundInfo = _groundDetector.GetGroundInfo();
                _driver.DriftUpdate(driftInput, driftGroundInfo);

                break;

            case CarState.Air:
                if (_groundDetector.GroundSearch())
                {
                    SwitchState(CarState.Move);
                    return;
                }
                break;
        }
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

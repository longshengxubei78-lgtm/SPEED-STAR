using UnityEngine;
using UnityEngine.InputSystem;

public enum CarState {
    Idle, 
    Move, 
    Drift,
    Air 
}

public class CarController : MonoBehaviour
{
    public CarState _currentState = CarState.Idle;

    [SerializeField] private float _maxSpeed = 0.0f;
    [SerializeField] private float _moveForce = 0.0f;
    [SerializeField] private Rigidbody _myRigibody = default;
    [SerializeField] private RaceFlowManager _raceFlowManager;
    [SerializeField] private PlayerInput playerInput = default;
    [SerializeField] private SpeedMeter _speedMeter = default;
    [SerializeField] private SoundBank _soundBank = default;
    [SerializeField] private LayerMask _roadLayer = default;

    private InputReader _inputReader;
    private bool _canControl;

    // readonly を外し、Awake で代入する
    private CarDriver _driver;
    private InputData _inputData;
    private GroundDetector _groundDetector;
    private CarPresenter _carPresenter;
    private CarSoundPresenter _carSoundPresenter;
    private JumpRotation _jumpRotation;

    private const string IdleSoundName = "Idle";
    private const string MoveSoundName = "Accel";

    private void Awake()
    {
        _jumpRotation = new JumpRotation(_myRigibody, _roadLayer);
        _inputData = new InputData();
        InputPresenter inputPresenter = new InputPresenter(_inputData);
        _carSoundPresenter = new CarSoundPresenter(_inputData, _soundBank);
        CarMove carMove = new CarMove(_myRigibody, _maxSpeed, _roadLayer);
        _inputReader = new InputReader(inputPresenter, playerInput);
        _groundDetector = new GroundDetector(transform , _roadLayer);
        _driver = new CarDriver(_myRigibody, transform, _moveForce, _maxSpeed,_groundDetector);
        _carPresenter = new CarPresenter(_speedMeter, _driver);

        Enter();
    }

    private void FixedUpdate()
    {
        if (!_canControl)
            return;
        FixedTick();
    }
    private void SwitchState(CarState newState)
    {
        Exit();
        _currentState = newState;
        Enter();
    }
    private void ApplyPhase(RacePhase phase, RacePhase nextPhase)
    {
        _canControl = nextPhase == RacePhase.Racing;
    }
    private void Start()
    {
        _raceFlowManager._OnPhaseChaged.Subscribe(ApplyPhase);
    }
    private void Update()
    {
        if (!_canControl)
            return;
        _inputReader?.Tick();
    }
    public void FixedTick()
    {
        switch (_currentState)
        {
            case CarState.Idle:
                _carPresenter.CurrentCarSpeed();
                if (_inputData._inputVertical != 0)
                {
                    SwitchState(CarState.Move);
                }
                break;

            case CarState.Move:
                _carPresenter.CurrentCarSpeed();
                if (!_groundDetector.GroundSearch())
                {
                    SwitchState(CarState.Air);
                    return;
                }
                if (_inputData._isDrifting)
                {
                    SwitchState(CarState.Drift);
                    return;
                }
                _driver.DriveUpdate(_inputData._inputHorizontal,_inputData._inputVertical);

                if (_inputData._inputVertical == 0 &&
                    Mathf.Approximately(_driver.GetCurrentSpeed(), 0))
                {
                    SwitchState(CarState.Idle);
                }
                break;

            case CarState.Drift:
                _carPresenter.CurrentCarSpeed();
                if (!_groundDetector.GroundSearch())
                {
                    SwitchState(CarState.Air);
                    return;
                }
                if (Mathf.Approximately(_driver.GetCurrentSpeed(), 0))
                {
                    SwitchState(CarState.Idle);
                    return;
                }
                if (!_inputData._isDrifting)
                {
                    SwitchState(CarState.Move);
                    return;
                }
                _driver.DriftUpdate(_inputData._inputHorizontal, _inputData._inputVertical);

                break;

            case CarState.Air:
                _jumpRotation.FixedTick();
                _carPresenter.CurrentCarSpeed();
                if (_groundDetector.GroundSearch())
                {
                    SwitchState(CarState.Move);
                }
                break;
        }
    }

    private void Enter()
    {
        switch (_currentState)
        {
            case CarState.Idle:
                _carSoundPresenter.PlaySound(IdleSoundName);
                break;
            case CarState.Move:
                _carSoundPresenter.PlaySound(MoveSoundName);
                break;
            case CarState.Drift:
                break;
            case CarState.Air:
                _jumpRotation.StartSpin(1f);
                break;
        }
    }

    private void Exit()
    {
        switch (_currentState)
        {
            case CarState.Idle:
                _carSoundPresenter.StopSound(IdleSoundName);
                break;
            case CarState.Move:
                _carSoundPresenter.StopSound(MoveSoundName);
                break;
            case CarState.Drift:
                break;
            case CarState.Air:
                _jumpRotation.EndSpin(1f);
                break;
        }
    }

    private void OnDisable()
    {
        _inputReader?.Dispose();
    }
}

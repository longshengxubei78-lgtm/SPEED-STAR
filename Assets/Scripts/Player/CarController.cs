using UnityEngine;
using UnityEngine.InputSystem;
public class CarController : MonoBehaviour
{
    [SerializeField] private float _maxSpeed = 0.0f;
    [SerializeField] private float _moveForce = 0.0f;
    [SerializeField] private Rigidbody _myRigibody = default;

    private InputReader _inputReader = default;
    private CarStateMachine _carStateMachine = default;

    [SerializeField] private RaceFlowManager _raceFlowManager;
    [SerializeField] private PlayerInput playerInput = default;
    [SerializeField]private SpeedMeter _speedMeter = default;
    [SerializeField] private SoundBank _soundBank = default;
    [SerializeField] private LayerMask _roadLayer = default;
    private bool _canControl;

    private void ApplyPhase(RacePhase phase , RacePhase nextPhase)
    {
        _canControl = nextPhase == RacePhase.Racing;
    }
    private void Awake()
    {
        JumpRotation jumpRotation = new JumpRotation(_myRigibody ,_roadLayer);
        InputData inputData = new InputData();
        InputPresenter inputPresenter = new InputPresenter(inputData);
        CarSoundPresenter carSoundPresenter = new CarSoundPresenter(inputData, _soundBank);
        CarMove carMove = new CarMove(_myRigibody, _maxSpeed, _roadLayer);
        _inputReader = new InputReader(inputPresenter, playerInput);
        CarDriver driver = new CarDriver(carMove, transform, _moveForce, inputData);
        GroundDetector groundDetector = new GroundDetector(transform);
        CarPresenter presenter = new CarPresenter(_speedMeter, driver);
        _carStateMachine = new CarStateMachine(driver, inputData, groundDetector ,presenter, carSoundPresenter,jumpRotation);
    }
    private void Start()
    {
        _raceFlowManager._OnPhaseChaged.Subscribe(ApplyPhase);
    }
    private void OnEnable()
    {
    }
    private void Update()
    {
        if (!_canControl)
            return;
        _inputReader?.Tick();
    }
    private void FixedUpdate()
    {
        if (!_canControl)
            return;
        _carStateMachine.FixedTick();
    }
    private void OnDisable()
    {
        _inputReader?.Dispose();
    }
}
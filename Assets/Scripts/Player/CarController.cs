using UnityEngine;
using UnityEngine.InputSystem;
public class CarController : MonoBehaviour
{
    [SerializeField] private float _maxSpeed = 0.0f;
    [SerializeField] private float _moveForce = 0.0f;
    [SerializeField] private Rigidbody _myRigibody = default;

    private CarDriver _driver = default;
    private CarMove _carMove = default;
    private GroundDetector _groundDetector = default;
    private InputReader _inputReader = default;
    private CarStateMachine _carStateMachine = default;
    private CarPresenter _presenter = default;

    [SerializeField] private PlayerInput playerInput = default;
    [SerializeField]private SpeedMeter _speedMeter = default;
    private void Awake()
    {
        _carMove = new CarMove(_myRigibody, _maxSpeed);
        _inputReader = new InputReader(this,playerInput);
        _driver = new CarDriver(_carMove, transform, _maxSpeed, _moveForce, _inputReader);
        _groundDetector = new GroundDetector(transform);
        _presenter = new CarPresenter(_speedMeter, _driver);
        _carStateMachine = new CarStateMachine(_driver, _inputReader, _groundDetector ,_presenter);
    }
    private void OnEnable()
    {
    }
    private void Update()
    {
        _carStateMachine?.Tick();
    }
    private void FixedUpdate()
    {
        _carStateMachine.FixedTick();
    }
    private void OnDisable()
    {
        _inputReader?.Dispose();
    }
    public void InputDrift(InputAction.CallbackContext context)
    {
        if (context.performed)
        {

        }
        else if (context.canceled) 
        {

        }
    }
}
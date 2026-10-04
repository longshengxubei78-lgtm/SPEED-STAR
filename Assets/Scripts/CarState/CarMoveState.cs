using UnityEngine;
public class CarMoveState : CarStateBase
{
    private CarDriver _driver;
    private InputReader _inputReader;
    private GroundDetector _groundDetector;
    private CarPresenter _carPresenter;
    public CarMoveState(CarStateMachine stateMachine, CarDriver driver, InputReader inputReader, GroundDetector groundDetector, CarPresenter carPresenter) : base(stateMachine)
    {
        _driver = driver;
        _inputReader = inputReader;
        _groundDetector = groundDetector;
        _carPresenter = carPresenter;
    }

    public override void Enter()
    {
        Debug.Log("Enter Move State");
    }
    public override void Tick()
    {
    }
    public override void FixedTick()
    {
        _carPresenter.CurrentCarSpeed();
        _inputReader.Tick();

        float currentSpeed = _driver.GetCurrentSpeed();

        bool isGrounded = _groundDetector.GroundSearch();

        if (!isGrounded)
        {
            _stateMachine.SwitchState(typeof(CarAirState));
        }
        if (currentSpeed==0)
        {
            _stateMachine.SwitchState(typeof(CarIdleState));

        }
        _driver.CarUpdate(1.5f);
    }
    public override void Exit()
    {
        Debug.Log("Exit Move State");
    }
}
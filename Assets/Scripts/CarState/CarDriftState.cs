using UnityEngine;
public class CarDriftState : CarStateBase
{
    private CarDriver _driver;
    private GroundDetector _groundDetector;
    private CarPresenter _carPresenter;
    public CarDriftState(CarStateMachine stateMachine, CarDriver driver, GroundDetector groundDetector, CarPresenter carPresente) : base(stateMachine)
    {
        _driver = driver;
        _groundDetector = groundDetector;
    }

    public override void Enter()
    {

    }
    public override void FixedTick()
    {
        _carPresenter.CurrentCarSpeed();

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
        _driver.CarUpdate();
    }
    public override void Exit()
    {

    }
    public override void Input()
    {
        // Handle input for drifting
    }
}
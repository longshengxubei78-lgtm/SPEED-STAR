using UnityEngine;
public class CarMoveState : CarStateBase
{
    private CarDriver _driver;
    private GroundDetector _groundDetector;
    private CarPresenter _carPresenter;
    private CarSoundPresenter _carSoundPresenter;
    private string _soundName = "Accel";
    public CarMoveState(CarStateMachine stateMachine, CarDriver driver, GroundDetector groundDetector, CarPresenter carPresenter, CarSoundPresenter carSoundPresenter) : base(stateMachine)
    {
        _driver = driver;
        _groundDetector = groundDetector;
        _carPresenter = carPresenter;
        _carSoundPresenter = carSoundPresenter;
    }

    public override void Enter()
    {
        _carSoundPresenter.PlaySound(_soundName);
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
        if (Mathf.Approximately(currentSpeed, 0))
        {
            _stateMachine.SwitchState(typeof(CarIdleState));

        }
        _driver.CarUpdate();
    }
    public override void Exit()
    {
        _carSoundPresenter.StopSound(_soundName);
    }
    public override void Input()
    {
        // Handle input for moving
    }
}
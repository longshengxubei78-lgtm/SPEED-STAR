public class CPUMoveState : CPUStateBase
{
    private CarDriver _carDriver;
    private GroundDetector _groundDetector;
    public CPUMoveState(CPUStateMachine stateMachine, CarDriver carDriver,GroundDetector groundDetector) : base(stateMachine)
    {
        _carDriver = carDriver;
        _groundDetector = groundDetector;
    }
    public override void Enter()
    {
    }
    public override void Exit()
    {

    }
    public override void Tick()
    {
    }
    public override void FixedTick()
    {
        float currentSpeed = _carDriver.GetCurrentSpeed();

        bool isGrounded = _groundDetector.GroundSearch();

        if (!isGrounded)
        {
            _stateMachine.SwitchState(typeof(CPUAirState));
        }
        if (currentSpeed == 0)
        {
            _stateMachine.SwitchState(typeof(CPUIdleState));

        }
        _carDriver.CarUpdate();
    }
    public override void Input()
    {
        // Handle input for moving
    }
}

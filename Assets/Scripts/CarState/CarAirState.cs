public class CarAirState : CarStateBase
{
    private InputReader _inputReader = default;
    private GroundDetector _groundDetecto = default;
    private CarPresenter _carPresenter = default;
    public CarAirState(CarStateMachine stateMachine, InputReader inputReader, GroundDetector groundDetector, CarPresenter carPresenter) : base(stateMachine)
    {
        _inputReader = inputReader;
        _groundDetecto = groundDetector;
        _carPresenter = carPresenter;
    }
    public override void Enter()
    {

    }
    public override void Tick()
    {

    }
    public override void FixedTick()
    {
        _carPresenter.CurrentCarSpeed();
        bool isGrounded = _groundDetecto.GroundSearch();

        if(isGrounded)
        {
            _stateMachine.SwitchState(typeof(CarMoveState));
        }
    }
    public override void Exit()
    {

    }
}

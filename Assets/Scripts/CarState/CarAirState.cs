public class CarAirState : CarStateBase
{
    private GroundDetector _groundDetecto = default;
    private CarPresenter _carPresenter = default;
    private CarSoundPresenter _carSoundPresenter = default;
    private JumpRotation _jumpRotation = default;
    public CarAirState(CarStateMachine stateMachine, GroundDetector groundDetector, CarPresenter carPresenter,CarSoundPresenter carSoundPresenter, JumpRotation jumpRotation) : base(stateMachine)
    {
        _groundDetecto = groundDetector;
        _carPresenter = carPresenter;
        _carSoundPresenter = carSoundPresenter;

        _jumpRotation = jumpRotation;
    }
    public override void Enter()
    {
        _jumpRotation.StartSpin(1f);
    }
    public override void FixedTick()
    {
        _jumpRotation.FixedTick();
        _carPresenter.CurrentCarSpeed();
        bool isGrounded = _groundDetecto.GroundSearch();

        if(isGrounded)
        {
            _stateMachine.SwitchState(typeof(CarMoveState));
        }
    }
    public override void Exit()
    {
        _jumpRotation.EndSpin(1f);

    }
    public override void Input()
    {
        // Handle input for air state
    }
}

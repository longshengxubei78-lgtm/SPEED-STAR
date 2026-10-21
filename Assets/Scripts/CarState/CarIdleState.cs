using UnityEngine;
public class CarIdleState : CarStateBase
{
    private InputData _inputData;
    private CarPresenter _carPresenter;
    private CarSoundPresenter _carSoundPresenter;
    private string _soundName = "Idle";
    public CarIdleState(CarStateMachine stateMachine, InputData inputData, CarPresenter carPresenter, CarSoundPresenter carSoundPresenter) : base(stateMachine)
    {
        _inputData = inputData;
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
        float getInputVertical = _inputData._inputVertical;
        
        if (getInputVertical != 0)
        {
            _stateMachine.SwitchState(typeof(CarMoveState));
            return;
        }
    }
    public override void Exit()
    {
        _carSoundPresenter.StopSound(_soundName);

    }
    public override void Input()
    {
        // Handle input for idle state
    }
}
using UnityEngine;
public class CarIdleState : CarStateBase
{
    private InputReader _inputReader;
    private CarPresenter _carPresenter;
    public CarIdleState(CarStateMachine stateMachine, InputReader inputReader, CarPresenter carPresenter) : base(stateMachine)
    {
        _inputReader = inputReader;
        _carPresenter = carPresenter;
    }
    public override void Enter()
    {
        Debug.Log("Entering Idle State");
    }
    public override void Tick()
    {
    }
    public override void FixedTick()
    {
        _carPresenter.CurrentCarSpeed();
        _inputReader.Tick();
        float getInputVertical = _inputReader._inputvertical;
        
        if (getInputVertical != 0)
        {
            _stateMachine.SwitchState(typeof(CarMoveState));
            return;
        }
    }
    public override void Exit()
    {
        Debug.Log("Exiting Idle State");
    }
}